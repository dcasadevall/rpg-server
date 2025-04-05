# RPG Character Management API Documentation

This document describes the **RPG Character Management API**, a RESTful service built using ASP.NET Core.

The API provides endpoints for managing player characters within a Dungeons & Dragons (DnD) inspired role-playing game system.

---

## Base URL

```
/api/v1
```

---

## Authentication

All requests to the API require a valid Authentication Token (e.g., JWT Bearer token).

Clients must include the token in the Authorization HTTP header:

`Authorization: Bearer <token>`

The server will extract the userId from the token and validate that the requested characterId belongs to the authenticated player.
Unauthorized or invalid access attempts will receive a 403 Forbidden response.

---

## Endpoints

> Note: All {id} path parameters in the character API refer to a UUID (Universally Unique Identifier) in RFC 4122 format.
> Example: 123e4567-e89b-12d3-a456-426614174000
> All equipment IDs (armorId, weaponId) are int16

### Character creation

#### Create a New Character

- **POST** `/characters`

- **Request Body:**
```json
{
  "name": "Pykey",
  "race": "Elf",
  "subRace": "High",
  "class": "Wizard"
}
```

- **Validation Rules:**
  - `name` is **optional**:
    - If not provided, a random name will be generated.
    - If provided, must:
      - Be between 1 and 15 characters.
      - Contain only letters (A-Z, a-z).
      - Not match any inappropriate/offensive words.
      - Not already be taken by another character.
  - `race` is **required**. Must be one of the following:
    - `Dwarf`
    - `Elf`
    - `Halfling`
    - `Human`
    - `Dragonborn`
    - `Gnome`
    - `Half-Elf`
    - `Half-Orc`
    - `Tiefling`
  - `subRace` is **optional**. If provided, must match the selected `race`:
    - `Dwarf`: `Hill`, `Mountain`
    - `Elf`: `High`, `Wood`, `Drow`
    - `Halfling`: `Lightfoot`, `Stout`
    - `Human`: *(no subrace)*
    - `Dragonborn`: *(no subrace)*
    - `Gnome`: `Forest`, `Rock`, `Deep`
    - `Half-Elf`: *(no subrace)*
    - `Half-Orc`: *(no subrace)*
    - `Tiefling`: *(no subrace)*
  - `class` is **required**. Must be one of the following:
    - `Cleric`
    - `Fighter`
    - `Rogue`
    - `Wizard`
    - `Barbarian`
    - `Bard`
    - `Druid`
    - `Monk`
    - `Paladin`
    - `Ranger`
    - `Sorcerer`
    - `Warlock`
---
- **Behavior:**
    - Creates a new character with the provided attributes.
    - Assigns a unique identifier (GUID) to the character.
    - Initializes the character's level, hit points, proficiency bonus, randomized ability scores and armor class.
    - If `name` is not provided, a random name is generated.

---

- **Successful Response:**
  - `201 Created`
  - Location header: `/characters/{id}`

---

- **Error Responses:**

|      Status       | Error Code             | Description                                                           |
|:-----------------:|:-----------------------|:----------------------------------------------------------------------|
| `400 Bad Request` | `INVALID_NAME_FORMAT`  | Provided `name` contains invalid characters or exceeds 15 characters. |
| `400 Bad Request` | `NAME_INAPPROPRIATE`   | Provided `name` is flagged as inappropriate.                          |
|  `409 Conflict`   | `NAME_ALREADY_TAKEN`   | Provided `name` is already in use by another character.               |
| `400 Bad Request` | `INVALID_RACE`         | Provided `race` is invalid.                                           |
| `400 Bad Request` | `INVALID_SUBRACE`      | Provided `subRace` is invalid for the selected `race`.                |
| `400 Bad Request` | `INVALID_CLASS`        | Provided `class` is invalid.                                          |
| `400 Bad Request` | `SUBRACE_WITHOUT_RACE` | Provided a `subRace` without a matching `race`.                       |

- **Error Response Body:**
```json
{
  "error": "INVALID_RACE",
  "message": "Race must be one of: Dwarf, Elf, Halfling, Human, Dragonborn, Gnome, Half-Elf, Half-Orc, Tiefling."
}
```
#### Retrieve Character Information

- **GET** `/characters/{id}`
- **Successful Response:**
  - `200 OK`
  - Returns the character's details.
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Pykey",
  "race": "Elf",
  "subRace": "Drow",
  "class": "Wizard",
  "level": 1,
  "hitPoints": 8,
  "proficiencyBonus": 2,
  "abilityScores": {
    "strength": 10,
    "dexterity": 14,
    "constitution": 12,
    "intelligence": 16,
    "wisdom": 10,
    "charisma": 8
  },
  "equipment": {
    "armor": 1458,
    "mainHand": 1478,
    "offHand": 1459
  },
  "armorClass": 12,
  "currency": {
    "gold": 15,
    "silver": 48,
    "bronze": 92
  }
}
```


- **Error Responses:**

  |      Status       | Error Code            | Description                                  |
  |:-----------------:|:----------------------|:---------------------------------------------|
  |  `404 Not Found`  | `CHARACTER_NOT_FOUND` | No character exists with the specified `id`. |
  | `400 Bad Request` | `INVALID_ID_FORMAT`   | Provided `id` is not a valid GUID.           |

- **Error Response Body:**

```json
{
  "error": "CHARACTER_NOT_FOUND",
  "message": "Character not found."
}
```

#### Initialize a Character's Currency

- **POST** `/characters/{id}/initial-currency`

- **Behavior:**
  - Randomly sets the character's starting currency by rolling dice:
    - **Gold**: Roll `1d20`
    - **Silver**: Roll `3d20`
    - **Bronze**: Roll `5d20`
  - The resulting values are assigned to the character's `currency` fields.


- **Successful Response:**
  - `200 OK`
  - Returns updated currency values.

```json
{
  "currency": {
    "gold": 17,
    "silver": 42,
    "bronze": 86
  }
}
```

- **Error Responses:**

|      Status       | Error Code                     | Description                                        |
|:-----------------:|:-------------------------------|:---------------------------------------------------|
|  `404 Not Found`  | `CHARACTER_NOT_FOUND`          | No character exists with the specified `id`.       |
| `400 Bad Request` | `INVALID_ID_FORMAT`            | Provided `id` is not a valid GUID.                 |
|  `409 Conflict`   | `CURRENCY_ALREADY_INITIALIZED` | Character's currency has already been initialized. |

- **Error Response Body:**
```json
{
  "error": "CURRENCY_ALREADY_INITIALIZED",
  "message": "This character's currency has already been set."
}
```

#### Delete a Character

- **DELETE** `/characters/{id}`

- **Behavior:**
  - Deletes the character with the specified `id` from the database.
  - All associated data (e.g., inventory, currency) will also be deleted.


- **Successful Response:**
  - `204 No Content`


- **Error Responses:**

|      Status       | Error Code            | Description                                  |
|:-----------------:|:----------------------|:---------------------------------------------|
|  `404 Not Found`  | `CHARACTER_NOT_FOUND` | No character exists with the specified `id`. |
| `400 Bad Request` | `INVALID_ID_FORMAT`   | Provided `id` is not a valid GUID.           |

---

### Player Health and Inventory Management

#### Update Character Hit Points

- **PATCH** `/characters/{id}/hitpoints`
- **Request Body:**
```json
{
  "delta": 5
}
```
- **Validation Rules:**
  - `delta` is **required** and must be a positive or negative integer.

- **Behavior:**
  - Adds or subtracts the specified `delta` from the character's current hit points.
  - If the character's hit points exceed their maximum, they are set to the maximum value.
  - If the character's hit points would become negative, they are set to zero.
  - If the character's hit points drop to zero, they are considered unconscious.


- **Successful Response:**
  - `200 OK`
  - Returns the updated hit points.
```json
{
  "hitPoints": 10
}
```

- **Error Responses:**

|      Status       | Error Code            | Description                                  |
|:-----------------:|:----------------------|:---------------------------------------------|
|  `404 Not Found`  | `CHARACTER_NOT_FOUND` | No character exists with the specified `id`. |
| `400 Bad Request` | `INVALID_ID_FORMAT`   | Provided `id` is not a valid GUID.           |
| `400 Bad Request` | `INVALID_DELTA`       | Provided `delta` is not a valid integer.     |

#### Equip Armor to a Character

- **PATCH** `/characters/{id}/armor/{armorId}`
- **Validation Rules:**
  - `armorId` must be a valid identifier for an armor item.
  - For this exercise, we do not manage player inventory, so we assume the armor is already in the character's inventory.


- **Behavior:**
  - Equips the specified armor to the given character.
  - If the character already has armor equipped, it will be replaced with the new armor.
  - The character's armor class will be updated based on the new armor's properties.
  - If the character has a two-handed weapon equipped and the new armor is a shield, the two-handed weapon will be unequipped.
  - If the character is dual-wielding and the new armor is a shield, the off-hand weapon will be unequipped.

- **Successful Response:**
- `200 OK`
  - Returns the updated armor class and any other relevant equipment details.
    (such as attack modifier since equipping a shield may change it due to un-equipping a weapon)

```json
{
  "armorClass": 15,
  "attackModifier": "DEX",
  "equipment" : {
    "armor": 1456,
    "mainHand" : 12345,
    "offHand" : 12346
  }
}
```

- **Error Responses:**

|      Status       | Error Code            | Description                                   |
|:-----------------:|:----------------------|:----------------------------------------------|
|  `404 Not Found`  | `CHARACTER_NOT_FOUND` | No character exists with the specified `id`.  |
| `400 Bad Request` | `INVALID_ID_FORMAT`   | Provided `id` is not a valid GUID.            |
|  `404 Not Found`  | `ARMOR_NOT_FOUND`     | No armor exists with the specified `armorId`. |
| `400 Bad Request` | `INVALID_ARMOR_ID`    | Provided `armorId` is not valid.              |

- **Error Response Body:**
```json
{
  "error": "ARMOR_NOT_FOUND",
  "message": "Armor not found."
}
```

#### Equip Weapon to a Character

- **PATCH** `/characters/{id}/weapons/{weaponId}`
- **Request Body:**
```json
{
  "offHand": true
}
```
- **Validation Rules:**
  - `offHand` is **optional** and defaults to `false`. It allows the character to equip a weapon in the off-hand slot.
  - `weaponId` must be a valid identifier for a weapon item in database.
- **Behavior:**
  - Equips the specified weapon to the given character.
  - If the character already has a weapon equipped, it will be replaced with the new weapon, unless:
     - `offhand` is `true` and the character is only equipping a one-handed weapon in the main hand
     - `offhand` is `false` and the character is only equipping a one-handed weapon in the off-hand
  - If the character equips a two-handed weapon, any equipped main, off-hand weapon and shield will be unequipped.
  - The character's attack bonus and damage will be updated based on the new weapon's properties.
- **Successful Response:**
  - `200 Ok`
  - Returns the updated weapon details, including attack modifier and armor class.
    Armor class may change because the character may have unequipped a shield.

```json
{
  "attackModifier": "STR",
  "armorClass": 12,
  "equipment" : {
    "mainHand" : 12345,
    "offHand" : 12346
  }
}
```

- **Error Responses:**
|      Status       | Error Code            | Description                                   |
|:-----------------:|:----------------------|:----------------------------------------------|
|  `404 Not Found`  | `CHARACTER_NOT_FOUND` | No character exists with the specified `id`.  |
| `400 Bad Request` | `INVALID_ID_FORMAT`   | Provided `id` is not a valid GUID.            |
|  `404 Not Found`  | `WEAPON_NOT_FOUND`    | No weapon exists with the specified `weaponId`. |
| `400 Bad Request` | `INVALID_WEAPON_ID`   | Provided `weaponId` is not valid.              |

- **Error Response Body:**
```json
{
  "error": "WEAPON_NOT_FOUND",
  "message": "Weapon not found."
}
```

---

### Currency Management

#### Modify a Character's Currency

- **PATCH** `/characters/{id}/currency`
- **Request Body:**
```json
{
  "gold": 5,
  "silver": -10,
  "bronze": 0
}
```
- **Validation Rules:**
  - The following currency / value pairs are **optional**, but at least one must be provided:
    - `gold`
    - `silver`
    - `bronze`
    - `copper`
    - `electrum`
    - `platinum`
  - Each value must be a positive or negative integer.
- **Behavior:**
  - Adds or subtracts the specified values from the character's current currency.
  - If a specified currency value would become negative, it is set to zero.
  - If a specified currency value exceeds the maximum allowed, it is set to the maximum value.
    Initially, a maximum value of 9999999 is set for each currency type, but this should be configurable per
    application settings.

- **Response:**
  - `200 OK`
  - Returns the updated currency values.
```json
{
  "currency": {
    "gold": 20,
    "silver": 38,
    "bronze": 92,
    "copper": 5
  }
}
```

- **Error Responses:**

|      Status       | Error Code               | Description                                  |
|:-----------------:|:-------------------------|:---------------------------------------------|
|  `404 Not Found`  | `CHARACTER_NOT_FOUND`    | No character exists with the specified `id`. |
| `400 Bad Request` | `INVALID_ID_FORMAT`      | Provided `id` is not a valid GUID.           |
| `400 Bad Request` | `INVALID_CURRENCY`       | Provided currency values are not valid.      |

- **Error Response Body:**
```json
{
  "error": "INVALID_CURRENCY",
  "message": "Currency values must be integers."
}
```

#### Exchange Currency

- **PATCH** `/characters/{id}/currency/exchange`
- **Request Body:**
```json
{
  "from": "gold",
  "to": "silver",
  "amount": 1
}
```
- **Validation Rules:**
  - `from` and `to` are **required** and must be one of:
    - `gold`
    - `silver`
    - `bronze`
    - `copper`
    - `electrum`
    - `platinum`
  - `amount` is **required** and must be a positive integer.
  - The `from` and `to` currencies must be different.
  - The `amount` must not exceed the available balance of the `from` currency.

- **Behavior:**
  - Converts the specified `amount` of currency from `from` to `to`.
  - The following standard D&D 5e conversions apply:
    * 10 copper (cp) = 1 silver (sp)
    * 5 silver (sp) = 1 electrum (ep)
    * 2 electrum (ep) = 1 gold (gp)
    * 10 gold (gp) = 1 platinum (pp) 
    

- **Successful Response:**
  - `200 OK`
    - Returns the updated currency values.
```json
{
  "currency": {
    "gold": 19,
    "silver": 40,
    "bronze": 92,
    "copper": 5
  }
}
```

- **Error Responses:**

|      Status       | Error Code                    | Description                                  |
|:-----------------:|:------------------------------|:---------------------------------------------|
|  `404 Not Found`  | `CHARACTER_NOT_FOUND`         | No character exists with the specified `id`. |
| `400 Bad Request` | `INVALID_ID_FORMAT`           | Provided `id` is not a valid GUID.           |
| `400 Bad Request` | `INVALID_CURRENCY`            | Provided currencies are not valid.           |
| `400 Bad Request` | `INVALID_CURRENCY_VALUE`      | Provided currency values are not valid.      |
| `400 Bad Request` | `INVALID_CURRENCY_CONVERSION` | Cannot convert between the same currency.    |

- **Error Response Body:**
```json
{
  "error": "INVALID_CURRENCY_CONVERSION",
  "message": "Cannot convert between the same currency."
}
```