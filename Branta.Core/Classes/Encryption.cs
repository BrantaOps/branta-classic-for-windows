using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Cryptography;
using System.Text;

namespace Branta.Core.Classes;

public class Encryption
{
    public static void EncryptEntities<T>(IEnumerable<T> entries)
        where T : EntityEntry
    {
        foreach (var entry in entries)
        {
            foreach (var property in entry.CurrentValues.Properties)
            {
                if (property.ClrType == typeof(string))
                {
                    if (entry.CurrentValues[property] is string currentValue)
                    {
                        entry.CurrentValues[property] = Protect(currentValue);
                    }
                }
            }
        }
    }

    public static void DecryptEntities<T>(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            DecryptEntity(entity);
        }
    }

    public static void DecryptEntity<T>(T entity)
    {
        foreach (var property in typeof(T).GetProperties())
        {
            if (property.PropertyType == typeof(string))
            {
                if (property.GetValue(entity) is string encryptedValue)
                {
                    property.SetValue(entity, Unprotect(encryptedValue));
                }
            }
        }
    }

    public static string Protect(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedBytes);
    }

    public static string Unprotect(string protectedData)
    {
        var protectedBytes = Convert.FromBase64String(protectedData);
        var bytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(bytes);
    }
}
