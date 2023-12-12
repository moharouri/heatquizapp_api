using HeatQuizAPI.Models.BaseModels;

namespace heatquizapp_api.Models.QuestionInformation
{
    public class InformationViewModel 
    {
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public int DataPoolId { get; set; }

        public string AddedByName { get; set; }

        public string Code { get; set; }

        public string? Latex { get; set; }

        public string? PDFURL { get; set; }
        public long? PDFSize { get; set; }

    }
}
