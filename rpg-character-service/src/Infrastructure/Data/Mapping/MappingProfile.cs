using AutoMapper;
using RPGCharacterService.Infrastructure.Data.Entities;
using RPGCharacterService.Models;
using RPGCharacterService.Models.Characters;
using RPGCharacterService.Models.Items;

namespace RPGCharacterService.Infrastructure.Data.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Character mappings
            CreateMap<CharacterEntity, Character>()
                .ForMember(dest => dest.AbilityScores, opt => opt.Ignore())
                .ForMember(dest => dest.Wealth, opt => opt.Ignore())
                .ForMember(dest => dest.Equipment, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    // Map ability scores
                    foreach (var abilityScoreEntity in src.AbilityScores)
                    {
                        dest.AbilityScores[abilityScoreEntity.GetAbilityType()] = abilityScoreEntity.Score;
                    }

                    // Map currency
                    if (src.Currency != null)
                    {
                        dest.Wealth.SetCurrencyAmount(CurrencyType.Copper, src.Currency.Copper);
                        dest.Wealth.SetCurrencyAmount(CurrencyType.Silver, src.Currency.Silver);
                        dest.Wealth.SetCurrencyAmount(CurrencyType.Electrum, src.Currency.Electrum);
                        dest.Wealth.SetCurrencyAmount(CurrencyType.Gold, src.Currency.Gold);
                        dest.Wealth.SetCurrencyAmount(CurrencyType.Platinum, src.Currency.Platinum);
                    }

                    // Map equipment
                    if (src.Equipment != null)
                    {
                        if (src.Equipment.MainHandItem != null)
                        {
                            dest.Equipment.EquipWeapon(context.Mapper.Map<Item>(src.Equipment.MainHandItem));
                        }

                        if (src.Equipment.OffHandItem != null)
                        {
                            dest.Equipment.EquipWeapon(context.Mapper.Map<Item>(src.Equipment.OffHandItem), true);
                        }

                        if (src.Equipment.ArmorItem != null)
                        {
                            dest.Equipment.EquipArmor(context.Mapper.Map<Item>(src.Equipment.ArmorItem));
                        }
                    }
                });

            CreateMap<Character, CharacterEntity>()
                .ForMember(dest => dest.AbilityScores, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.Equipment, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    // Map ability scores
                    dest.AbilityScores = new List<AbilityScoreEntity>();
                    foreach (var abilityScore in src.AbilityScores)
                    {
                        dest.AbilityScores.Add(new AbilityScoreEntity
                        {
                            CharacterId = dest.Id,
                            AbilityType = (int)abilityScore.Key,
                            Score = abilityScore.Value
                        });
                    }

                    // Map currency
                    dest.Currency = new CurrencyEntity
                    {
                        CharacterId = dest.Id,
                        Copper = src.Wealth.GetCurrencyAmount(CurrencyType.Copper),
                        Silver = src.Wealth.GetCurrencyAmount(CurrencyType.Silver),
                        Electrum = src.Wealth.GetCurrencyAmount(CurrencyType.Electrum),
                        Gold = src.Wealth.GetCurrencyAmount(CurrencyType.Gold),
                        Platinum = src.Wealth.GetCurrencyAmount(CurrencyType.Platinum)
                    };
                });

            // Item mappings
            CreateMap<ItemEntity, Item>()
                .ForMember(dest => dest.EquipmentStats, opt => opt.MapFrom(src =>
                    src.EquipmentType.HasValue
                        ? new EquipmentStats
                        {
                            EquipmentType = (EquipmentType)src.EquipmentType.Value,
                            ArmorStats = src.EquipmentType.Value == (int)EquipmentType.Armor && src.ArmorStats != null
                                ? new ArmorStats
                                {
                                    BaseArmorClass = src.ArmorStats.BaseArmorClass,
                                    ArmorType = src.ArmorStats.GetArmorType()
                                }
                                : null,
                            WeaponStats = src.EquipmentType.Value == (int)EquipmentType.Weapon && src.WeaponStats != null
                                ? new WeaponStats
                                {
                                    WeaponProperties = src.WeaponStats.GetWeaponProperties(),
                                    RangeType = src.WeaponStats.GetRangeType()
                                }
                                : null
                        }
                        : null));
        }
    }
}
