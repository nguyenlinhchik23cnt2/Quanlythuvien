using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quanlythuvien.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        public int? AuthorId { get; set; }
        public Author? author { get; set; }
        [ForeignKey("AuthorId")]

        public int? PublisherId { get; set; }

        [ForeignKey("PublisherId")]
        public Publisher? Publisher { get; set; }

        [Range(1800, 2100, ErrorMessage = "Năm xuất bản không hợp lệ")]
        public int? YearPublished { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ")]
        public int Quantity { get; set; }

        public string? ImagePath { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public string? Location { get; set; }

        public string? DownloadLink { get; set; }

        public bool Status { get; set; } = true;
        // 🔹 THÊM MỚI
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Quan hệ nhiều-nhiều
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}
