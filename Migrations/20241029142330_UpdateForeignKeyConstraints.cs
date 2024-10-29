using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformanceSurvey.Migrations
{
    public partial class UpdateForeignKeyConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing foreign key constraints if necessary
            migrationBuilder.DropForeignKey(
                name: "FK_assignment_Question_questions_QuestionId",
                table: "assignment_Question");

            // Example of changing another foreign key to NoAction
            migrationBuilder.DropForeignKey(
                name: "AnotherForeignKeyName", // replace with actual foreign key name
                table: "another_table"); // replace with actual table name

            // Add the foreign key constraint with NO ACTION on delete and update
            migrationBuilder.AddForeignKey(
                name: "FK_assignment_Question_questions_QuestionId",
                table: "assignment_Question",
                column: "QuestionId",
                principalTable: "questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction
            );

            // Restore other foreign keys with NO ACTION
            migrationBuilder.AddForeignKey(
                name: "AnotherForeignKeyName", // replace with actual foreign key name
                table: "another_table", // replace with actual table name
                column: "AnotherColumn", // replace with actual column name
                principalTable: "another_principal_table", // replace with actual principal table name
                principalColumn: "AnotherPrincipalColumn", // replace with actual principal column name
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the modified foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_assignment_Question_questions_QuestionId",
                table: "assignment_Question");

            // Restore the previous foreign key constraints if needed
            migrationBuilder.AddForeignKey(
                name: "FK_assignment_Question_questions_QuestionId",
                table: "assignment_Question",
                column: "QuestionId",
                principalTable: "questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade,
                onUpdate: ReferentialAction.Cascade
            );

            // Restore the other foreign key with Cascade if needed
            migrationBuilder.DropForeignKey(
                name: "AnotherForeignKeyName", // replace with actual foreign key name
                table: "another_table"); // replace with actual table name

            migrationBuilder.AddForeignKey(
                name: "AnotherForeignKeyName", // replace with actual foreign key name
                table: "another_table", // replace with actual table name
                column: "AnotherColumn", // replace with actual column name
                principalTable: "another_principal_table", // replace with actual principal table name
                principalColumn: "AnotherPrincipalColumn", // replace with actual principal column name
                onDelete: ReferentialAction.Cascade, // Restore Cascade or previous setting
                onUpdate: ReferentialAction.Cascade
            );
        }
    }
}
