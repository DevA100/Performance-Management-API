using Microsoft.EntityFrameworkCore;
using PerformanceSurvey.Context;
using PerformanceSurvey.iRepository;
using PerformanceSurvey.Models;

namespace PerformanceSurvey.Repository
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly ApplicationDbContext _context;

        public ResponseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(Response response)
        {
            _context.responses.Add(response);
            await _context.SaveChangesAsync();
        }

        public async Task<Response> AddAsync(Response textResponse)
        {
            _context.responses.Add(textResponse);
            await _context.SaveChangesAsync();
            return textResponse;
        }

        public async Task<List<Response>> GetResponsesByDepartmentIdAsync(int departmentId)
        {
            return await _context.responses
                .Include(r => r.User)
                .Include(r => r.Question) 
                .ThenInclude(q => q.Department) 
                .Include(r => r.QuestionOption) 
                .Where(r => r.Question.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task ClearResponsesByDepartmentIdAsync(int departmentId)
        {
            var responses = await _context.responses
                .Where(r => r.Question.DepartmentId == departmentId)
                .ToListAsync();

            _context.responses.RemoveRange(responses);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllResponsesAsync()
        {
            var allResponses = await _context.responses.ToListAsync();
            _context.responses.RemoveRange(allResponses);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Response>> GetAllResponsesAsync()
        {
            return await _context.responses
                .Include(r => r.Question)
                .ThenInclude(q => q.Department)
                .ToListAsync();
        }


    }

}
