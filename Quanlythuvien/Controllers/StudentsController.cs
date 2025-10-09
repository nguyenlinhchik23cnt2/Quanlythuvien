using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Quanlythuvien.Controllers
{
    [Authorize] // Tất cả người dùng phải đăng nhập
    public class StudentsController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public StudentsController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        // =========================
        // Admin/Librarian: quản lý sinh viên
        // =========================
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }

        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null) return NotFound();

            return View(student);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Create([Bind("StudentId,Username,PasswordHash,Fullname,Email,Phone,Address,Status")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,Username,PasswordHash,Fullname,Email,Phone,Address,Status")] Student student)
        {
            if (id != student.StudentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(e => e.StudentId == student.StudentId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Student: chỉ xem trang Home
        // =========================
        [Authorize(Roles = "Student")]
        public IActionResult Home()
        {
            var username = User.Identity?.Name;
            var student = _context.Students.FirstOrDefault(s => s.Username == username);

            if (student == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.StudentName = student.Fullname;
            return View();
        }
    }
}
