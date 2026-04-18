using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoryQuest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxRewardedAttemptPerUserToQuizEconomy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxRewardedAttemptsPerUser",
                table: "QuizEconomyRules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxRewardedAttemptsPerUser",
                table: "QuizEconomyRules");
        }
    }
}
