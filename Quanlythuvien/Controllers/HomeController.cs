using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;
using System.Diagnostics;
using System.Linq;

namespace Quanlythuvien.Controllers
{
    [Authorize(Roles = "Admin,Librarian,Student")]
    public class HomeController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public HomeController(QuanlythuvienDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null
        }

        // --- Giữ nguyên logic gốc ---
        public IActionResult Index(int categoryId = 0, string searchQuery = "")
        {
            var booksQuery = _context.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Categories)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .AsQueryable();

            if (categoryId != 0)
                booksQuery = booksQuery.Where(b => b.BookCategories.Any(c => c.CateId == categoryId));

            if (!string.IsNullOrWhiteSpace(searchQuery))
                booksQuery = booksQuery.Where(b => b.Title.Contains(searchQuery));

            var books = booksQuery
                .Select(b => new HomeBookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Authors = b.BookAuthors != null ? b.BookAuthors.Select(ba => ba.Author.AuthorName).ToList() : new List<string>(),
                    PublisherName = b.Publisher != null ? b.Publisher.PublisherName : "",
                    YearPublished = b.YearPublished,
                    ImagePath = b.ImagePath ?? "",
                    CateName = b.BookCategories.Count > 0 ? string.Join(", ", b.BookCategories.Select(c => c.Categories.CateName)) : "Chưa phân loại"
                })
                .ToList();

            // Lấy top 5 sách được mượn nhiều
            var popularBooks = _context.Borroweds
                .GroupBy(bd => bd.BookId)
                .AsEnumerable()
                .Select(g => new PopularBookViewModel
                {
                    BookId = g.Key ?? 0,
                    Title = g.Select(x => x.Book?.Title ?? "").FirstOrDefault() ?? "",
                    TotalBorrows = g.Count()
                })
                .OrderByDescending(x => x.TotalBorrows)
                .Take(5)
                .ToList();

            ViewBag.PopularBooks = popularBooks;
            ViewBag.Books = books;

            ViewBag.Categories = _context.Categories.ToList(); 
            ViewBag.SelectedCateId = categoryId;               
            ViewBag.SearchQuery = searchQuery;

            return View();


            return View();
        }

        public IActionResult Details(int id)
        {
            if (_context.Books == null)
            {
                return View("Lỗi", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier, Message = "Dữ liệu không khả dụng." });
            }

            var book = _context.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Categories)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .Where(b => b.BookId == id)
                .Select(b => new HomeBookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Authors = b.BookAuthors != null && b.BookAuthors.Any()
                    ? b.BookAuthors.Select(ba => ba.Author != null ? ba.Author.AuthorName : "Không rõ").ToList()
                    : new List<string>(),
                    PublisherName = b.Publisher != null ? b.Publisher.PublisherName : "",
                    YearPublished = b.YearPublished,
                    ImagePath = b.ImagePath ?? "",
                    CateName = b.BookCategories.Any()
                    ? string.Join(", ", b.BookCategories.Select(c => c.Categories != null ? c.Categories.CateName : "Không rõ"))
                    : "Chưa phân loại",
                    Description = b.Description ?? "Không có mô tả",
                    Location = b.Location ?? "Không rõ",
                    DownloadLink = b.DownloadLink ?? "",
                    Quantity = b.Quantity,
                    Status = b.Status
                })
                .FirstOrDefault();

            if (book == null)
            {
                return View("Lỗi", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier, Message = "Sách không tồn tại." });
            }

            ViewBag.Book = book;
            return View();
        }

        // ✅ PHẦN THÊM MỚI - CHỨC NĂNG MƯỢN SÁCH
        
        [Authorize(Roles = "Student,Admin,Librarian")]
        [HttpPost]
        public async Task<IActionResult> BorrowBook(int id)
        {
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);
                if (book == null)
                {
                    TempData["Error"] = "Không tìm thấy sách.";
                    return RedirectToAction("Details", new { id });
                }

                if (book.Quantity <= 0)
                {
                    TempData["Error"] = "Sách hiện đã hết.";
                    return RedirectToAction("Details", new { id });
                }

                int? studentId = HttpContext.Session.GetInt32("StudentId");
                if (studentId == null)
                {
                    TempData["Error"] = "Vui lòng đăng nhập lại (session trống).";
                    return RedirectToAction("Details", new { id });
                }

                var borrowed = new Borrowed
                {
                    BookId = id,
                    StudentId = studentId.Value,
                    BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                    DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)), // 👈 ngày hẹn trả
                    ReturnDate = null, // chưa trả
                    Status = true,
                    BookStatus = "Đang mượn"
                };

                _context.Borroweds.Add(borrowed);

                // Giảm số lượng sách
                book.Quantity -= 1;
                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Mượn sách thành công!";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi mượn sách: " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }


        // ✅ HẾT PHẦN THÊM MỚI

        public IActionResult Privacy() => View();
        public IActionResult Introduce() => View();
        public IActionResult Contact() => View();
        
        public IActionResult Resources() => View();
        [HttpPost]
        public IActionResult Contact(string Name, string Email, string Message)
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Message))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            ViewBag.Success = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ✅ ĐOẠN MỚI: 
        [Authorize(Roles = "Student,Admin,Librarian")]
        public async Task<IActionResult> StudentCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories); // View: Views/Home/StudentCategories.cshtml
        }

        // ✅ ĐOẠN MỚI: Lọc sách theo thể loại (chỉ xem)
        [Authorize(Roles = "Student,Admin,Librarian")]
        public async Task<IActionResult> StudentBooksByCategory(int cateId)
        {
            var category = await _context.Categories
                .Include(c => c.BookCategories)
                .ThenInclude(bc => bc.Book)
                .FirstOrDefaultAsync(c => c.CateId == cateId);

            if (category == null) return NotFound();

            ViewBag.CategoryName = category.CateName;
            return View(category); // View: Views/Home/StudentBooksByCategory.cshtml
        }

    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public string Message { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
