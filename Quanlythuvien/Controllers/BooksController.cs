using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;
using System.Security.Claims;


namespace Quanlythuvien.Controllers
{
    //[Authorize(Roles = "Admin,Librarian")]
    public class BooksController : Controller
    {
        private readonly QuanlythuvienDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BooksController(QuanlythuvienDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var quanlythuvienDbContext = _context.Books.Include(b => b.Publisher);
            return View(await quanlythuvienDbContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewBag.Publishers = new SelectList(_context.Publishers, "PublisherId", "PublisherName");
            ViewBag.Categories = new MultiSelectList(_context.Categories, "CateId", "CateName");
            ViewBag.Authors = new MultiSelectList(_context.Authors, "AuthorId", "AuthorName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    Book book,
    IFormFile? imageFile,
    List<int>? SelectedCategoryIds,
    List<int>? SelectedAuthorIds,
    string? NewAuthorName,
    string? NewCateName,
    string? NewPublisherName)
        {
            if (ModelState.IsValid)
            {
                // 🖼️ Upload ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    string folder = Path.Combine(_env.WebRootPath, "images/books");
                    Directory.CreateDirectory(folder);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await imageFile.CopyToAsync(stream);

                    book.ImagePath = "/images/books/" + fileName;
                }

                // 🏷️ Nếu nhập NXB mới
                if (!string.IsNullOrEmpty(NewPublisherName))
                {
                    var newPub = new Publisher { PublisherName = NewPublisherName };
                    _context.Publishers.Add(newPub);
                    await _context.SaveChangesAsync();
                    book.PublisherId = newPub.PublisherId;
                }

                // Lưu sách
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                // 🧩 Nếu nhập thêm thể loại mới
                if (!string.IsNullOrEmpty(NewCateName))
                {
                    var newCate = new Category { CateName = NewCateName };
                    _context.Categories.Add(newCate);
                    await _context.SaveChangesAsync();
                    SelectedCategoryIds ??= new List<int>();
                    SelectedCategoryIds.Add(newCate.CateId);
                }

                // 🧩 Nếu nhập thêm tác giả mới
                if (!string.IsNullOrEmpty(NewAuthorName))
                {
                    var newAuthor = new Author { AuthorName = NewAuthorName };
                    _context.Authors.Add(newAuthor);
                    await _context.SaveChangesAsync();
                    SelectedAuthorIds ??= new List<int>();
                    SelectedAuthorIds.Add(newAuthor.AuthorId);
                }

                // Gắn thể loại
                if (SelectedCategoryIds != null)
                {
                    foreach (var id in SelectedCategoryIds)
                        _context.BookCategories.Add(new BookCategory { BookId = book.BookId, CateId = id });
                }

                // Gắn tác giả
                if (SelectedAuthorIds != null)
                {
                    foreach (var id in SelectedAuthorIds)
                        _context.BookAuthors.Add(new BookAuthor { BookId = book.BookId, AuthorId = id });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi validation -> load lại dropdown
            ViewBag.Publishers = new SelectList(_context.Publishers, "PublisherId", "PublisherName", book.PublisherId);
            ViewBag.Categories = new MultiSelectList(_context.Categories, "CateId", "CateName", SelectedCategoryIds);
            ViewBag.Authors = new MultiSelectList(_context.Authors, "AuthorId", "AuthorName", SelectedAuthorIds);
            return View(book);
        }




        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId", book.PublisherId);
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,PublisherId,YearPublished,Quantity,ImagePath,Description,Location,DownloadLink,Status")] Book book, IFormFile? imageFile)
        {
            if (id != book.BookId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "images/books");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        book.ImagePath = "/images/books/" + uniqueFileName;
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId", book.PublisherId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [Authorize(Roles = "Student,Admin,Librarian")]
        [HttpPost]
        public async Task<IActionResult> BorrowBook(int id)
        {
            // ✅ Lấy StudentId từ Claim hoặc Session
            int? studentId = null;
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(claimId))
            {
                studentId = int.Parse(claimId);
            }
            else
            {
                studentId = HttpContext.Session.GetInt32("StudentId");
            }

            if (studentId == null)
            {
                TempData["Error"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Details", new { id });
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                TempData["Error"] = "Không tìm thấy sách.";
                return RedirectToAction("Index");
            }

            if (book.Quantity <= 0)
            {
                TempData["Error"] = "Sách đã hết.";
                return RedirectToAction("Details", new { id });
            }

            var borrow = new Borrowed
            {
                StudentId = studentId,
                BookId = id,
                BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                Status = true
            };

            _context.Borroweds.Add(borrow);
            book.Quantity -= 1;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Mượn sách thành công!";
            return RedirectToAction("Index");
        }
        private bool BookExists(int id) => _context.Books.Any(e => e.BookId == id);
    }
}