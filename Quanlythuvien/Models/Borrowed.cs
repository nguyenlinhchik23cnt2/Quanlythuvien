using System;
using System.Collections.Generic;

namespace Quanlythuvien.Models;

public partial class Borrowed
{
    public int BorrowId { get; set; }

    public int? StudentId { get; set; }

    public int? BookId { get; set; }

    public DateOnly? BorrowDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public int? LibraId { get; set; }

    public decimal? FineAmount { get; set; }

    public string? BookStatus { get; set; }

    public bool? Status { get; set; }

    public virtual Book? Book { get; set; }

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public virtual Librarian? Libra { get; set; }

    public virtual Student? Student { get; set; }
}
