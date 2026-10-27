
using Hostelhub.Data;
using Hostelhub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hostelhub.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users
                    .FirstOrDefault(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError(
                        "Email",
                        "An account with this email already exists."
                    );

                    return View(user);
                }

                user.Role = "Student";

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Email == email &&
                    u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString(
                    "UserName", user.FullName);
                HttpContext.Session.SetString(
                    "UserRole", user.Role);

                // Redirect according to user role
                if (user.Role == "Admin" ||
                    user.Role == "Warden")
                {
                    return RedirectToAction(
                        "Dashboard", "Admin");
                }

                return RedirectToAction(
                    "Dashboard", "Student");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
