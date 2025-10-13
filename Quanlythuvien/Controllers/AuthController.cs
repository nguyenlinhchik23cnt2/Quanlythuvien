
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;

using System.Security.Claims;

namespace Quanlythuvien.Controllers
{
    public class AuthController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public AuthController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. Admin
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == username && a.PasswordHash == password);
            if (admin != null)
            {
                HttpContext.Session.SetString("Role", "Admin");
                await SignInUser(admin.Username, "Admin");
                return RedirectToAction("Index", "Home");
            }


        

            // 2. Librarian
            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(l => l.Username.ToLower().Trim() == username.ToLower().Trim()
                                       && l.PasswordHash.Trim() == password.Trim());

            if (librarian != null)
            {
                HttpContext.Session.SetString("Role", "Librarian");
                await SignInUser(librarian.Username, "Librarian");
                return RedirectToAction("Index", "Home");
            }



            // 3. Student
            var student = await _context.Students
    .FirstOrDefaultAsync(s => s.Username == username && s.PasswordHash == password);
            if (student != null)
            {
                // ✅ Lưu StudentId và tên vào Session
                HttpContext.Session.SetInt32("StudentId", student.StudentId);
                HttpContext.Session.SetString("StudentName", student.Fullname ?? student.Username);
                HttpContext.Session.SetString("Role", "Student");

                // ✅ Đăng nhập cookie
                await SignInUser(student.Username, "Student");

                return RedirectToAction("Index", "Home");
            }



            ViewBag.Error = "❌ Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        private async Task SignInUser(string username, string role)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                };


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login","Auth");
        }

        public IActionResult AccessDenied() => Content("🚫 Bạn không có quyền truy cập!");


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Student model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra username đã tồn tại chưa
                var exist = await _context.Students.FirstOrDefaultAsync(s => s.Username == model.Username);
                if (exist != null)
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                    return View(model);
                }

                // Thêm Student mới

                _context.Students.Add(model);
                await _context.SaveChangesAsync();

                // Tự động đăng nhập sau khi đăng ký
                await SignInUser(model.Username, "Student");

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
