using HeatQuizAPI.Database;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Models.LevelsOfDifficulty;
using Microsoft.AspNetCore.Identity;

namespace HeatQuizAPI.Services
{
    public interface ISeedDatabase
    {
        void SeedLevelsOfDifficulty();
        void SeedRolesAndFirstUsers();
        void SeedDatapools();
    }

    public class SeedDatabase : ISeedDatabase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly string _adminPassword = "AdminHeatQuiz1234!";
        private readonly string _hquserPassword = "EditorHeatQuiz1234!";

        public SeedDatabase(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //Function to seed roles and first users "admin" - "hq_user"
        public void SeedRolesAndFirstUsers()
        {
            var roles = new List<string>() { "admin", "course_editor" };

            foreach (var role in roles)
            {
                _roleManager.CreateAsync(new IdentityRole()
                {
                    Name = role.ToLower(),
                    NormalizedName = role.ToUpper()
                }).Wait();
            }
                
            //Create users 
            var Admin = new User()
            {
                Name = "Admin",
                UserName = "admin",
                NormalizedUserName = "admin".ToUpper(),
                Email = "admin@rwth-aachen.de",
                NormalizedEmail = "admin@rwth-aachen.de".ToUpper(),
                RegisteredOn = DateTime.UtcNow
            };

            var HQUser = new User()
            {
                Name = "HQUser",
                UserName = "hq_user",
                NormalizedUserName = "hq_user".ToUpper(),
                Email = "hq_user@rwth-aachen.de",
                NormalizedEmail = "hq_user@rwth-aachen.de".ToUpper(),
                RegisteredOn = DateTime.UtcNow,
                ProfilePicture=""
            };

            //Set passwords
            _userManager.CreateAsync(Admin, _adminPassword).Wait();
            _userManager.CreateAsync(HQUser, _hquserPassword).Wait();

            //Add roles
            _userManager.AddToRoleAsync(Admin, roles[0].ToLower()).Wait();
            _userManager.AddToRoleAsync(HQUser, roles[1].ToLower()).Wait();

        }

        //Function to seed datapools
        public void SeedDatapools()
        {
            //Easy
            _context.DataPools.Add(new DataPool()
            {
                Name = "Default Datapool",
                NickName = "Default Datapool",
                IsHidden = false
            });

           _context.SaveChangesAsync().Wait();
        }

        //Function to seed levels of difficulty
        public void SeedLevelsOfDifficulty()
        {
            //Easy
            _context.LevelsOfDifficulty.Add(new LevelOfDifficulty()
            {
                Name="Easy",
                HexColor= "#417505"
            });

            //Medium
            _context.LevelsOfDifficulty.Add(new LevelOfDifficulty()
            {
                Name = "Medium",
                HexColor = "#f8e71c"

            });

            //Hard
            _context.LevelsOfDifficulty.Add(new LevelOfDifficulty()
            {
                Name = "Hard",
                HexColor = "#d0021b"

            });

            _context.SaveChangesAsync().Wait();
        }
    }

   
}
