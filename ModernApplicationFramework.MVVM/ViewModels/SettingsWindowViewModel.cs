﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    [Export(typeof(SettingsWindowViewModel))]
    public class SettingsWindowViewModel : Screen
    {
        private SettingsPageViewModel _selectedPage;
        private IEnumerable<ISettingsPage> _settingPages;

        public SettingsWindowViewModel()
        {
            DisplayName = "Options";
        }

        public ICommand CancelCommand => new Command(Cancel);

        public ICommand OkCommand => new Command(ApplyChanges);

        public List<SettingsPageViewModel> Pages { get; private set; }

        public SettingsPageViewModel SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                NotifyOfPropertyChange();
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var pages = new List<SettingsPageViewModel>();
            _settingPages = IoC.GetAll<ISettingsPage>();

            foreach (var settingPage in _settingPages)
            {
                var parentCollection = GetParentCollection(settingPage, pages);

                var page = parentCollection.FirstOrDefault(m => m.Name == settingPage.Name);

                if (page == null)
                {
                    page = new SettingsPageViewModel
                    {
                        Name = settingPage.Name
                    };
                    parentCollection.Add(page);
                }

                page.Pages.Add(settingPage);
            }

            Pages = pages;
            SelectedPage = GetFirstLeafPageRecursive(pages);
        }

        private static SettingsPageViewModel GetFirstLeafPageRecursive(List<SettingsPageViewModel> pages)
        {
            if (!pages.Any())
                return null;
            var firstPage = pages.First();
            return !firstPage.Children.Any() ? firstPage : GetFirstLeafPageRecursive(firstPage.Children);
        }

        private void ApplyChanges()
        {
            foreach (var settingsEditor in _settingPages)
            {
                settingsEditor.Apply();
            }

            TryClose(true);
        }

        private void Cancel()
        {
            TryClose(false);
        }

        private List<SettingsPageViewModel> GetParentCollection(ISettingsPage settingPage,
                                                                List<SettingsPageViewModel> pages)
        {
            if (string.IsNullOrEmpty(settingPage.Path))
            {
                return pages;
            }

            var path = settingPage.Path.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pathElement in path)
            {
                var page = pages.FirstOrDefault(s => s.Name == pathElement);

                if (page == null)
                {
                    page = new SettingsPageViewModel {Name = pathElement};
                    pages.Add(page);
                }

                pages = page.Children;
            }

            return pages;
        }
    }
}