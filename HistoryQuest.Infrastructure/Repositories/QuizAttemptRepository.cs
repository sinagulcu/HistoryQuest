

using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Repositories;

public class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly HistoryQuestDbContext _context;

    public QuizAttemptRepository(HistoryQuestDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(QuizAttempt attempt)
    {
        await _context.QuizAttempts.AddAsync(attempt);
    }

    public async Task<QuizAttempt?> GetActiveAttemptAsync(Guid quizId, Guid studentId, CancellationToken ct = default)
    {
        return await _context.QuizAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a =>
                a.QuizId == quizId &&
                a.StudentId == studentId &&
                !a.IsCompleted, ct);
    }

    public async Task<QuizAttempt?> GetByIdAsync(Guid attemptId)
    {
        return await _context.QuizAttempts
            .Include(a => a.Answers)
            .Include(a => a.Quiz)
            .FirstOrDefaultAsync(a => a.Id == attemptId);
    }

    public async Task<List<QuizAttempt>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.QuizAttempts
            .Include(a => a.Quiz)
            .Where(a => a.StudentId == studentId)
            .ToListAsync();
    }

    public void Update(QuizAttempt attempt)
    {
        _context.QuizAttempts.Update(attempt);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
