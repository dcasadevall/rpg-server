-- Schema for RPG Character Database

-- Enable UUID extension for GUID support
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create Characters table
CREATE TABLE Characters (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name VARCHAR(100) NOT NULL,
    Race VARCHAR(50) NOT NULL,
    Subrace VARCHAR(50) NOT NULL,
    Class VARCHAR(50) NOT NULL,
    HitPoints INT NOT NULL DEFAULT 10,
    Level INT NOT NULL DEFAULT 1,
    InitFlags INT NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create AbilityScores table
CREATE TABLE AbilityScores (
    Id SERIAL PRIMARY KEY,
    CharacterId UUID NOT NULL REFERENCES Characters(Id) ON DELETE CASCADE,
    AbilityType INT NOT NULL, -- Enum: 0=Strength, 1=Dexterity, 2=Constitution, 3=Intelligence, 4=Wisdom, 5=Charisma
    Score INT NOT NULL,
    UNIQUE(CharacterId, AbilityType)
);

-- Create Currency table
CREATE TABLE Currencies (
    Id SERIAL PRIMARY KEY,
    CharacterId UUID NOT NULL REFERENCES Characters(Id) ON DELETE CASCADE,
    Copper INT NOT NULL DEFAULT 0,
    Silver INT NOT NULL DEFAULT 0,
    Electrum INT NOT NULL DEFAULT 0,
    Gold INT NOT NULL DEFAULT 0,
    Platinum INT NOT NULL DEFAULT 0,
    UNIQUE(CharacterId)
);

-- Create Items table
CREATE TABLE Items (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    EquipmentType INT NULL -- Enum: 0=Armor, 1=Weapon, 2=Shield, NULL=Not equipment
);

-- Create ArmorStats table
CREATE TABLE ArmorStats (
    Id SERIAL PRIMARY KEY,
    ItemId INT NOT NULL REFERENCES Items(Id) ON DELETE CASCADE,
    BaseArmorClass INT NOT NULL DEFAULT 0,
    ArmorType INT NOT NULL, -- Enum: 0=None, 1=Light, 2=Medium, 3=Heavy
    UNIQUE(ItemId)
);

-- Create WeaponStats table
CREATE TABLE WeaponStats (
    Id SERIAL PRIMARY KEY,
    ItemId INT NOT NULL REFERENCES Items(Id) ON DELETE CASCADE,
    WeaponProperties INT NOT NULL DEFAULT 0, -- Flags: 1=Light, 2=Heavy, 4=TwoHanded, etc.
    RangeType INT NOT NULL DEFAULT 0, -- Enum: 0=Melee, 1=Ranged
    UNIQUE(ItemId)
);

-- Create CharacterEquipment table to track equipped items
CREATE TABLE CharacterEquipment (
    Id SERIAL PRIMARY KEY,
    CharacterId UUID NOT NULL REFERENCES Characters(Id) ON DELETE CASCADE,
    MainHandItemId INT NULL REFERENCES Items(Id) ON DELETE SET NULL,
    OffHandItemId INT NULL REFERENCES Items(Id) ON DELETE SET NULL,
    ArmorItemId INT NULL REFERENCES Items(Id) ON DELETE SET NULL,
    UNIQUE(CharacterId)
);

-- Create indexes for performance
CREATE INDEX idx_characters_name ON Characters(Name);
CREATE INDEX idx_abilityscores_characterid ON AbilityScores(CharacterId);
CREATE INDEX idx_currencies_characterid ON Currencies(CharacterId);
CREATE INDEX idx_characterequipment_characterid ON CharacterEquipment(CharacterId);

-- Create function to update timestamps
CREATE OR REPLACE FUNCTION update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create trigger to update timestamps on Characters
CREATE TRIGGER update_characters_timestamp
BEFORE UPDATE ON Characters
FOR EACH ROW
EXECUTE FUNCTION update_timestamp();
