﻿using Branta.Classes;
using Branta.Commands;
using Branta.Features.Main;
using Branta.Features.Settings;
using Branta.Features.Settings.ExtendedKey;
using Branta.Services;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace Branta.Features.ClipboardGuardian;

public partial class ClipboardGuardianCommand : BaseCommand
{
    private readonly NotificationCenter _notificationCenter;
    private readonly LanguageStore _languageStore;
    private readonly SettingsStore _settings;
    private readonly ExtendedKeyStore _extendedKeyStore;
    private readonly ILogger<ClipboardGuardianCommand> _logger;
    private readonly ClipboardItemStore _clipboardItemStore;

    private const int SeedWordMin = 12;
    private const int SeedWordMax = 24;

    private string LastClipboardContent { get; set; }
    private HashSet<string> Bip39Words { get; set; }

    public ClipboardGuardianCommand(NotificationCenter notificationCenter, LanguageStore languageStore, SettingsStore settings,
        ExtendedKeyStore extendedKeyStore, ClipboardItemStore clipboardItemStore, ILogger<ClipboardGuardianCommand> logger)
    {
        _notificationCenter = notificationCenter;
        _languageStore = languageStore;
        _settings = settings;
        _extendedKeyStore = extendedKeyStore;
        _clipboardItemStore = clipboardItemStore;
        _logger = logger;
    }

    public override void Execute(object parameter)
    {
        var clipBoardContent = Application.Current.Dispatcher.Invoke(() => Clipboard.GetText().Trim());

        if (clipBoardContent == LastClipboardContent)
        {
            return;
        }

        LastClipboardContent = clipBoardContent;

        var clipboardItem = Process(clipBoardContent);

        _clipboardItemStore.UpdateClipboardItem(clipboardItem);

        if (clipboardItem?.Notification != null)
        {
            _notificationCenter.Notify(clipboardItem.Notification);
        }
    }

    private ClipboardItem Process(string clipBoardContent)
    {
        if (CheckForBitcoinAddress(clipBoardContent))
        {
            var extendedKey = AddressService.GetExtendedKeyByAddress(_extendedKeyStore.ExtendedKeys, clipBoardContent);

            if (extendedKey != null)
            {
                var text = _languageStore.Format("ClipboardGuardian_AddressDetectedFrom", extendedKey.Name);

                return new ClipboardItem
                {
                    Name = _languageStore.Get("ClipboardGuardian_BitcoinAddressName"),
                    Value = text,
                    Notification = _settings.ClipboardGuardian.BitcoinAddressesEnabled ? new Notification
                    {
                        Title = _languageStore.Get("ClipboardGuardian_BitcoinAddressTitle"),
                        Message = text,
                    } : null
                };
            }

            return new ClipboardItem(_settings.ClipboardGuardian.BitcoinAddressesEnabled,
                _languageStore,
                "BitcoinAddress",
                clipBoardContent);
        }

        if (CheckForSeedPhrase(clipBoardContent))
        {
            return new ClipboardItem(_settings.ClipboardGuardian.SeedPhraseEnabled,
                _languageStore,
                "SeedPhrase");
        }

        if (CheckForXPub(clipBoardContent))
        {
            return new ClipboardItem(_settings.ClipboardGuardian.ExtendedPublicKeyEnabled,
                _languageStore,
                "BitcoinPublicKey");
        }

        if (CheckForXPrv(clipBoardContent))
        {
            return new ClipboardItem(_settings.ClipboardGuardian.PrivateKeyEnabled,
                _languageStore,
                "BitcoinPrivateKey");
        }

        if (CheckForNPub(clipBoardContent))
        {
            return new ClipboardItem(_settings.ClipboardGuardian.NostrPublicKeyEnabled,
                _languageStore,
                "NostrPublicKey");
        }

        if (CheckForNPrv(clipBoardContent))
        {
            return new ClipboardItem(_settings.ClipboardGuardian.NostrPrivateKeyEnabled,
                _languageStore,
                "NostrPrivateKey");
        }

        return null;
    }

    public static bool CheckForBitcoinAddress(string value)
    {
        return BitcoinAddressRegex().IsMatch(value) || SegwitAddressRegex().IsMatch(value) || TestNetRegex().IsMatch(value);
    }

    public bool CheckForSeedPhrase(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        if (!content.Contains(' ') && !content.Contains(Environment.NewLine))
        {
            return false;
        }

        var spaced = content.Split(' ');
        var newLined = content.Split(Environment.NewLine);

        var userSeedWords = new HashSet<string>();

        if (spaced.Length >= newLined.Length)
        {
            userSeedWords = new HashSet<string>(spaced);
        }
        else if (newLined.Length > spaced.Length)
        {
            userSeedWords = new HashSet<string>(newLined);
        }

        // TODO - handle the case where the line broke, and it's a valid seed with \n on a line. Strip each word of \n.

        if (userSeedWords.Count < SeedWordMin || userSeedWords.Count > SeedWordMax)
        {
            return false;
        }

        if (Bip39Words == null)
        {
            LoadBip39Words();
        }

        return !userSeedWords.Select(w => w.ToLower()).Except(Bip39Words!).Any();
    }

    public static bool CheckForXPub(string value)
    {
        return ExtendedKeyRegex().IsMatch(value);
    }

    public static bool CheckForXPrv(string value)
    {
        return value.StartsWith("xprv") && value.Length > 10;
    }

    public static bool CheckForNPub(string value)
    {
        return NPubAddressRegex().IsMatch(value);
    }

    public static bool CheckForNPrv(string value)
    {
        return NPrvAddressRegex().IsMatch(value);
    }

    private void LoadBip39Words()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "Branta.Assets.bip39wordlist.txt";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);

            var words = reader.ReadToEnd();
            Bip39Words = words.Split(Environment.NewLine).ToHashSet();
        }
        catch (Exception ex)
        {
            _logger?.LogInformation(ex.Message);
        }
    }

    [GeneratedRegex("^[13][a-km-zA-HJ-NP-Z1-9]{25,34}$")]
    private static partial Regex BitcoinAddressRegex();

    [GeneratedRegex("^bc1[0-9a-zA-Z]{25,65}$")]
    private static partial Regex SegwitAddressRegex();

    [GeneratedRegex("^tb1[0-9a-zA-Z]{25,65}$")]
    public static partial Regex TestNetRegex();

    [GeneratedRegex("^npub[0-9a-z]{58,65}$")]
    private static partial Regex NPubAddressRegex();

    [GeneratedRegex("^nsec[0-9a-z]{58,65}$")]
    private static partial Regex NPrvAddressRegex();

    [GeneratedRegex("^([xyYzZtuUvV]pub[1-9A-HJ-NP-Za-km-z]{79,108})$")]
    private static partial Regex ExtendedKeyRegex();
}