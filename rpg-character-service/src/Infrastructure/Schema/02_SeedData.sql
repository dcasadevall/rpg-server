-- Seed data for Items, ArmorStats, and WeaponStats

-- Insert weapons
INSERT INTO Items (Name, EquipmentType) VALUES
    ('Longsword', 1),
    ('Shortsword', 1),
    ('Greatsword', 1),
    ('Rapier', 1),
    ('Dagger', 1),
    ('Longbow', 1),
    ('Shortbow', 1),
    ('Crossbow', 1),
    ('Hand Crossbow', 1),
    ('Light Hammer', 1),
    ('Warhammer', 1),
    ('Battleaxe', 1),
    ('Greataxe', 1),
    ('Halberd', 1),
    ('Spear', 1),
    ('Javelin', 1),
    ('Trident', 1),
    ('Whip', 1),
    ('Sling', 1),
    ('Dart', 1);

-- Insert armor
INSERT INTO Items (Name, EquipmentType) VALUES
    ('Padded Armor', 0),
    ('Leather Armor', 0),
    ('Studded Leather Armor', 0),
    ('Hide Armor', 0),
    ('Chain Shirt', 0),
    ('Scale Mail', 0),
    ('Breastplate', 0),
    ('Half Plate', 0),
    ('Ring Mail', 0),
    ('Chain Mail', 0),
    ('Splint Mail', 0),
    ('Plate Mail', 0);

-- Insert shields
INSERT INTO Items (Name, EquipmentType) VALUES
    ('Shield', 2),
    ('Buckler', 2),
    ('Tower Shield', 2),
    ('Small Shield', 2);

-- Insert weapon stats
INSERT INTO WeaponStats (ItemId, WeaponProperties, RangeType) VALUES
(1, 8, 0),  -- Longsword: Versatile, Melee
(2, 1, 0),  -- Shortsword: Light, Melee
(3, 132, 1), -- Longbow: Ammunition, Loading, Ranged
(4, 33, 0),  -- Dagger: Light, Finesse, Melee
(5, 6, 0);   -- Greatsword: Heavy, TwoHanded, Melee

-- Insert armor stats
INSERT INTO ArmorStats (ItemId, BaseArmorClass, ArmorType) VALUES
(6, 11, 1), -- Leather Armor: AC 11, Light
(7, 13, 2), -- Chain Shirt: AC 13, Medium
(8, 18, 3); -- Plate Armor: AC 18, Heavy

-- Reset sequence after direct id inserts
SELECT setval('items_id_seq', (SELECT MAX(id) FROM Items), true);
