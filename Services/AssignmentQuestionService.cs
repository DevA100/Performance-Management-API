using PerformanceSurvey.iRepository;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Repository;
using PerformanceSurvey.Utilities;
using System.Text;
using System.Security.Cryptography;
using PerformanceSurvey.Models.DTOs.ResponseDTOs;


namespace PerformanceSurvey.Services
{
    public class AssignmentQuestionService : IAssignmentQuestionService
    {
        private readonly IAssignmentQuestionRepository _assignmentQuestionRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IDepartmentRepository _departmentRepository;

        public AssignmentQuestionService(
            IAssignmentQuestionRepository assignmentQuestionRepository,
            IQuestionRepository questionRepository,
            IUserRepository userRepository, 
            IEmailService emailService,
            IDepartmentRepository departmentRepository)
        {
            _assignmentQuestionRepository = assignmentQuestionRepository;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _departmentRepository = departmentRepository;
        }

        public async Task AssignQuestionsToMultipleUsersAsync(AssignmentQuestionMultipleDto assignmentQuestionMultipleDto)
        {
            // Step 1: Fetch all questions for the provided department IDs
            var allQuestions = await _questionRepository.GetDepartmentQuestionsByDepartmentIdsAsync(assignmentQuestionMultipleDto.DepartmentId);

            // Step 3: Fetch all users from the Users provided in the DTO
            var allUsers = await _userRepository.GetUsersByIdsAsync(assignmentQuestionMultipleDto.UserId);

            // Step 5: Create assignments for the users
            var assignments = allUsers.SelectMany(user =>
                allQuestions.Select(question => new AssignmentQuestion
                {
                    UserId = user.UserId,
                    QuestionId = question.QuestionId,
                    DepartmentId = question.DepartmentId,
                    AssignedAt = DateTime.UtcNow
                })).ToList();

            // Step 6: Save the assignments in the database using the repository method
            await _assignmentQuestionRepository.AssignQuestionsToMultipleUsersAsync(assignments);

            // Step 7: Generate passwords, save them, and send emails
            foreach (var user in allUsers)
            {
                // Generate a new secure password
                var newPassword = PasswordGenerator.GenerateSecurePassword();

                // Hash the password if needed (using your preferred hashing mechanism)
                var hashedPassword = HashPassword(newPassword); // Implement HashPassword based on your requirements

                // Save the hashed password to the user's password field
                user.Password = hashedPassword;

                // Update the user's password in the database
                await _userRepository.UpdateUserPasswordAsync(user);

                // Compose the email content
                var emailBody = $"Hello {user.Name},<br>Your new password is: <b>{newPassword}</b>";

                // Send the new password to the user via email
                await _emailService.SendEmailAsync(user.UserEmail, "Your New Password", emailBody);
            }
        }

        // Example of a simple HashPassword function
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }





        public async Task AssignQuestionsToDepartmentAsync(AssignQuestionsToDepartmentDto dto)
        {
            // Step 1: Fetch all question IDs from the source department
            var questionIds = await _questionRepository.GetDepartmentQuestionsByDepartmentIdAsync(dto.SourceDepartmentId);

            // Step 2: Fetch all users in the target department
            var usersInTargetDepartment = await _userRepository.GetUsersByDepartmentIdAsync(dto.TargetDepartmentId);

            // Step 3: Create assignments for the users in the target department
            var assignments = usersInTargetDepartment.SelectMany(user =>
                questionIds.Select(questionId => new AssignmentQuestion
                {
                    UserId = user.UserId,
                    QuestionId = questionId.QuestionId,
                    DepartmentId = dto.SourceDepartmentId, // Assigning questions from source department to users in the target department
                    AssignedAt = DateTime.UtcNow
                })).ToList();

            // Step 4: Save the assignments in the database using the repository method
            await _assignmentQuestionRepository.AssignQuestionsToDepartmentAsync(assignments);

            
            // Step 5: Generate passwords, save them, and send emails to users in the target department
            foreach (var user in usersInTargetDepartment)
            {
                // Generate a new secure password
                var newPassword = PasswordGenerator.GenerateSecurePassword();

                // Hash the password
                var hashedPassword = HashPassword(newPassword); // Implement HashPassword method as shown below

                // Save the hashed password to the user's password field
                user.Password = hashedPassword;

                // Update the user's password in the database
                await _userRepository.UpdateUserPasswordAsync(user);

                // Compose the email content
                var emailBody = $"Hello {user.Name},<br>Your new password is: <b>{newPassword}</b>";

                // Send the new password to the user via email
                await _emailService.SendEmailAsync(user.UserEmail, "Your New Password", emailBody);
            }
        }

        // Example of a simple HashPassword function



        public async Task AssignDiffQuestionsToDepartmentAsync(AssignDiffQuestionsToDepartmentDto dto)
        {

            // Step 1: Fetch all question IDs from the source departments
            var questionIds = await _questionRepository.GetDepartmentQuestionsByDepartmentIdsAsync(dto.SourceDepartmentId);

            // Step 2: Fetch all users in the target department
            var usersInTargetDepartment = await _userRepository.GetUsersByDepartmentIdAsync(dto.TargetDepartmentId);

            // Step 3: Create assignments for the users in the target department
            var assignments = usersInTargetDepartment.SelectMany(user =>
                dto.SourceDepartmentId.SelectMany(departmentId =>
                    questionIds.Where(q => q.DepartmentId == departmentId).Select(question => new AssignmentQuestion
                    {
                        UserId = user.UserId,
                        QuestionId = question.QuestionId,
                        DepartmentId = departmentId,
                        AssignedAt = DateTime.UtcNow
                    }))).ToList();

            // Step 4: Save the assignments in the database using the repository method
            await _assignmentQuestionRepository.AssignQuestionsToDepartmentAsync(assignments);

            // Step 5: Generate passwords, hash them, update user records, and send emails
            foreach (var user in usersInTargetDepartment)
            {
                // Generate a new secure password
                var newPassword = PasswordGenerator.GenerateSecurePassword();

                // Hash the password
                var hashedPassword = HashPassword(newPassword);

                // Update the user's password in the database
                user.Password = hashedPassword;
                await _userRepository.UpdateUserPasswordAsync(user);

                // Compose the email content
                var emailBody = $"Hello {user.Name},<br>Your new password is: <b>{newPassword}</b>";

                // Send the new password to the user via email
                await _emailService.SendEmailAsync(user.UserEmail, "Your New Password", emailBody);
            }
        }





        public async Task AssignDiffQuestionsToDiffDepartmentAsync(AssignDiffQuestionsToDiffDepartmentDto dto)
        {

            // Step 1: Fetch all question IDs from the source departments
            var questionIds = await _questionRepository.GetDepartmentQuestionsByDepartmentIdsAsync(dto.SourceDepartmentId);

            // Step 2: Fetch all users in the target department
            var usersInTargetDepartment = await _userRepository.GetUsersByDepartmentIdsAsync(dto.TargetDepartmentId);

            // Step 3: Create assignments for the users in the target department
            var assignments = usersInTargetDepartment.SelectMany(user =>
                dto.SourceDepartmentId.SelectMany(departmentId =>
                    questionIds.Where(q => q.DepartmentId == departmentId).Select(question => new AssignmentQuestion
                    {
                        UserId = user.UserId,
                        QuestionId = question.QuestionId,
                        DepartmentId = departmentId,
                        AssignedAt = DateTime.UtcNow
                    }))).ToList();

            // Step 4: Save the assignments in the database using the repository method
            await _assignmentQuestionRepository.AssignQuestionsToDepartmentAsync(assignments);

            // Step 5: Generate passwords, hash them, update user records, and send emails
            foreach (var user in usersInTargetDepartment)
            {
                // Generate a new secure password
                var newPassword = PasswordGenerator.GenerateSecurePassword();

                // Hash the password
                var hashedPassword = HashPassword(newPassword);

                // Update the user's password in the database
                user.Password = hashedPassword;
                await _userRepository.UpdateUserPasswordAsync(user);

                // Compose the email content
                var emailBody = $"Hello {user.Name},<br>Your new password is: <b>{newPassword}</b>";

                // Send the new password to the user via email
                await _emailService.SendEmailAsync(user.UserEmail, "Your New Password", emailBody);
            }
        }






        public async Task AssignQuestionsToSingleUsersAsync(AssignmentQuestionSingleUserDto assignmentQuestionSingleUserDto)
        {
            
            // Fetch all questions by department ID
            var questions = await _questionRepository.GetDepartmentQuestionsByDepartmentIdAsync(assignmentQuestionSingleUserDto.DepartmentId);

            // Fetch the user by ID
            var user = await _userRepository.GetUserByIdAsync(assignmentQuestionSingleUserDto.UserId);

            // Check if the user exists
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // Create assignments for the single user
            var assignments = questions.Select(question => new AssignmentQuestion
            {
                UserId = assignmentQuestionSingleUserDto.UserId,
                QuestionId = question.QuestionId, // Use the question IDs fetched from the department
                DepartmentId = assignmentQuestionSingleUserDto.DepartmentId,
                AssignedAt = DateTime.UtcNow
            }).ToList();

            // Add assignments to the database using the repository method
            await _assignmentQuestionRepository.AssignQuestionsToSingleUsersAsync(assignments);

            // Generate a new secure password
            var newPassword = PasswordGenerator.GenerateSecurePassword();

            // Hash the password
            var hashedPassword = HashPassword(newPassword);

            // Update the user's password in the database
            user.Password = hashedPassword;
            await _userRepository.UpdateUserPasswordAsync(user);

            // Compose the email content
            var emailBody = $"Hello {user.Name},<br>Your new password is: <b>{newPassword}</b>";

            // Send the new password to the user via email
            await _emailService.SendEmailAsync(user.UserEmail, "Your New Password", emailBody);
        }


        // Get assignment questions for a single user by UserId
        public async Task<IEnumerable<GetQuestionByDepartmentDto>> GetAssignmentQuestionsByUserIdAsync(int userId)
        {
            var assignments = await _assignmentQuestionRepository.GetAssignmentByUserIdAsync(userId);

            if (!assignments.Any())
            {
                return new List<GetQuestionByDepartmentDto>();
            }

            var departmentIds = assignments.Select(a => a.DepartmentId).Distinct().ToList();
            var departments = await _departmentRepository.GetDepartmentsByIdsAsync(departmentIds); // Fetch departments

            var questions = await _questionRepository.GetDepartmentQuestionsByDepartmentIdsAsync(departmentIds);

            var questionDtos = questions.Select(q =>
            {
                var department = departments.FirstOrDefault(d => d.DepartmentId == q.DepartmentId);

                return new GetQuestionByDepartmentDto
                {
                    DepartmentId = q.DepartmentId,
                    DepartmentName = q.Department.DepartmentName,
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Options = q.Options?.Select(o => new QuestionOptionDto
                    {
                        OptionId = o.OptionId,
                        Text = o.Text,
                    }).ToList()
                };
            }).ToList();

            return questionDtos;
        }


        // Get assignment questions for multiple users by UserIds
        public async Task<IEnumerable<GetQuestionByDepartmentDto>> GetAssignmentQuestionsByUserIdsAsync(IEnumerable<int> userIds)
        {
            // Fetch all assignments for the given user IDs
            var assignments = await _assignmentQuestionRepository.GetAssignmentByUserIdsAsync(userIds);

            // Handle case where no assignments are found
            if (!assignments.Any())
            {
                return new List<GetQuestionByDepartmentDto>(); // Return empty list if no assignments are found
            }

            // Collect all unique department IDs from the assignments
            var departmentIds = assignments.Select(a => a.DepartmentId).Distinct().ToList();

            // Fetch all questions associated with these department IDs
            var questions = await _questionRepository.GetDepartmentQuestionsByDepartmentIdsAsync(departmentIds);

            // Map questions to DTOs if necessary
            var questionDtos = questions.Select(q => new GetQuestionByDepartmentDto
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                Options = q.Options?.Select(o => new QuestionOptionDto
                {
                    OptionId = o.OptionId,
                    Text = o.Text,
                    // Map other properties if necessary
                }).ToList()
                // Add other mappings as needed
            }).ToList();

            return questionDtos;
        }

        public async Task DeleteAnsweredAssignmentQuestionsByUserIdAsync(int userId)
        {
            // Fetch all assignments for the user
            var assignments = await _assignmentQuestionRepository.GetAssignmentByUserIdAsync(userId);

            // Filter out assignments where the question status is 'answered' (status = 1)
            var answeredAssignments = assignments.Where(a => a.status == 1).ToList();

            // If there are any answered assignments, delete them
            if (answeredAssignments.Any())
            {
                await _assignmentQuestionRepository.DeleteAssignmentsAsync(answeredAssignments);
            }
        }


        public async Task<IEnumerable<UserResponse>> GetUsersWithPendingAssignmentsAsync()
        {
            var users = await _assignmentQuestionRepository.GetUsersWithPendingAssignmentsAsync();

            return users.Select(u => new UserResponse
            {
                UserId = u.UserId,
                Name = u.Name,
                UserEmail = u.UserEmail
            }).ToList();
        }


    }
}