﻿using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interface provides the basic structure required for a toolbar hoster
    /// </summary>
    public interface IToolBarHostViewModel : ICommandBarHost, IHasMainWindowViewModel
    {
        /// <summary>
        /// The command to handle a right click onto the toolbar host
        /// </summary>
        ICommand OpenContextMenuCommand { get; }

        /// <summary>
        /// The context menu of the toolbar host
        /// </summary>
        ContextMenu ContextMenu { get; }

        /// <summary>
        /// The top toolbar tray
        /// </summary>
        ToolBarTray TopToolBarTray { get; set; }

        /// <summary>
        /// The left toolbar tray
        /// </summary>
        ToolBarTray LeftToolBarTray { get; set; }

        /// <summary>
        /// The right toolbar tray
        /// </summary>
        ToolBarTray RightToolBarTray { get; set; }

        /// <summary>
        /// The bottom toolbar tray
        /// </summary>
        ToolBarTray BottomToolBarTray { get; set; }


        /// <summary>
        /// Defines the toolbar scope of the host.
        /// </summary>
        ToolbarScope ToolbarScope { get; }

        /// <summary>
        /// Adds a new tool bar to the host
        /// </summary>
        /// <param name="definition">The data model of the toolbar</param>
        void AddToolbarDefinition(ToolbarDefinition definition);

        /// <summary>
        /// Removes a toolbar from the host
        /// </summary>
        /// <param name="definition">The model of the toolbar</param>
        void RemoveToolbarDefinition(ToolbarDefinition definition);

        /// <summary>
        /// Gets an unique localized default toolbar name
        /// </summary>
        /// <returns></returns>
        string GetUniqueToolBarName();
    }
}