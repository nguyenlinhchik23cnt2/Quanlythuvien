using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quanlythuvien.Migrations
{
    /// <inheritdoc />
    public partial class InitRebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    adminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    passwordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    fullname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admins__AD0500A654D7C909", x => x.adminId);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    authorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    authorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    bio = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Authors__8E2731B9443EFDF0", x => x.authorId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    cateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cateName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__A88B4DE4970CD8AE", x => x.cateId);
                });

            migrationBuilder.CreateTable(
                name: "Librarians",
                columns: table => new
                {
                    libraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    passwordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    fullname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    hireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Libraria__1C1CFE1020271DB3", x => x.libraId);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    publisherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    publisherName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Publishe__7E8A0D96A30BB380", x => x.publisherId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    roleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__CD98462A9C11AFE1", x => x.roleId);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    studentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    passwordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    fullname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Students__4D11D63C8C29625F", x => x.studentId);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    bookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    publisherId = table.Column<int>(type: "int", nullable: true),
                    yearPublished = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    imagePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    downloadLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AuthorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Books__8BE5A10D5AE877E2", x => x.bookId);
                    table.ForeignKey(
                        name: "FK_Book_Publisher",
                        column: x => x.publisherId,
                        principalTable: "Publishers",
                        principalColumn: "publisherId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "authorId");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    userRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    adminId = table.Column<int>(type: "int", nullable: true),
                    libraId = table.Column<int>(type: "int", nullable: true),
                    studentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__CD3149CC1890F14B", x => x.userRoleId);
                    table.ForeignKey(
                        name: "FK_UserRoles_Admins",
                        column: x => x.adminId,
                        principalTable: "Admins",
                        principalColumn: "adminId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Librarians",
                        column: x => x.libraId,
                        principalTable: "Librarians",
                        principalColumn: "libraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles",
                        column: x => x.roleId,
                        principalTable: "Roles",
                        principalColumn: "roleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Students",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "studentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookAuthors",
                columns: table => new
                {
                    bookId = table.Column<int>(type: "int", nullable: false),
                    authorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookAuth__0307D216E1C99C57", x => new { x.bookId, x.authorId });
                    table.ForeignKey(
                        name: "FK_BookAuthors_Authors",
                        column: x => x.authorId,
                        principalTable: "Authors",
                        principalColumn: "authorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAuthors_Books",
                        column: x => x.bookId,
                        principalTable: "Books",
                        principalColumn: "bookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookCategories",
                columns: table => new
                {
                    bookId = table.Column<int>(type: "int", nullable: false),
                    cateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookCate__D16D15D3AB546749", x => new { x.bookId, x.cateId });
                    table.ForeignKey(
                        name: "FK_BookCategories_Books",
                        column: x => x.bookId,
                        principalTable: "Books",
                        principalColumn: "bookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCategories_Categories",
                        column: x => x.cateId,
                        principalTable: "Categories",
                        principalColumn: "cateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Borrowed",
                columns: table => new
                {
                    borrowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: true),
                    bookId = table.Column<int>(type: "int", nullable: true),
                    borrowDate = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    dueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    returnDate = table.Column<DateOnly>(type: "date", nullable: true),
                    libraId = table.Column<int>(type: "int", nullable: true),
                    fineAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    bookStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Borrowed__73982486F6330C42", x => x.borrowId);
                    table.ForeignKey(
                        name: "FK_Borrowed_Books_bookId",
                        column: x => x.bookId,
                        principalTable: "Books",
                        principalColumn: "bookId");
                    table.ForeignKey(
                        name: "FK_Borrowed_Librarians",
                        column: x => x.libraId,
                        principalTable: "Librarians",
                        principalColumn: "libraId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Borrowed_Students",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "studentId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Fines",
                columns: table => new
                {
                    fineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    borrowId = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    paid = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    paidDate = table.Column<DateOnly>(type: "date", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Fines__76725B3A99BCEDA4", x => x.fineId);
                    table.ForeignKey(
                        name: "FK_Fines_Borrowed",
                        column: x => x.borrowId,
                        principalTable: "Borrowed",
                        principalColumn: "borrowId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Admins__AB6E6164CDFBC1C0",
                table: "Admins",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Admins__F3DBC572C9997B8D",
                table: "Admins",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Authors__3D70AA69DFF44EBF",
                table: "Authors",
                column: "authorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthors_AuthorId",
                table: "BookAuthors",
                column: "authorId");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthors_BookId",
                table: "BookAuthors",
                column: "bookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookCategories_BookId",
                table: "BookCategories",
                column: "bookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookCategories_CateId",
                table: "BookCategories",
                column: "cateId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_publisherId",
                table: "Books",
                column: "publisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowed_BookId",
                table: "Borrowed",
                column: "bookId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowed_libraId",
                table: "Borrowed",
                column: "libraId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowed_StudentId",
                table: "Borrowed",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "UQ__Categori__BD4278AEDBEE9180",
                table: "Categories",
                column: "cateName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fines_borrowId",
                table: "Fines",
                column: "borrowId");

            migrationBuilder.CreateIndex(
                name: "UQ__Libraria__AB6E6164684D619E",
                table: "Librarians",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Libraria__F3DBC572CC014602",
                table: "Librarians",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Publishe__22E7F395FD1B80E2",
                table: "Publishers",
                column: "publisherName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__B19478616DCA5C52",
                table: "Roles",
                column: "roleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "UQ__Students__AB6E6164BAAFFB05",
                table: "Students",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Students__F3DBC5725A8DB598",
                table: "Students",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_adminId",
                table: "UserRoles",
                column: "adminId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_libraId",
                table: "UserRoles",
                column: "libraId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_roleId",
                table: "UserRoles",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_studentId",
                table: "UserRoles",
                column: "studentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthors");

            migrationBuilder.DropTable(
                name: "BookCategories");

            migrationBuilder.DropTable(
                name: "Fines");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Borrowed");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Librarians");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Publishers");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
