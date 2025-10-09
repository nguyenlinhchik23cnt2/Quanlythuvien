using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Quanlythuvien.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int? PublisherId { get; set; }
        public int? YearPublished { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string DownloadLink { get; set; }
        public bool Status { get; set; }

        public Publisher Publisher { get; set; }


        public ICollection<BookCategory> Categories { get; set; }

        public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

        public ICollection<BookAuthor> BookAuthors { get; set; } // Sử dụng tên đúng

        [NotMapped]
        public ICollection<Author> Authors
        {
            get => BookAuthors?.Select(ba => ba.Author).ToList();
            set => BookAuthors = value?.Select(a => new BookAuthor { Author = a }).ToList();
        }
    }

}