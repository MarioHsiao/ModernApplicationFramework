﻿using System.Collections.Generic;
using System.Windows.Controls;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Core.Themes;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IToolBarHostViewModel : IHasTheme
    {
        IMainWindowViewModel MainWindowViewModel { get; }

        Command OpenContextMenuCommand { get; }
        ToolBarTray BottomToolBarTay { get; set; }
        ToolBarTray LeftToolBarTay { get; set; }
        ToolBarTray RightToolBarTay { get; set; }
        ToolBarTray TopToolBarTay { get; set; }

        /// <summary>
        /// Adds new Toolbar to HostControl
        /// </summary>
        /// <param name="toolBar">Toolbar object</param>
        /// <param name="visible">Toolbar visibility</param>
        /// <param name="dock">Toolbar orientation</param>
        void AddToolBar(ToolBar toolBar, bool visible, Dock dock);

        /// <summary>
        /// Change Orientation of Toolbar
        /// </summary>
        /// <param name="name">IdentifierName of Toolbar</param>
        /// <param name="newValue">New Orientation Value</param>
        void ChangeToolBarDock(string name, Dock newValue);

        /// <summary>
        /// Change Visibility of Toolbar
        /// </summary>
        /// <param name="name">IdentifierName of Toolbar</param>
        /// <param name="newValue">New Visibility Value</param>
        void ChangeToolBarVisibility(string name, bool newValue);

        /// <summary>
        /// Get a Toolbar by Name
        /// </summary>
        /// <param name="name">IdentifierName of Toolbar</param>
        /// <returns>Found Toolbar Object</returns>
        ToolBar GetToolBar(string name);

        /// <summary>
        /// Get Orientation of Toolbar
        /// </summary>
        /// <param name="name">Identifier Name of Toolbar</param>
        /// <returns>Orientation</returns>
        Dock GetToolBarDock(string name);

        /// <summary>
        /// Returns a list of Toolbar Objects
        /// </summary>
        /// <returns></returns>
        List<ToolBar> GetToolBars();

        /// <summary>
        /// Get Toolbar Visibility
        /// </summary>
        /// <param name="name">Identifier Name of Toolbar</param>
        /// <returns>Bool of visibility</returns>
        bool GetToolBarVisibility(string name);

        void HideToolBar(ToolBar toolBar, Dock dock);
        void HideToolBarByName(string name);

        /// <summary>
        /// Removes Toolbar from ToolBarHostControl
        /// </summary>
        /// <param name="name"></param>
        void RemoveToolBar(string name);

        void ShowToolBar(ToolBar toolBar, Dock dock);
        void ShowToolBarByName(string name);
        void UpdateDock(string name, Dock newValue);
        void UpdateVisibility(string name, bool newValue);
    }
}