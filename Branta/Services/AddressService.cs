using Branta.Core.Data.Domain;
using Branta.Core.Enums;
using NBitcoin;

namespace Branta.Services;

public class AddressService
{
    public static ExtendedKey GetExtendedKeyByAddress(IEnumerable<ExtendedKey> extendedKeys, string address)
    {
        var network = GetNetwork();

        var extendedPublicKeys = extendedKeys.ToDictionary(k => k, k => ExtPubKey.Parse(k.Value, network));

        for (uint i = 0; i < 120; i++)
        {
            foreach (var (key, extPubKey) in extendedPublicKeys)
            {
                var childAddress = extPubKey.Derive(new KeyPath($"0/{i}")).PubKey.GetAddress(GetNBitcoinAddressType(key.AddressType), network);

                if (address == childAddress.ToString())
                {
                    return key;
                }
            }
        }

        return null;
    }

    private static ScriptPubKeyType GetNBitcoinAddressType(AddressType addressType)
    {
        return addressType switch
        {
            AddressType.Segwit => ScriptPubKeyType.Segwit,
            AddressType.SegwitP2SH => ScriptPubKeyType.SegwitP2SH,
            AddressType.Legacy => ScriptPubKeyType.Legacy,
            _ => ScriptPubKeyType.Segwit,
        };
    }

    private static Network GetNetwork()
    {
        return Network.Main;
    }
}
