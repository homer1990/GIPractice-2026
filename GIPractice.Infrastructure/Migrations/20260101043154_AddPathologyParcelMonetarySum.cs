using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPathologyParcelMonetarySum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PathologyParcels_Pathologists_PathologistId",
                table: "PathologyParcels");

            migrationBuilder.DropForeignKey(
                name: "FK_PathologyReports_PathologyParcels_PathologyParcelId",
                table: "PathologyReports");

            migrationBuilder.AddColumn<decimal>(
                name: "MonetarySum",
                table: "PathologyParcels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PathologistId1",
                table: "PathologyParcels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PathologyParcels_PathologistId1",
                table: "PathologyParcels",
                column: "PathologistId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PathologyParcels_Pathologists_PathologistId",
                table: "PathologyParcels",
                column: "PathologistId",
                principalTable: "Pathologists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PathologyParcels_Pathologists_PathologistId1",
                table: "PathologyParcels",
                column: "PathologistId1",
                principalTable: "Pathologists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PathologyReports_PathologyParcels_PathologyParcelId",
                table: "PathologyReports",
                column: "PathologyParcelId",
                principalTable: "PathologyParcels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PathologyParcels_Pathologists_PathologistId",
                table: "PathologyParcels");

            migrationBuilder.DropForeignKey(
                name: "FK_PathologyParcels_Pathologists_PathologistId1",
                table: "PathologyParcels");

            migrationBuilder.DropForeignKey(
                name: "FK_PathologyReports_PathologyParcels_PathologyParcelId",
                table: "PathologyReports");

            migrationBuilder.DropIndex(
                name: "IX_PathologyParcels_PathologistId1",
                table: "PathologyParcels");

            migrationBuilder.DropColumn(
                name: "MonetarySum",
                table: "PathologyParcels");

            migrationBuilder.DropColumn(
                name: "PathologistId1",
                table: "PathologyParcels");

            migrationBuilder.AddForeignKey(
                name: "FK_PathologyParcels_Pathologists_PathologistId",
                table: "PathologyParcels",
                column: "PathologistId",
                principalTable: "Pathologists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PathologyReports_PathologyParcels_PathologyParcelId",
                table: "PathologyReports",
                column: "PathologyParcelId",
                principalTable: "PathologyParcels",
                principalColumn: "Id");
        }
    }
}
