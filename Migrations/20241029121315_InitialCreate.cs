using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformanceSurvey.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_questions_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_users_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "DepartmentId");
                });

            migrationBuilder.CreateTable(
                name: "question_Option",
                columns: table => new
                {
                    OptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_Option", x => x.OptionId);
                    table.ForeignKey(
                        name: "FK_question_Option_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assignment_Question",
                columns: table => new
                {
                    AssignmentQuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment_Question", x => x.AssignmentQuestionId);
                    table.ForeignKey(
                        name: "FK_assignment_Question_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_assignment_Question_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "QuestionId"
                    );
                    table.ForeignKey(
                        name: "FK_assignment_Question_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "responses",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_responses", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_responses_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_responses_question_Option_OptionId",
                        column: x => x.OptionId,
                        principalTable: "question_Option",
                        principalColumn: "OptionId");
                    table.ForeignKey(
                        name: "FK_responses_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "QuestionId"
                        );
                    table.ForeignKey(
                        name: "FK_responses_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_assignment_Question_DepartmentId",
                table: "assignment_Question",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_assignment_Question_QuestionId",
                table: "assignment_Question",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_assignment_Question_UserId",
                table: "assignment_Question",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_question_Option_QuestionId",
                table: "question_Option",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_DepartmentId",
                table: "questions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_responses_DepartmentId",
                table: "responses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_responses_OptionId",
                table: "responses",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_responses_QuestionId",
                table: "responses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_responses_UserId",
                table: "responses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_DepartmentId",
                table: "users",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "assignment_Question");

            migrationBuilder.DropTable(
                name: "responses");

            migrationBuilder.DropTable(
                name: "question_Option");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
