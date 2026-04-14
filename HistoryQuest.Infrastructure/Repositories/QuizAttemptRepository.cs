

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
            .AsNoTracking()
            .Include(a => a.Answers)
            .Where(a => a.QuizId == quizId && a.StudentId == studentId && !a.IsCompleted)
            .OrderByDescending(a => a.StartedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<QuizAttempt?> GetByIdAsync(Guid attemptId, CancellationToken ct = default)
    {
        return await _context.QuizAttempts
            .AsNoTracking()
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId, ct);
    }

    public async Task<List<QuizAttempt>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.QuizAttempts
            .AsNoTracking()
            .Include(a => a.Quiz)
            .Where(a => a.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<int> CompleteAttemptAsync(Guid attemptId, int score, IEnumerable<AttemptAnswer> answers, CancellationToken ct = default)
    {
        await _context.AttemptAnswers.AddRangeAsync(answers, ct);

        var affected = await _context.QuizAttempts
            .Where(a => a.Id == attemptId && !a.IsCompleted)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(a => a.Score, score)
            .SetProperty(a => a.Status, Domain.Models.AttemptStatus.Completed)
            .SetProperty(a => a.IsCompleted, true)
            .SetProperty(a => a.CompletedAt, DateTime.UtcNow),
            ct);

        if (affected > 0)
            await _context.SaveChangesAsync(ct);

        return affected;
    }

    public async Task<int> MarkSettledAsync(Guid attemptId, CancellationToken ct = default)
    {
        var affected = await _context.QuizAttempts
            .Where(a => a.Id == attemptId && !a.IsSettled)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(a => a.IsSettled, true)
            .SetProperty(a => a.SettledAt, DateTime.UtcNow),
            ct);

        return affected;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
