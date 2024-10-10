using Branta.Features.InstallerVerification;
using Branta.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Branta.Features.WalletDashboard;

public class WalletDashboardViewModel : ObservableObject
{
    public InstallerVerificationViewModel InstallerVerificationViewModel { get; set; }
    public WalletVerificationViewModel WalletVerificationViewModel { get; set; }

    public WalletDashboardViewModel(
        InstallerVerificationViewModel installerVerificationViewModel,
        WalletVerificationViewModel walletVerificationViewModel)
    {
        InstallerVerificationViewModel = installerVerificationViewModel;
        WalletVerificationViewModel = walletVerificationViewModel;
    }
}
