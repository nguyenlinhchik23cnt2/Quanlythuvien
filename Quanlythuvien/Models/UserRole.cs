using System;
using System.Collections.Generic;

namespace Quanlythuvien.Models;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public int RoleId { get; set; }

    public int? AdminId { get; set; }

    public int? LibraId { get; set; }

    public int? StudentId { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Librarian? Libra { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Student? Student { get; set; }
}
