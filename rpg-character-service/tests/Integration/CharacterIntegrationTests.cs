using System.Net;
using System.Net.Http.Json;
using RPGCharacterService.Dtos.Character;
using RPGCharacterService.Dtos.Currency.Requests;
using RPGCharacterService.Dtos.Currency.Responses;
using RPGCharacterService.Dtos.Dice;
using RPGCharacterService.Dtos.Equipment;
using RPGCharacterService.Dtos.Stats.Responses;
using RPGCharacterService.Entities;
using RPGCharacterService.Entities.Characters;

namespace RPGCharacterService.IntegrationTests;

/// <summary>
/// Integration tests for the RPG Character Service.
/// These tests require a running instance of the service.
/// Set the TEST_API_ENDPOINT environment variable to specify the endpoint.
/// e.g. TEST_API_ENDPOINT=http://localhost:5266
/// </summary>
public class CharacterIntegrationTests : IDisposable {
  private readonly string baseUrl;
  private readonly HttpClient client;
  private Guid? testCharacterId;

  public CharacterIntegrationTests() {
    baseUrl = Environment.GetEnvironmentVariable("TEST_API_ENDPOINT") ?? "http://localhost:5266";
    client = new HttpClient {
      BaseAddress = new Uri(baseUrl)
    };
  }

  public void Dispose() {
    if (testCharacterId.HasValue) {
      try {
        // Use a separate HttpClient to avoid any issues with the disposed client
        using var cleanupClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        cleanupClient.DeleteAsync($"{baseUrl}/api/v1/characters/{testCharacterId}").Wait();
      } catch (Exception ex) {
        Console.WriteLine($"Failed to cleanup test character {testCharacterId}: {ex.Message}");
      }
    }
    client.Dispose();
  }

  [Fact]
  public async Task GetCharacters_ReturnsSuccessStatusCode() {
    try {
      // Arrange & Act
      var response = await client.GetAsync($"{baseUrl}/api/v1/characters");

      // Assert
      await response.EnsureSuccessStatusCodeWithLogging();
      response
        .StatusCode
        .Should()
        .Be(HttpStatusCode.OK);
    } catch (Exception ex) {
      throw new Exception($"Test failed. Make sure the service is running at {baseUrl}. Error: {ex.Message}", ex);
    }
  }

  [Fact]
  public async Task CharacterLifecycle_ShouldWorkCorrectly() {
    // Create a character With a random 15 character name
    var random = new Random();
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    var name = new string(Enumerable
                          .Repeat(chars, 15)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
    var createRequest = new CreateCharacterRequest {
      Name = name,
      Race = "Human",
      Class = "Fighter",
    };

    var createResponse = await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters", createRequest);
    await createResponse.EnsureSuccessStatusCodeWithLogging();
    var createdCharacter = await createResponse.Content.ReadFromJsonAsync<CharacterResponse>();
    testCharacterId = createdCharacter!.Id;

    // Try to modify currency (should error)
    var modifyCurrencyRequest = new ModifyCurrencyRequest {
      Copper = 10,
      Silver = 5,
      Gold = 2,
      Platinum = 1
    };

    var modifyCurrencyResponse =
      await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/currency", modifyCurrencyRequest);
    modifyCurrencyResponse
      .StatusCode
      .Should()
      .Be(HttpStatusCode.Conflict);

    // Try to exchange currency (should error)
    var exchangeCurrencyRequest = new ExchangeCurrencyRequest {
      From = CurrencyType.Gold,
      To = CurrencyType.Silver,
      Amount = 1
    };

    var exchangeCurrencyResponse =
      await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/currency/exchange",
                                    exchangeCurrencyRequest);
    exchangeCurrencyResponse
      .StatusCode
      .Should()
      .Be(HttpStatusCode.Conflict);

    // Initialize currency. Empty body
    var initCurrencyResponse =
      await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/currency/init", new object());
    initCurrencyResponse.EnsureSuccessStatusCode();

    // Verify we have some currency and store it for later verifications after modify / exchange
    var initialCurrency = await initCurrencyResponse.Content.ReadFromJsonAsync<CurrencyResponse>();
    initialCurrency.Should().NotBeNull();
    initialCurrency!.Copper.Should().BeGreaterThan(0);
    initialCurrency.Silver.Should().BeGreaterThan(0);
    initialCurrency.Gold.Should().BeGreaterThan(0);
    initialCurrency.Electrum.Should().Be(0);
    initialCurrency.Platinum.Should().Be(0);

    // Modify currency
    var modifyResponse =
      await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/currency", modifyCurrencyRequest);
    modifyResponse.EnsureSuccessStatusCode();

    // Verify state after currency modification
    var modifiedCurrency = await modifyResponse.Content.ReadFromJsonAsync<CurrencyResponse>();
    modifiedCurrency.Should().NotBeNull();
    modifiedCurrency!.Copper.Should().Be(initialCurrency.Copper + modifyCurrencyRequest.Copper);
    modifiedCurrency.Silver.Should().Be(initialCurrency.Silver + modifyCurrencyRequest.Silver);
    modifiedCurrency.Gold.Should().Be(initialCurrency.Gold + modifyCurrencyRequest.Gold);
    modifiedCurrency.Platinum.Should().Be(initialCurrency.Platinum + modifyCurrencyRequest.Platinum);

    // Exchange currency
    var exchangeResponse = await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/currency/exchange",
                                                         exchangeCurrencyRequest);
    exchangeResponse.EnsureSuccessStatusCode();

    // Verify state after currency exchange
    var exchangedCurrency = await exchangeResponse.Content.ReadFromJsonAsync<CurrencyResponse>();
    exchangedCurrency.Should().NotBeNull();
    // Gold to Silver exchange rate is 1:10
    exchangedCurrency!.Gold.Should().Be(modifiedCurrency.Gold - exchangeCurrencyRequest.Amount);
    exchangedCurrency.Silver.Should().Be(modifiedCurrency.Silver + (exchangeCurrencyRequest.Amount * 10));

    // Get character and verify stats
    var getCharacterResponse =
      await client.GetFromJsonAsync<CharacterResponse>($"{baseUrl}/api/v1/characters/{testCharacterId}");
    getCharacterResponse
      .Should()
      .NotBeNull();
    getCharacterResponse!
      .Name
      .Should()
      .Be(createRequest.Name);
    getCharacterResponse
      .Race
      .Should()
      .Be(createRequest.Race);
    getCharacterResponse
      .Class
      .Should()
      .Be(createRequest.Class);
    getCharacterResponse
      .Level
      .Should()
      .Be(1);
    getCharacterResponse
      .Wealth
      .Should()
      .NotBeNull();
    // Verify we have ability scores assigned, and store them:
    getCharacterResponse
      .AbilityScores
      .Should()
      .NotBeNull();
    getCharacterResponse
      .AbilityScores
      .Should()
      .ContainKey(AbilityScore.Strength)
      .And
      .ContainKey(AbilityScore.Dexterity)
      .And
      .ContainKey(AbilityScore.Constitution)
      .And
      .ContainKey(AbilityScore.Intelligence)
      .And
      .ContainKey(AbilityScore.Wisdom)
      .And
      .ContainKey(AbilityScore.Charisma);

    // Verify Max Hit Points and Hitpoints
    // This is the kind of logic that makes integration tests harder to maintain (they are still worth having)
    var constitution = getCharacterResponse.AbilityScores[AbilityScore.Constitution];
    var constitutionModifier = (int) Math.Floor((decimal) (constitution - 10)) / 2;
    var expectedMaxHitPoints = 10 + constitutionModifier * getCharacterResponse.Level;
    getCharacterResponse
      .MaxHitPoints
      .Should()
      .Be(expectedMaxHitPoints);
    getCharacterResponse
      .HitPoints
      .Should()
      .Be(expectedMaxHitPoints);

    // Modify hit points
    var updateHitPointsRequest = new {Delta = -5};
    var expectedHitPointsAfter = Math.Max(0, expectedMaxHitPoints - 5);
    var updateHitPointsResponse =
      await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/stats/hitpoints", updateHitPointsRequest);
    updateHitPointsResponse.EnsureSuccessStatusCode();

    // Read response and verify that we have the proper hit points
    var hitPointUpdateResult = await updateHitPointsResponse.Content.ReadFromJsonAsync<HitPointUpdateResponse>();
    hitPointUpdateResult
      .Should()
      .NotBeNull();
    hitPointUpdateResult!
      .HitPoints
      .Should()
      .Be(expectedHitPointsAfter);

    // Roll dice a few times
    for (var i = 0; i < 3; i++) {
      var rollResponse = await client.GetAsync($"{baseUrl}/api/v1/dice/roll?sides=4&count=2");
      rollResponse.EnsureSuccessStatusCode();
      var rollResult = await rollResponse.Content.ReadFromJsonAsync<RollDiceResponse>();
      rollResult
        .Should()
        .NotBeNull();
      rollResult!
        .Results
        .Sum()
        .Should()
        .BeGreaterThanOrEqualTo(2)
        .And
        .BeLessThanOrEqualTo(8);
    }

    // Add Main Hand
    var mainHandId = 22; // Rapier - Finesse
    var offHandId = 4;
    var equipMainHandRequest = new EquipWeaponRequest {
      OffHand = false
    };
    var addMainHandResponse =
      await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/equipment/weapon/{mainHandId}",
                                    equipMainHandRequest);
    addMainHandResponse.EnsureSuccessStatusCode();

    var equipOffHandRequest = new EquipWeaponRequest {
      OffHand = true
    };
    var equipOffHandResponse =
      await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/equipment/weapon/{offHandId}",
                                    equipOffHandRequest);
    equipOffHandResponse.EnsureSuccessStatusCode();

    // Read response and verify that we have the proper main / offhand equipped
    var equipmentResult = await equipOffHandResponse.Content.ReadFromJsonAsync<EquipmentResponse>();
    equipmentResult!
      .Equipment
      .MainHandId
      .Should()
      .Be(mainHandId);
    equipmentResult!
      .Equipment
      .OffHandId
      .Should()
      .Be(offHandId);

    var shieldId = 50;
    var equipShieldResponse =
      await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/equipment/shield/{shieldId}",
                                    new object());
    equipShieldResponse.EnsureSuccessStatusCode();

    // Read response and verify we have a shield
    var equipShieldResult = await equipShieldResponse.Content.ReadFromJsonAsync<EquipmentResponse>();
    equipShieldResult!
      .Equipment
      .MainHandId
      .Should()
      .Be(mainHandId);
    equipShieldResult!
      .Equipment
      .OffHandId
      .Should()
      .Be(shieldId);

    var armorId = 40;
    var equipArmorResponse =
      await client.PatchAsJsonAsync($"{baseUrl}/api/v1/characters/{testCharacterId}/equipment/armor/{armorId}",
                                    new object());
    equipArmorResponse.EnsureSuccessStatusCode();

    // Read response and verify we have armor
    var equipArmorResult = await equipArmorResponse.Content.ReadFromJsonAsync<EquipmentResponse>();
    equipArmorResult!
      .Equipment
      .MainHandId
      .Should()
      .Be(mainHandId);
    equipArmorResult!
      .Equipment
      .OffHandId
      .Should()
      .Be(shieldId);
    equipArmorResult!
      .Equipment
      .ArmorId
      .Should()
      .Be(armorId);

    // View character and verify equipment
    var characterWithEquipment =
      await client.GetFromJsonAsync<CharacterResponse>($"{baseUrl}/api/v1/characters/{testCharacterId}");
    characterWithEquipment
      .Should()
      .NotBeNull();
    characterWithEquipment!
      .Equipment
      .Should()
      .NotBeNull();
    characterWithEquipment
      .Equipment
      .MainHandId
      .Should()
      .Be(mainHandId);
    characterWithEquipment
      .Equipment
      .OffHandId
      .Should()
      .Be(shieldId);
    characterWithEquipment
      .Equipment
      .ArmorId
      .Should()
      .Be(armorId);

    // Base AC from studded leather (12) + shield (2) + Dex modifier
    var dexterityModifier = characterWithEquipment.AbilityModifiers[AbilityScore.Dexterity];
    characterWithEquipment.ArmorClass.Should().Be(12 + 2 + dexterityModifier);

    // Verify weapon damage modifier
    var strengthModifier = characterWithEquipment.AbilityModifiers[AbilityScore.Strength];
    // Rapier is a finesse weapon, so it should use the higher of Strength or Dexterity
    var expectedWeaponModifier = Math.Max(strengthModifier, dexterityModifier);
    if (strengthModifier > dexterityModifier) {
      characterWithEquipment
        .WeaponDamageModifier
        .Should()
        .Be(AbilityScore.Strength);
    }
    else {
      characterWithEquipment
        .WeaponDamageModifier
        .Should()
        .Be(AbilityScore.Dexterity);
    }

    // Delete character
    var deleteResponse = await client.DeleteAsync($"{baseUrl}/api/v1/characters/{testCharacterId}");
    deleteResponse.EnsureSuccessStatusCode();

    // Get character should return error
    var getDeletedResponse = await client.GetAsync($"{baseUrl}/api/v1/characters/{testCharacterId}");
    getDeletedResponse
      .StatusCode
      .Should()
      .Be(HttpStatusCode.NotFound);

    // List characters should not contain deleted character
    var finalCharacters = await client.GetFromJsonAsync<List<CharacterResponse>>($"{baseUrl}/api/v1/characters");
    finalCharacters
      .Should()
      .NotContain(c => c.Id == testCharacterId);
  }
}
