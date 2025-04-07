namespace RPGCharacterService.Models.Items
{
    [Flags]
    public enum WeaponProperty
    {
        None = 0,
        Light = 1 << 0, // 1
        Heavy = 1 << 1, // 2
        TwoHanded = 1 << 2, // 4
        Versatile = 1 << 3, // 8
        Thrown = 1 << 4, // 16
        Finesse = 1 << 5, // 32
        Loading = 1 << 6, // 64
        Ammunition = 1 << 7 // 128
    }

    public enum WeaponRangeType
    {
        Melee,
        Ranged,
    }

    public enum WeaponCategory
    {
        Simple,
        Martial,
    }

    public record Weapon(
        ushort Id,
        string Name,
        string Description,
        int DamageBonus = 0,
        WeaponProperty Properties = WeaponProperty.None,
        WeaponCategory Category = WeaponCategory.Simple,
        WeaponRangeType RangeType = WeaponRangeType.Melee) : Equipment(Id, Name, Description, 0, DamageBonus);
}
