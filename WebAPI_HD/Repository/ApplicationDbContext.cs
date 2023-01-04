using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI_HD.Model;

namespace WebAPI_HD.Repository
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        /* protected readonly IConfiguration Configuration;

         public ApplicationDbContext(IConfiguration configuration)
         {
             Configuration = configuration;
         }

         protected override void OnConfiguring(DbContextOptionsBuilder options)
         {
             // connect to sql server database
             options.UseSqlServer(Configuration.GetConnectionString("ConnStr"));
         }

         public DbSet<User> Users { get; set; }*/
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        }
    }
    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(255);
            builder.Property(u => u.LastName).HasMaxLength(255);
        }
    }
}
