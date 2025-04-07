using System.ComponentModel.DataAnnotations;

namespace RPGCharacterService.Dtos.Character.Requests 
{
    public record CreateCharacterRequest
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string Name { get; init; }
        
        [Required]
        [StringLength(50)]
        public string Race { get; init; }
        
        [StringLength(50)]
        public string? Subrace { get; init; }
        
        [Required]
        [StringLength(50)]
        public string Class { get; init; }
    }
}
