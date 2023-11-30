using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Models.LevelsOfDifficulty;
using heatquizapp_api.Models.Courses;
using heatquizapp_api.Models.Questions;
using heatquizapp_api.Models.Series;
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

        //Questions
        public DbSet<QuestionBase> QuestionBase { get; set; }

        //Series
        public DbSet<QuestionSeries> QuestionSeries { get; set; }
        public DbSet<QuestionSeriesElement> QuestionSeriesElement { get; set; }
        public DbSet<QuestionSeriesStatistic> QuestionSeriesStatistic { get; set; }

        //Courses
        public DbSet<Course> Courses { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

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

    }
}
