using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quanlythuvien.Models
{
    public class Category
    {
        [Key]
        public int CateId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thể loại")]
        public string CateName { get; set; } = string.Empty;

        // Quan hệ nhiều-nhiều
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}
