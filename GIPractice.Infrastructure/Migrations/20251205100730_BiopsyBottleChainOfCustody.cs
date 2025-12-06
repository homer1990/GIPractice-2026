using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BiopsyBottleChainOfCustody : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CollectedAtUtc",
                table: "BiopsyBottles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "BiopsyBottles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BiopsyBottles_PatientId",
                table: "BiopsyBottles",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_BiopsyBottles_Patients_PatientId",
                table: "BiopsyBottles",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BiopsyBottles_Patients_PatientId",
                table: "BiopsyBottles");

            migrationBuilder.DropIndex(
                name: "IX_BiopsyBottles_PatientId",
                table: "BiopsyBottles");

            migrationBuilder.DropColumn(
                name: "CollectedAtUtc",
                table: "BiopsyBottles");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "BiopsyBottles");
        }
    }
}
