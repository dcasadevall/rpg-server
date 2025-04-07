namespace RPGCharacterService.Models.Items;

public abstract record Equipment(ushort Id, 
                                 string Name, 
                                 string Description, 
                                 int ArmorBonus = 0, 
                                 int DamageBonus = 0) : Item(Id, Name, Description);
