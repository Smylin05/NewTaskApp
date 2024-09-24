using Microsoft.EntityFrameworkCore;

namespace NewTaskApp.Models
{
    public class TaskDatabaseContext:DbContext
    {
        public TaskDatabaseContext()
        {
        }

        public TaskDatabaseContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Admin> Admins { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Tasks> tasks { get; set; }


    }

    
}
