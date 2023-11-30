using AutoMapper;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Models.LevelsOfDifficulty;
using heatquizapp_api.Models.Auxillary;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.ClickImageTrees;
using heatquizapp_api.Models.Courses;
using heatquizapp_api.Models.InterpretedTrees;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Series;
using heatquizapp_api.Models.Topics;

namespace HeatQuizAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public const string FILES_PATH = "http://localhost:5000/Files/";//

        private void mapImage<S, SVM>(IMemberConfigurationExpression<S, SVM, string> opt) where S : IImageCarrier
        {
            opt.MapFrom(c => c.ImageURL != null ? $"{FILES_PATH}/{c.ImageURL}" : "");
        }

        private void mapPDF<S, SVM>(IMemberConfigurationExpression<S, SVM, string?> opt) where S : IPDFCarrier
        {
            opt.MapFrom(c => c.PDFURL != null ? $"{FILES_PATH}/{c.PDFURL}" : "");
        }

        private void mapUser<S,SVM>(IMemberConfigurationExpression<S, SVM, string> opt) where S : IAddedByCarrier
        {
            opt.MapFrom(c => c.AddedBy.Name);
        }

        public MappingProfile() {


            //Base entity
            CreateMap<BaseEntity, BaseEntityViewModel>();

            //Datapools
            CreateMap<DataPool, DataPoolViewModel>();
            CreateMap<DataPool, DataPoolViewModelAdmin>();

            CreateMap<DataPoolAccess, DataPoolAccessViewModel>();

            //Level of difficulty
            CreateMap<LevelOfDifficulty, LevelOfDifficultyViewModel>();

            //Courses
            CreateMap<Course, CourseViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt))
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            //Series
            CreateMap<QuestionSeries, QuestionSeriesViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            //Questions
            CreateMap<QuestionBase, QuestionBaseViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt))
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt))
                .ForMember(vm => vm.PDFURL, opt => mapPDF(opt));

            //Topics
            CreateMap<Topic, TopicViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            CreateMap<Subtopic, SubtopicViewModel>();

            //Trees
            CreateMap<ImageAnswerGroup, ImageAnswerGroupViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            CreateMap<ImageAnswer, ImageAnswerViewModel>()
               .ForMember(vm => vm.ImageURL, opt => mapImage(opt));

            CreateMap<InterpretedImageGroup, InterpretedImageGroupViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            CreateMap<InterpretedImage, InterpretedImageViewModel>()
               .ForMember(vm => vm.ImageURL, opt => mapImage(opt));

            CreateMap<LeftGradientValue, InterpretationValueViewModel>();
            CreateMap<RightGradientValue, InterpretationValueViewModel>();
            CreateMap<RationOfGradientsValue, InterpretationValueViewModel>();
            CreateMap<JumpValue, InterpretationValueViewModel>();

            //Keyboard

        }
    }
}
