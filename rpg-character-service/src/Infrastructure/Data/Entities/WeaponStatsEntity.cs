using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Infrastructure.Data.Entities {
  /// <summary>
  /// Database entity representing weapon-specific statistics.
  /// </summary>
  public class WeaponStatsEntity {
    /// <summary>
    /// The unique identifier for these weapon stats.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the item these weapon stats belong to.
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// The properties of this weapon (e.g., Finesse, Two-Handed, etc.) stored as a bitwise flag.
    /// </summary>
    public int WeaponProperties { get; set; }

    /// <summary>
    /// The range type of this weapon (Melee or Ranged).
    /// </summary>
    public int RangeType { get; set; }

    // Navigation property

    /// <summary>
    /// The item these weapon stats belong to.
    /// </summary>
    public ItemEntity Item { get; set; } = null!;

    /// <summary>
    /// Gets the weapon properties as an enum.
    /// </summary>
    public WeaponProperty GetWeaponProperties() => (WeaponProperty) WeaponProperties;

    /// <summary>
    /// Sets the weapon properties from an enum.
    /// </summary>
    public void SetWeaponProperties(WeaponProperty weaponProperties) => WeaponProperties = (int) weaponProperties;

    /// <summary>
    /// Gets the range type as an enum.
    /// </summary>
    public WeaponRangeType GetRangeType() => (WeaponRangeType) RangeType;

    /// <summary>
    /// Sets the range type from an enum.
    /// </summary>
    public void SetRangeType(WeaponRangeType rangeType) => RangeType = (int) rangeType;
  }
}
