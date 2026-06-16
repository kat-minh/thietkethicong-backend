using Cms.Service.DTOs;
using Cms.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

[ApiController]
[Route("api/settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ISiteSettingService _settings;
    public SettingsController(ISiteSettingService settings) => _settings = settings;

    /// <summary>Site configuration (public — used by the website).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get() => Ok(await _settings.GetAsync());

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SiteSettingDto dto)
        => Ok(await _settings.UpdateAsync(dto));
}
