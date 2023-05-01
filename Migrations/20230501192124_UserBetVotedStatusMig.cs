using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetMe_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class UserBetVotedStatusMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasVoted",
                table: "UserBets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasVoted",
                table: "UserBets");
        }
    }
}
