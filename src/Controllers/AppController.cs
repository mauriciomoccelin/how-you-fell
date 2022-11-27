using HowYouFell.Api.Data;
using HowYouFell.Api.Models;
using HowYouFell.Api.Models.Inputs;
using HowYouFell.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HowYouFell.Api.Controllers;

[Authorize]
[Route("app")]
[ApiController]
public class AppController : ControllerBase
{
    private readonly IAspNetUserService aspNetUser;
    private readonly ILogger<AppController> logger;
    private readonly IMongoRepository mongoRepository;

    public AppController(
        IAspNetUserService aspNetUser,
        ILogger<AppController> logger,
        IMongoRepository mongoRepository
    )
    {
        this.aspNetUser = aspNetUser;
        this.logger = logger;
        this.mongoRepository = mongoRepository;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("tenants/{id:length(24)}")]
    public async Task<ActionResult<Tenant>> GetTenant([FromRoute] string id)
    {
        if (!aspNetUser.HasUserEmail())
        {
            return Unauthorized();
        }
        
        var userEmail = aspNetUser.GetUserEmail();

        var filterDefinitionBuilder = Builders<Tenant>.Filter;
        var filter = filterDefinitionBuilder.Eq(p => p.Id, id);
        
        filter &= (
            filterDefinitionBuilder.ElemMatch(
                f => f.Equips,
                f => f.AllowEmails.Contains(userEmail)
            )
        );

        var query = mongoRepository
            .GetCollection<Tenant>()
            .Find(filter);

        var tenant = await query.FirstOrDefaultAsync();

        if (tenant is null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("tenants/register")]
    public async Task<IActionResult> RegisterTenant()
    {
        if (!aspNetUser.CanRegisterTenant())
        {
            return Unauthorized();
        }

        var tenant = Tenant.Factory.Create(Guid.NewGuid().ToString());

        tenant.Equips.Add(TenantEquip.Factory.Create("Admins"));

        tenant.Threads.Add(TenantThread.Factory.Create("Me"));
        tenant.Threads.Add(TenantThread.Factory.Create("Team"));
        tenant.Threads.Add(TenantThread.Factory.Create("Company"));
        tenant.Threads.Add(TenantThread.Factory.Create("Proccess"));

        await mongoRepository
            .GetCollection<Tenant>()
            .InsertOneAsync(tenant);

        return CreatedAtAction(nameof(GetTenant), new { Id = tenant.Id }, null);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("persons/register")]
    public async Task<IActionResult> RegisterPerson()
    {
        if (!aspNetUser.HasUserEmail())
        {
            return Unauthorized();
        }

        var userEmail = aspNetUser.GetUserEmail();
        var person = Person.Factory.Create(userEmail);

        await mongoRepository
            .GetCollection<Person>()
            .InsertOneAsync(person);

        return CreatedAtAction(nameof(GetPerson), new { Id = person.Id }, null);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("persons")]
    public async Task<IActionResult> GetPerson()
    {
        if (!aspNetUser.HasUserEmail())
        {
            return Unauthorized();
        }
        
        var userEmail = aspNetUser.GetUserEmail();

        var filterDefinitionBuilder = Builders<Person>.Filter;
        var filter = filterDefinitionBuilder.Eq(p => p.Email, userEmail);

        var query = mongoRepository
            .GetCollection<Person>()
            .Find(filter);

        var person = await query.FirstOrDefaultAsync();

        if (person is null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("persons/add-felling")]
    public async Task<IActionResult> AddPersonFelling(
        [FromBody] CreateTenantThreadFellingInput model
    )
    {
        if (!aspNetUser.HasUserEmail())
        {
            return Unauthorized();
        }

        var userEmail = aspNetUser.GetUserEmail();

        var filterTenantDefinitionBuilder = Builders<Tenant>.Filter;
        var filterTenant = filterTenantDefinitionBuilder.Eq(p => p.Id, model.TenantId);

        var queryTenant = mongoRepository
            .GetCollection<Tenant>()
            .Find(filterTenant);

        var tenant = await queryTenant.FirstOrDefaultAsync();

        if (tenant is null)
        {
            return NotFound();
        }

        var isEmailAllowed = tenant.Equips
            .Where(equip => equip.Id == model.TeamId)
            .Any(equip => equip.AllowEmails.Contains(userEmail));

        if (!isEmailAllowed)
        {
            return Unauthorized();
        }

        var isTrheadValid = tenant.Threads.Any(
            trhead => trhead.Id == model.ThreadId
        );

        if (!isTrheadValid)
        {
            return Unauthorized();
        }

        var felling = PersonFelling.Factory.Create(
            model.TeamId,
            model.ThreadId,
            model.TenantId,
            model.Description,
            model.Type
        );

        var update = Builders<Person>.Update
            .Push<PersonFelling>(e => e.Fellings, felling);

        var filterPersonDefinitionBuilder = Builders<Person>.Filter;
        var filterPerson = filterPersonDefinitionBuilder.Eq(p => p.Email, userEmail);

        await mongoRepository
            .GetCollection<Person>()
            .UpdateOneAsync(filterPerson, update);

        return CreatedAtAction(nameof(GetPerson), new { Id = tenant.Id }, null);
    }
}
