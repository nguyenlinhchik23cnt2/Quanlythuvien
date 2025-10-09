namespace Quanlythuvien.Models
{
    public class HomeBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string PublisherName { get; set; }
        public int? YearPublished { get; set; }
        public string ImagePath { get; set; }
        public string CateName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string DownloadLink { get; set; }
        public int Quantity { get; set; }
        public bool Status { get; set; }



        public HomeBookViewModel()
        {
            Authors = new List<string>();
        }
    }
}