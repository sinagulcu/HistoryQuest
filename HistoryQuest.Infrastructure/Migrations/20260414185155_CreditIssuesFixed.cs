using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoryQuest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreditIssuesFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EntryFeeChargedAt",
                table: "QuizAttempts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "QuizAttempts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEntryFeeCharged",
                table: "QuizAttempts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSettled",
                table: "QuizAttempts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SettledAt",
                table: "QuizAttempts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryFeeChargedAt",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "IsEntryFeeCharged",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "IsSettled",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "SettledAt",
                table: "QuizAttempts");
        }
    }
}
