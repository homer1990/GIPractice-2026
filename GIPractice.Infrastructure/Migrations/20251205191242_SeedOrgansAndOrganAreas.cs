using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedOrgansAndOrganAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BiopsyBottleOrganAreas_BiopsyBottles_BiopsyBottleId",
                table: "BiopsyBottleOrganAreas");

            migrationBuilder.DropForeignKey(
                name: "FK_BiopsyBottleOrganAreas_OrganAreas_OrganAreaId",
                table: "BiopsyBottleOrganAreas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BiopsyBottleOrganAreas",
                table: "BiopsyBottleOrganAreas");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrganAreas");

            migrationBuilder.DropColumn(
                name: "Organ",
                table: "OrganAreas");

            migrationBuilder.RenameTable(
                name: "BiopsyBottleOrganAreas",
                newName: "BiopsyBottleOrganArea");

            migrationBuilder.RenameColumn(
                name: "OrganAreaId",
                table: "BiopsyBottleOrganArea",
                newName: "OrganAreasId");

            migrationBuilder.RenameColumn(
                name: "BiopsyBottleId",
                table: "BiopsyBottleOrganArea",
                newName: "BiopsyBottlesId");

            migrationBuilder.RenameIndex(
                name: "IX_BiopsyBottleOrganAreas_OrganAreaId",
                table: "BiopsyBottleOrganArea",
                newName: "IX_BiopsyBottleOrganArea_OrganAreasId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "OrganAreas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultName",
                table: "OrganAreas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BiopsyBottleOrganArea",
                table: "BiopsyBottleOrganArea",
                columns: new[] { "BiopsyBottlesId", "OrganAreasId" });

            migrationBuilder.CreateTable(
                name: "Organs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganAreaOrgans",
                columns: table => new
                {
                    OrganId = table.Column<int>(type: "int", nullable: false),
                    OrganAreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganAreaOrgans", x => new { x.OrganId, x.OrganAreaId });
                    table.ForeignKey(
                        name: "FK_OrganAreaOrgans_OrganAreas_OrganAreaId",
                        column: x => x.OrganAreaId,
                        principalTable: "OrganAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganAreaOrgans_Organs_OrganId",
                        column: x => x.OrganId,
                        principalTable: "Organs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "OrganAreas",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "DefaultName", "IsDeleted", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "ESOPHAGUS_PROX", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5867), null, "Esophagus proximal third", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5868), null },
                    { 2, "ESOPHAGUS_MID", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5874), null, "Esophagus middle third", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5874), null },
                    { 3, "ESOPHAGUS_DIST", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5876), null, "Esophagus distal third", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5876), null },
                    { 4, "GEJ", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5877), null, "Gastroesophageal junction", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5878), null },
                    { 5, "STOMACH_FUNDUS", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5879), null, "Stomach fundus", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5879), null },
                    { 6, "STOMACH_BODY", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5880), null, "Stomach body", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5880), null },
                    { 7, "STOMACH_INCISURA", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5881), null, "Stomach incisura", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5881), null },
                    { 8, "STOMACH_ANTRUM", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5882), null, "Stomach antrum", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5882), null },
                    { 9, "PYLORUS", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5883), null, "Pylorus", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5884), null },
                    { 10, "DUODENUM_BULB", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5885), null, "Duodenum bulb", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5885), null },
                    { 11, "DUODENUM_SECOND", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5886), null, "Duodenum second part", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5886), null },
                    { 12, "DUODENUM_THIRD", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5887), null, "Duodenum third part (horizontal)", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5888), null },
                    { 13, "CECUM", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5889), null, "Cecum", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5889), null },
                    { 14, "ILEOCECAL_VALVE", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5890), null, "Ileocecal valve", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5890), null },
                    { 15, "COLON_ASC", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5891), null, "Colon ascending", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5891), null },
                    { 16, "COLON_HEP_FLEX", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5892), null, "Colon hepatic flexure", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5892), null },
                    { 17, "COLON_TRANS", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5893), null, "Colon transverse", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5893), null },
                    { 18, "COLON_SPL_FLEX", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5894), null, "Colon splenic flexure", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5895), null },
                    { 19, "COLON_DESC", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5896), null, "Colon descending", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5896), null },
                    { 20, "COLON_SIGMOID", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5897), null, "Colon sigmoid", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5897), null },
                    { 21, "RECTUM_AREA", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5898), null, "Rectum", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5898), null },
                    { 22, "ANAL_CANAL", new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5899), null, "Anal canal", false, new DateTime(2025, 12, 5, 19, 12, 41, 930, DateTimeKind.Utc).AddTicks(5899), null }
                });

            migrationBuilder.InsertData(
                table: "Organs",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "DefaultName", "IsDeleted", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "ESOPHAGUS", new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1887), null, "Esophagus", false, new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1887), null },
                    { 2, "STOMACH", new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1890), null, "Stomach", false, new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1891), null },
                    { 3, "DUODENUM", new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1892), null, "Duodenum", false, new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1892), null },
                    { 4, "JEJUNUM", new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1893), null, "Jejunum", false, new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1894), null },
                    { 5, "ILEUM", new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1894), null, "Ileum", false, new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1895), null },
                    { 6, "COLON", new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1895), null, "Colon", false, new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1896), null },
                    { 7, "RECTUM", new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1897), null, "Rectum", false, new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1897), null },
                    { 8, "ANUS", new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1898), null, "Anus", false, new DateTime(2025, 12, 5, 19, 12, 41, 931, DateTimeKind.Utc).AddTicks(1898), null }
                });

            migrationBuilder.InsertData(
                table: "OrganAreaOrgans",
                columns: new[] { "OrganAreaId", "OrganId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 4, 2 },
                    { 5, 2 },
                    { 6, 2 },
                    { 7, 2 },
                    { 8, 2 },
                    { 9, 2 },
                    { 10, 3 },
                    { 11, 3 },
                    { 12, 3 },
                    { 14, 5 },
                    { 13, 6 },
                    { 14, 6 },
                    { 15, 6 },
                    { 16, 6 },
                    { 17, 6 },
                    { 18, 6 },
                    { 19, 6 },
                    { 20, 6 },
                    { 21, 7 },
                    { 22, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganAreas_Code",
                table: "OrganAreas",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganAreaOrgans_OrganAreaId",
                table: "OrganAreaOrgans",
                column: "OrganAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Organs_Code",
                table: "Organs",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BiopsyBottleOrganArea_BiopsyBottles_BiopsyBottlesId",
                table: "BiopsyBottleOrganArea",
                column: "BiopsyBottlesId",
                principalTable: "BiopsyBottles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BiopsyBottleOrganArea_OrganAreas_OrganAreasId",
                table: "BiopsyBottleOrganArea",
                column: "OrganAreasId",
                principalTable: "OrganAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BiopsyBottleOrganArea_BiopsyBottles_BiopsyBottlesId",
                table: "BiopsyBottleOrganArea");

            migrationBuilder.DropForeignKey(
                name: "FK_BiopsyBottleOrganArea_OrganAreas_OrganAreasId",
                table: "BiopsyBottleOrganArea");

            migrationBuilder.DropTable(
                name: "OrganAreaOrgans");

            migrationBuilder.DropTable(
                name: "Organs");

            migrationBuilder.DropIndex(
                name: "IX_OrganAreas_Code",
                table: "OrganAreas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BiopsyBottleOrganArea",
                table: "BiopsyBottleOrganArea");

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "OrganAreas",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DropColumn(
                name: "DefaultName",
                table: "OrganAreas");

            migrationBuilder.RenameTable(
                name: "BiopsyBottleOrganArea",
                newName: "BiopsyBottleOrganAreas");

            migrationBuilder.RenameColumn(
                name: "OrganAreasId",
                table: "BiopsyBottleOrganAreas",
                newName: "OrganAreaId");

            migrationBuilder.RenameColumn(
                name: "BiopsyBottlesId",
                table: "BiopsyBottleOrganAreas",
                newName: "BiopsyBottleId");

            migrationBuilder.RenameIndex(
                name: "IX_BiopsyBottleOrganArea_OrganAreasId",
                table: "BiopsyBottleOrganAreas",
                newName: "IX_BiopsyBottleOrganAreas_OrganAreaId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "OrganAreas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrganAreas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Organ",
                table: "OrganAreas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BiopsyBottleOrganAreas",
                table: "BiopsyBottleOrganAreas",
                columns: new[] { "BiopsyBottleId", "OrganAreaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BiopsyBottleOrganAreas_BiopsyBottles_BiopsyBottleId",
                table: "BiopsyBottleOrganAreas",
                column: "BiopsyBottleId",
                principalTable: "BiopsyBottles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BiopsyBottleOrganAreas_OrganAreas_OrganAreaId",
                table: "BiopsyBottleOrganAreas",
                column: "OrganAreaId",
                principalTable: "OrganAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
