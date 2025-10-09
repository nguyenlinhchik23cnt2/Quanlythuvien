using System.ComponentModel.DataAnnotations;
using Quanlythuvien.Models;

namespace Quanlythuvien.Models
{
    public partial class Category
    {

        public int CateId { get; set; }
        public string CateName { get; set; } = null!;
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}