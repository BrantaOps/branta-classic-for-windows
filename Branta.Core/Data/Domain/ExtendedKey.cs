using Branta.Core.Enums;

namespace Branta.Core.Data.Domain;

public class ExtendedKey : BaseEntity
{
    public string Name { get; set; }

    public string Value { get; set; }

    public AddressType AddressType { get; set; }
}
