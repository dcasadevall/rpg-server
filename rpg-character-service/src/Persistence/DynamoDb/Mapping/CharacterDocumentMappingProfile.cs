using AutoMapper;
using RPGCharacterService.Entities;
using RPGCharacterService.Entities.Characters;
using RPGCharacterService.Entities.Items;
using RPGCharacterService.Persistence.DynamoDb.Models;
using System;

namespace RPGCharacterService.Persistence.DynamoDb.Mapping {
  /// <summary>
  /// AutoMapper profile for mapping between Character and CharacterDocument.
  /// </summary>
  public class CharacterDocumentMappingProfile : Profile {
    public CharacterDocumentMappingProfile() {
      // Add mapping from Guid to CharacterDocument
      CreateMap<Guid, CharacterDocument>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ToString()));

      // Add explicit mapping for Equipment
      CreateMap<EquipmentDocument, Equipment>()
        .ConstructUsing((src, ctx) => new Equipment(
          src.MainHand != null ? MapDocumentToItem(src.MainHand) : null,
          src.OffHand != null ? MapDocumentToItem(src.OffHand) : null,
          src.Armor != null ? MapDocumentToItem(src.Armor) : null
        ));

      // Add explicit mapping for Equipment to EquipmentDocument
      CreateMap<Equipment, EquipmentDocument>()
        .ForMember(dest => dest.MainHand, opt => opt.MapFrom(src => src.MainHand != null ? MapItemToDocument(src.MainHand) : null))
        .ForMember(dest => dest.OffHand, opt => opt.MapFrom(src => src.OffHand != null ? MapItemToDocument(src.OffHand) : null))
        .ForMember(dest => dest.Armor, opt => opt.MapFrom(src => src.Armor != null ? MapItemToDocument(src.Armor) : null));

      // Add mapping for Wealth
      CreateMap<Wealth, WealthDocument>()
        .ForMember(dest => dest.Copper, opt => opt.MapFrom(src => src.Copper))
        .ForMember(dest => dest.Silver, opt => opt.MapFrom(src => src.Silver))
        .ForMember(dest => dest.Electrum, opt => opt.MapFrom(src => src.Electrum))
        .ForMember(dest => dest.Gold, opt => opt.MapFrom(src => src.Gold))
        .ForMember(dest => dest.Platinum, opt => opt.MapFrom(src => src.Platinum));

      // Add mapping for WealthDocument to Wealth
      CreateMap<WealthDocument, Wealth>()
        .ConstructUsing((src, ctx) => new Wealth(new Dictionary<CurrencyType, int> {
          {CurrencyType.Copper, src.Copper},
          {CurrencyType.Silver, src.Silver},
          {CurrencyType.Electrum, src.Electrum},
          {CurrencyType.Gold, src.Gold},
          {CurrencyType.Platinum, src.Platinum}
        }));

      CreateMap<Character, CharacterDocument>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
        .ForMember(dest => dest.AbilityScores,
                   opt => opt.MapFrom(src => src.AbilityScores.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value)))
        .ForMember(dest => dest.Equipment, opt => opt.MapFrom(src => src.Equipment))
        .ForMember(dest => dest.Wealth, opt => opt.MapFrom(src => src.Wealth));

      CreateMap<CharacterDocument, Character>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Race, opt => opt.MapFrom(src => src.Race))
        .ForMember(dest => dest.Subrace, opt => opt.MapFrom(src => src.Subrace))
        .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
        .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
        .ForMember(dest => dest.HitPoints, opt => opt.MapFrom(src => src.HitPoints))
        .ForMember(dest => dest.AbilityScores, opt =>
          opt.MapFrom(src => src.AbilityScores.ToDictionary(kvp => Enum.Parse<AbilityScore>(kvp.Key), kvp => kvp.Value)))
        .ForMember(dest => dest.Equipment, opt => opt.MapFrom(src => src.Equipment))
        .ForMember(dest => dest.Wealth, opt => opt.MapFrom(src => src.Wealth));
    }

    private static ItemDocument MapItemToDocument(Item item) {
      return new ItemDocument {
        Id = item.Id,
        Name = item.Name,
        EquipmentStats = item.EquipmentStats != null ? new EquipmentStatsDocument {
          EquipmentType = item.EquipmentStats.EquipmentType,
          ArmorStats = item.EquipmentStats.ArmorStats != null ? new ArmorStatsDocument {
            BaseArmorClass = item.EquipmentStats.ArmorStats.BaseArmorClass,
            ArmorType = item.EquipmentStats.ArmorStats.ArmorType
          } : null,
          WeaponStats = item.EquipmentStats.WeaponStats != null ? new WeaponStatsDocument {
            WeaponProperties = item.EquipmentStats.WeaponStats.WeaponProperties,
            RangeType = item.EquipmentStats.WeaponStats.RangeType
          } : null
        } : null
      };
    }

    private static Item MapDocumentToItem(ItemDocument document) {
      return new Item {
        Id = document.Id,
        Name = document.Name,
        EquipmentStats = document.EquipmentStats != null ? new EquipmentStats {
          EquipmentType = document.EquipmentStats.EquipmentType,
          ArmorStats = document.EquipmentStats.ArmorStats != null ? new ArmorStats {
            BaseArmorClass = document.EquipmentStats.ArmorStats.BaseArmorClass,
            ArmorType = document.EquipmentStats.ArmorStats.ArmorType
          } : null,
          WeaponStats = document.EquipmentStats.WeaponStats != null ? new WeaponStats {
            WeaponProperties = document.EquipmentStats.WeaponStats.WeaponProperties,
            RangeType = document.EquipmentStats.WeaponStats.RangeType
          } : null
        } : null
      };
    }
  }
}
