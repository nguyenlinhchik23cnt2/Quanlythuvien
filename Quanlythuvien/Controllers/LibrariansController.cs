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
    [Authorize(Roles = "Admin,Librarian")]
    public class LibrariansController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public LibrariansController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        // GET: Librarians
        public async Task<IActionResult> Index()
        {
            return View(await _context.Librarians.ToListAsync());
        }

        // GET: Librarians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(m => m.LibraId == id);
            if (librarian == null)
            {
                return NotFound();
            }

            return View(librarian);
        }

        // GET: Librarians/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Librarians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibraId,Username,PasswordHash,Fullname,Email,HireDate,Status")] Librarian librarian)
        {
            if (ModelState.IsValid)
            {
                _context.Add(librarian);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(librarian);
        }

        // GET: Librarians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null)
            {
                return NotFound();
            }
            return View(librarian);
        }

        // POST: Librarians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LibraId,Username,PasswordHash,Fullname,Email,HireDate,Status")] Librarian librarian)
        {
            if (id != librarian.LibraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(librarian);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibrarianExists(librarian.LibraId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(librarian);
        }

        // GET: Librarians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(m => m.LibraId == id);
            if (librarian == null)
            {
                return NotFound();
            }

            return View(librarian);
        }

        // POST: Librarians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian != null)
            {
                _context.Librarians.Remove(librarian);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibrarianExists(int id)
        {
            return _context.Librarians.Any(e => e.LibraId == id);
        }

        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Dashboard()
        {
            var model = new LibrarianDashboardViewModel
            {
                PendingApprovalsCount = await _context.Borroweds
                    .CountAsync(b => b.BookStatus == "Pending" && b.Status == true),

                CurrentBorrowsCount = await _context.Borroweds
                    .CountAsync(b => b.BookStatus == "Borrowed" && b.Status == true),

                OverdueBooksCount = await _context.Borroweds
                    .CountAsync(b => b.BookStatus == "Borrowed"
                        && b.DueDate < DateOnly.FromDateTime(DateTime.Now)
                        && b.Status == true),

                TotalUnpaidFines = await _context.Borroweds
                    .Where(b => b.Status == true && b.FineAmount > 0)
                    .SumAsync(b => (decimal?)b.FineAmount) ?? 0,

                PendingBorrows = await _context.Borroweds
                    .Include(b => b.Book)
                    .Include(b => b.Student)
                    .Where(b => b.BookStatus == "Pending" && b.Status == true)
                    .OrderBy(b => b.BorrowDate)
                    .Take(10)
                    .ToListAsync(),

                DueSoonBorrows = await _context.Borroweds
                    .Include(b => b.Book)
                    .Include(b => b.Student)
                    .Where(b => b.BookStatus == "Borrowed"
                        && b.DueDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(3))
                        && b.DueDate > DateOnly.FromDateTime(DateTime.Now)
                        && b.Status == true)
                    .OrderBy(b => b.DueDate)
                    .Take(10)
                    .ToListAsync(),

                OverdueBorrows = await _context.Borroweds
                    .Include(b => b.Book)
                    .Include(b => b.Student)
                    .Where(b => b.BookStatus == "Borrowed"
                        && b.DueDate < DateOnly.FromDateTime(DateTime.Now)
                        && b.Status == true)
                    .OrderBy(b => b.DueDate)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }

        // GET: Librarian/BorrowHistory
        public async Task<IActionResult> BorrowHistory(string status = "")
        {
            var query = _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Student)
                .Include(b => b.Libra)
                .Where(b => b.Status == true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.BookStatus == status);
            }

            var borrows = await query
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(borrows);
        }

        // GET: Librarian/SearchStudent
        public async Task<IActionResult> SearchStudent(string keyword = "")
        {
            var students = new List<Student>();

            if (!string.IsNullOrEmpty(keyword))
            {
                students = await _context.Students
                    .Where(s => s.Status == true
                        && (s.Fullname.Contains(keyword)
                            || s.Username.Contains(keyword)
                            || (s.Email != null && s.Email.Contains(keyword))
                            || (s.Phone != null && s.Phone.Contains(keyword))))
                    .ToListAsync();
            }

            ViewBag.Keyword = keyword;
            return View(students);
        }

        // GET: Librarian/StudentBorrows/{id}
        public async Task<IActionResult> StudentBorrows(int id)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                TempData["Error"] = "Không tìm thấy sinh viên!";
                return RedirectToAction(nameof(SearchStudent));
            }

            var borrows = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Where(b => b.StudentId == id && b.Status == true)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            ViewBag.Student = student;
            return View(borrows);
        }

        // GET: Librarian/ApproveBorrow/{id}
        public async Task<IActionResult> ApproveBorrow(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(b => b.BorrowId == id);

            if (borrow == null)
            {
                return NotFound();
            }

            ViewBag.Librarians = await _context.Librarians
                .Where(l => l.Status == true)
                .ToListAsync();

            return View(borrow);
        }

        // POST: Librarian/ApproveBorrow/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBorrow(int id, string action, int? libraId)
        {
            var borrow = await _context.Borroweds
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.BorrowId == id);

            if (borrow == null)
            {
                TempData["Error"] = "Không tìm thấy yêu cầu mượn sách!";
                return RedirectToAction(nameof(Dashboard));
            }

            if (action == "approve")
            {
                borrow.BookStatus = "Borrowed";
                borrow.LibraId = libraId;

                // Update book availability if needed
                if (borrow.Book != null)
                {
                    borrow.Book.Quantity--;
                }

                TempData["Success"] = "Đã duyệt yêu cầu mượn sách thành công!";
            }
            else if (action == "reject")
            {
                borrow.BookStatus = "Rejected";
                borrow.Status = false;

                TempData["Success"] = "Đã từ chối yêu cầu mượn sách!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Librarian/ProcessReturn/{id}
        public async Task<IActionResult> ProcessReturn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(b => b.BorrowId == id);

            if (borrow == null)
            {
                return NotFound();
            }

            // Calculate fine if overdue
            if (borrow.DueDate < DateOnly.FromDateTime(DateTime.Now))
            {
                var daysLate = (DateOnly.FromDateTime(DateTime.Now).DayNumber - borrow.DueDate.Value.DayNumber);
                borrow.FineAmount = daysLate * 5000; // 5000 VND per day
            }

            ViewBag.Librarians = await _context.Librarians
                .Where(l => l.Status == true)
                .ToListAsync();

            return View(borrow);
        }

        // POST: Librarian/ProcessReturn/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessReturn(int id, decimal? fineAmount, int? libraId)
        {
            var borrow = await _context.Borroweds
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.BorrowId == id);

            if (borrow == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin mượn sách!";
                return RedirectToAction(nameof(Dashboard));
            }

            borrow.BookStatus = "Returned";
            borrow.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
            borrow.FineAmount = fineAmount ?? 0;
            borrow.LibraId = libraId;

            // Update book availability if needed
            if (borrow.Book != null)
            {
                borrow.Book.Quantity++;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xử lý trả sách thành công!";
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Librarian/ChangePassword/{id}
        public async Task<IActionResult> ChangePassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel
            {
                LibraId = librarian.LibraId,
                Username = librarian.Username
            };

            return View(model);
        }

        // POST: Librarian/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var librarian = await _context.Librarians.FindAsync(model.LibraId);
                if (librarian == null)
                {
                    return NotFound();
                }

                /*Verify old password(you should use proper password hashing)*//*
                 if (librarian.PasswordHash != passwordHash(model.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng!");
                    return View(model);
                }

                *//*Update password*//*
                librarian.PasswordHash = passwordHash(model.NewPassword);
                _context.Update(librarian);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đổi mật khẩu thành công!";
                return RedirectToAction(nameof(Details), new { id = model.LibraId });*/
            }

            return View(model);
        }
    }
}

