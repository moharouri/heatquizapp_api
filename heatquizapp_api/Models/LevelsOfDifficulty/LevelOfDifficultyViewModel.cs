using System.ComponentModel.DataAnnotations;

namespace HeatQuizAPI.Models.LevelsOfDifficulty
{
    public class LevelOfDifficultyViewModel 
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string HexColor { get; set; }
    }
}
