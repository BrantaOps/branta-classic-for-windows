using Branta.Commands;
using Branta.Features.ClipboardGuardian;
using Branta.Features.Settings.ExtendedKey;
using Branta.Features.Settings.Preferences;
using Branta.Features.WalletDashboard;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace Branta.Features.Main;

public partial class MainViewModel : ObservableObject
{
    private readonly Timer _updateAppTimer;
    private readonly Timer _clipboardGuardianTimer;
    private readonly IServiceProvider _serviceProvider;

    private static readonly int ICON_ONLY_WIDTH = 900;

    public ObservableObject CurrentViewModel { get; set; }

    public ICommand UpdateAppCommand { get; }
    public ICommand HelpCommand { get; }
    public ICommand ClipboardGuardianCommand { get; }

    public bool IsDashboardActive => CurrentViewModel is WalletDashboardViewModel;

    public bool IsClipboardActive => CurrentViewModel is ClipboardGuardianViewModel;

    public bool IsKeysActive => CurrentViewModel is ExtendedKeyManagerViewModel;

    public bool IsSettingsActive => CurrentViewModel is SettingsViewModel;

    [ObservableProperty]
    private bool _isSmallScreen = true;

    [RelayCommand]
    public void NavigateDashboard()
    {
        Navigate<WalletDashboardViewModel>();
    }

    [RelayCommand]
    public void NavigateClipboard()
    {
        Navigate<ClipboardGuardianViewModel>();
    }

    [RelayCommand]
    public void NavigateKeys()
    {
        Navigate<ExtendedKeyManagerViewModel>();
    }

    [RelayCommand]
    public void NavigateSettings()
    {
        Navigate<SettingsViewModel>();
    }

    public MainViewModel(UpdateAppCommand updateAppCommand, ClipboardGuardianCommand clipboardGuardianCommand, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ClipboardGuardianCommand = clipboardGuardianCommand;

        UpdateAppCommand = updateAppCommand;
        HelpCommand = new HelpCommand();
        
        _clipboardGuardianTimer = new Timer(new TimeSpan(0, 0, 0, 0, milliseconds: 500));
        _clipboardGuardianTimer.Elapsed += (_, _) => ClipboardGuardianCommand.Execute(null);
        _clipboardGuardianTimer.Start();

        _updateAppTimer = new Timer(new TimeSpan(24, 0, 0));
        _updateAppTimer.Elapsed += (_, _) => UpdateAppCommand.Execute(null);
        _updateAppTimer.Start();

        UpdateAppCommand.Execute(null);

        Navigate<WalletDashboardViewModel>();
    }

    private void Navigate<T>() where T : ObservableObject
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<T>();
        OnPropertyChanged(nameof(CurrentViewModel));

        OnPropertyChanged(nameof(IsDashboardActive));
        OnPropertyChanged(nameof(IsClipboardActive));
        OnPropertyChanged(nameof(IsKeysActive));
        OnPropertyChanged(nameof(IsSettingsActive));
    }

    public void WidthUpdated(double newWidth)
    {
        if (IsSmallScreen == false && newWidth < ICON_ONLY_WIDTH)
        {
            IsSmallScreen = true;
        }
        else if (IsSmallScreen == true && newWidth > ICON_ONLY_WIDTH)
        {
            IsSmallScreen = false;
        }
    }
}