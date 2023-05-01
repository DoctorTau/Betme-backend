using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetMe_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class BetWinOutcomeIdMigr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WinOutcomeId",
                table: "Bets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinOutcomeId",
                table: "Bets");
        }
    }
}
