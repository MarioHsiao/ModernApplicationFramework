﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox.Services
{
    [Export(typeof(IToolboxService))]
    public  class ToolboxService : IToolboxService
    {
        private readonly ToolboxItemHost _host;
        private readonly IToolboxStateProvider _stateProvider;
        private readonly IToolbox _toolbox;

        internal static IToolboxService Instance { get; private set; }

        [ImportingConstructor]
        public ToolboxService(ToolboxItemHost host, IToolboxStateProvider stateProvider, IToolbox toolbox)
        {
            _host = host;
            _stateProvider = stateProvider;
            _toolbox = toolbox;
            Instance = this;
        }

        public IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource()
        {
            return _stateProvider.ItemsSource.ToList();
        }


        public void StoreCurrentLayout()
        {
            _stateProvider.ItemsSource.Clear();
            _stateProvider.ItemsSource.AddRange(_toolbox.CurrentLayout);
        }

        public void AddCategory(IToolboxCategory category, bool supressRefresh = false)
        {
            InsertCategory(_stateProvider.ItemsSource.Count, category);
        }

        public void InsertCategory(int index, IToolboxCategory category, bool supressRefresh = false)
        {
            if (_stateProvider.ItemsSource.LastOrDefault() == ToolboxItemCategory.DefaultCategory)
                index--;
            _stateProvider.ItemsSource.Insert(index, category);
            _host.RegisterNode(category);
            if (!supressRefresh)
                _toolbox.RefreshView();
        }

        public void RemoveCategory(IToolboxCategory category, bool supressRefresh = false)
        {
            if (_stateProvider.ItemsSource.Contains(category))
                _stateProvider.ItemsSource.Remove(category);
            _host.DeleteNode(category);
            if (!supressRefresh)
                _toolbox.RefreshView();
        }

        public IReadOnlyCollection<string> GetAllToolboxCategoryNames()
        {
            return _host.AllCategories.Select(x => x.Name).ToList();
        }
    }
}
