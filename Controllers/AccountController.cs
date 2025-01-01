using Microsoft.AspNetCore.Mvc;
using DehghaniTodoApp.Models;
using DehghaniTodoApp.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Linq;
using System.Text;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Show the SignUp page
    public IActionResult SignUp()
    {
        return View();
    }

    // POST: Handle SignUp form submission
    [HttpPost]
    public IActionResult SignUp(string username, string password, string confirmPassword)
    {
        // Check if passwords match
        if (password != confirmPassword)
        {
            ViewBag.Error = "Passwords do not match.";
            return View();
        }

        // Check if username already exists
        var existingUser = _context.Users.FirstOrDefault(u => u.Username == username);
        if (existingUser != null)
        {
            ViewBag.Error = "Username is already taken.";
            return View();
        }

        // Hash the password before saving to the database
        var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.UTF8.GetBytes(username), // Use username as the salt
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        // Create a new User object and save it to the database
        var newUser = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            Role = "User" // Assign default role
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        // Redirect to Login page after successful registration
        return RedirectToAction("Login");
    }

    // GET: Show the Login page
    public IActionResult Login()
    {
        // Check if user is already logged in (check session for "Username")
        if (HttpContext.Session.GetString("Username") != null)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                // User is logged in but not an admin, redirect them to their user dashboard
                return RedirectToAction("Index", "Task"); // Redirect to user home or dashboard page
            }
            else
            {
                // Admin is logged in, redirect them to the admin dashboard
                return RedirectToAction("UserList", "Admin");
            }
        }

        // Show the login page if not logged in
        return View();
    }

    // POST: Handle Login form submission
[HttpPost]
public IActionResult Login(string username, string password)
{
    // Find the user by username
    var user = _context.Users.FirstOrDefault(u => u.Username == username);

    if (user == null)
    {
        ViewBag.Error = "User not found.";
        return View();
    }

    // Verify the password by comparing the hash
    var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: Encoding.UTF8.GetBytes(username), // Use username as the salt
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));

    if (user.PasswordHash != passwordHash)
    {
        ViewBag.Error = "Invalid password.";
        return View();
    }

    // Store the user role and username in the session after successful login
    HttpContext.Session.SetString("UserRole", user.Role);
    HttpContext.Session.SetString("Username", user.Username); // Optionally, store the username

    // Redirect to the appropriate page (e.g., admin page or dashboard)
    if (user.Role == "Admin")
    {
        return RedirectToAction("UserList", "Admin"); // Admins should be redirected to Admin dashboard
    }
    else
    {
        return RedirectToAction("Index", "Task"); // Normal users should be redirected to Task Index
    }
}

    public IActionResult Logout()
    {
        // Clear the session data to log the user out
        HttpContext.Session.Remove("Username");
        HttpContext.Session.Remove("UserRole");

        // Redirect to the Login page after logout
        return RedirectToAction("Login");
    }
}
