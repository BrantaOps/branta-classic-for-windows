using Branta.Core.Data;
using Branta.Core.Enums;
using Branta.Core.Classes;
using Microsoft.EntityFrameworkCore;

namespace Branta.Features.Settings.ExtendedKey;

public class ExtendedKeyStore
{
    private readonly BrantaContext _brantaContext;

    private List<Core.Data.Domain.ExtendedKey> _extendedKeys = [];

    public IEnumerable<Core.Data.Domain.ExtendedKey> ExtendedKeys => _extendedKeys;

    public event Action OnExtendedKeyUpdate;

    public bool IsLoading = true;

    public ExtendedKeyStore(BrantaContext brantaContext)
    {
        _brantaContext = brantaContext;
    }

    public async Task LoadAsync()
    {
        _extendedKeys = await _brantaContext.ExtendedKey
            .ToListAsync();

        Encryption.DecryptEntities(_extendedKeys);

        IsLoading = false;

        OnExtendedKeyUpdate?.Invoke();
    }

    public async Task AddAsync(string name, string value, AddressType addressType)
    {
        var extendedKey = new Core.Data.Domain.ExtendedKey()
        {
            Name = name,
            Value = value,
            AddressType = addressType,
            DateCreated = DateTime.Now,
            DateUpdated = DateTime.Now
        };

        await _brantaContext.ExtendedKey.AddAsync(extendedKey);
        await _brantaContext.SaveChangesAsync();

        await LoadAsync();
    }

    public async Task UpdateAsync(int id, string name, string value, AddressType addressType)
    {
        var extendedKey = _extendedKeys.FirstOrDefault(k => k.Id == id);

        if (extendedKey == null)
        {
            return;
        }

        extendedKey.Name = name;
        extendedKey.Value = value;
        extendedKey.AddressType = addressType;
        extendedKey.DateUpdated = DateTime.Now;

        _brantaContext.ExtendedKey.Update(extendedKey);
        await _brantaContext.SaveChangesAsync();

        await LoadAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var extendedKey = _extendedKeys.FirstOrDefault(k => k.Id == id);

        _brantaContext.ExtendedKey.Remove(extendedKey);
        await _brantaContext.SaveChangesAsync();

        await LoadAsync();
    }
}
