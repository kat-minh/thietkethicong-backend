using System.Security.Claims;
using Cms.Service.DTOs;
using Cms.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserAdminService _users;
    public UsersController(IUserAdminService users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _users.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username, email and password are required." });
        }

        var created = await _users.CreateAsync(request);
        return created is null
            ? Conflict(new { message = "Username or email already exists." })
            : Ok(created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Username and email are required." });

        try
        {
            var updated = await _users.UpdateAsync(id, request);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Prevent deleting yourself.
        var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentId == id.ToString())
            return BadRequest(new { message = "You cannot delete your own account." });

        return await _users.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
