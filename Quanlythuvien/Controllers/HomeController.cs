using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;

using System.Diagnostics;
using System.Linq;

namespace Quanlythuvien.Controllers
{
    /// <summary>
    [Authorize(Roles = "Admin,Librarian,Student")]

    /// 
    public class HomeController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public HomeController(QuanlythuvienDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null
        }

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
                    Authors = b.BookAuthors != null ? b.BookAuthors.Select(ba => ba.Author.AuthorName).ToList() : new List<string>(), // Điều hướng qua BookAuthors
                    PublisherName = b.Publisher != null ? b.Publisher.PublisherName : "",
                    YearPublished = b.YearPublished,
                    ImagePath = b.ImagePath ?? "",
                    CateName = b.BookCategories.Count > 0 ? string.Join(", ", b.BookCategories.Select(c => c.Categories.CateName)) : "Chưa phân loại"
                })
                .ToList();

            // Lấy top 5 sách được mượn nhiều
            var popularBooks = _context.Borroweds
                .GroupBy(bd => bd.BookId)
                .AsEnumerable() // 👈 chuyển dữ liệu sang LINQ to Objects ở đây
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
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchQuery = searchQuery;

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

        public IActionResult Privacy() => View();
        public IActionResult Introduce() => View();

        public IActionResult Contact() => View();

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
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public string Message { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}