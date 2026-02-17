using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly HistoryQuestDbContext _context;

    public QuestionRepository(HistoryQuestDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
    }

    public async Task<Question?> GetByIdAsync(Guid id)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<List<Question>> GetByTeacherIdAsync(Guid teacherId)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .Where(q => q.CreatedByTeacherId == teacherId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
