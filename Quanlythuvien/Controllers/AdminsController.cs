
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;
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
            return View(await _context.Admins.ToListAsync());
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
