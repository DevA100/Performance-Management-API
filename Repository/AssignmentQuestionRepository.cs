using PerformanceSurvey.Context;
using PerformanceSurvey.iRepository;
using PerformanceSurvey.Models;
using Microsoft.EntityFrameworkCore;

public class AssignmentQuestionRepository : IAssignmentQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AssignQuestionsToMultipleUsersAsync(List<AssignmentQuestion> assignments)
    {
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task AssignQuestionsToDepartmentAsync(List<AssignmentQuestion> assignments)
    {
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task AssignDiffQuestionsToDepartmentAsync(List<AssignmentQuestion> assignments)
    {
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task AssignDiffQuestionsToDiffDepartmentAsync(List<AssignmentQuestion> assignments)
    {
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }
    public async Task AssignQuestionsToSingleUsersAsync(List<AssignmentQuestion> assignments)
    {
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }


    public async Task<IEnumerable<AssignmentQuestion>> GetAssignmentByUserIdAsync(int userId)
    {
        var assignments = await _context.assignment_Question
            .Include(a => a.Department) 
            .Include(a => a.Question)   
            .Where(a => a.UserId == userId) 
            .ToListAsync();

        return assignments;
    }


    public async Task<IEnumerable<AssignmentQuestion>> GetAssignmentByUserIdsAsync(IEnumerable<int> userIds)
    {
        var assignments = await _context.assignment_Question
            .Where(a => userIds.Contains(a.UserId))
            .Include(a => a.Department) 
            .Include(a => a.Question)   
            .ToListAsync();

        return assignments;
    }
    public async Task DeleteAssignmentsAsync(IEnumerable<AssignmentQuestion> assignments)
    {
        _context.assignment_Question.RemoveRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AssignmentQuestion assignment)
    {
        _context.assignment_Question.Update(assignment); 
        await _context.SaveChangesAsync();
    }

    public async Task<AssignmentQuestion> GetAssignmentByUserAndQuestionAsync(int userId, int questionId)
    {
        return await _context.assignment_Question
            .FirstOrDefaultAsync(a => a.UserId == userId && a.QuestionId == questionId);
    }

    public async Task<IEnumerable<User>> GetUsersWithPendingAssignmentsAsync()
    {
        return await _context.assignment_Question
            .Where(a => a.status == 0)  
            .Select(a => a.User)        
            .Distinct()                 
            .ToListAsync();
    }

    public async Task<IEnumerable<AssignmentQuestion>> GetPendingAssignmentsAsync()
    {
        return await _context.assignment_Question
            .Where(a => a.status == 0) 
            .ToListAsync();
    }

    public async Task DeletePendingAssignmentsAsync(IEnumerable<AssignmentQuestion> assignments)
    {
        _context.assignment_Question.RemoveRange(assignments);
        await _context.SaveChangesAsync();
    }

}