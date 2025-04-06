using RPGCharacterService.Models;

namespace RPGCharacterService.Services
{
    public interface IDiceService
    {
        int[] Roll(DiceSides sides, int count);
    }
    
    public class DiceService : IDiceService
    {
        private readonly Random random = new();

        public int[] Roll(DiceSides sides, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than 1.");
            }

            return Enumerable.Range(0, count)
                             .Select(_ => random.Next(1, (int)sides + 1))
                             .ToArray();
        }
    }
} 
