using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformanceSurvey.Migrations
{
    public partial class UpdateForeignKeyConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assignment_Question_questions_QuestionId",
                table: "assignment_Question");

            migrationBuilder.DropForeignKey(
                name: "AnotherForeignKeyName", 
                table: "another_table"); 

            migrationBuilder.AddForeignKey(
                name: "FK_assignment_Question_questions_QuestionId",
                table: "assignment_Question",
                column: "QuestionId",
                principalTable: "questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction
            );

            migrationBuilder.AddForeignKey(
                name: "AnotherForeignKeyName", 
                table: "another_table", 
                column: "AnotherColumn", 
                principalTable: "another_principal_table", 
                principalColumn: "AnotherPrincipalColumn", 
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assignment_Question_questions_QuestionId",
                table: "assignment_Question");

            migrationBuilder.AddForeignKey(
                name: "FK_assignment_Question_questions_QuestionId",
                table: "assignment_Question",
                column: "QuestionId",
                principalTable: "questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade,
                onUpdate: ReferentialAction.Cascade
            );

            migrationBuilder.DropForeignKey(
                name: "AnotherForeignKeyName", 
                table: "another_table"); 

            migrationBuilder.AddForeignKey(
                name: "AnotherForeignKeyName", 
                table: "another_table",
                column: "AnotherColumn", 
                principalTable: "another_principal_table", 
                principalColumn: "AnotherPrincipalColumn", 
                onDelete: ReferentialAction.Cascade, 
                onUpdate: ReferentialAction.Cascade
            );
        }
    }
}
