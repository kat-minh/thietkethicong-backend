using Cms.Service.DTOs;
using Cms.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize] // mutations require auth; reads are opened up below
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projects;

    public ProjectsController(IProjectService projects) => _projects = projects;

    /// <summary>List projects (public — used by the website and the admin grid).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(await _projects.GetAllAsync());

    /// <summary>Get a single project by slug (public).</summary>
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var project = await _projects.GetBySlugAsync(slug);
        return project is null ? NotFound() : Ok(project);
    }

    /// <summary>Create a project (admin only).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectMutationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required." });

        var created = await _projects.CreateAsync(request);
        if (created is null)
            return Conflict(new { message = "A project with this slug already exists." });

        return CreatedAtAction(nameof(GetBySlug), new { slug = created.Slug }, created);
    }

    /// <summary>Update a project by slug (admin only).</summary>
    [HttpPut("{slug}")]
    public async Task<IActionResult> Update(string slug, [FromBody] ProjectMutationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required." });

        try
        {
            var updated = await _projects.UpdateAsync(slug, request);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Delete a project by slug (admin only).</summary>
    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete(string slug)
    {
        var deleted = await _projects.DeleteAsync(slug);
        return deleted ? NoContent() : NotFound();
    }
}
