﻿using System;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.MarketplaceService;
using Windows.UI.Xaml;

namespace Sample.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        Services.SettingsService _settings;
        MarketplaceService _marketplace;

        public SettingsPageViewModel()
        {
            _settings = Services.SettingsService.GetInstance();
            _marketplace = MarketplaceService.Create();
        }

        // Settings

        public bool UseShellBackButton
        {
            get => _settings.UseShellBackButton;
            set => Set(() => { _settings.UseShellBackButton = value; });
        }

        public bool UseLightThemeButton
        {
            get => _settings.AppTheme.Equals(ApplicationTheme.Light);
            set => Set(() => { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; });
        }

        public string BusyText
        {
            get => _settings.BusyText;
            set => Set(() => { _settings.BusyText = value; });
        }

        public void ShowBusy() => Views.Busy.ShowBusyFor(BusyText, 5000);


        // About

        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public async void Review() => await _marketplace.LaunchAppReviewInStoreAsync();
    }
}