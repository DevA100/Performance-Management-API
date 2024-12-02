using PerformanceSurvey.iRepository;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models;
using Microsoft.EntityFrameworkCore;
using PerformanceSurvey.Repository;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PerformanceSurvey.Services
{
    public class ResponseService : IResponseService
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAssignmentQuestionRepository _assignmentQuestionRepository;
        public ResponseService(IResponseRepository responseRepository, IQuestionRepository questionRepository, IAssignmentQuestionRepository assignmentQuestionRepository)
        {
            _responseRepository = responseRepository;
            _questionRepository = questionRepository;
            _assignmentQuestionRepository = assignmentQuestionRepository;
        }

        public async Task SaveResponseAsync(SaveMultipleChoiceResponseDto dto)
        {
            // Fetch the existing QuestionOption from the database if OptionId is provided
            QuestionOption? questionOption = null;

            if (dto.OptionId != 0)
            {
                // Fetch the QuestionOption from the repository using the option ID from the DTO
                 questionOption = await _questionRepository.GetOptionByIdAsync(dto.OptionId);

                // Check if the QuestionOption exists and is valid
                if (questionOption == null || questionOption.QuestionId != dto.QuestionId)
                {
                    throw new Exception("Invalid Option: The specified QuestionOption does not exist or does not belong to the specified question.");
                }
            }

            // Create the Response entity
            var response = new Response
            {
                QuestionId = dto.QuestionId,
                UserId = dto.UserId,
                DepartmentId = dto.DepartmentId,
                QuestionOption = questionOption,
                CreatedAt = DateTime.UtcNow,
                Score = CalculateScore(dto) // Set the score based on your logic
            };

            // Save the response using the repository
            await _responseRepository.SaveAsync(response);

            // Update the assignment status for the user and question
            await UpdateAssignmentStatusAsync(dto.UserId, dto.QuestionId);


        }
        private async Task UpdateAssignmentStatusAsync(int userId, int questionId)
        {
            // Fetch the assignment for the user and question
            var assignment = await _assignmentQuestionRepository.GetAssignmentByUserAndQuestionAsync(userId, questionId);

            if (assignment != null)
            {
                assignment.status = 1; // Mark as answered
                await _assignmentQuestionRepository.UpdateAsync(assignment);
            }
        }

        private int CalculateScore(SaveMultipleChoiceResponseDto dto)
        {
            // Implement your logic here to determine the score.
            // For example, if the score is based on the selected option:
            var option = _questionRepository.GetOptionByIdAsync(dto.OptionId).Result;
            if (option != null)
            {
                // Example logic: return a score based on option properties
                return option.Score; // Assuming option has a ScoreValue field
            }
            // Return a default value if no logic applies
            return 0;
        }

        public async Task<SaveTextResponseDto> SaveTextResponseAsync(SaveTextResponseDto textResponseDto)
        {
            // Convert DTO to entity
            var textResponse = new Response
            {
                QuestionId = textResponseDto.QuestionId,
                UserId = textResponseDto.UserId,
                ResponseText = textResponseDto.ResponseText,
                DepartmentId = textResponseDto.DepartmentId,
                                CreatedAt = DateTime.UtcNow
            };

            // Use the AddAsync method of the repository
            var savedResponse = await _responseRepository.AddAsync(textResponse);

            // Update the DTO with the generated ResponseId from the saved entity
            await UpdateAssignmentStatusAsync(textResponseDto.UserId, textResponseDto.QuestionId);

            return textResponseDto;

            

        }
        public async Task<List<GetResponsesByDepartmentIdDto>> GetResponsesByDepartmentIdAsync(int departmentId)
        {
            // Retrieve the list of Response objects from the repository
            var responses = await _responseRepository.GetResponsesByDepartmentIdAsync(departmentId);

            // Check if responses are null or empty
            if (responses == null || !responses.Any())
            {
                return new List<GetResponsesByDepartmentIdDto>();
            }

            // Map the Response objects to GetResponsesByDepartmentIdDto objects
            var responseDtos = responses.Select(response => new GetResponsesByDepartmentIdDto
            {
                ResponseId = response.ResponseId,
                QuestionId = response.QuestionId,
                DepartmentId = response.DepartmentId,
                DepartmentName = response.Question?.Department?.DepartmentName ?? "Unknown", // Safe navigation and default value
                ResponseText = string.IsNullOrEmpty(response.ResponseText) ? "NIL" : response.ResponseText, // Check for empty or null
                OptionId = response.OptionId,
                Text = string.IsNullOrEmpty(response.QuestionOption?.Text) ? "NIL" : response.QuestionOption.Text, // Check for empty or null
                Score = response.Score,
                Name = response.User?.Name ?? "Unknown",
                CreatedAt = response.CreatedAt,
                QuestionText = response.Question?.QuestionText ?? "No question text"


            }).ToList();

            // Return the list of DTOs
            return responseDtos;
        }


        public async Task ClearResponsesByDepartmentIdAsync(int departmentId)
        {
            await _responseRepository.ClearResponsesByDepartmentIdAsync(departmentId);
        }

        public async Task ClearAllResponsesAsync()
        {
            await _responseRepository.ClearAllResponsesAsync();
        }

        // Method to generate Excel file by departmentId
        //public async Task<byte[]> ExportResponsesToExcelAsync(int? departmentId = null)
        //{
        //    // Set the license context for EPPlus
        //    ExcelPackage.LicenseContext = LicenseContext.Commercial;

        //    var responses = departmentId.HasValue
        //        ? await _responseRepository.GetResponsesByDepartmentIdAsync(departmentId.Value)
        //        : await _responseRepository.GetAllResponsesAsync(); // Assuming method exists to get all responses

        //    using var package = new ExcelPackage();
        //    var worksheet = package.Workbook.Worksheets.Add("Responses");

        //    // Add header
        //   // worksheet.Cells[1, 2].Value = "QuestionId";
        //    //worksheet.Cells[1, 3].Value = "DepartmentId";
        //    worksheet.Cells[1, 1].Value = "DepartmentName";
        //    worksheet.Cells[1, 2].Value = "QuestionText"; // Fixed index
        //    worksheet.Cells[1, 3].Value = "ResponseText";
        //    worksheet.Cells[1, 4].Value = "Text"; // Fixed index
        //    worksheet.Cells[1, 5].Value = "Score";
        //    worksheet.Cells[1, 6].Value = "UserName";
        //    worksheet.Cells[1, 7].Value = "CreatedAt";
        //    // Populate data
        //    int row = 2;
        //    foreach (var response in responses)
        //    {
        //        //worksheet.Cells[row, 2].Value = response.QuestionId;
        //        //worksheet.Cells[row, 3].Value = response.DepartmentId;
        //        worksheet.Cells[row, 1].Value = response.Question?.Department?.DepartmentName ?? "Unknown";
        //        worksheet.Cells[row, 2].Value = response.Question?.QuestionText ?? "Nil";
        //        worksheet.Cells[row, 3].Value = string.IsNullOrEmpty(response.ResponseText) ? "Nil" : response.ResponseText;
        //        worksheet.Cells[row, 4].Value = response.QuestionOption?.Text ?? "Nil";
        //        worksheet.Cells[row, 5].Value = response.Score;
        //        worksheet.Cells[row, 6].Value = response.User?.Name ?? "Unknown";
        //        worksheet.Cells[row, 7].Value = response.CreatedAt;
        //        row++;

        //    }

        //    return package.GetAsByteArray();
        //}




        //public async Task<byte[]> ExportResponsesToExcelAsync(int? departmentId = null)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.Commercial;

        //    var responses = departmentId.HasValue
        //        ? await _responseRepository.GetResponsesByDepartmentIdAsync(departmentId.Value)
        //        : await _responseRepository.GetAllResponsesAsync();

        //    // Group responses by UserId and DepartmentId
        //    var groupedResponses = responses
        //        .GroupBy(r => new { r.UserId, r.DepartmentId, r.User?.Name, r.CreatedAt.Date })
        //        .ToDictionary(g => g.Key, g => g.ToList());

        //    using var package = new ExcelPackage();
        //    var worksheet = package.Workbook.Worksheets.Add("Responses");

        //    // Get all unique questions
        //    var questions = responses
        //        .Select(r => r.Question)
        //        .Distinct()
        //        .OrderBy(q => q.QuestionId)
        //        .ToList();

        //    // Add headers
        //    int col = 1;
        //    worksheet.Cells[1, col++].Value = "Department";
        //    worksheet.Cells[1, col++].Value = "UserName";

        //    // Add question headers
        //    foreach (var question in questions)
        //    {
        //        worksheet.Cells[1, col++].Value = question.QuestionText;
        //    }

        //    // Add Score and Created At headers
        //    worksheet.Cells[1, col++].Value = "Total Score";
        //    worksheet.Cells[1, col].Value = "Created At";

        //    // Populate data
        //    int row = 2;
        //    foreach (var group in groupedResponses)
        //    {
        //        col = 1;

        //        // Basic info
        //        worksheet.Cells[row, col++].Value = group.Value.First().Question?.Department?.DepartmentName ?? "Unknown";
        //        worksheet.Cells[row, col++].Value = group.Key.Name ?? "Unknown";

        //        // Fill in responses for each question
        //        foreach (var question in questions)
        //        {
        //            var response = group.Value.FirstOrDefault(r => r.QuestionId == question.QuestionId);

        //            worksheet.Cells[row, col++].Value = response != null
        //                ? (!string.IsNullOrEmpty(response.ResponseText)
        //                    ? response.ResponseText
        //                    : response.QuestionOption?.Text ?? "Nil")
        //                : "N/A";
        //        }

        //        // Calculate and add total score
        //        double totalScore = group.Value
        //            .Where(r => r.Score.HasValue)
        //            .Average(r => r.Score ?? 0);

        //        worksheet.Cells[row, col++].Value = totalScore > 0 ? Math.Round(totalScore, 2) : 0;

        //        // Add creation date
        //        worksheet.Cells[row, col].Value = group.Key.Date;

        //        row++;
        //    }

        //    // Auto-fit columns
        //    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        //    // Add some basic styling
        //    using (var range = worksheet.Cells[1, 1, 1, col])
        //    {
        //        range.Style.Font.Bold = true;
        //        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        //    }

        //    // Format score column to show 2 decimal places
        //    var scoreColumn = worksheet.Column(col - 1);
        //    scoreColumn.Style.Numberformat.Format = "#,##0.00";

        //    return package.GetAsByteArray();
        //}






        public async Task<byte[]> ExportResponsesToExcelAsync(int? departmentId = null)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;

            var responses = departmentId.HasValue
                ? await _responseRepository.GetResponsesByDepartmentIdAsync(departmentId.Value)
                : await _responseRepository.GetAllResponsesAsync();

            // Group responses by UserId and DepartmentId
            var groupedResponses = responses
                .GroupBy(r => new { r.UserId, r.DepartmentId, r.User?.Name, r.CreatedAt.Date })
                .ToDictionary(g => g.Key, g => g.ToList());

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Responses");

            // Get all unique questions
            var questions = responses
                .Select(r => r.Question)
                .Distinct()
                .OrderBy(q => q.QuestionId)
                .ToList();

            // Add headers
            int col = 1;
            worksheet.Cells[1, col++].Value = "Department";
            worksheet.Cells[1, col++].Value = "UserName";

            // Track question columns for styling
            var questionStartCol = col;
            var questionEndCol = col + questions.Count - 1;

            // Add question headers
            foreach (var question in questions)
            {
                worksheet.Cells[1, col++].Value = question.QuestionText;
            }

            // Add Score and Created At headers
            var scoreColumn = col;
            worksheet.Cells[1, col++].Value = "Total Score";
            worksheet.Cells[1, col].Value = "Created At";

            // Populate data
            int row = 2;
            foreach (var group in groupedResponses)
            {
                col = 1;

                // Basic info
                worksheet.Cells[row, col++].Value = group.Value.First().Question?.Department?.DepartmentName ?? "Unknown";
                worksheet.Cells[row, col++].Value = group.Key.Name ?? "Unknown";

                // Fill in responses for each question
                foreach (var question in questions)
                {
                    var response = group.Value.FirstOrDefault(r => r.QuestionId == question.QuestionId);

                    worksheet.Cells[row, col++].Value = response != null
                        ? (!string.IsNullOrEmpty(response.ResponseText)
                            ? response.ResponseText
                            : response.QuestionOption?.Text ?? "Nil")
                        : "N/A";
                }

                // Calculate and add total score
                double totalScore = group.Value
                    .Where(r => r.Score.HasValue)
                    .Average(r => r.Score ?? 0);

                worksheet.Cells[row, col++].Value = totalScore > 0 ? Math.Round(totalScore, 2) : 0;

                // Add creation date
                worksheet.Cells[row, col].Value = group.Key.Date;

                row++;
            }

            // Style the worksheet
            using (var range = worksheet.Cells[1, 1, 1, col])
            {
                // Header styling
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Question columns styling
            for (int i = questionStartCol; i <= questionEndCol; i++)
            {
                var questionColumn = worksheet.Column(i);
                questionColumn.Width = 20; // Set fixed width
                questionColumn.Style.WrapText = true; // Enable text wrapping
                questionColumn.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            // Department and Username columns
            worksheet.Column(1).Width = 15; // Department
            worksheet.Column(2).Width = 20; // Username

            // Score column formatting
            worksheet.Column(scoreColumn).Width = 12;
            worksheet.Column(scoreColumn).Style.Numberformat.Format = "#,##0.00";

            // Date column formatting
            worksheet.Column(col).Width = 12;
            worksheet.Column(col).Style.Numberformat.Format = "dd-MMM-yyyy";

            // Set row height for header
            worksheet.Row(1).Height = 40; // Adjust this value as needed

            // Set minimum row height for data rows to accommodate wrapped text
            for (int i = 2; i <= row - 1; i++)
            {
                worksheet.Row(i).Height = 35; // Adjust this value as needed
            }

            return package.GetAsByteArray();
        }










    }

}
