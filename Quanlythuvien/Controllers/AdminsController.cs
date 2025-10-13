
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;


namespace Quanlythuvien.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public AdminsController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách các loại để hiển thị chi tiết
            var adminList = await _context.Admins.ToListAsync();
            var librarianList = await _context.Librarians.ToListAsync();
            var studentList = await _context.Students.ToListAsync();
            var bookList = await _context.Books.ToListAsync();
            var borrowList = await _context.Borroweds
                                   .Include(b => b.Student)
                                   .Include(b => b.Book)
                                   .ToListAsync();

            // Gán vào ViewBag
            ViewBag.AdminList = adminList;
            ViewBag.LibrarianList = librarianList;
            ViewBag.StudentList = studentList;
            ViewBag.BookList = bookList;
            ViewBag.BorrowList = borrowList;

            // Thống kê tổng số
            ViewBag.AdminCount = adminList.Count;
            ViewBag.LibrarianCount = librarianList.Count;
            ViewBag.StudentCount = studentList.Count;
            ViewBag.BookCount = bookList.Count;
            ViewBag.BorrowedCount = borrowList.Count;

            return View();
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var admin = await _context.Admins.FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null) return NotFound();

            return View(admin);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,Username,PasswordHash,Fullname,Email,CreatedAt")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin); // ❌ Không hash, lưu thẳng
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Username,PasswordHash,Fullname,Email,CreatedAt")] Admin admin)
        {
            if (id != admin.AdminId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin); // ❌ Không hash, lưu thẳng
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Admins.Any(e => e.AdminId == admin.AdminId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var admin = await _context.Admins.FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null) return NotFound();

            return View(admin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null) _context.Admins.Remove(admin);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
