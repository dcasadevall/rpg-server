using System.Net;
using System.Net.Http.Json;
using RPGCharacterService.Dtos.Character;
using RPGCharacterService.Dtos.Currency.Requests;
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
public class CharacterIntegrationTests {
  private readonly string baseUrl;
  private readonly HttpClient client;

  public CharacterIntegrationTests() {
    baseUrl = Environment.GetEnvironmentVariable("TEST_API_ENDPOINT") ?? "http://localhost:5266";
    client = new HttpClient {
      BaseAddress = new Uri(baseUrl)
    };
  }

  [Fact]
  public async Task GetCharacters_ReturnsSuccessStatusCode() {
    try {
      // Arrange & Act
      var response = await client.GetAsync($"{baseUrl}/api/v1/characters");

      // Assert
      response.EnsureSuccessStatusCode();
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
    // List characters (empty)
    var characters = await client.GetFromJsonAsync<List<CharacterResponse>>($"{baseUrl}/api/v1/characters");
    characters
      .Should()
      .BeEmpty();

    // Create a character
    var createRequest = new CreateCharacterRequest {
      Name = "Test Character",
      Race = "Human",
      Class = "Fighter",
    };

    var createResponse = await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters", createRequest);
    createResponse.EnsureSuccessStatusCode();
    var createdCharacter = await createResponse.Content.ReadFromJsonAsync<CharacterResponse>();
    var characterId = createdCharacter!.Id;

    // Try to modify currency (should error)
    var modifyCurrencyRequest = new ModifyCurrencyRequest {
      Copper = 10,
      Silver = 5,
      Gold = 2,
      Platinum = 1
    };

    var modifyCurrencyResponse =
      await client.PutAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/currency", modifyCurrencyRequest);
    modifyCurrencyResponse
      .StatusCode
      .Should()
      .Be(HttpStatusCode.BadRequest);

    // Try to exchange currency (should error)
    var exchangeCurrencyRequest = new ExchangeCurrencyRequest {
      From = CurrencyType.Gold,
      To = CurrencyType.Silver,
      Amount = 1
    };

    var exchangeCurrencyResponse =
      await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/currency/exchange",
                                   exchangeCurrencyRequest);
    exchangeCurrencyResponse
      .StatusCode
      .Should()
      .Be(HttpStatusCode.BadRequest);

    // Initialize currency. Empty body
    var initCurrencyResponse =
      await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/currency/initialize", new object());
    initCurrencyResponse.EnsureSuccessStatusCode();

    // TODO: Verify we have some currency and store it for later verifications after modify / exchange

    // Modify currency
    var modifyResponse =
      await client.PutAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/currency", modifyCurrencyRequest);
    modifyResponse.EnsureSuccessStatusCode();

    // TODO: Verify state

    // Exchange currency
    var exchangeResponse = await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/currency/exchange",
                                                        exchangeCurrencyRequest);
    exchangeResponse.EnsureSuccessStatusCode();

    // TODO: Verify state

    // Get character and verify stats
    var getCharacterResponse =
      await client.GetFromJsonAsync<CharacterResponse>($"{baseUrl}/api/v1/characters/{characterId}");
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
    var updateHitPointsRequest = new {HitPoints = -5};
    var expectedHitPointsAfter = Math.Max(0, expectedMaxHitPoints - 5);
    var updateHitPointsResponse =
      await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/hit-points", updateHitPointsRequest);
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
    var mainHandId = 2;
    var offHandId = 4;
    var equipMainHandRequest = new EquipWeaponRequest {
      OffHand = false
    };
    var addMainHandResponse =
      await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/equipment/weapon/{mainHandId}",
                                   equipMainHandRequest);
    addMainHandResponse.EnsureSuccessStatusCode();

    var equipOffHandRequest = new EquipWeaponRequest {
      OffHand = true
    };
    var equipOffHandResponse =
      await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/equipment/weapon/{offHandId}",
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
      await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/equipment/shield/{shieldId}",
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
      await client.PostAsJsonAsync($"{baseUrl}/api/v1/characters/{characterId}/equipment/armor/{armorId}",
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
      await client.GetFromJsonAsync<CharacterResponse>($"{baseUrl}/api/v1/characters/{characterId}");
    characterWithEquipment
      .Should()
      .NotBeNull();
    characterWithEquipment!
      .Equipment
      .Should()
      .NotBeNull();
    characterWithEquipment
      .Equipment
      .MainHand
      .Should()
      .Be(mainHandId);
    characterWithEquipment
      .Equipment
      .OffHand
      .Should()
      .Be(shieldId);
    characterWithEquipment
      .Equipment
      .Armor
      .Should()
      .Be(armorId);
    // TODO: Verify armor class and weapon damage bonus

    // Delete character
    var deleteResponse = await client.DeleteAsync($"{baseUrl}/api/v1/characters/{characterId}");
    deleteResponse.EnsureSuccessStatusCode();

    // Get character should return error
    var getDeletedResponse = await client.GetAsync($"{baseUrl}/api/v1/characters/{characterId}");
    getDeletedResponse
      .StatusCode
      .Should()
      .Be(HttpStatusCode.NotFound);

    // List characters should be empty
    var finalCharacters = await client.GetFromJsonAsync<List<CharacterResponse>>($"{baseUrl}/api/v1/characters");
    finalCharacters
      .Should()
      .BeEmpty();
  }
}
