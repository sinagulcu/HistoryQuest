using HistoryQuest.Application.Categories.DTOs;
using HistoryQuest.Application.Categories.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Teacher ,Admin")]
public class CategoryController : ControllerBase
{
    private readonly CreateCategoryCommand _createCategoryCommand;
    private readonly GetCategoriesQuery _getCategoriesQuery;
    private readonly GetCategoryByIdQuery _getCategoryByIdQuery;
    private readonly UpdateCategoryCommand _updateCategoryCommand;
    private readonly DeleteCategoryCommand _deleteCategoryCommand;

    public CategoryController(
        CreateCategoryCommand createCategoryCommand,
        GetCategoriesQuery getCategoriesQuery,
        GetCategoryByIdQuery getCategoryByIdQuery,
        UpdateCategoryCommand updateCategoryCommand,
        DeleteCategoryCommand deleteCategoryCommand)
    {
        _createCategoryCommand = createCategoryCommand;
        _getCategoriesQuery = getCategoriesQuery;
        _getCategoryByIdQuery = getCategoryByIdQuery;
        _updateCategoryCommand = updateCategoryCommand;
        _deleteCategoryCommand = deleteCategoryCommand;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var result = await _getCategoriesQuery.ExecuteAsync(includeDeleted, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _getCategoryByIdQuery.ExecuteAsync(id, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var categoryId = await _createCategoryCommand.ExecuteAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = categoryId }, new { id = categoryId });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await _updateCategoryCommand.ExecuteAsync(id, request, cancellationToken);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await _deleteCategoryCommand.ExecuteAsync(id, cancellationToken);

        return NoContent();
    }
}