
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hostelhub.Data;
using Hostelhub.Models;

namespace HostelHub.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserName = userName;
            ViewBag.RoomNumber = "A-204";
            ViewBag.RoomType = "2 Sharing";
            ViewBag.RoomStatus = "Allocated";
            ViewBag.TotalComplaints = 3;
            ViewBag.PendingComplaints = 1;
            ViewBag.ResolvedComplaints = 2;
            ViewBag.LatestNotice =
                "Hostel maintenance will be carried out this Sunday.";
            ViewBag.MessRating = "4.2 / 5";

            return View();
        }

        public IActionResult Complaints()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var complaints = _context.Complaints
                .Where(c => c.UserId == GetCurrentUserId(userName))
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(complaints);
        }

        [HttpGet]
        public IActionResult SubmitComplaint()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new Complaint());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitComplaint(Complaint complaint)
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(complaint);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == userName);

            if (user == null)
            {
                ModelState.AddModelError("", "Student account not found.");
                return View(complaint);
            }

            complaint.UserId = user.Id;
            complaint.Status = "Pending";
            complaint.CreatedAt = DateTime.Now;

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Complaint submitted successfully!";
            return RedirectToAction(nameof(Complaints));
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("UserName"));
        }

        private int GetCurrentUserId(string userName)
        {
            return _context.Users
                .Where(u => u.FullName == userName)
                .Select(u => u.Id)
                .FirstOrDefault();
        }
    }
}
