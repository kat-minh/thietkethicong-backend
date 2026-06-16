using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

/// <summary>Job openings. GET public (careers page), mutations require auth.</summary>
[Route("api/jobs")]
public class JobsController : SimpleCrudController<JobPosting>
{
    public JobsController(ISimpleRepository<JobPosting> r) : base(r) { }
}

/// <summary>Public application form submissions + admin inbox.</summary>
[ApiController]
[Route("api/job-applications")]
[Authorize]
public class JobApplicationsController : ControllerBase
{
    private readonly ISimpleRepository<JobApplication> _repo;
    public JobApplicationsController(ISimpleRepository<JobApplication> repo) => _repo = repo;

    public record JobApplicationRequest(
        string Name, string Phone, string Email, string Position,
        string? CoverLetter, string CvUrl);

    /// <summary>Public submission from the "Ứng tuyển" popup.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] JobApplicationRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) ||
            string.IsNullOrWhiteSpace(req.Phone) ||
            string.IsNullOrWhiteSpace(req.Email) ||
            string.IsNullOrWhiteSpace(req.Position) ||
            string.IsNullOrWhiteSpace(req.CvUrl))
        {
            return BadRequest(new { message = "Vui lòng nhập đủ thông tin và đính kèm CV." });
        }

        var entity = new JobApplication
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Phone = req.Phone.Trim(),
            Email = req.Email.Trim(),
            Position = req.Position.Trim(),
            CoverLetter = req.CoverLetter?.Trim() ?? string.Empty,
            CvUrl = req.CvUrl.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
        };
        await _repo.AddAsync(entity);
        return Created($"/api/job-applications/{entity.Id}", new { entity.Id });
    }

    /// <summary>Admin inbox, newest first.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var all = await _repo.GetAllAsync();
        return Ok(all.OrderByDescending(a => a.CreatedAt));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
    {
        var all = await _repo.GetAllAsync();
        return Ok(new { count = all.Count(a => !a.IsRead) });
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var a = await _repo.GetByIdAsync(id);
        if (a is null) return NotFound();
        a.IsRead = true;
        await _repo.UpdateAsync(a);
        return Ok(a);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _repo.DeleteAsync(id) ? NoContent() : NotFound();
}
