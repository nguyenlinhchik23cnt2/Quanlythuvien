using Microsoft.EntityFrameworkCore;

namespace Quanlythuvien.Models
{
    public partial class QuanlythuvienDbContext : DbContext
    {
        public QuanlythuvienDbContext()
        {
        }

        public QuanlythuvienDbContext(DbContextOptions<QuanlythuvienDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Borrowed> Borroweds { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Fine> Fines { get; set; }
        public virtual DbSet<Librarian> Librarians { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        public virtual DbSet<BookAuthor> BookAuthors { get; set; }
        public virtual DbSet<BookCategory> BookCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId).HasName("PK__Admins__AD0500A654D7C909");
                entity.HasIndex(e => e.Email, "UQ__Admins__AB6E6164CDFBC1C0").IsUnique();
                entity.HasIndex(e => e.Username, "UQ__Admins__F3DBC572C9997B8D").IsUnique();
                entity.Property(e => e.AdminId).HasColumnName("adminId");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("createdAt");
                entity.Property(e => e.Email).HasMaxLength(50).IsUnicode(false).HasColumnName("email");
                entity.Property(e => e.Fullname).HasMaxLength(100).HasColumnName("fullname");
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsUnicode(false).HasColumnName("passwordHash");
                entity.Property(e => e.Username).HasMaxLength(50).IsUnicode(false).HasColumnName("username");
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.AuthorId).HasName("PK__Authors__8E2731B9443EFDF0");
                entity.HasIndex(e => e.AuthorName, "UQ__Authors__3D70AA69DFF44EBF").IsUnique();
                entity.Property(e => e.AuthorId).HasColumnName("authorId");
                entity.Property(e => e.AuthorName).HasMaxLength(100).HasColumnName("authorName");
                entity.Property(e => e.Bio).HasMaxLength(300).HasColumnName("bio");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.BookId).HasName("PK__Books__8BE5A10D5AE877E2");
                entity.HasIndex(e => e.Title, "IX_Books_Title");
                entity.Property(e => e.BookId).HasColumnName("bookId");
                entity.Property(e => e.Description).HasMaxLength(500).HasColumnName("description");
                entity.Property(e => e.DownloadLink).HasMaxLength(255).HasColumnName("downloadLink");
                entity.Property(e => e.ImagePath).HasMaxLength(255).HasColumnName("imagePath");
                entity.Property(e => e.Location).HasMaxLength(50).HasColumnName("location");
                entity.Property(e => e.PublisherId).HasColumnName("publisherId");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.Status).HasDefaultValue(true).HasColumnName("status");
                entity.Property(e => e.Title).HasMaxLength(100).HasColumnName("title");
                entity.Property(e => e.YearPublished).HasColumnName("yearPublished");

                entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                    .HasForeignKey(d => d.PublisherId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Book_Publisher");
            });

            modelBuilder.Entity<BookAuthor>(entity =>
            {
                entity.HasKey(e => new { e.BookId, e.AuthorId }).HasName("PK__BookAuth__0307D216E1C99C57");
                entity.HasIndex(e => e.AuthorId, "IX_BookAuthors_AuthorId");
                entity.HasIndex(e => e.BookId, "IX_BookAuthors_BookId");
                entity.Property(e => e.BookId).HasColumnName("bookId");
                entity.Property(e => e.AuthorId).HasColumnName("authorId");

                entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BookAuthors_Authors");

                entity.HasOne(d => d.Book).WithMany(p => p.BookAuthors)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BookAuthors_Books");
            });

            modelBuilder.Entity<BookCategory>(entity =>
            {
                entity.HasKey(e => new { e.BookId, e.CateId }).HasName("PK__BookCate__D16D15D3AB546749");
                entity.HasIndex(e => e.BookId, "IX_BookCategories_BookId");
                entity.HasIndex(e => e.CateId, "IX_BookCategories_CateId");
                entity.Property(e => e.BookId).HasColumnName("bookId");
                entity.Property(e => e.CateId).HasColumnName("cateId");

                // Cấu hình rõ ràng khóa ngoại để tránh shadow property
                entity.HasOne(d => d.Books).WithMany(p => p.Categories)
                    .HasForeignKey(d => d.BookId)
                    .HasPrincipalKey(b => b.BookId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BookCategories_Books");

                entity.HasOne(d => d.Categories).WithMany(p => p.BookCategories)
                    .HasForeignKey(d => d.CateId)
                    .HasPrincipalKey(c => c.CateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BookCategories_Categories");
            });

            modelBuilder.Entity<Borrowed>(entity =>
            {
                entity.HasKey(e => e.BorrowId).HasName("PK__Borrowed__73982486F6330C42");
                entity.ToTable("Borrowed", tb =>
                {
                    tb.HasTrigger("TRG_Borrowed_CalculateFine");
                    tb.HasTrigger("TRG_Borrowed_Insert");
                    tb.HasTrigger("TRG_Borrowed_Update");
                });
                entity.HasIndex(e => e.BookId, "IX_Borrowed_BookId");
                entity.HasIndex(e => e.StudentId, "IX_Borrowed_StudentId");
                entity.Property(e => e.BorrowId).HasColumnName("borrowId");
                entity.Property(e => e.BookId).HasColumnName("bookId");
                entity.Property(e => e.BookStatus).HasMaxLength(50).HasColumnName("bookStatus");
                entity.Property(e => e.BorrowDate).HasDefaultValueSql("(getdate())").HasColumnName("borrowDate");
                entity.Property(e => e.DueDate).HasColumnName("dueDate");
                entity.Property(e => e.FineAmount).HasDefaultValue(0m).HasColumnType("decimal(10, 2)").HasColumnName("fineAmount");
                entity.Property(e => e.LibraId).HasColumnName("libraId");
                entity.Property(e => e.ReturnDate).HasColumnName("returnDate");
                entity.Property(e => e.Status).HasDefaultValue(true).HasColumnName("status");
                entity.Property(e => e.StudentId).HasColumnName("studentId");
                entity.HasOne(d => d.Libra).WithMany(p => p.Borroweds)
                    .HasForeignKey(d => d.LibraId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Borrowed_Librarians");

                entity.HasOne(d => d.Student).WithMany(p => p.Borroweds)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Borrowed_Students");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.CateId).HasName("PK__Categori__A88B4DE4970CD8AE");
                entity.HasIndex(e => e.CateName, "UQ__Categori__BD4278AEDBEE9180").IsUnique();
                entity.Property(e => e.CateId).HasColumnName("cateId");
                entity.Property(e => e.CateName).HasMaxLength(50).HasColumnName("cateName");
            });

            modelBuilder.Entity<Fine>(entity =>
            {
                entity.HasKey(e => e.FineId).HasName("PK__Fines__76725B3A99BCEDA4");
                entity.Property(e => e.FineId).HasColumnName("fineId");
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)").HasColumnName("amount");
                entity.Property(e => e.BorrowId).HasColumnName("borrowId");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("createdAt");
                entity.Property(e => e.Paid).HasDefaultValue(false).HasColumnName("paid");
                entity.Property(e => e.PaidDate).HasColumnName("paidDate");

                entity.HasOne(d => d.Borrow).WithMany(p => p.Fines)
                    .HasForeignKey(d => d.BorrowId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Fines_Borrowed");
            });

            modelBuilder.Entity<Librarian>(entity =>
            {
                entity.HasKey(e => e.LibraId).HasName("PK__Libraria__1C1CFE1020271DB3");
                entity.HasIndex(e => e.Email, "UQ__Libraria__AB6E6164684D619E").IsUnique();
                entity.HasIndex(e => e.Username, "UQ__Libraria__F3DBC572CC014602").IsUnique();
                entity.Property(e => e.LibraId).HasColumnName("libraId");
                entity.Property(e => e.Email).HasMaxLength(50).IsUnicode(false).HasColumnName("email");
                entity.Property(e => e.Fullname).HasMaxLength(100).HasColumnName("fullname");
                entity.Property(e => e.HireDate).HasColumnName("hireDate");
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsUnicode(false).HasColumnName("passwordHash");
                entity.Property(e => e.Status).HasDefaultValue(true).HasColumnName("status");
                entity.Property(e => e.Username).HasMaxLength(50).IsUnicode(false).HasColumnName("username");
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__7E8A0D96A30BB380");
                entity.HasIndex(e => e.PublisherName, "UQ__Publishe__22E7F395FD1B80E2").IsUnique();
                entity.Property(e => e.PublisherId).HasColumnName("publisherId");
                entity.Property(e => e.PublisherName).HasMaxLength(100).HasColumnName("publisherName");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PK__Roles__CD98462A9C11AFE1");
                entity.HasIndex(e => e.RoleName, "UQ__Roles__B19478616DCA5C52").IsUnique();
                entity.Property(e => e.RoleId).HasColumnName("roleId");
                entity.Property(e => e.RoleName).HasMaxLength(50).HasColumnName("roleName");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId).HasName("PK__Students__4D11D63C8C29625F");
                entity.HasIndex(e => e.Email, "IX_Students_Email");
                entity.HasIndex(e => e.Email, "UQ__Students__AB6E6164BAAFFB05").IsUnique();
                entity.HasIndex(e => e.Username, "UQ__Students__F3DBC5725A8DB598").IsUnique();
                entity.Property(e => e.StudentId).HasColumnName("studentId");
                entity.Property(e => e.Address).HasMaxLength(100).HasColumnName("address");
                entity.Property(e => e.Email).HasMaxLength(50).IsUnicode(false).HasColumnName("email");
                entity.Property(e => e.Fullname).HasMaxLength(100).HasColumnName("fullname");
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsUnicode(false).HasColumnName("passwordHash");
                entity.Property(e => e.Phone).HasMaxLength(10).IsUnicode(false).HasColumnName("phone");
                entity.Property(e => e.Status).HasDefaultValue(true).HasColumnName("status");
                entity.Property(e => e.Username).HasMaxLength(50).IsUnicode(false).HasColumnName("username");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__CD3149CC1890F14B");
                entity.Property(e => e.UserRoleId).HasColumnName("userRoleId");
                entity.Property(e => e.AdminId).HasColumnName("adminId");
                entity.Property(e => e.LibraId).HasColumnName("libraId");
                entity.Property(e => e.RoleId).HasColumnName("roleId");
                entity.Property(e => e.StudentId).HasColumnName("studentId");

                entity.HasOne(d => d.Admin).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserRoles_Admins");

                entity.HasOne(d => d.Libra).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.LibraId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserRoles_Librarians");

                entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_UserRoles_Roles");

                entity.HasOne(d => d.Student).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserRoles_Students");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}