namespace Quanlythuvien.Models;

public class BookCategory
{
    public int BookId { get; set; }
    public int CateId { get; set; }

    public virtual Book Book { get; set; } = null!;
    public virtual Category Categories { get; set; } = null!;
}