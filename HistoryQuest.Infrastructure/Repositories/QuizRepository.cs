
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace HistoryQuest.Infrastructure.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly HistoryQuestDbContext _context;

    public QuizRepository(HistoryQuestDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
    }

    public async Task<List<Quiz>> GetByTeacherIdAsync(Guid teacherId, bool includeDeleted = false)
    {
        var query = _context.Quizzes
            .Include(q => q.QuizQuestions)
            .Include(q => q.Category)
            .Include(q => q.CreatedByTeacher)
            .Where(q => q.CreatedByTeacherId == teacherId);

        if (!includeDeleted)
            query = query.Where(q => !q.IsDeleted);

        return await query.ToListAsync();
    }

    public async Task<Quiz?> GetByIdAsync(Guid quizId)
    {
        return await _context.Quizzes
            .Include(q => q.QuizQuestions)
            .ThenInclude(qq => qq.Question)
            .ThenInclude(q => q.Options)
            .Include(q => q.Category)
            .Include(q => q.CreatedByTeacher)
            .FirstOrDefaultAsync(q => q.Id == quizId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
