using HistoryQuest.Application.Questions.DTOs;
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

    public async Task<List<Question>> GetAllQuestionsAsync(bool includedDeleted = false)
    {
        var query = _context.Questions
            .Include(q => q.Options)
            .AsQueryable();

        if (!includedDeleted)
            query = query.Where(q => !q.IsDeleted);

        return await query
            .OrderByDescending(q => q.Id)
            .ToListAsync();
    }

    public async Task<List<QuestionListItemDto>> GetAllForAdminPanelAsync(bool includedDeleted = false)
    {
        var query = _context.Questions.AsQueryable();

        if(!includedDeleted)
            query = query.Where(q => !q.IsDeleted);

        return await query
            .Select(q => new QuestionListItemDto
            {
                Id = q.Id,
                Text = q.Text,
                TextPreview = q.Text.Length > 100 ? q.Text.Substring(0, 100) + "..." : q.Text,
                Difficulty = q.Difficulty.ToString(),
                CategoryId = q.CategoryId,
                CategoryName = _context.Categories
                .Where(c => c.Id == q.CategoryId)
                .Select(c => c.Name)
                .FirstOrDefault(),
                CreatedByTeacherId = q.CreatedByTeacherId,
                CreatedByTeacherUserName = _context.Users
                .Where(u => u.Id == q.CreatedByTeacherId)
                .Select(u => u.UserName)
                .FirstOrDefault() ?? "",
                CreatedByTeacherFullName = _context.Users
                .Where(u => u.Id == q.CreatedByTeacherId)
                .Select(u => (u.FirstName + " " + u.LastName).Trim())
                .FirstOrDefault() ?? "",
                IsDeleted = q.IsDeleted,
                DeletedAt = q.DeletedAt,
                Options = q.Options.Select(o => new OptionForTeacherDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            })
        .OrderByDescending(x => x.Id)
        .ToListAsync();
    }

    public async Task<List<Question>> GetByTeacherIdAsync(Guid teacherId, bool includeDeleted = false)
    {
        var query = _context.Questions
            .Include(q => q.Options)
            .Where(q => q.CreatedByTeacherId == teacherId);

        if(!includeDeleted)
            query = query.Where(q => !q.IsDeleted);

        return await query.ToListAsync();
    }

    public async Task<Question?> GetByIdIncludingDeletedAsync(Guid id)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
