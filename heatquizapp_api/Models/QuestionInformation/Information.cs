using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;
using heatquizapp_api.Models.Questions;

namespace heatquizapp_api.Models.QuestionInformation
{
    public class Information : IAddedByCarrier
    {
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public DataPool DataPool { get; set; }
        public int DataPoolId { get; set; }

        public User AddedBy { get; set; }
        public string AddedById { get; set; }

        public string Code { get; set; }

        public string? Latex { get; set; }

        public string? PDFURL { get; set; }
        public long? PDFSize { get; set; }

        public List<QuestionBase> Questions = new List<QuestionBase>();

    }
}
