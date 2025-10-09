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
    public class FinesController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public FinesController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        // GET: Fines
        public async Task<IActionResult> Index()
        {
            var quanlythuvienDbContext = _context.Fines.Include(f => f.Borrow);
            return View(await quanlythuvienDbContext.ToListAsync());
        }

        // GET: Fines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fine = await _context.Fines
                .Include(f => f.Borrow)
                .FirstOrDefaultAsync(m => m.FineId == id);
            if (fine == null)
            {
                return NotFound();
            }

            return View(fine);
        }

        // GET: Fines/Create
        public IActionResult Create()
        {
            ViewData["BorrowId"] = new SelectList(_context.Borroweds, "BorrowId", "BorrowId");
            return View();
        }

        // POST: Fines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FineId,BorrowId,Amount,Paid,PaidDate,CreatedAt")] Fine fine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowId"] = new SelectList(_context.Borroweds, "BorrowId", "BorrowId", fine.BorrowId);
            return View(fine);
        }

        // GET: Fines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fine = await _context.Fines.FindAsync(id);
            if (fine == null)
            {
                return NotFound();
            }
            ViewData["BorrowId"] = new SelectList(_context.Borroweds, "BorrowId", "BorrowId", fine.BorrowId);
            return View(fine);
        }

        // POST: Fines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FineId,BorrowId,Amount,Paid,PaidDate,CreatedAt")] Fine fine)
        {
            if (id != fine.FineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FineExists(fine.FineId))
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
            ViewData["BorrowId"] = new SelectList(_context.Borroweds, "BorrowId", "BorrowId", fine.BorrowId);
            return View(fine);
        }

        // GET: Fines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fine = await _context.Fines
                .Include(f => f.Borrow)
                .FirstOrDefaultAsync(m => m.FineId == id);
            if (fine == null)
            {
                return NotFound();
            }

            return View(fine);
        }

        // POST: Fines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine != null)
            {
                _context.Fines.Remove(fine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FineExists(int id)
        {
            return _context.Fines.Any(e => e.FineId == id);
        }
    }
}
