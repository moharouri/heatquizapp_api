using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Models.LevelsOfDifficulty;
using heatquizapp_api.Models.QuestionInformation;
using heatquizapp_api.Models.Topics;
using System.ComponentModel.DataAnnotations;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionBaseViewModel : BaseEntityViewModel
    {
        public int Type { get; set; }

        public string Code { get; set; }

        //Added By
        public string AddedByName { get; set; }

        //Level of difficulty
        public LevelOfDifficultyViewModel LevelOfDifficulty { get; set; }
        public int LevelOfDifficultyId { get; set; }

        //Subtopic
        public SubtopicViewModel Subtopic { get; set; }
        public int SubtopicId { get; set; }

        //Image
        public string ImageURL { get; set; }
        public long ImageSize { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        //Latex
        public string? Latex { get; set; }

        //PDF Solution
        public string? PDFURL { get; set; }
        public long? PDFSize { get; set; }

        //Question explanation
        public InformationViewModel Information { get; set; }
        public int? InformationId { get; set; }

    }
}
