using PerformanceSurvey.iRepository;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models;
using Microsoft.EntityFrameworkCore;
using PerformanceSurvey.Repository;
using PerformanceSurvey.Models.RequestDTOs.ResponseDTOs;
using PerformanceSurvey.Models.RequestDTOs;

namespace PerformanceSurvey.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repository;

        public QuestionService(IQuestionRepository repository)
        {
            _repository = repository;
        }

        public async Task<MultipleChoiceQuestionResponse> CreateDepartmentMultipleQuestionAsync(MultipleChoiceQuestionRequest questionDto)
        {
            var question = new Question
            {
                QuestionText = questionDto.QuestionText,
                DepartmentId = questionDto.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                Options = questionDto.Options?.Select(o => new QuestionOption 
                {
                    Text = o.Text,
                    Score = o.Score ?? 0 
                }).ToList()
            };

            await _repository.AddDepartmentMultiplechoiceQuestionAsync(question);

            var response = new MultipleChoiceQuestionResponse
            {
                QuestionText = question.QuestionText,
                Options = question.Options.Select(o => new QuestionOptionDto
                {
                    Text = o.Text,
                    Score = o.Score
                }).ToList()
            };

            return response; 
        }

        public async Task<TextQuestionResponse> CreateDepartmentTextQuestionAsync(TextQuestionReqest questionDto)
        {
            var question = new Question
            {
                QuestionText = questionDto.QuestionText,
                DepartmentId = questionDto.DepartmentId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddDepartmentTextQuestionAsync(question);

            var response = new TextQuestionResponse
            {
                QuestionText = question.QuestionText,
            };

            return response; 
        }


        public async Task<QuestionDto> GetDepartmentQuestionAsync(int id)
        {
            var question = await _repository.GetDepartmentQuestionAsync(id);

            if (question == null)
            {
                return null; 
            }

            var questionDto = new QuestionDto
            {
                QuestionText = question.QuestionText,
                Options = question.Options?.Select(o => new QuestionOptionDto
                {
                    Text = o.Text,
                    Score = o.Score 
                }).ToList()
            };

            return questionDto;
        }


        public async Task<IEnumerable<QuestionDto>> GetAllDepartmentQuestionsAsync()
        {
            var questions = await _repository.GetAllDepartmentQuestionsAsync();

            return questions.Select(q => new QuestionDto
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                Options = q.Options?.Select(o => new QuestionOptionDto
                {
                    Text = o.Text,
                }).ToList()
            }).ToList();
        }

        public async Task<MultipleChoiceQuestionResponse> UpdateDepartmentMultipleChoiceQuestionAsync(int id, MultipleChoiceQuestionRequest questionDto)
        {
            var question = await _repository.GetDepartmentQuestionAsync(id);

            if (question == null)
            {
                return null; 
            }

            question.QuestionText = questionDto.QuestionText;
            question.DepartmentId = questionDto.DepartmentId;

            foreach (var option in question.Options.ToList())
            {
                if (!questionDto.Options.Any(o => o.OptionId == option.OptionId))
                {
                    await _repository.DeleteDepartmentQuestionAsync(option.OptionId); 
                }
                else
                {
                    var updatedOption = questionDto.Options.First(o => o.OptionId == option.OptionId);
                    option.Text = updatedOption.Text;
                    option.Score = updatedOption.Score ?? 0;
                }
            }

            foreach (var newOptionDto in questionDto.Options.Where(o => o.OptionId == 0))
            {
                question.Options.Add(new QuestionOption
                {
                    Text = newOptionDto.Text,
                    Score = newOptionDto.Score ?? 0,
                    QuestionId = id
                });
            }

            await _repository.UpdateDepartmentMultipleChoiceQuestionAsync(question);

            var response = new MultipleChoiceQuestionResponse
            {
                QuestionText = question.QuestionText,
                Options = question.Options.Select(o => new QuestionOptionDto
                {
                    Text = o.Text,
                    Score = o.Score
                }).ToList()
            };

            return response;
        }

        public async Task<QuestionOptionDto> GetOptionByIdAsync(int optionId)
        {
            var option = await _repository.GetOptionByIdAsync(optionId);

            if (option == null)
            {
                return null; 
            }

            return new QuestionOptionDto
            {
                OptionId = option.OptionId,
                Text = option.Text,
                Score = option.Score
            };
        }

        public async Task<IEnumerable<QuestionOptionDto>> GetAllOptionsAsync()
        {
            var options = await _repository.GetAllOptionsAsync();

            return options.Select(option => new QuestionOptionDto
            {
                OptionId = option.OptionId,
                Text = option.Text,
                Score = option.Score
            }).ToList();
        }


        public async Task<TextQuestionResponse> UpdateDepartmentTextQuestionAsync(int id, TextQuestionReqest questionDto)
        {
            var question = await _repository.GetDepartmentQuestionAsync(id);

            if (question == null)
            {
                return null; 
            }

            question.QuestionText = questionDto.QuestionText;
            question.DepartmentId = questionDto.DepartmentId;

            await _repository.UpdateDepartmentTextQuestionAsync(question);

            var response = new TextQuestionResponse
            {
                QuestionText = question.QuestionText,
            };

            return response;
        }


        public async Task<Question> DeleteDepartmentQuestionAsync(int id)
        {
            var question = await _repository.GetDepartmentQuestionAsync(id);
            if (question != null)
            {
                await _repository.DeleteDepartmentQuestionAsync(id);
            }
            return question;
        }

        public async Task<IEnumerable<GetQuestionByDepartmentDto>> GetDepartmentQuestionsByDepartmentIdAsync(int departmentId)
        {
            var questions = await _repository.GetDepartmentQuestionsByDepartmentIdAsync(departmentId);
            return questions.Select(q => new GetQuestionByDepartmentDto
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                Options = q.Options?.Select(o => new QuestionOptionDto
                {
                    Text = o.Text,
                }).ToList()
            }) .ToList();
        }

        public async Task<IEnumerable<GetQuestionByDepartmentDto>> GetDepartmentQuestionsByDepartmentIdsAsync(IEnumerable<int> departmentIds)
        {
            var questions = await _repository.GetDepartmentQuestionsByDepartmentIdsAsync(departmentIds);

            var questionDtos = questions.Select(q => new GetQuestionByDepartmentDto
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                Options = q.Options?.Select(o => new QuestionOptionDto
                {
                    Text = o.Text,
                }).ToList()
            });

            return questionDtos;
        }

    }

}
