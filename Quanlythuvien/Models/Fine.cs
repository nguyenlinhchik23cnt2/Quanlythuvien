using System;
using System.Collections.Generic;

namespace Quanlythuvien.Models;

public partial class Fine
{
    public int FineId { get; set; }

    public int? BorrowId { get; set; }

    public decimal Amount { get; set; }

    public bool? Paid { get; set; }

    public DateOnly? PaidDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Borrowed? Borrow { get; set; }
}
