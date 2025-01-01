using System.Collections.Generic;

namespace DehghaniTodoApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // e.g., "Admin", "User"

        // Navigation property to associate tasks with users
        public ICollection<TodoTask> Tasks { get; set; }
    }
}
