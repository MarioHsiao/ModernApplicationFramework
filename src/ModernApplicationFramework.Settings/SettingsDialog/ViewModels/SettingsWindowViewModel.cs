﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Properties;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces.Settings;
using ISettingsPage = ModernApplicationFramework.Settings.Interfaces.ISettingsPage;

namespace ModernApplicationFramework.Settings.SettingsDialog.ViewModels
{
    [Export(typeof(SettingsWindowViewModel))]
    public sealed class SettingsWindowViewModel : Screen
    {
        private SettingsPageContainerViewModel _selectedPageContainer;
        private IEnumerable<ISettingsPage> _settingPages;
        private readonly ISettingsDialogSettings _settings;

        public ICommand CancelCommand => new Command(Cancel);

        public ICommand OkCommand => new Command(ApplyChanges);

        public List<SettingsPageContainerViewModel> Pages { get; private set; }

        public SettingsPageContainerViewModel SelectedPageContainer
        {
            get => _selectedPageContainer;
            set
            {
                _selectedPageContainer = value;
                NotifyOfPropertyChange();
            }
        }

        public SettingsWindowViewModel()
        {
            DisplayName = CommonUI_Resources.OptionsDialog_Name;

            _settings = IoC.Get<ISettingsDialogSettings>();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var pages = new List<SettingsPageContainerViewModel>();

            _settingPages = IoC.GetAll<ISettingsPage>().OrderBy(x => x.SortOrder);

            var groups = _settingPages.Select(p => p.Category.Root).Distinct().OrderBy(x => x.SortOrder);
            foreach (var settingsCategory in groups)
                FillRecursive(settingsCategory, pages);
            Pages = pages;
            SelectedPageContainer = GetFirstLeafPageRecursive(pages);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            foreach (var settingsPage in _settingPages)
                settingsPage.Activate();
        }

        protected override void OnViewLoaded(object view)
        {
            if (!(view is DialogWindow control))
                return;
            _dialogControl = control;
            
            if (_settings == null)
                return;
            var dialogWidth = _settings.SettingsDialogWidth;
            if (dialogWidth != 0)
                _dialogControl.Width = dialogWidth;
            var dialogHeight = _settings.SettingsDialogHeight;
            if (dialogHeight != 0)
                _dialogControl.Height = dialogHeight;
        }

        private DialogWindow _dialogControl;

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                if (_dialogControl != null && _settings != null)
                {
                    _settings.SettingsDialogWidth = (int)_dialogControl.Width;
                    _settings.SettingsDialogHeight = (int)_dialogControl.Height;
                }
            }
            base.OnDeactivate(close);
        }


        private static SettingsPageContainerViewModel GetFirstLeafPageRecursive(List<SettingsPageContainerViewModel> pages)
        {
            if (!pages.Any())
                return null;
            var firstPage = pages.First();
            return !firstPage.Children.Any() ? firstPage : GetFirstLeafPageRecursive(firstPage.Children);
        }

        private static IList<SettingsPageContainerViewModel> GetParentCollection(ISettingsPage settingPage,
            IList<SettingsPageContainerViewModel> pages)
        {
            if (settingPage.Category == null)
                return pages;

            foreach (var category in settingPage.Category.Path)
            {
                var page = pages.FirstOrDefault(s => s.Category == category);
                if (page == null)
                {
                    page = new SettingsPageContainerViewModel { Text = category.Text, Category = category };
                    page.Pages.Add(settingPage);
                    pages.Add(page);
                }
                else if (page.Pages.First().Name == settingPage.Name)
                {
                    page.Pages.Add(settingPage);
                }
                pages = page.Children;
            }
            return pages;
        }

        private void FillRecursive(SettingsPageCategory category, IList<SettingsPageContainerViewModel> pagesList)
        {
            var subGroups = category.Children.OrderBy(x => x.SortOrder);

            foreach (var settingsCategory in subGroups)
                FillRecursive(settingsCategory, pagesList);

            var toppages = _settingPages.Where(x => x.Category == category);
            foreach (var settingPage in toppages)
            {
                var parentCollection = GetParentCollection(settingPage, pagesList);

                var page = parentCollection.FirstOrDefault(m => m.Text == settingPage.Name);

                if (page == null)
                {
                    page = new SettingsPageContainerViewModel
                    {
                        Text = settingPage.Name,
                        Category = settingPage.Category
                    };
                    parentCollection.Add(page);
                }
                page.Pages.Add(settingPage);
            }
        }

        private void ApplyChanges()
        {
            if (_settingPages.Any(settingPage => !settingPage.CanApply()))
                return;
            var close = true;
            foreach (var settingPage in _settingPages)
                if (!settingPage.Apply())
                    close = false;
            if (!close)
                return;
            IoC.Get<ISettingsManager>().SaveCurrent();
            TryClose(true);
        }

        private void Cancel()
        {
            foreach (var settingPage in _settingPages)
                settingPage.Cancel();
            TryClose(false);
        }
    }
}