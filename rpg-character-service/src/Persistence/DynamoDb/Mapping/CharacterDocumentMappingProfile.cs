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

      CreateMap<Character, CharacterDocument>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
        .ForMember(dest => dest.AbilityScores,
                   opt => opt.MapFrom(src => src.AbilityScores.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value)))
        .ForMember(dest => dest.Equipment,
                   opt => opt.MapFrom(src => new EquipmentDocument {
                     MainHand = src.Equipment.MainHand != null ? MapItemToDocument(src.Equipment.MainHand) : null,
                     OffHand = src.Equipment.OffHand != null ? MapItemToDocument(src.Equipment.OffHand) : null,
                     Armor = src.Equipment.Armor != null ? MapItemToDocument(src.Equipment.Armor) : null
                   }))
        .ForMember(dest => dest.Wealth,
                   opt => opt.MapFrom(src => new WealthDocument {
                     Copper = src.Wealth.Copper,
                     Silver = src.Wealth.Silver,
                     Electrum = src.Wealth.Electrum,
                     Gold = src.Wealth.Gold,
                     Platinum = src.Wealth.Platinum
                   }));

      CreateMap<CharacterDocument, Character>()
        .ConstructUsing((src, ctx) => new Character {
          Id = Guid.Parse(src.Id),
          Name = src.Name,
          Race = src.Race,
          Subrace = src.Subrace,
          Class = src.Class,
          Level = src.Level,
          HitPoints = src.HitPoints,
          AbilityScores = src.AbilityScores.ToDictionary(kvp => Enum.Parse<AbilityScore>(kvp.Key), kvp => kvp.Value),
          Equipment = new Equipment(
            src.Equipment.MainHand != null ? MapDocumentToItem(src.Equipment.MainHand) : null,
            src.Equipment.OffHand != null ? MapDocumentToItem(src.Equipment.OffHand) : null,
            src.Equipment.Armor != null ? MapDocumentToItem(src.Equipment.Armor) : null
          ),
          Wealth = new Wealth(new Dictionary<CurrencyType, int> {
            {CurrencyType.Copper, src.Wealth.Copper},
            {CurrencyType.Silver, src.Wealth.Silver},
            {CurrencyType.Electrum, src.Wealth.Electrum},
            {CurrencyType.Gold, src.Wealth.Gold},
            {CurrencyType.Platinum, src.Wealth.Platinum}
          })
        });
    }

    private static ItemDocument MapItemToDocument(Item item) {
      return new ItemDocument {
        Id = item.Id.ToString(),
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

    private static Item? MapDocumentToItem(ItemDocument document) {
      return new Item {
        Id = int.Parse(document.Id),
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
