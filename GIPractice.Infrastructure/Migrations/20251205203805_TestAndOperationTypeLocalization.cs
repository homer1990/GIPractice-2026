using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestAndOperationTypeLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "LocalizationStrings",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Domain", "IsDeleted", "Language", "Text", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { 2001, "H_Pylori_UreaBreath", new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2871), null, "TestType", false, "el", "Δοκιμασία αναπνοής ουρίας για H. pylori", new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2872), null },
                    { 2002, "FecalOccultBlood", new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2872), null, "TestType", false, "el", "Απόκρυφο αίμα κοπράνων", new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2873), null },
                    { 2101, "Polypectomy", new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2873), null, "OperationType", false, "el", "Πολυπεκτομή", new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2874), null },
                    { 2102, "HemostasisClip", new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2874), null, "OperationType", false, "el", "Αιμόσταση με κλιπ", new DateTime(2025, 12, 5, 20, 38, 5, 463, DateTimeKind.Utc).AddTicks(2875), null }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2002);

            migrationBuilder.DeleteData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2101);

            migrationBuilder.DeleteData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2102);

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5285), new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5287) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5293), new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5293) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1001,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5294), new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5295) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1002,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5296), new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5296) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8966), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8967) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8971), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8971) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8972), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8972) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8973), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8973) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8974), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8974) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8975), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8976) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8976), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8977) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8978), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8978) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8979), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8979) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8980), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8980) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8981), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8981) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8982), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8982) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8983), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8984) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8984), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8985) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8986), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8986) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8987), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8987) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8988), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8988) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8989), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8989) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8990), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8991) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8991), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8992) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8993), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8993) });

            migrationBuilder.UpdateData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8994), new DateTime(2025, 12, 5, 20, 28, 16, 980, DateTimeKind.Utc).AddTicks(8994) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4794), new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4795) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4798), new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4798) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4799), new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4799) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4800), new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4801) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4801), new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4802) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4802), new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4803) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4803), new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4804) });

            migrationBuilder.UpdateData(
                table: "Organs",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4804), new DateTime(2025, 12, 5, 20, 28, 16, 981, DateTimeKind.Utc).AddTicks(4805) });
        }
    }
}
