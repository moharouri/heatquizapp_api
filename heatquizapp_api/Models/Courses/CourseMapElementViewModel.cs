using HeatQuizAPI.Models.BaseModels;
using heatquizapp_api.Models.Series;

namespace heatquizapp_api.Models.Courses
{
    public class CourseMapElementViewModel : BaseEntityViewModel
    {
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
        public string ExternalVideoLink { get; set; }

        //Map attachment
        public MapElementLinkViewModel MapAttachment { get; set; }
        public int? MapAttachmentId { get; set; }

        //PDF
        public string PDFURL { get; set; }
        public long PDFSize { get; set; }

        //Series
        public QuestionSeriesViewModel QuestionSeries { get; set; }
        public int? QuestionSeriesId { get; set; }

        //Required element
        public CourseMapElementViewModel RequiredElement { get; set; }
        public int? RequiredElementId { get; set; }

        public int Threshold { get; set; }

        //Badges
        public List<CourseMapElementBadgeViewModel> Badges { get; set; } = new List<CourseMapElementBadgeViewModel>();

        //Pop up icons
        public CourseMapElementImages.CourseMapElementImagesViewModel CourseMapElementImages { get; set; }
        public int? CourseMapElementImagesId { get; set; }
    }
}
