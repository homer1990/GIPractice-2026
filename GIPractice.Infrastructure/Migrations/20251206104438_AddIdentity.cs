using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5182), new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5184) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5191), new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5191) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1001,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5192), new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5193) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1002,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5194), new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5194) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2001,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5195), new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5195) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2002,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5196), new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5196) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2101,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5197), new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5197) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2102,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5198), new DateTime(2025, 12, 6, 10, 44, 38, 214, DateTimeKind.Utc).AddTicks(5199) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1491), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1492) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1496), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1496) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1498), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1498) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1499), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1500) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1501), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1501) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1502), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1502) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1503), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1503) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1504), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1505) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1506), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1506) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1507), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1507) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1508), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1508) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1509), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1510) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1511), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1511) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1512), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1512) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1513), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1513) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1514), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1514) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1515), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1515) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1516), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1517) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1518), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1518) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1519), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1519) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1520), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1520) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1521), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(1521) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8059), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8060) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8063), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8063) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8064), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8065) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8066), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8066) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8067), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8067) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8068), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8068) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8069), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8069) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8070), new DateTime(2025, 12, 6, 10, 44, 38, 216, DateTimeKind.Utc).AddTicks(8070) });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2858), new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2860) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2867), new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2867) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1001,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2869), new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2869) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1002,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2870), new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2870) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2001,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2871), new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2872) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2002,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2872), new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2873) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2101,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2873), new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2874) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2102,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2874), new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2875) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8790), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8791) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8794), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8795) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8796), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8796) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8797), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8798) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8798), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8799) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8800), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8800) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8801), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(8801) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9116), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9116) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9117), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9117) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9118), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9119) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9120), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9120) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9121), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9121) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9122), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9122) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9123), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9123) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9124), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9124) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9125), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9126) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9127), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9127) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9128), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9128) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9129), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9129) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9130), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9131) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9131), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9132) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9133), new DateTime(2025, 12, 5, 20, 38, 5, 464, DateTimeKind.Utc).AddTicks(9133) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5481), new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5482) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5484), new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5485) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5486), new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5486) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5487), new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5488) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5489), new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5489) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5490), new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5490) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5491), new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5491) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5492), new DateTime(2025, 12, 5, 20, 38, 5, 465, DateTimeKind.Utc).AddTicks(5492) });
        }
    }
}
