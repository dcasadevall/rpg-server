using AutoMapper;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence.DynamoDb.Models;

namespace RPGCharacterService.Persistence.DynamoDb.Mapping {
  /// <summary>
  /// AutoMapper profile for mapping between Item and ItemDocument.
  /// </summary>
  public class ItemDocumentMappingProfile : Profile {
    public ItemDocumentMappingProfile() {
      // Add mapping for EquipmentStatsDocument to EquipmentStats
      CreateMap<EquipmentStatsDocument, EquipmentStats>()
        .ForMember(dest => dest.EquipmentType, opt => opt.MapFrom(src => src.EquipmentType))
        .ForMember(dest => dest.ArmorStats, opt => opt.MapFrom(src => src.ArmorStats))
        .ForMember(dest => dest.WeaponStats, opt => opt.MapFrom(src => src.WeaponStats));

      // Add mapping for ArmorStatsDocument to ArmorStats
      CreateMap<ArmorStatsDocument, ArmorStats>()
        .ForMember(dest => dest.BaseArmorClass, opt => opt.MapFrom(src => src.BaseArmorClass))
        .ForMember(dest => dest.ArmorType, opt => opt.MapFrom(src => src.ArmorType));

      // Add mapping for WeaponStatsDocument to WeaponStats
      CreateMap<WeaponStatsDocument, WeaponStats>()
        .ForMember(dest => dest.WeaponProperties, opt => opt.MapFrom(src => src.WeaponProperties))
        .ForMember(dest => dest.RangeType, opt => opt.MapFrom(src => src.RangeType));

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
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => int.Parse(src.Id)))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.EquipmentStats, opt => opt.MapFrom(src => src.EquipmentStats));
    }
  }
}
