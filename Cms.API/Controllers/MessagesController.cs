using Cms.Service.DTOs;
using Cms.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IContactMessageService _messages;
    public MessagesController(IContactMessageService messages) => _messages = messages;

    /// <summary>Public contact-form submission.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateContactMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { message = "Name, email and message are required." });
        }

        var created = await _messages.CreateAsync(request);
        return Created($"/api/messages/{created.Id}", created);
    }

    /// <summary>Admin inbox.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _messages.GetAllAsync());

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount() => Ok(new { count = await _messages.CountUnreadAsync() });

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var updated = await _messages.MarkReadAsync(id);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _messages.DeleteAsync(id) ? NoContent() : NotFound();
}
