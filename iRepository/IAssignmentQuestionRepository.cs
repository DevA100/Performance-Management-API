using PerformanceSurvey.Models;

namespace PerformanceSurvey.iRepository
{
    public interface IAssignmentQuestionRepository
    {
        Task AssignQuestionsToMultipleUsersAsync(List<AssignmentQuestion> assignments);
        Task AssignQuestionsToDepartmentAsync(List<AssignmentQuestion> assignments);
        Task AssignDiffQuestionsToDepartmentAsync(List<AssignmentQuestion> assignments);
        Task AssignDiffQuestionsToDiffDepartmentAsync(List<AssignmentQuestion> assignments);
        Task AssignQuestionsToSingleUsersAsync(List<AssignmentQuestion> assignments);
        Task<IEnumerable<AssignmentQuestion>> GetAssignmentByUserIdAsync(int  userId);

        Task<IEnumerable<AssignmentQuestion>> GetAssignmentByUserIdsAsync(IEnumerable<int> userIds);
        Task DeleteAssignmentsAsync(IEnumerable<AssignmentQuestion> assignments);
        Task<AssignmentQuestion> GetAssignmentByUserAndQuestionAsync(int userId, int questionId);
        Task UpdateAsync(AssignmentQuestion assignment);
        Task<IEnumerable<User>> GetUsersWithPendingAssignmentsAsync();
        Task<IEnumerable<AssignmentQuestion>> GetPendingAssignmentsAsync();
        Task DeletePendingAssignmentsAsync(IEnumerable<AssignmentQuestion> assignments);

    }
}
