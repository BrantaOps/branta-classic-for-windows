using Branta.Classes;
using Branta.Core.Data.Domain;
using Branta.Core.Enums;
using Branta.Features.ClipboardGuardian;
using Branta.Features.Main;
using Branta.Features.Settings.ExtendedKey;
using Branta.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace Branta.ViewModels;

public class AddressTypeOption
{
    public string Name { get; set; }

    public AddressType Value { get; set; }
}

public partial class EditExtendedKeyViewModel : ObservableValidator, IValidateViewModel
{
    private readonly ExtendedKeyStore _extendedKeyStore;

    public readonly ExtendedKey ExtendedKey;

    public LanguageStore LanguageStore { get; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    [CustomValidation(typeof(EditExtendedKeyViewModel), nameof(ValidateNameRequired))]
    [CustomValidation(typeof(EditExtendedKeyViewModel), nameof(ValidateNameUnique))]
    private string _name;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    [CustomValidation(typeof(EditExtendedKeyViewModel), nameof(ValidateExtendedKeyRequired))]
    [CustomValidation(typeof(EditExtendedKeyViewModel), nameof(ValidateExtendedKey))]
    private string _value;

    [ObservableProperty]
    private AddressTypeOption _selectedAddressType;

    public List<AddressTypeOption> AddressTypes { get; set; }

    public Action CloseAction { get; set; }

    public IEnumerable<ExtendedKey> ExtendedKeys => _extendedKeyStore.ExtendedKeys;

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    public async Task Submit()
    {
        if (ExtendedKey == null)
        {
            await _extendedKeyStore.AddAsync(Name, Value, SelectedAddressType.Value);
        }
        else
        {
            await _extendedKeyStore.UpdateAsync(ExtendedKey.Id, Name, Value, SelectedAddressType.Value);
        }

        CloseAction?.Invoke();
    }

    public bool CanSubmit()
    {
        return Name != null && Value != null && !HasErrors;
    }

    public EditExtendedKeyViewModel(ExtendedKeyStore extendedKeyStore, ExtendedKey extendedKey, LanguageStore languageStore)
    {
        _extendedKeyStore = extendedKeyStore;
        ExtendedKey = extendedKey;
        LanguageStore = languageStore;

        var defaultAddressType = AddressType.Segwit;

        if (ExtendedKey != null)
        {
            Name = ExtendedKey.Name;
            Value = ExtendedKey.Value;
            defaultAddressType = ExtendedKey.AddressType;
        }

        AddressTypes = [
            new()
            {
                Name = "Legacy",
                Value = AddressType.Legacy
            },
            new() {
                Name = "Segwit",
                Value = AddressType.Segwit
            },
            new() {
                Name = "Segwit (P2SH)",
                Value = AddressType.SegwitP2SH
            }];

        SelectedAddressType = AddressTypes.FirstOrDefault(at => at.Value == defaultAddressType);
    }

    public static ValidationResult ValidateNameRequired(string value, ValidationContext context)
    {
        return context.Validate<EditExtendedKeyViewModel>(
            value,
            (_, v) => string.IsNullOrEmpty(v),
            "Validation_ExtendedKey_Name_Required");
    }

    public static ValidationResult ValidateNameUnique(string value, ValidationContext context)
    {
        return context.Validate<EditExtendedKeyViewModel>(
            value,
            (viewModel, v) => viewModel.ExtendedKeys.Any(k => k.Name == v && k.Id != viewModel.ExtendedKey?.Id),
            "Validation_ExtendedKey_Name_Unique");
    }

    public static ValidationResult ValidateExtendedKeyRequired(string value, ValidationContext context)
    {
        return context.Validate<EditExtendedKeyViewModel>(
            value,
            (_, v) => string.IsNullOrEmpty(v),
            "Validation_ExtendedKey_Value_Required");
    }

    public static ValidationResult ValidateExtendedKey(string value, ValidationContext context)
    {
        return context.Validate<EditExtendedKeyViewModel>(
            value,
            (_, value) => !ClipboardGuardianCommand.CheckForXPub(value),
            "Validation_ExtendedKey_Value_Valid");
    }
}
