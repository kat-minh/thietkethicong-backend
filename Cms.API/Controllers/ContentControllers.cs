using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

/// <summary>Generic CRUD: GET is public, mutations require auth.</summary>
[ApiController]
public abstract class SimpleCrudController<T> : ControllerBase where T : class, IContentEntity, new()
{
    protected readonly ISimpleRepository<T> Repo;
    protected SimpleCrudController(ISimpleRepository<T> repo) => Repo = repo;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await Repo.GetAllAsync());

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] T input)
    {
        if (input.Id == Guid.Empty) input.Id = Guid.NewGuid();
        await Repo.AddAsync(input);
        return Ok(input);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] T input)
    {
        input.Id = id;
        return await Repo.UpdateAsync(input) ? Ok(input) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
        => await Repo.DeleteAsync(id) ? NoContent() : NotFound();
}

[Route("api/testimonials")]
public class TestimonialsController : SimpleCrudController<Testimonial>
{
    public TestimonialsController(ISimpleRepository<Testimonial> r) : base(r) { }
}

[Route("api/team")]
public class TeamController : SimpleCrudController<TeamMember>
{
    public TeamController(ISimpleRepository<TeamMember> r) : base(r) { }
}

[Route("api/process")]
public class ProcessController : SimpleCrudController<ProcessStep>
{
    public ProcessController(ISimpleRepository<ProcessStep> r) : base(r) { }
}

[Route("api/stats")]
public class StatsController : SimpleCrudController<StatItem>
{
    public StatsController(ISimpleRepository<StatItem> r) : base(r) { }
}

[Route("api/faqs")]
public class FaqsController : SimpleCrudController<Faq>
{
    public FaqsController(ISimpleRepository<Faq> r) : base(r) { }
}

[Route("api/partners")]
public class PartnersController : SimpleCrudController<Partner>
{
    public PartnersController(ISimpleRepository<Partner> r) : base(r) { }
}

[Route("api/philosophy")]
public class PhilosophyController : SimpleCrudController<Philosophy>
{
    public PhilosophyController(ISimpleRepository<Philosophy> r) : base(r) { }
}

[Route("api/awards")]
public class AwardsController : SimpleCrudController<Award>
{
    public AwardsController(ISimpleRepository<Award> r) : base(r) { }
}

[Route("api/certifications")]
public class CertificationsController : SimpleCrudController<Certification>
{
    public CertificationsController(ISimpleRepository<Certification> r) : base(r) { }
}
