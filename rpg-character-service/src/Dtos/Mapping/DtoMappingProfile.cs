using AutoMapper;
using RPGCharacterService.Dtos.Character;
using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Dtos.Equipment;
using RPGCharacterService.Models;

namespace RPGCharacterService.Dtos.Mapping {
  /// <summary>
  /// AutoMapper profile for mapping between DTOs and domain models.
  /// </summary>
  public class DtoMappingProfile : Profile {
    public DtoMappingProfile() {
      // Character to CharacterResponse mapping
      CreateMap<Models.Characters.Character, CharacterResponse>()
        .ForMember(dest => dest.MaxHitPoints, opt => opt.MapFrom(src => src.CalculateMaxHitPoints()))
        .ForMember(dest => dest.ArmorClass, opt => opt.MapFrom(src => src.CalculateArmorClass()))
        .ForMember(dest => dest.ProficiencyBonus, opt => opt.MapFrom(src => src.CalculateProficiencyBonus()))
        .ForMember(dest => dest.AbilityModifiers, opt => opt.MapFrom(src => src.CalculateAllAbilityModifiers()));

      // Wealth to CurrencyResponse mapping
      CreateMap<Wealth, CurrencyResponse>();

      // ModifyCurrencyRequest to Dictionary<CurrencyType, int> mapping
      CreateMap<ModifyCurrencyRequest, Dictionary<CurrencyType, int>>()
        .ConstructUsing((src, ctx) => new Dictionary<CurrencyType, int> {
          {CurrencyType.Copper, src.Copper ?? 0},
          {CurrencyType.Silver, src.Silver ?? 0},
          {CurrencyType.Electrum, src.Electrum ?? 0},
          {CurrencyType.Gold, src.Gold ?? 0},
          {CurrencyType.Platinum, src.Platinum ?? 0}
        });

      // Character to EquipmentResponse mapping
      CreateMap<Models.Characters.Character, EquipmentResponse>()
        .ForMember(dest => dest.ArmorClass, opt => opt.MapFrom(src => src.CalculateArmorClass()))
        .ForMember(dest => dest.WeaponDamageModifier, opt => opt.MapFrom(src => src.CalculateWeaponDamageModifier()))
        .ForMember(dest => dest.Equipment,
                   opt => opt.MapFrom(src => new EquipmentDetails {
                     MainHandId = src.Equipment.MainHand != null ? src.Equipment.MainHand.Id : null,
                     OffHandId = src.Equipment.OffHand != null ? src.Equipment.OffHand.Id : null,
                     ArmorId = src.Equipment.Armor != null ? src.Equipment.Armor.Id : null
                   }));
    }
  }
}
