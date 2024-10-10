using Branta.Features.Main;

namespace Branta.Features.ClipboardGuardian;

public class ClipboardItemStore
{
    private readonly LanguageStore _languageStore;

    public ClipboardItem ClipboardItem { private set; get; }

    public event Action OnClipboardItemUpdate;

    public ClipboardItemStore(LanguageStore languageStore)
    {
        _languageStore = languageStore;
    }

    public void UpdateClipboardItem(ClipboardItem clipboardItem)
    {
        ClipboardItem = clipboardItem ?? new ClipboardItem
    {
        Value = _languageStore.Get("ClipboardGuardian_None"),
        IsDefault = true
    };

        OnClipboardItemUpdate?.Invoke();
    }
}