using Cms.Service.DTOs;
using Cms.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

[ApiController]
[Route("api/services")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IServiceItemService _services;
    public ServicesController(IServiceItemService services) => _services = services;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _services.GetAllAsync());

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var item = await _services.GetBySlugAsync(slug);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ServiceMutationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required." });

        var created = await _services.CreateAsync(request);
        return created is null
            ? Conflict(new { message = "A service with this slug already exists." })
            : CreatedAtAction(nameof(GetBySlug), new { slug = created.Slug }, created);
    }

    [HttpPut("{slug}")]
    public async Task<IActionResult> Update(string slug, [FromBody] ServiceMutationRequest request)
    {
        try
        {
            var updated = await _services.UpdateAsync(slug, request);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete(string slug)
        => await _services.DeleteAsync(slug) ? NoContent() : NotFound();
}
