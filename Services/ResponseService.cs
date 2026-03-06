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
            QuestionOption? questionOption = null;

            if (dto.OptionId != 0)
            {
                 questionOption = await _questionRepository.GetOptionByIdAsync(dto.OptionId);

                if (questionOption == null || questionOption.QuestionId != dto.QuestionId)
                {
                    throw new Exception("Invalid Option: The specified QuestionOption does not exist or does not belong to the specified question.");
                }
            }

            var response = new Response
            {
                QuestionId = dto.QuestionId,
                UserId = dto.UserId,
                DepartmentId = dto.DepartmentId,
                QuestionOption = questionOption,
                CreatedAt = DateTime.UtcNow,
                Score = CalculateScore(dto) 
            };

            await _responseRepository.SaveAsync(response);

            await UpdateAssignmentStatusAsync(dto.UserId, dto.QuestionId);


        }
        private async Task UpdateAssignmentStatusAsync(int userId, int questionId)
        {
            var assignment = await _assignmentQuestionRepository.GetAssignmentByUserAndQuestionAsync(userId, questionId);

            if (assignment != null)
            {
                assignment.status = 1; 
                await _assignmentQuestionRepository.UpdateAsync(assignment);
            }
        }

        private int CalculateScore(SaveMultipleChoiceResponseDto dto)
        {

            var option = _questionRepository.GetOptionByIdAsync(dto.OptionId).Result;
            if (option != null)
            {
                
                return option.Score; 
            }
            return 0;
        }

        public async Task<SaveTextResponseDto> SaveTextResponseAsync(SaveTextResponseDto textResponseDto)
        {
            var textResponse = new Response
            {
                QuestionId = textResponseDto.QuestionId,
                UserId = textResponseDto.UserId,
                ResponseText = textResponseDto.ResponseText,
                DepartmentId = textResponseDto.DepartmentId,
                                CreatedAt = DateTime.UtcNow
            };

            var savedResponse = await _responseRepository.AddAsync(textResponse);

            await UpdateAssignmentStatusAsync(textResponseDto.UserId, textResponseDto.QuestionId);

            return textResponseDto;

            

        }
        public async Task<List<GetResponsesByDepartmentIdDto>> GetResponsesByDepartmentIdAsync(int departmentId)
        {
            var responses = await _responseRepository.GetResponsesByDepartmentIdAsync(departmentId);

            if (responses == null || !responses.Any())
            {
                return new List<GetResponsesByDepartmentIdDto>();
            }

            var responseDtos = responses.Select(response => new GetResponsesByDepartmentIdDto
            {
                ResponseId = response.ResponseId,
                QuestionId = response.QuestionId,
                DepartmentId = response.DepartmentId,
                DepartmentName = response.Question?.Department?.DepartmentName ?? "Unknown", 
                ResponseText = string.IsNullOrEmpty(response.ResponseText) ? "NIL" : response.ResponseText, 
                Text = string.IsNullOrEmpty(response.QuestionOption?.Text) ? "NIL" : response.QuestionOption.Text, 
                Score = response.Score,
                CreatedAt = response.CreatedAt,
                QuestionText = response.Question?.QuestionText ?? "No question text"


            }).ToList();

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

      
        public async Task<byte[]> ExportResponsesToExcelAsync(int? departmentId = null)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;

            var responses = departmentId.HasValue
                ? await _responseRepository.GetResponsesByDepartmentIdAsync(departmentId.Value)
                : await _responseRepository.GetAllResponsesAsync();

            var groupedResponses = responses
                .GroupBy(r => new { r.UserId, r.DepartmentId, r.CreatedAt.Date })
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Responses");

            var questions = responses
                .Select(r => r.Question)
                .Distinct()
                .OrderBy(q => q.QuestionId)
                .ToList();

            int col = 1;
            worksheet.Cells[1, col++].Value = "Department";

            var questionStartCol = col;
            var questionEndCol = col + questions.Count - 1;

            foreach (var question in questions)
            {
                worksheet.Cells[1, col++].Value = question.QuestionText;
            }

            var totalScoreColumn = col;
            worksheet.Cells[1, col++].Value = "Total Score";
            var createdAtColumn = col;
            worksheet.Cells[1, col].Value = "Created At";

            int row = 2;
            foreach (var group in groupedResponses)
            {
                col = 1;

                worksheet.Cells[row, col++].Value = group.First().Question?.Department?.DepartmentName ?? "Unknown";

                double totalScore = 0;
                int scoreCount = 0;

                foreach (var question in questions)
                {
                    var response = group.FirstOrDefault(r => r.QuestionId == question.QuestionId);

                    if (response != null)
                    {
                        if (response.Score.HasValue) 
                        {
                            worksheet.Cells[row, col].Value = Math.Round((double)response.Score.Value, 2);
                            totalScore += (double)response.Score.Value;
                            scoreCount++;
                        }
                        else if (!string.IsNullOrWhiteSpace(response.ResponseText)) 
                        {
                            worksheet.Cells[row, col].Value = response.ResponseText;
                        }
                        else
                        {
                            worksheet.Cells[row, col].Value = "NIL";
                        }
                    }
                    else
                    {
                        worksheet.Cells[row, col].Value = "NIL";
                    }

                    col++;
                }

                double averageScore = scoreCount > 0 ? totalScore / scoreCount : 0;
                worksheet.Cells[row, totalScoreColumn].Value = Math.Round(averageScore, 2);

                worksheet.Cells[row, createdAtColumn].Value = group.Key.Date.ToString("dd-MMM-yyyy");

                row++;
            }

            using (var range = worksheet.Cells[1, 1, 1, col])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            for (int i = questionStartCol; i <= questionEndCol; i++)
            {
                var questionColumn = worksheet.Column(i);
                questionColumn.Width = 20;
                questionColumn.Style.WrapText = true;
                questionColumn.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            worksheet.Column(1).Width = 20;

            worksheet.Column(totalScoreColumn).Width = 12;
            worksheet.Column(totalScoreColumn).Style.Numberformat.Format = "#,##0.00";

            worksheet.Column(createdAtColumn).Width = 12;
            worksheet.Column(createdAtColumn).Style.Numberformat.Format = "dd-MMM-yyyy";

            worksheet.Row(1).Height = 40;

            for (int i = 2; i <= row - 1; i++)
            {
                worksheet.Row(i).Height = 35;
            }

            return package.GetAsByteArray();
        }








    }

}
