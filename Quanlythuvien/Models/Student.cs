using System;
using System.Collections.Generic;

namespace Quanlythuvien.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Borrowed> Borroweds { get; set; } = new List<Borrowed>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
