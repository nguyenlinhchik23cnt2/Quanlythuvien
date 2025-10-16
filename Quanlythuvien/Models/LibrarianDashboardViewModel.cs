using Microsoft.AspNetCore.Mvc;

namespace Quanlythuvien.Models
{
    public class LibrarianDashboardViewModel 
    {
        public int PendingApprovalsCount { get; set; }
        public int CurrentBorrowsCount { get; set; }
        public int OverdueBooksCount { get; set; }
        public decimal TotalUnpaidFines { get; set; }
        public List<Borrowed> PendingBorrows { get; set; } = new();
        public List<Borrowed> DueSoonBorrows { get; set; } = new();
        public List<Borrowed> OverdueBorrows { get; set; } = new();
    }
}
