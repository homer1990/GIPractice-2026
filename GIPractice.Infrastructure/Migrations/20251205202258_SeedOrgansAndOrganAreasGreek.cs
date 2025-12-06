using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedOrgansAndOrganAreasGreek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalizationStrings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Domain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationStrings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LocalizationStrings",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Domain", "IsDeleted", "Language", "Text", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "STOMACH", new DateTime(2025, 12, 5, 20, 22, 57, 926, DateTimeKind.Utc).AddTicks(1413), null, "Organ", false, "el", "Στομάχι", new DateTime(2025, 12, 5, 20, 22, 57, 926, DateTimeKind.Utc).AddTicks(1414), null },
                    { 2, "GEJ", new DateTime(2025, 12, 5, 20, 22, 57, 926, DateTimeKind.Utc).AddTicks(1420), null, "OrganArea", false, "el", "Γαστροοισοφαγική συμβολή", new DateTime(2025, 12, 5, 20, 22, 57, 926, DateTimeKind.Utc).AddTicks(1420), null }
                });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5448), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5449) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5452), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5453) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5454), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5454) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5455), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5456) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5457), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5457) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5486), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5486) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5487), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5487) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5488), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5489) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5490), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5490) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5491), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5492) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5493), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5493) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5494), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5494) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5495), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5495) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5496), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5497) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5498), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5498) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5499), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5499) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5500), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5500) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5501), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5501) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5502), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5503) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5503), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5504) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5505), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5505) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5506), new DateTime(2025, 12, 5, 20, 22, 57, 927, DateTimeKind.Utc).AddTicks(5506) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1524), new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1524) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1526), new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1527) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1528), new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1528) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1529), new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1530) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1530), new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1531) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1532), new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1532) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1533), new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1533) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1534), new DateTime(2025, 12, 5, 20, 22, 57, 928, DateTimeKind.Utc).AddTicks(1534) });

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationStrings_Domain_Code_Language",
                table: "LocalizationStrings",
                columns: new[] { "Domain", "Code", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalizationStrings");

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5867), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5868) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5874), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5874) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5876), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5876) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5877), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5878) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5879), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5879) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5880), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5880) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5881), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5881) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5882), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5882) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5883), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5884) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5885), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5885) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5886), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5886) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5887), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5888) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5889), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5889) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5890), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5890) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5891), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5891) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5892), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5892) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5893), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5893) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5894), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5895) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5896), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5896) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5897), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5897) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5898), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5898) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5899), new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5899) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1887), new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1887) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1890), new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1891) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1892), new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1892) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1893), new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1894) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1894), new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1895) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1895), new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1896) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1897), new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1897) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1898), new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1898) });
        }
    }
}
