using Microsoft.EntityFrameworkCore;
using WebAPI_HD.Model;

namespace WebAPI_HD.Repository
{
    public class ApplicationDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("ConnStr"));
        }

        public DbSet<User> Users { get; set; }
    }
}
