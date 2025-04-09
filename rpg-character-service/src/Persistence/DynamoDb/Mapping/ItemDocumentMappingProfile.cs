using AutoMapper;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence.DynamoDb.Models;

namespace RPGCharacterService.Persistence.DynamoDb.Mapping {
  /// <summary>
  /// AutoMapper profile for mapping between Item and ItemDocument.
  /// </summary>
  public class ItemDocumentMappingProfile : Profile {
    public ItemDocumentMappingProfile() {
      CreateMap<Item, ItemDocument>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
        .ForMember(dest => dest.EquipmentStats,
                   opt => opt.MapFrom(src => src.EquipmentStats != null ? new EquipmentStatsDocument {
                     EquipmentType = src.EquipmentStats.EquipmentType,
                     ArmorStats = src.EquipmentStats.ArmorStats != null ? new ArmorStatsDocument {
                       BaseArmorClass = src.EquipmentStats.ArmorStats.BaseArmorClass,
                       ArmorType = src.EquipmentStats.ArmorStats.ArmorType
                     } : null,
                     WeaponStats = src.EquipmentStats.WeaponStats != null ? new WeaponStatsDocument {
                       WeaponProperties = src.EquipmentStats.WeaponStats.WeaponProperties,
                       RangeType = src.EquipmentStats.WeaponStats.RangeType
                     } : null
                   } : null));

      CreateMap<ItemDocument, Item>()
        .ConstructUsing((src, ctx) => new Item {
          Id = int.Parse(src.Id),
          Name = src.Name,
          EquipmentStats = src.EquipmentStats != null ? new EquipmentStats {
            EquipmentType = src.EquipmentStats.EquipmentType,
            ArmorStats = src.EquipmentStats.ArmorStats != null ? new ArmorStats {
              BaseArmorClass = src.EquipmentStats.ArmorStats.BaseArmorClass,
              ArmorType = src.EquipmentStats.ArmorStats.ArmorType
            } : null,
            WeaponStats = src.EquipmentStats.WeaponStats != null ? new WeaponStats {
              WeaponProperties = src.EquipmentStats.WeaponStats.WeaponProperties,
              RangeType = src.EquipmentStats.WeaponStats.RangeType
            } : null
          } : null
        });
    }
  }
}
