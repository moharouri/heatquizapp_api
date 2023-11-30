using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Models.LevelsOfDifficulty;
using heatquizapp_api.Models.StatisticsAndStudentFeedback;
using heatquizapp_api.Models.Topics;
using heatquizapp_api.Models.QuestionInformation;
using heatquizapp_api.Models.Auxillary;

namespace heatquizapp_api.Models.Questions
{
    public class QuestionBase : BaseEntity, IImageCarrier, IAddedByCarrier, IPDFCarrier
    {
        public int Type { get; set; }

        public string Code { get; set; }

        //Added By
        public User AddedBy { get; set; }
        public string AddedById { get; set; }


        //Level of difficulty
        public LevelOfDifficulty LevelOfDifficulty { get; set; }
        public int LevelOfDifficultyId { get; set; }

        //Subtopic
        public Subtopic Subtopic { get; set; }
        public int SubtopicId { get; set; }

        //Image
        public string ImageURL { get; set; }
        public long ImageSize { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        //PDF Solution
        public string? PDFURL { get; set; }
        public long? PDFSize { get; set; }

        //Question explanation
        public Information Information { get; set; }
        public int? InformationId { get; set; }

        //Statistics
        public List<QuestionStatistic> QuestionStatistics { get; set; } = new List<QuestionStatistic>();
        public List<QuestionPDFStatistic> QuestionPDFStatistics { get; set; } = new List<QuestionPDFStatistic>();
        public List<QuestionStudentFeedback> StudentFeedback { get; set; } = new List<QuestionStudentFeedback>();


    }
}
