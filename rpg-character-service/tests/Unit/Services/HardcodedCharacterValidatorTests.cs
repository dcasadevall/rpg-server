using RPGCharacterService.Exceptions.Character;
using RPGCharacterService.Services;

namespace RPGCharacterService.Tests.Unit.Services {
    public class HardcodedCharacterValidatorTests {
        private readonly HardcodedCharacterValidator _validator;

        public HardcodedCharacterValidatorTests() {
            _validator = new HardcodedCharacterValidator();
        }

        [Fact]
        public void ValidateCharacterDetails_WithValidDetails_DoesNotThrow() {
            // Arrange
            var name = "Legolas";
            var race = "Elf";
            var subrace = "Wood";
            var characterClass = "Ranger";

            // Act & Assert
            var exception = Record.Exception(() => _validator.ValidateCharacterDetails(name, race, subrace, characterClass));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateName_WithEmptyName_ThrowsInvalidCharacterNameException(string name) {
            // Act & Assert
            Assert.Throws<InvalidCharacterNameException>(() => _validator.ValidateCharacterDetails(name, "Human", null, "Fighter"));
        }

        [Theory]
        [InlineData("A")]
        [InlineData("AB")]
        [InlineData("ThisNameIsWayTooLongForTheValidator")]
        public void ValidateName_WithInvalidLength_ThrowsInvalidCharacterNameException(string name) {
            // Act & Assert
            Assert.Throws<InvalidCharacterNameException>(() => _validator.ValidateCharacterDetails(name, "Human", null, "Fighter"));
        }

        [Theory]
        [InlineData("Legolas1")]
        [InlineData("Gandalf!")]
        [InlineData("Aragorn ")]
        public void ValidateName_WithNonLetterCharacters_ThrowsInvalidCharacterNameException(string name) {
            // Act & Assert
            Assert.Throws<InvalidCharacterNameException>(() => _validator.ValidateCharacterDetails(name, "Human", null, "Fighter"));
        }

        [Theory]
        [InlineData("admin")]
        [InlineData("MODERATOR")]
        [InlineData("GameMaster")]
        public void ValidateName_WithInappropriateName_ThrowsInappropriateNameException(string name) {
            // Act & Assert
            Assert.Throws<InappropriateNameException>(() => _validator.ValidateCharacterDetails(name, "Human", null, "Fighter"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("InvalidRace")]
        public void ValidateRace_WithInvalidRace_ThrowsInvalidRaceException(string race) {
            // Act & Assert
            Assert.Throws<InvalidRaceException>(() => _validator.ValidateCharacterDetails("Legolas", race, null, "Fighter"));
        }

        [Theory]
        [InlineData("Dwarf", "InvalidSubrace")]
        [InlineData("Elf", "InvalidSubrace")]
        [InlineData("Human", "InvalidSubrace")] // Human doesn't have subraces
        public void ValidateSubrace_WithInvalidSubrace_ThrowsInvalidRaceException(string race, string subrace) {
            // Act & Assert
            Assert.Throws<InvalidRaceException>(() => _validator.ValidateCharacterDetails("Legolas", race, subrace, "Fighter"));
        }

        [Theory]
        [InlineData("Dwarf", "Hill")]
        [InlineData("Elf", "High")]
        [InlineData("Halfling", "Lightfoot")]
        public void ValidateSubrace_WithValidSubrace_DoesNotThrow(string race, string subrace) {
            // Act & Assert
            var exception = Record.Exception(() => _validator.ValidateCharacterDetails("Legolas", race, subrace, "Fighter"));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("InvalidClass")]
        public void ValidateClass_WithInvalidClass_ThrowsInvalidCharacterClassException(string characterClass) {
            // Act & Assert
            Assert.Throws<InvalidCharacterClassException>(() => _validator.ValidateCharacterDetails("Legolas", "Human", null, characterClass));
        }

        [Theory]
        [InlineData("Fighter")]
        [InlineData("Wizard")]
        [InlineData("Rogue")]
        public void ValidateClass_WithValidClass_DoesNotThrow(string characterClass) {
            // Act & Assert
            var exception = Record.Exception(() => _validator.ValidateCharacterDetails("Legolas", "Human", null, characterClass));
            Assert.Null(exception);
        }
    }
}
