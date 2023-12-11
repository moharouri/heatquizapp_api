using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Questions;

namespace HeatQuizAPI.Models.LevelsOfDifficulty
{
    public class LevelOfDifficulty 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string HexColor { get; set; }

        //Relationships
        public List<QuestionBase> Questions { get; set; } = new List<QuestionBase>();
    }
}
