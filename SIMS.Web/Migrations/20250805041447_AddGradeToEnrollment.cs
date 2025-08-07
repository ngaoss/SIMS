using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddGradeToEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Grade",
                table: "Enrollments",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Enrollments");
        }
    }
}
