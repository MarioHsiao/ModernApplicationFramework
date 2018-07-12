﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolboxService))]
    internal class ToolboxService : IToolboxService
    {
        private readonly ToolboxItemHost _host;
        private readonly IToolboxStateProvider _stateProvider;
        private readonly IToolbox _toolbox;
        private readonly ToolboxItemDefinitionHost _definitionHost;

        internal static IToolboxService Instance { get; private set; }

        [ImportingConstructor]
        public ToolboxService(ToolboxItemHost host, IToolboxStateProvider stateProvider, IToolbox toolbox, ToolboxItemDefinitionHost definitionHost)
        {
            _host = host;
            _stateProvider = stateProvider;
            _toolbox = toolbox;
            _definitionHost = definitionHost;
            Instance = this;
        }

        public IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource()
        {
            return _stateProvider.ItemsSource.ToList();
        }

        public void StoreCurrentLayout()
        {
            InternalStoreLayout(_toolbox.CurrentLayout);
        }

        public void StoreAndApplyLayout(IEnumerable<IToolboxCategory> layout)
        {
            InternalStoreLayout(layout);
            _toolbox.RefreshView();
        }

        private void InternalStoreLayout(IEnumerable<IToolboxCategory> layout)
        {
            _stateProvider.ItemsSource.Clear();
            _stateProvider.ItemsSource.AddRange(layout);
        }

        public void AddCategory(IToolboxCategory category, bool suppressRefresh = false)
        {
            var index = _stateProvider.ItemsSource.Count;
            if (_stateProvider.ItemsSource.LastOrDefault() == ToolboxCategory.DefaultCategory)
                index--;
            InsertCategory(index, category);
        }

        public void InsertCategory(int index, IToolboxCategory category, bool supressRefresh = false)
        {
            _stateProvider.ItemsSource.Insert(index, category);
            _host.RegisterNode(category);
            if (!supressRefresh)
                _toolbox.RefreshView();
        }

        public void RemoveCategory(IToolboxCategory category, bool cascading = true, bool supressRefresh = false)
        {
            if (_stateProvider.ItemsSource.Contains(category))
                _stateProvider.ItemsSource.Remove(category);
            if (cascading)
            {
                foreach (var item in category.Items.ToList())
                    category.RemoveItem(item);
            }
            _host.DeleteNode(category);
            if (!supressRefresh)
                _toolbox.RefreshView();
        }

        public IToolboxCategory GetCategoryById(Guid guid)
        {
            return guid == Guid.Empty ? null : _host.AllCategories.FirstOrDefault(x => x.Id.Equals(guid));
        }

        public IToolboxItem GetItemById(Guid guid)
        {
            return guid == Guid.Empty ? null : _host.AllItems.FirstOrDefault(x => x.Id.Equals(guid));
        }

        public IEnumerable<IToolboxItem> FindItemsByDefintion(ToolboxItemDefinitionBase definition)
        {
            return _host.AllItems.Where(x => x.DataSource.Id == definition.Id);
        }

        public IReadOnlyCollection<string> GetAllToolboxCategoryNames()
        {
            return _host.AllCategories.Select(x => x.Name).ToList();
        }
    }
}
