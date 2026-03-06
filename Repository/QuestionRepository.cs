using Microsoft.EntityFrameworkCore;
using PerformanceSurvey.Context;
using PerformanceSurvey.iRepository;
using PerformanceSurvey.Models;

namespace PerformanceSurvey.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Question> GetDepartmentQuestionAsync(int id)
        {
            return await _context.questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == id);
        }

        public async Task<IEnumerable<Question>> GetAllDepartmentQuestionsAsync()
        {
            return await _context.questions
                .Include(q => q.Options)
                .ToListAsync();
        }

        public async Task AddDepartmentMultiplechoiceQuestionAsync(Question question)
        {
            await _context.questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task AddDepartmentTextQuestionAsync(Question question)
        {
            await _context.questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDepartmentMultipleChoiceQuestionAsync(Question question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task<QuestionOption> GetOptionByIdAsync(int optionId)
        {
            return await _context.question_Option
                                 .FirstOrDefaultAsync(option => option.OptionId == optionId);
        }

        public async Task<IEnumerable<QuestionOption>> GetAllOptionsAsync()
        {
            return await _context.question_Option.ToListAsync();
        }

        public async Task UpdateDepartmentTextQuestionAsync(Question question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartmentQuestionAsync(int id)
        {
            var question = await GetDepartmentQuestionAsync(id);
            if (question != null)
            {
                _context.questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Question>> GetDepartmentQuestionsByDepartmentIdAsync(int departmentId)
        {
            return await _context.questions
                .Where(dq => dq.DepartmentId == departmentId && !dq.IsDisabled)
                .Include(dq => dq.Options) 
                .ToListAsync();
        }
        public async Task<IEnumerable<Question>> GetDepartmentQuestionsByDepartmentIdsAsync(IEnumerable<int> departmentId)
        {
            return await _context.questions
                .Where(dq => departmentId.Contains(dq.DepartmentId) && !dq.IsDisabled)
                .Include(dq => dq.Options) 
                .ToListAsync();
        }
    }

}
