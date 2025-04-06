using System.ComponentModel.DataAnnotations;

namespace RPGCharacterService.Dtos.Character.Requests 
{
    public class CreateCharacterRequest
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Race { get; set; }
        
        [StringLength(50)]
        public string? Subrace { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Class { get; set; }
    }
}
