using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoryQuest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixQuizDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestion_Questions_QuestionId",
                table: "QuizQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestion_Quiz_QuizId",
                table: "QuizQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizQuestion",
                table: "QuizQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quiz",
                table: "Quiz");

            migrationBuilder.RenameTable(
                name: "QuizQuestion",
                newName: "QuizQuestions");

            migrationBuilder.RenameTable(
                name: "Quiz",
                newName: "Quizzes");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestion_QuestionId",
                table: "QuizQuestions",
                newName: "IX_QuizQuestions_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizQuestions",
                table: "QuizQuestions",
                columns: new[] { "QuizId", "QuestionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_Questions_QuestionId",
                table: "QuizQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_Quizzes_QuizId",
                table: "QuizQuestions",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_Questions_QuestionId",
                table: "QuizQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_Quizzes_QuizId",
                table: "QuizQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizQuestions",
                table: "QuizQuestions");

            migrationBuilder.RenameTable(
                name: "Quizzes",
                newName: "Quiz");

            migrationBuilder.RenameTable(
                name: "QuizQuestions",
                newName: "QuizQuestion");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestions_QuestionId",
                table: "QuizQuestion",
                newName: "IX_QuizQuestion_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quiz",
                table: "Quiz",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizQuestion",
                table: "QuizQuestion",
                columns: new[] { "QuizId", "QuestionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestion_Questions_QuestionId",
                table: "QuizQuestion",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestion_Quiz_QuizId",
                table: "QuizQuestion",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
