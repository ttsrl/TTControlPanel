using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Models;
using TTGitServer.Models;

namespace TTControlPanel.Services
{
    public class DBContext : DbContext
    {

        private static bool started = false;
        public static bool Started { get => started; }
        public static DbContextOptions<DBContext> Options;

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
            started = true;
            Options = options;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationVersion> ApplicationsVersions { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<ProductKey> ProductKeys { get; set; }
        public DbSet<HID> Hids { get; set; }
        public DbSet<LastLog> LastLogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Working> Workings { get; set; }


        public DbSet<AuthorizationLog> AuthorizationLogs { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<SshKey> SshKeys { get; set; }
        public DbSet<TeamRepositoryRole> TeamRepositoryRoles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserTeamRole> UserTeamRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<User>().Property(u => u.Visible).HasDefaultValue(true);


            //modelBuilder.Entity<TeamRepositoryRole>().HasKey(t => new { t.Team.ID, t.Repository.ID });
            //modelBuilder.Entity<UserTeamRole>().HasKey(t => new { t.User.Id, t.Team.ID });
        }

        public static async Task Initialize(DBContext context, Cryptography crypt)
        {
            await context.Database.EnsureCreatedAsync();
            if (await context.Users.AnyAsync()) return;

            var roles = new List<Role>
            {
                new Role
                {
                    Name = "Administrator",
                    Description = "Amministratore"
                }
            };
            // set all grants to administrator
            foreach (var g in Role.GetGrantNames()) roles[0][g] = true;
            var users = new List<User>
            {
                new User
                {
                    Role = roles[0],
                    Name = "Francesco",
                    Surname = "Fioravanti",
                    Password = await crypt.Argon2HashAsync("francesco1995"),
                    Username = "Administrator",
                    Email = "francesco.f@ttautomazioni.it",
                    Ban = false
                }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}