using Microsoft.AspNetCore.Mvc;
using DehghaniTodoApp.Models;
using DehghaniTodoApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

public class TaskController : Controller
{
    private readonly AppDbContext _context;

    public TaskController(AppDbContext context)
    {
        _context = context;
    }

    // Helper method to retrieve the current user based on the session username
    private User GetCurrentUser()
    {
        var username = HttpContext.Session.GetString("Username");
        return _context.Users.FirstOrDefault(u => u.Username == username);
    }

    public IActionResult Index()
    {
        var user = GetCurrentUser();

        if (user == null)
        {
            TempData["Error"] = "You must be logged in.";
            return RedirectToAction("Login", "Account");
        }

        // Retrieve tasks for the logged-in user
        var tasks = _context.TodoTasks.Where(t => t.UserId == user.Id).ToList();
        return View(tasks);
    }

    // Display the form to create a new task
    public IActionResult Create()
    {
        return View();
    }

    // Handle form submission for creating a new task
    [HttpPost]
    public IActionResult Create(TodoTask task)
    {
        // Log the incoming task data to debug
        Console.WriteLine($"Title: {task.Title}, Description: {task.Description}");

        // Validate the task data
        if (string.IsNullOrEmpty(task.Title))
        {
            TempData["Error"] = "Title is required.";
            return View(task); // Return with error message if title is missing
        }

        var user = GetCurrentUser();
        if (user == null)
        {
            TempData["Error"] = "You must be logged in to create a task.";
            return RedirectToAction("Login", "Account");
        }

        // Assign the user ID to the task and add it to the database
        task.UserId = user.Id;
        _context.TodoTasks.Add(task);
        _context.SaveChanges();

        // Redirect to the Index page after task creation
        return RedirectToAction("Index");
    }

    // Display the details of a specific task
    public IActionResult Details(int id)
    {
        var task = _context.TodoTasks.FirstOrDefault(t => t.Id == id);

        if (task == null || task.UserId != GetUserId())
        {
            return NotFound(); // Return NotFound if task does not exist or user is unauthorized
        }

        return View(task);
    }

    // Handle the completion of a task
    [HttpPost]
    public IActionResult Complete(int id)
    {
        var task = _context.TodoTasks.FirstOrDefault(t => t.Id == id);

        if (task != null && task.UserId == GetUserId())
        {
            task.IsCompleted = true;
            _context.TodoTasks.Update(task);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    // Helper method to retrieve the user ID from the session
    private int GetUserId()
    {
        var user = GetCurrentUser();
        return user?.Id ?? 0;
    }
}
