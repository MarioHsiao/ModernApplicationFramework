﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(AddCategoryCommandDefinition))]
    public class AddCategoryCommandDefinition : CommandDefinition<IAddCategoryCommand>
    {
        public override string Name => ToolboxResources.AddCategoryCommand_Name;
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.AddCategoryCommand_Name), CultureInfo.InvariantCulture);
        public override string Text => ToolboxResources.AddCategoryCommand_Text;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{D7D3206E-0BBD-41E4-96DF-07EA57571586}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IAddCategoryCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IAddCategoryCommand))]
    internal class AddCategoryCommand : CommandDefinitionCommand, IAddCategoryCommand
    {
        private readonly IToolboxService _service;

        [ImportingConstructor]
        public AddCategoryCommand(IToolboxService service)
        {
            _service = service;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var c = new ToolboxCategory();
            c.CreatedCancelled += C_CreatedCancelled;
            c.Created += C_Created;
            _service.AddCategory(c);
        }

        private void C_CreatedCancelled(object sender, EventArgs e)
        {
            if (!(sender is IToolboxCategory category))
                return;

            category.Created -= C_Created;
            category.CreatedCancelled -= C_CreatedCancelled;
            _service.RemoveCategory(category);
        }

        private void C_Created(object sender, EventArgs e)
        {
            if (!(sender is IToolboxNode node))
                return;
            node.Created -= C_Created;
            node.CreatedCancelled -= C_CreatedCancelled;
        }
    }
}
