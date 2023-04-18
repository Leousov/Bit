using Microsoft.EntityFrameworkCore;
namespace Bit
{
    public class ApplicationContext :DbContext
    {
        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Rights> Rights { get; set; } = null!;
        public DbSet<User_Rights> UserRights { get; set; } = null!;
        public DbSet<Data> Data { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
        { 
            Database.EnsureCreated(); 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasData(
                new Bit.Users { Id = 1, Login = "admin", Password = "40bd001563085fc35165329ea1ff5c5ecbdbbeef" });
            modelBuilder.Entity<Rights>().HasData(
                new Bit.Rights { Id = 1, Right = "READ" },
                new Bit.Rights { Id = 2, Right = "WRITE" }
                ) ;
            modelBuilder.Entity<User_Rights>().HasData(
                new Bit.User_Rights { ID = 1, Right_Id = 1, User_Id = 1 },
                new Bit.User_Rights { ID = 2, Right_Id = 2, User_Id = 1 }
                );
        }
    }
}
