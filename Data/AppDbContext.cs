using Microsoft.EntityFrameworkCore;
using DehghaniTodoApp.Data;  // For AppDbContext
using DehghaniTodoApp.Models;  // For User and TodoTask

namespace DehghaniTodoApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoTask> TodoTasks { get; set; }
    }
}
