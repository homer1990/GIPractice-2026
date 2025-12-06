using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EndoscopyTypeLocalizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "LocalizationStrings",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "Domain", "IsDeleted", "Language", "Text", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { 1001, "Gastroscopy", new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5294), null, "EndoscopyType", false, "el", "Γαστροσκόπηση", new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5295), null },
                    { 1002, "Colonoscopy", new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5296), null, "EndoscopyType", false, "el", "Κολονοσκόπηση", new DateTime(2025, 12, 5, 20, 28, 16, 979, DateTimeKind.Utc).AddTicks(5296), null }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 926, DateTimeKind.Utc).AddTicks(1413), new DateTime(2025, 12, 5, 20, 22, 57, 926, DateTimeKind.Utc).AddTicks(1414) });

            migrationBuilder.UpdateData(
                table: "LocalizationStrings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2025, 12, 5, 20, 22, 57, 926, DateTimeKind.Utc).AddTicks(1420), new DateTime(2025, 12, 5, 20, 22, 57, 926, DateTimeKind.Utc).AddTicks(1420) });

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
        }
    }
}
