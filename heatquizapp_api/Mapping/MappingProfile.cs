using AutoMapper;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Models.LevelsOfDifficulty;
using heatquizapp_api.Models.Auxillary;
using heatquizapp_api.Models.BaseModels;
using heatquizapp_api.Models.ClickImageTrees;
using heatquizapp_api.Models.CourseMapElementImages;
using heatquizapp_api.Models.Courses;
using heatquizapp_api.Models.DefaultQuestionImages;
using heatquizapp_api.Models.InterpretedTrees;
using heatquizapp_api.Models.Keyboard;
using heatquizapp_api.Models.QuestionInformation;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Questions.KeyboardQuestion;
using heatquizapp_api.Models.Questions.MultipleChoiceQuestion;
using heatquizapp_api.Models.Questions.SimpleClickableQuestion;
using heatquizapp_api.Models.Series;
using heatquizapp_api.Models.StatisticsAndStudentFeedback;
using heatquizapp_api.Models.Topics;

namespace HeatQuizAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public const string FILES_PATH = "http://localhost:5000/Files/";//

        public static string GetGeneralImageURL<T>(T q) where T : IImageCarrier
        {
            return (q.ImageURL != null ? $"{FILES_PATH}/{q.ImageURL}" : "");
        }

        public static string GetQuestionPDFURL(QuestionBase q)
        {
            return (q.PDFURL != null ? $"{FILES_PATH}/{q.PDFURL}" : "");
        }

        public static string GetUserProfilePictureURL<S>(S u) where S : User
        {
            return u.ProfilePicture != null ? $"{FILES_PATH}/{u.ProfilePicture}" : "";
        }

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

        private void mapProfilePicture(IMemberConfigurationExpression<QuestionComment, QuestionCommentViewModel, string> opt)
        {
            opt.MapFrom(c => c.AddedBy.ProfilePicture != null ? $"{FILES_PATH}/{c.AddedBy.ProfilePicture}" : null);
        }

        private string? mapAnyProperty(string value)
        {
            return value != null ? $"{FILES_PATH}/{value}" : null;
        }

        public MappingProfile() {


            //Base entity
            CreateMap<BaseEntity, BaseEntityViewModel>();

            //Datapools
            CreateMap<DataPool, DataPoolViewModel>();
            CreateMap<DataPool, DataPoolViewModelAdmin>();

            CreateMap<DataPoolAccess, DataPoolAccessViewModel>(); 
            CreateMap<DatapoolNotificationSubscription, DatapoolNotificationSubscriptionViewModel>(); 

            //Level of difficulty
            CreateMap<LevelOfDifficulty, LevelOfDifficultyViewModel>();

            //Explanations 
            CreateMap<Information, InformationViewModel>()
                .ForMember(vm => vm.PDFURL, opt => mapPDF(opt))
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            //Courses
            CreateMap<Course, CourseViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt))
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            //Series
            CreateMap<QuestionSeries, QuestionSeriesViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            CreateMap<QuestionSeriesElement, QuestionSeriesElementViewModel>(); 
            CreateMap<QuestionSeriesStatistic, QuestionSeriesStatisticViewModel>(); 

            //Keyboard
            CreateMap<Keyboard, KeyboardViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            CreateMap<KeyboardNumericKey, KeyboardNumericKeyViewModel>();
            CreateMap < KeyboardNumericKeyRelation, KeyboardNumericKeyRelationViewModel > ();

            CreateMap<KeyboardVariableKey, KeyboardVariableKeyViewModel>();
            CreateMap<KeyboardVariableKeyVariation, KeyboardVariableKeyVariationViewModel>();
            CreateMap<KeyboardVariableKeyRelation, KeyboardVariableKeyRelationViewModel>(); 
            CreateMap<KeyboardVariableKeyImageRelation, KeyboardVariableKeyImageRelationViewModel>(); 

            CreateMap<KeysList, KeysListViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            //Questions
            CreateMap<QuestionBase, QuestionBaseViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt))
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt))
                .ForMember(vm => vm.PDFURL, opt => mapPDF(opt)); 

            CreateMap<QuestionBase, QuestionBaseCommentsViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt)); 

            CreateMap<QuestionCommentSection, QuestionCommentSectionViewModel>();

            CreateMap<QuestionComment, QuestionCommentViewModel>()
              .ForMember(vm => vm.AddedByName, opt => mapUser(opt))
              .ForMember(vm => vm.AddedByProfilePicture, opt => mapProfilePicture(opt));


            CreateMap<QuestionCommentSectionTag, QuestionCommentSectionTagViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt));

            CreateMap<QuestionCommentTag, QuestionCommentTagViewModel>()
                .ForMember(vm => vm.AddedByName, opt => opt.MapFrom(t => t.User.Name));

            //Clickable question 
            CreateMap<SimpleClickableQuestion, SimpleClickableQuestionViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt))
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt))
                .ForMember(vm => vm.PDFURL, opt => mapPDF(opt));

            CreateMap<ClickImage, ClickImageViewModel>();
            CreateMap<ClickChart, ClickChartViewModel>();

            //Multiple choice question 
            CreateMap<MultipleChoiceQuestion, MultipleChoiceQuestionViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt))
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt))
                .ForMember(vm => vm.PDFURL, opt => mapPDF(opt));

            CreateMap<MultipleChoiceQuestionChoice, MultipleChoiceQuestionChoiceViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt));

            //Keyboard question
            CreateMap<KeyboardQuestion, KeyboardQuestionViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt))
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt))
                .ForMember(vm => vm.PDFURL, opt => mapPDF(opt)); 
            
            CreateMap<KeyboardQuestionAnswer, KeyboardQuestionAnswerViewModel>(); 
            
            CreateMap<KeyboardQuestionAnswerElement, KeyboardQuestionAnswerElementViewModel>()
                .ForMember(vm => vm.TextPresentation, opt => 
                opt.MapFrom(g => g.NumericKeyId != null ? g.NumericKey.NumericKey.TextPresentation : g.Image.Variation.TextPresentation));

            CreateMap<KeyboardQuestionWrongAnswer, KeyboardQuestionWrongAnswerViewModel>(); 

            //Student feedback
            CreateMap<QuestionStudentFeedback, QuestionStudentFeedbackViewModel>();

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

            //Maps
            CreateMap<CourseMap, CourseMapViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt));

            CreateMap<CourseMapElement, CourseMapElementViewModel>()
                .ForMember(vm => vm.PDFURL, opt => mapPDF(opt));

            CreateMap<CourseMapElementBadge, CourseMapElementBadgeViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt));

            CreateMap<CourseMapBadgeSystem, CourseMapBadgeSystemViewModel>();

            CreateMap<CourseMapBadgeSystemEntity, CourseMapBadgeSystemEntityViewModel>()
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt));

            //Map pop-up icons
            CreateMap<CourseMapElementImages, CourseMapElementImagesViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt))
                .ForMember(vm => vm.PDF, opt => opt.MapFrom(v => mapAnyProperty(v.PDF)))
                .ForMember(vm => vm.Play, opt => opt.MapFrom(v => mapAnyProperty(v.Play)))
                .ForMember(vm => vm.Link, opt => opt.MapFrom(v => mapAnyProperty(v.Link)))
                .ForMember(vm => vm.Video, opt => opt.MapFrom(v => mapAnyProperty(v.Video)));

            //Default question image
            CreateMap<DefaultQuestionImage, DefaultQuestionImageViewModel>()
                .ForMember(vm => vm.AddedByName, opt => mapUser(opt))
                .ForMember(vm => vm.ImageURL, opt => mapImage(opt));
        }
    }
}
