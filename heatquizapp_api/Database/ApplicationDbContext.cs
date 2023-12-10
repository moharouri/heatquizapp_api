using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Models.LevelsOfDifficulty;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection.Emit;

namespace HeatQuizAPI.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>, IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        //Base values
        public DbSet<DataPool> DataPools { get; set; }

        public DbSet<LevelOfDifficulty> LevelsOfDifficulty { get; set; }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<Subtopic> Subtopics { get; set; }

        public DbSet<UserLinkedPlayerKey> UserLinkedPlayerKeys { get; set; }
        public DbSet<DatapoolNotificationSubscription> DatapoolNotificationSubscriptions { get; set; }

        //Explanations
        public DbSet<Information> Information { get; set; }

        //Keyboard
        public DbSet<Keyboard> Keyboards { get; set; }
        public DbSet<KeyboardNumericKey> NumericKeys { get; set; }
        public DbSet<KeyboardVariableKey> VariableKeys { get; set; }

        public DbSet<KeysList> KeysLists { get; set; }

        //Keyboard answers elements
        public DbSet<AbastractKeyboardAnswerElement> AbastractKeyboardAnswerElements { get; set; }

        //Questions
        public DbSet<QuestionBase> QuestionBase { get; set; }
        public DbSet<QuestionStatistic> QuestionStatistic { get; set; }
        public DbSet<QuestionStudentFeedback> QuestionStudentFeedback { get; set; }

        public DbSet<QuestionCommentSection> QuestionCommentSection { get; set; }
        public DbSet<QuestionComment> QuestionComments { get; set; }
        public DbSet<QuestionCommentTag> QuestionCommentTags { get; set; }
        public DbSet<QuestionCommentSectionTag> QuestionCommentSectionTags { get; set; }

        //Clickable question
        public DbSet<SimpleClickableQuestion> SimpleClickableQuestions { get; set; }

        public DbSet<ClickImage> ClickImage { get; set; }
        public DbSet<ClickChart> ClickChart { get; set; }

        //Multiple choice quesiton
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }

        //Keyboard question
        public DbSet<KeyboardQuestion> KeyboardQuestion { get; set; }

        public DbSet<KeyboardQuestionAnswer> KeyboardQuestionAnswer { get; set; }
        public DbSet<KeyboardQuestionWrongAnswer> KeyboardQuestionWrongAnswers { get; set; }


        //Series
        public DbSet<QuestionSeries> QuestionSeries { get; set; }
        public DbSet<QuestionSeriesElement> QuestionSeriesElement { get; set; }
        public DbSet<QuestionSeriesStatistic> QuestionSeriesStatistic { get; set; }

        //Courses
        public DbSet<Course> Courses { get; set; }

        //Maps
        public DbSet<CourseMap> CourseMap { get; set; }
        public DbSet<CourseMapElement> CourseMapElement { get; set; }
        public DbSet<MapElementLink> MapElementLink { get; set; }
        public DbSet<CourseMapElementBadge> CourseMapElementBadge { get; set; }
        public DbSet<CourseMapPDFStatistics> CourseMapPDFStatistics { get; set; }
        public DbSet<CourseMapBadgeSystem> CourseMapBadgeSystem { get; set; }
        public DbSet<CourseMapBadgeSystemEntity> CourseMapBadgeSystemEntity { get; set; }

        public DbSet<CourseMapKey> CourseMapKeys { get; set; }

        //Pop up icons in maps
        public DbSet<CourseMapElementImages> CourseMapElementImages { get; set; }

        //Click trees
        public DbSet<ImageAnswerGroup> ImageAnswerGroups { get; set; }
        public DbSet<ImageAnswer> ImageAnswers { get; set; }

        //Interpreted trees
        public DbSet<InterpretedImageGroup> InterpretedImageGroups { get; set; }
        public DbSet<InterpretedImage> InterpretedImages { get; set; }

        public DbSet<LeftGradientValue> LeftGradientValues { get; set; }
        public DbSet<RightGradientValue> RightGradientValues { get; set; }
        public DbSet<RationOfGradientsValue> RationOfGradientsValues { get; set; }
        public DbSet<JumpValue> JumpValues { get; set; }

        //Auxillary tables 
        public DbSet<DefaultQuestionImage> DefaultQuestionImages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public ApplicationDbContext()
        {

        }

        //Creation of the database
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            //Get configuration settings -- connection string, database name ... etc
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            //Connect and create the context instance
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString);

            return new ApplicationDbContext(builder.Options);
        }

        //Setup the creation of the database
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename the default ASP user and roles
            SetIdenetityTablesName(builder);

            //Configure map attachment relation
            MapElementAttachmentSetup(builder);

            //Configure question comment section
            QuestionCommentSectionSetup(builder);
        }

        //Define the names of identity tables
        private void SetIdenetityTablesName(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "User");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");

            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
           });
        }

        //Create automatic timestamp creation and update process -- PostgreSQL
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x =>
                x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).DateCreated = DateTime.Now;
                }
                ((BaseEntity)entity.Entity).DateModified = DateTime.Now;
            }
        }

        private void MapElementAttachmentSetup(ModelBuilder builder)
        {
            builder.Entity<MapElementLink>()
                .HasOne(r => r.Element)
                .WithOne(k => k.MapAttachment)
                .HasForeignKey<MapElementLink>(r => r.ElementId);

            builder.Entity<MapElementLink>()
               .HasOne(r => r.Map)
               .WithMany(k => k.Attachments);

        }

        private void QuestionCommentSectionSetup(ModelBuilder builder)
        {
            builder.Entity<QuestionBase>()
                .HasOne(r => r.CommentSection)
                .WithOne(k => k.Question)
                .HasForeignKey<QuestionCommentSection>(r => r.QuestionId);
        }


    }
}
