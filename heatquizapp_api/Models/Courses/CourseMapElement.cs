using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Auxillary;
using heatquizapp_api.Models.Series;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapElement : BaseEntity, IPDFCarrier 
    {
        public CourseMap Map { get; set; }
        public int MapId { get; set; }

        public string Title { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Length { get; set; }

        //Badge
        public int BadgeX { get; set; }
        public int BadgeY { get; set; }

        public int BadgeWidth { get; set; }
        public int BadgeLength { get; set; }

        //Link
        public string? ExternalVideoLink { get; set; }

        //Map attachment
        public MapElementLink MapAttachment { get; set; }
        public int? MapAttachmentId { get; set; }

        //PDF
        public string? PDFURL { get; set; }
        public long PDFSize { get; set; }

        //PDF click statistics
        public List<CourseMapPDFStatistics> PDFStatistics { get; set; } = new List<CourseMapPDFStatistics>();

        //Series
        public QuestionSeries QuestionSeries { get; set; }
        public int? QuestionSeriesId { get; set; }

        //Required element
        public CourseMapElement RequiredElement { get; set; }
        public int? RequiredElementId { get; set; }

        public int Threshold { get; set; }

        //Badges
        public List<CourseMapElementBadge> Badges { get; set; } = new List<CourseMapElementBadge>();
        
        //Pop up icons
        public CourseMapElementImages.CourseMapElementImages CourseMapElementImages { get; set; }
        public int? CourseMapElementImagesId { get; set; }        
    }
}
