using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;


namespace Quanlythuvien.Controllers
{
   
    public class BorrowedsController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public BorrowedsController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        // GET: Borroweds
        public async Task<IActionResult> Index()
        {
            var quanlythuvienDbContext = _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student);

            return View(await quanlythuvienDbContext.ToListAsync());
        }

        // GET: Borroweds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var borrowed = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(m => m.BorrowId == id);

            if (borrowed == null) return NotFound();

            return View(borrowed);
        }

        // GET: Borroweds/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId");
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId");
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId");
            return View();
        }

        // POST: Borroweds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BorrowId,StudentId,BookId,BorrowDate,DueDate,ReturnDate,LibraId,FineAmount,BookStatus,Status")] Borrowed borrowed)
        {
            if (ModelState.IsValid)
            {
                _context.Add(borrowed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", borrowed.BookId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId", borrowed.LibraId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", borrowed.StudentId);

            return View(borrowed);
        }

        // GET: Borroweds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var borrowed = await _context.Borroweds.FindAsync(id);
            if (borrowed == null) return NotFound();

            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", borrowed.BookId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId", borrowed.LibraId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", borrowed.StudentId);

            return View(borrowed);
        }

        // POST: Borroweds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BorrowId,StudentId,BookId,BorrowDate,DueDate,ReturnDate,LibraId,FineAmount,BookStatus,Status")] Borrowed borrowed)
        {
            if (id != borrowed.BorrowId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowedExists(borrowed.BorrowId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", borrowed.BookId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId", borrowed.LibraId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", borrowed.StudentId);

            return View(borrowed);
        }

        // GET: Borroweds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var borrowed = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(m => m.BorrowId == id);

            if (borrowed == null) return NotFound();

            return View(borrowed);
        }

        // POST: Borroweds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowed = await _context.Borroweds.FindAsync(id);
            if (borrowed != null) _context.Borroweds.Remove(borrowed);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowedExists(int id) => _context.Borroweds.Any(e => e.BorrowId == id);

        // ===============================
        // ✅ CHỨC NĂNG MƯỢN SÁCH CHO SINH VIÊN
        // ===============================

        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var username = User.Identity?.Name;
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Username == username);

            if (student == null)
            {
                TempData["Error"] = "Không tìm thấy sinh viên đang đăng nhập!";
                return RedirectToAction("Index", "Books");
            }

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                TempData["Error"] = "Không tìm thấy sách.";
                return RedirectToAction("Index", "Books");
            }

            // Kiểm tra đã mượn sách này chưa
            bool alreadyBorrowed = await _context.Borroweds
                .AnyAsync(b => b.StudentId == student.StudentId && b.BookId == bookId && b.ReturnDate == null);

            if (alreadyBorrowed)
            {
                TempData["Error"] = "📖 Bạn đã mượn sách này và chưa trả!";
                return RedirectToAction("MyBorrowedBooks");
            }

            // Nếu sách còn
            if (book.Quantity <= 0)
            {
                TempData["Error"] = "❌ Sách này đã hết!";
                return RedirectToAction("Index", "Books");
            }

            var borrowed = new Borrowed
            {
                StudentId = student.StudentId,
                BookId = book.BookId,
                BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                Status = true
            };

            _context.Borroweds.Add(borrowed);

            // Trừ số lượng
            book.Quantity--;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"✅ Bạn đã mượn sách '{book.Title}' thành công!";
            return RedirectToAction("MyBorrowedBooks");
        }


        // ===============================
        // ✅ Danh sách sách sinh viên đang mượn
        // ===============================
        [Authorize(Roles = "Student,Admin,Librarian")]
        public async Task<IActionResult> MyBorrowedBooks()
        {
            var username = User.Identity?.Name;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Username == username);

            if (student == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Index", "Books");
            }

            var borrowedBooks = await _context.Borroweds
                .Include(b => b.Book)
                .Where(b => b.StudentId == student.StudentId && b.ReturnDate == null)
                .ToListAsync();

            return View(borrowedBooks);
        }

        // lọc theo gmail
        // ===============================
        // ✅ Bộ lọc Gmail trong quản lý phiếu mượn
        // ===============================
        [Authorize(Roles = "Admin,Librarian")]
        [HttpGet]
        public async Task<IActionResult> Index(string? gmail)
        {
            var query = _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student)
                .AsQueryable();

            if (!string.IsNullOrEmpty(gmail))
            {
                ViewBag.CurrentGmail = gmail;

                gmail = gmail.Trim();
                query = query.Where(b => b.Student.Email.Contains(gmail));
            }

            ViewBag.CurrentGmail = gmail;

            return View(await query.ToListAsync());
        }

    }
}

