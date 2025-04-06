namespace RPGCharacterService.Services
{
    public interface IDiceService
    {
        int[] Roll(int sides, int count);
    }
    
    public class DiceService : IDiceService
    {
        private readonly Random random = new();

        public int[] Roll(int sides, int count)
        {
            if (!IsValidDiceType(sides))
                throw new ArgumentException($"Invalid dice type: d{sides}. Valid types are d4, d6, d8, d10, d12, and d20.");
                
            if (count <= 0 || count > 100)
                throw new ArgumentException("Count must be between 1 and 100.");
                
            return Enumerable.Range(0, count)
                .Select(_ => random.Next(1, sides + 1))
                .ToArray();
        }
        
        private bool IsValidDiceType(int sides)
        {
            return sides == 4 || sides == 6 || sides == 8 || sides == 10 || sides == 12 || sides == 20;
        }
    }
} 
