namespace DehghaniTodoApp.Models
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }

        // Navigation property to link task to user
        public User User { get; set; }
    }
}
