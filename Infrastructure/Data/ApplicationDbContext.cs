using Core.Entity;
using Core.Configurations;
using Microsoft.EntityFrameworkCore;
using Core.DTO;
namespace Infrastructure.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<UsersSkills> UserSkills { get; set; }
        public DbSet<UserCertificates> UserCertificates { get; set; }
        public DbSet<UserEmploymentRecords> UserEmploymentRecords { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            #region NoKey Region 
           modelBuilder.Entity<UsersSkills>(e => { e.HasKey(us => new { us.UserId, us.SkillId }); });
           modelBuilder.Entity<UserRows>(e => { e.HasNoKey().ToView(null); });
            #endregion

            #region Configuration
            new ConfigurationEntityUserInfo().Configure(modelBuilder.Entity<UserInfo>());
            #endregion
        }
    }
}