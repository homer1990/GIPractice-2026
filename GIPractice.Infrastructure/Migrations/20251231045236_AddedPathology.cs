using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPathology : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pathologists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PricingPlanJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pathologists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PathologyParcels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PathologistId = table.Column<int>(type: "int", nullable: false),
                    ParcelCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DispatchedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PathologyParcels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PathologyParcels_Pathologists_PathologistId",
                        column: x => x.PathologistId,
                        principalTable: "Pathologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PathologyReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    EndoscopyId = table.Column<int>(type: "int", nullable: false),
                    PathologistId = table.Column<int>(type: "int", nullable: false),
                    PathologyParcelId = table.Column<int>(type: "int", nullable: true),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClinicalInfo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MacroscopyText = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    DiagnosisText = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsUrgent = table.Column<bool>(type: "bit", nullable: false),
                    DocumentFileId = table.Column<int>(type: "int", nullable: true),
                    DocumentKind = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PathologyReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PathologyReports_Endoscopies_EndoscopyId",
                        column: x => x.EndoscopyId,
                        principalTable: "Endoscopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PathologyReports_MediaFiles_DocumentFileId",
                        column: x => x.DocumentFileId,
                        principalTable: "MediaFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PathologyReports_Pathologists_PathologistId",
                        column: x => x.PathologistId,
                        principalTable: "Pathologists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PathologyReports_PathologyParcels_PathologyParcelId",
                        column: x => x.PathologyParcelId,
                        principalTable: "PathologyParcels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PathologyReports_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PathologyParcels_PathologistId_ParcelCode",
                table: "PathologyParcels",
                columns: new[] { "PathologistId", "ParcelCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PathologyReports_DocumentFileId",
                table: "PathologyReports",
                column: "DocumentFileId");

            migrationBuilder.CreateIndex(
                name: "IX_PathologyReports_EndoscopyId",
                table: "PathologyReports",
                column: "EndoscopyId");

            migrationBuilder.CreateIndex(
                name: "IX_PathologyReports_PathologistId_EndoscopyId",
                table: "PathologyReports",
                columns: new[] { "PathologistId", "EndoscopyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PathologyReports_PathologyParcelId",
                table: "PathologyReports",
                column: "PathologyParcelId");

            migrationBuilder.CreateIndex(
                name: "IX_PathologyReports_PatientId",
                table: "PathologyReports",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PathologyReports");

            migrationBuilder.DropTable(
                name: "PathologyParcels");

            migrationBuilder.DropTable(
                name: "Pathologists");
        }
    }
}
