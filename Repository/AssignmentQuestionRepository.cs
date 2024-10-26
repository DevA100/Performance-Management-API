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
        // Add assignments to the database
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task AssignQuestionsToDepartmentAsync(List<AssignmentQuestion> assignments)
    {
        // Add assignments to the database
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task AssignDiffQuestionsToDepartmentAsync(List<AssignmentQuestion> assignments)
    {
        // Add assignments to the database
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task AssignDiffQuestionsToDiffDepartmentAsync(List<AssignmentQuestion> assignments)
    {
        // Add assignments to the database
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }
    public async Task AssignQuestionsToSingleUsersAsync(List<AssignmentQuestion> assignments)
    {
        // Add assignments to the database
        _context.assignment_Question.AddRange(assignments);
        await _context.SaveChangesAsync();
    }


    public async Task<IEnumerable<AssignmentQuestion>> GetAssignmentByUserIdAsync(int userId)
    {
        var assignments = await _context.assignment_Question
            .Include(a => a.Department) // Include related Department entity
            .Include(a => a.Question)   // Include related Question entity
            .Where(a => a.UserId == userId) // Fetch all matching records for the user
            .ToListAsync();

        return assignments;
    }


    public async Task<IEnumerable<AssignmentQuestion>> GetAssignmentByUserIdsAsync(IEnumerable<int> userIds)
    {
        // Fetch the assignments for multiple users
        var assignments = await _context.assignment_Question
            .Where(a => userIds.Contains(a.UserId))
            .Include(a => a.Department) // Include related Department entity
            .Include(a => a.Question)   // Include related Question entity
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
        _context.assignment_Question.Update(assignment); // Assuming 'assignment_Question' is your DbSet
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
            .Where(a => a.status == 0)  // Filter for status 0
            .Select(a => a.User)        // Select users
            .Distinct()                 // Ensure unique users
            .ToListAsync();
    }

}