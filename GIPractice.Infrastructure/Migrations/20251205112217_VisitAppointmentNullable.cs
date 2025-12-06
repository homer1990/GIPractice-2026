using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIPractice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VisitAppointmentNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Appointments_AppointmentId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_AppointmentId",
                table: "Visits");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "Visits",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_AppointmentId",
                table: "Visits",
                column: "AppointmentId",
                unique: true,
                filter: "[AppointmentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Appointments_AppointmentId",
                table: "Visits",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Appointments_AppointmentId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_AppointmentId",
                table: "Visits");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "Visits",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_AppointmentId",
                table: "Visits",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Appointments_AppointmentId",
                table: "Visits",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
