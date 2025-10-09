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
            var quanlythuvienDbContext = _context.Borroweds.Include(b => b.Book).Include(b => b.Libra).Include(b => b.Student);
            return View(await quanlythuvienDbContext.ToListAsync());
        }

        // GET: Borroweds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowed = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(m => m.BorrowId == id);
            if (borrowed == null)
            {
                return NotFound();
            }

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            if (id == null)
            {
                return NotFound();
            }

            var borrowed = await _context.Borroweds.FindAsync(id);
            if (borrowed == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", borrowed.BookId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId", borrowed.LibraId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", borrowed.StudentId);
            return View(borrowed);
        }

        // POST: Borroweds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BorrowId,StudentId,BookId,BorrowDate,DueDate,ReturnDate,LibraId,FineAmount,BookStatus,Status")] Borrowed borrowed)
        {
            if (id != borrowed.BorrowId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowedExists(borrowed.BorrowId))
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
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", borrowed.BookId);
            ViewData["LibraId"] = new SelectList(_context.Librarians, "LibraId", "LibraId", borrowed.LibraId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", borrowed.StudentId);
            return View(borrowed);
        }

        // GET: Borroweds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowed = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(m => m.BorrowId == id);
            if (borrowed == null)
            {
                return NotFound();
            }

            return View(borrowed);
        }

        // POST: Borroweds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowed = await _context.Borroweds.FindAsync(id);
            if (borrowed != null)
            {
                _context.Borroweds.Remove(borrowed);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowedExists(int id)
        {
            return _context.Borroweds.Any(e => e.BorrowId == id);
        }
    }
}
