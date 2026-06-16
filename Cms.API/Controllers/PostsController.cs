using Cms.Service.DTOs;
using Cms.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _posts;
    public PostsController(IPostService posts) => _posts = posts;

    /// <summary>All posts incl. drafts (admin grid).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _posts.GetAllAsync());

    /// <summary>Published posts only (public website).</summary>
    [HttpGet("published")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublished() => Ok(await _posts.GetPublishedAsync());

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var post = await _posts.GetBySlugAsync(slug);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostMutationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required." });

        var created = await _posts.CreateAsync(request);
        return created is null
            ? Conflict(new { message = "A post with this slug already exists." })
            : CreatedAtAction(nameof(GetBySlug), new { slug = created.Slug }, created);
    }

    [HttpPut("{slug}")]
    public async Task<IActionResult> Update(string slug, [FromBody] PostMutationRequest request)
    {
        try
        {
            var updated = await _posts.UpdateAsync(slug, request);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete(string slug)
        => await _posts.DeleteAsync(slug) ? NoContent() : NotFound();
}
