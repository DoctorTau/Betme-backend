using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetMe_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class OutcomeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Selections",
                table: "Outcomes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Selections",
                table: "Outcomes");
        }
    }
}
