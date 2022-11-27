using Microsoft.AspNetCore.Mvc;

namespace HowYouFell.Test.Unit;

[Collection(nameof(AppControllerTestFixture))]
public class AppController_Test
{
    private readonly AppControllerTestFixture fixture;

    public AppController_Test(AppControllerTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try get by id, but has no e-mail in current context")]
    public async Task GetTenant_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var controller = fixture.GenereteAppController();

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(false);

        // Act
        var actionResult = await controller.GetTenant(fixture.Faker.Random.String());
        var result = actionResult.Result as UnauthorizedResult;

        // Assert
        Assert.Equal(401, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try get tenant by id, but not found")]
    public async Task GetTenant_InvalidParameters_ReturnsNotFound()
    {
        // Arrange
        var controller = fixture.GenereteAppController();
        var tenantsCursor = fixture.GenerateAsyncCursor<Tenant>(Enumerable.Empty<Tenant>());
        var tenantsCollection = fixture.GenereteMongoCollection<Tenant>(tenantsCursor);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.GetUserEmail())
            .Returns(fixture.Faker.Internet.Email());

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Tenant>())
            .Returns(tenantsCollection);

        // Act
        var actionResult = await controller.GetTenant(fixture.Faker.Random.String());
        var result = actionResult.Result as NotFoundResult;

        // Assert
        Assert.Equal(404, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.GetUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Verify(sp => sp.GetCollection<Tenant>(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When get tenant by id with success")]
    public async Task GetTenant_RigthParameters_ReturnsOk()
    {
        // Arrange
        var controller = fixture.GenereteAppController();
        var tenants = fixture.GenereteTenants();
        var tenantsCursor = fixture.GenerateAsyncCursor<Tenant>(tenants);
        var tenantsCollection = fixture.GenereteMongoCollection<Tenant>(tenantsCursor);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.GetUserEmail())
            .Returns(fixture.Faker.Internet.Email());

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Tenant>())
            .Returns(tenantsCollection);

        // Act
        var actionResult = await controller.GetTenant(fixture.Faker.Random.String());
        var result = actionResult.Result as OkObjectResult;

        // Assert
        var tenantFound = result?.Value as Tenant;
        
        Assert.Equal(200, result?.StatusCode);
        Assert.Equal(tenants.First().Id, tenantFound?.Id);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.GetUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Verify(sp => sp.GetCollection<Tenant>(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try register tenant, but has no authorized e-mail in current context")]
    public async Task RegisterTenant_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var controller = fixture.GenereteAppController();

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.CanRegisterTenant())
            .Returns(false);

        // Act
        var actionResult = await controller.RegisterTenant();
        var result = actionResult as UnauthorizedResult;

        // Assert
        Assert.Equal(401, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.CanRegisterTenant(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When register tenant with authorized e-mail in allowed e-mails")]
    public async Task RegisterTenant_WithAllowedEmail_ReturnsCreateAt()
    {
        // Arrange
        var controller = fixture.GenereteAppController();

        var allowedEmail = fixture.Faker.Internet.Email();

        var tenants = fixture.GenereteTenants();
        var tenantsCursor = fixture.GenerateAsyncCursor<Tenant>(tenants);
        var tenantsCollection = fixture.GenereteMongoCollection<Tenant>(tenantsCursor);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.CanRegisterTenant())
            .Returns(true);

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Tenant>())
            .Returns(tenantsCollection);

        // Act
        var actionResult = await controller.RegisterTenant();
        var result = actionResult as CreatedAtActionResult;

        // Assert
        Assert.Equal(201, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.CanRegisterTenant(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try register person, but has no authorization")]
    public async Task RegisterPerson_WithoutAuthentication_ReturnsUnAuthorized()
    {
        // Arrange
        var controller = fixture.GenereteAppController();
        var allowedEmail = fixture.Faker.Internet.Email();

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(false);

        // Act
        var actionResult = await controller.RegisterPerson();
        var result = actionResult as UnauthorizedResult;

        // Assert
        Assert.Equal(401, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When register person with success")]
    public async Task RegisterPerson_WithSuccess_ReturnsCreateAt()
    {
        // Arrange
        var controller = fixture.GenereteAppController();

        var allowedEmail = fixture.Faker.Internet.Email();

        var persons = fixture.GeneretePersons();
        var personsCursor = fixture.GenerateAsyncCursor<Person>(persons);
        var personsCollection = fixture.GenereteMongoCollection<Person>(personsCursor);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.GetUserEmail())
            .Returns(fixture.Faker.Internet.Email());

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Person>())
            .Returns(personsCollection);

        // Act
        var actionResult = await controller.RegisterPerson();
        var result = actionResult as CreatedAtActionResult;

        // Assert
        Assert.Equal(201, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.GetUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try get person, but has no authorization")]
    public async Task GetPerson_WithoutAuthentication_ReturnsUnAuthorized()
    {
        // Arrange
        var controller = fixture.GenereteAppController();
        var allowedEmail = fixture.Faker.Internet.Email();

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(false);

        // Act
        var actionResult = await controller.GetPerson();
        var result = actionResult as UnauthorizedResult;

        // Assert
        Assert.Equal(401, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try get person, but found")]
    public async Task GetPerson_InvalidParameters_ReturnsNotFound()
    {
        // Arrange
        var controller = fixture.GenereteAppController();

        var allowedEmail = fixture.Faker.Internet.Email();

        var personsCursor = fixture.GenerateAsyncCursor<Person>(Enumerable.Empty<Person>());
        var personsCollection = fixture.GenereteMongoCollection<Person>(personsCursor);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.GetUserEmail())
            .Returns(fixture.Faker.Internet.Email());

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Person>())
            .Returns(personsCollection);

        // Act
        var actionResult = await controller.GetPerson();
        var result = actionResult as NotFoundResult;

        // Assert
        Assert.Equal(404, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.GetUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When get person with success")]
    public async Task GetPerson_WithSuccess_ReturnsOk()
    {
        // Arrange
        var controller = fixture.GenereteAppController();

        var allowedEmail = fixture.Faker.Internet.Email();

        var persons = fixture.GeneretePersons();
        var personsCursor = fixture.GenerateAsyncCursor<Person>(persons);
        var personsCollection = fixture.GenereteMongoCollection<Person>(personsCursor);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.GetUserEmail())
            .Returns(fixture.Faker.Internet.Email());

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Person>())
            .Returns(personsCollection);

        // Act
        var actionResult = await controller.GetPerson();
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.GetUserEmail(), Times.Once);
    }
    
    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try add felling, but has no authorized e-mail in current context")]
    public async Task AddPersonFelling_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var controller = fixture.GenereteAppController();

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(false);

        // Act
        var actionResult = await controller.AddPersonFelling(
            new CreateTenantThreadFellingInput()
        );

        // Assert

        var result = actionResult as UnauthorizedResult;
        Assert.Equal(401, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try add felling, but not found tenant")]
    public async Task AddPersonFelling_NotFoundTenatById_ReturnsNotFound()
    {
        // Arrange
        var controller = fixture.GenereteAppController();
        var tenants = Enumerable.Empty<Tenant>();
        var tenantsCursor = fixture.GenerateAsyncCursor<Tenant>(tenants);
        var tenantsCollection = fixture.GenereteMongoCollection<Tenant>(tenantsCursor);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Tenant>())
            .Returns(tenantsCollection);

        // Act
        var actionResult = await controller.AddPersonFelling(
            new CreateTenantThreadFellingInput()
        );

        // Assert
        var result = actionResult as NotFoundResult;
        Assert.Equal(404, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try add felling, but e-mail is not allowed on equip")]
    public async Task AddPersonFelling_EmailNotAllowedOnEquip_ReturnsUnauthorized()
    {
        // Arrange
        var controller = fixture.GenereteAppController();
        var tenants = fixture.GenereteTenants();
        var tenantEquips = fixture.GenereteTenantEquips();
        var tenantsCursor = fixture.GenerateAsyncCursor<Tenant>(tenants);
        var tenantsCollection = fixture.GenereteMongoCollection<Tenant>(tenantsCursor);

        var tenantEquip = tenantEquips.First();
        tenants.First().Equips.Add(tenantEquip);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Tenant>())
            .Returns(tenantsCollection);

        // Act
        var actionResult = await controller.AddPersonFelling(
            new CreateTenantThreadFellingInput
            {
                TeamId = tenantEquip.Id!
            }
        );

        // Assert
        var result = actionResult as UnauthorizedResult;
        Assert.Equal(401, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.GetUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When try add felling, but e-mail is not allowed on equip")]
    public async Task AddPersonFelling_ThreadIsNotValid_ReturnsUnauthorized()
    {
        // Arrange
        var controller = fixture.GenereteAppController();
        var tenants = fixture.GenereteTenants();
        var tenantEquips = fixture.GenereteTenantEquips();
        var tenantThreads = fixture.GenereteTenantThreads();
        var tenantsCursor = fixture.GenerateAsyncCursor<Tenant>(tenants);
        var tenantsCollection = fixture.GenereteMongoCollection<Tenant>(tenantsCursor);

        var tenantEquip = tenantEquips.First();
        var tenantThread = tenantThreads.First();
        tenants.First().Equips.Add(tenantEquip);
        tenants.First().Threads.Add(tenantThread);

        var allowedEmail = fixture.Faker.Internet.Email();
        tenantEquip.AllowEmails.Add(allowedEmail);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.GetUserEmail())
            .Returns(allowedEmail);

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Tenant>())
            .Returns(tenantsCollection);

        // Act
        var actionResult = await controller.AddPersonFelling(
            new CreateTenantThreadFellingInput
            {
                TeamId = tenantEquip.Id!,
                ThreadId = tenantEquip.Id!
            }
        );

        // Assert
        var result = actionResult as UnauthorizedResult;
        Assert.Equal(401, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.GetUserEmail(), Times.Once);
    }

    [Trait("Category", "Controller")]
    [Fact(DisplayName = "When add felling with success to thread")]
    public async Task AddPersonFelling_WithSuccess_ReturnsCreatedAt()
    {
        // Arrange
        var controller = fixture.GenereteAppController();

        var tenants = fixture.GenereteTenants();
        var tenantEquips = fixture.GenereteTenantEquips();
        var tenantThreads = fixture.GenereteTenantThreads();
        var tenantsCursor = fixture.GenerateAsyncCursor<Tenant>(tenants);
        var tenantsCollection = fixture.GenereteMongoCollection<Tenant>(tenantsCursor);

        var persons = fixture.GeneretePersons();
        var personsCursor = fixture.GenerateAsyncCursor<Person>(persons);
        var personsCollection = fixture.GenereteMongoCollection<Person>(personsCursor);

        var tenant = tenants.First();
        var tenantEquip = tenantEquips.First();
        var tenantThread = tenantThreads.First();

        tenant.Equips.Add(tenantEquip);
        tenant.Threads.Add(tenantThread);

        var allowedEmail = fixture.Faker.Internet.Email();
        tenantEquip.AllowEmails.Add(allowedEmail);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.HasUserEmail())
            .Returns(true);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Setup(x => x.GetUserEmail())
            .Returns(allowedEmail);

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Tenant>())
            .Returns(tenantsCollection);

        fixture.Mocker
            .GetMock<IMongoRepository>()
            .Setup(sp => sp.GetCollection<Person>())
            .Returns(personsCollection);

        // Act
        var actionResult = await controller.AddPersonFelling(
            new CreateTenantThreadFellingInput
            {
                TeamId = tenantEquip.Id!,
                ThreadId = tenantThread.Id!,
                TenantId = tenant.Id!
            }
        );

        // Assert
        var result = actionResult as CreatedAtActionResult;
        Assert.Equal(201, result?.StatusCode);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.HasUserEmail(), Times.Once);

        fixture.Mocker
            .GetMock<IAspNetUserService>()
            .Verify(x => x.GetUserEmail(), Times.Once);
    }
}
