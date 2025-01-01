using Microsoft.AspNetCore.Mvc;
using DehghaniTodoApp.Models;
using DehghaniTodoApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // Admin Dashboard - Show list of users
    public IActionResult UserList()
    {
        // Check if the user is logged in as admin
        var userRole = HttpContext.Session.GetString("UserRole");

        // If no session or user is not an admin, redirect to the Login page
        if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
        {
            TempData["Error"] = "You're not admin, please login as an admin user";
            return RedirectToAction("Login", "Account"); // Redirect to Login page
        }

        var users = _context.Users.ToList();
        return View(users);
    }

    // Edit User - Show the edit form
    public IActionResult EditUser(int id)
    {
        // Check if the user is logged in as admin
        var userRole = HttpContext.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
        {
            TempData["Error"] = "You're not admin, please login as an admin user";
            return RedirectToAction("Login", "Account"); // Redirect to Login page
        }

        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // Edit User - Handle the form submission
    [HttpPost]
    public IActionResult EditUser(int id, string username, string role)
    {
        // Check if the user is logged in as admin
        var userRole = HttpContext.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
        {
            TempData["Error"] = "You're not admin, please login as an admin user";
            return RedirectToAction("Login", "Account"); // Redirect to Login page
        }

        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Username = username;
        user.Role = role;

        _context.Users.Update(user);
        _context.SaveChanges();

        return RedirectToAction("UserList");
    }

    // Delete User
    public IActionResult DeleteUser(int id)
    {
        // Check if the user is logged in as admin
        var userRole = HttpContext.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
        {
            TempData["Error"] = "You're not admin, please login as an admin user";
            return RedirectToAction("Login", "Account"); // Redirect to Login page
        }

        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        _context.SaveChanges();

        return RedirectToAction("UserList");
    }
}
