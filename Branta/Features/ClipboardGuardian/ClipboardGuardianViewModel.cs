using Branta.Features.Main;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Branta.Features.ClipboardGuardian;

public partial class ClipboardGuardianViewModel : ObservableObject
{
    [ObservableProperty]
    private ClipboardItemViewModel _clipboardItem;

    private readonly ClipboardItemStore _clipboardItemStore;
    private readonly LanguageStore _languageStore;

    public ClipboardGuardianViewModel(ClipboardItemStore clipboardItemStore, LanguageStore languageStore)
    {
        _clipboardItemStore = clipboardItemStore;
        _languageStore = languageStore;

        RefreshClipboardItem();
        _clipboardItemStore.OnClipboardItemUpdate += RefreshClipboardItem;
    }

    private void RefreshClipboardItem()
    {
        ClipboardItem = new ClipboardItemViewModel(_clipboardItemStore.ClipboardItem, _languageStore);
    }
}