using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.LevelsOfDifficulty
{
    public class AddEditLevelOfDifficultyViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string HexColor { get; set; }
    }
}
