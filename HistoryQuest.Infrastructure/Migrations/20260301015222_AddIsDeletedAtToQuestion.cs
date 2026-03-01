using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoryQuest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedAtToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Questions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Questions");
        }
    }
}
