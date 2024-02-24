using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanvasLMS.Migrations
{
    /// <inheritdoc />
    public partial class addinginsertcareercourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CareerCourse",
                columns: new[] { "CareersId", "CoursesId" },
                values: new object[] { 102, "169005" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CareerCourse",
                keyColumns: new[] { "CareersId", "CoursesId" },
                keyValues: new object[] { 102, "169005" });
        }
    }
}
