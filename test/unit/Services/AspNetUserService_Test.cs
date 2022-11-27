namespace HowYouFell.Test.Unit;

[Collection(nameof(AspNetUserServiceTestFixture))]
public class AspNetUser_Test
{
    private readonly AspNetUserServiceTestFixture fixture;

    public AspNetUser_Test(AspNetUserServiceTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Trait("Category", "Services")]
    [Fact(DisplayName = "When check if authentication provide user e-mail, but throws")]
    public void HasUserEmail_WithoutHttpContext_ThrowArgumentNullException()
    {
        // Arrange
        var service = fixture.GenereteAspNetUser();
        var context = new DefaultHttpContext();

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Setup(x => x.HttpContext)
            .Returns(default(DefaultHttpContext));

        // Act
        Action action = () => service.HasUserEmail();

        // Assert
        Assert.Throws<ArgumentNullException>(action);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Verify(x => x.HttpContext, Times.Once);
    }

    [Trait("Category", "Services")]
    [Fact(DisplayName = "When check if authentication without user e-mail on identity claim, returns false")]
    public void HasUserEmail_WithoutEmailClaim_ReturnsFalse()
    {
        // Arrange
        var service = fixture.GenereteAspNetUser();
        var context = fixture.GenereteHttpContext();

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Setup(x => x.HttpContext)
            .Returns(context);

        // Act
        var result = service.HasUserEmail();

        // Assert
        Assert.False(result);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Verify(x => x.HttpContext, Times.Once);
    }

    [Trait("Category", "Services")]
    [Fact(DisplayName = "When check if authentication with user e-mail on identity claim, returns false")]
    public void HasUserEmail_WithEmailClaim_ReturnsFalse()
    {
        // Arrange
        var service = fixture.GenereteAspNetUser();
        var context = fixture.GenereteHttpContext(true);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Setup(x => x.HttpContext)
            .Returns(context);

        // Act
        var result = service.HasUserEmail();

        // Assert
        Assert.True(result);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Verify(x => x.HttpContext, Times.Once);
    }

    [Trait("Category", "Services")]
    [Fact(DisplayName = "When get user e-mail from identity claim, returns string empty")]
    public void GetUserEmail_WithoutEmailClaim_ReturnsStringEmpty()
    {
        // Arrange
        var service = fixture.GenereteAspNetUser();
        var context = fixture.GenereteHttpContext();

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Setup(x => x.HttpContext)
            .Returns(context);

        // Act
        var result = service.GetUserEmail();

        // Assert
        Assert.Empty(result);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Verify(x => x.HttpContext, Times.Once);
    }

    [Trait("Category", "Services")]
    [Fact(DisplayName = "When get user e-mail from identity claim, returns user email")]
    public void GetUserEmail_WithEmailClaim_ReturnsUserEmail()
    {
        // Arrange
        var service = fixture.GenereteAspNetUser();
        var context = fixture.GenereteHttpContext(true);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Setup(x => x.HttpContext)
            .Returns(context);

        // Act
        var result = service.GetUserEmail();

        // Assert
        Assert.Equal(result, fixture.UserEmail);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Verify(x => x.HttpContext, Times.Once);
    }

    [Trait("Category", "Services")]
    [Fact(DisplayName = "When get user e-mail from identity claim, returns string empty")]
    public void CanRegisterTenant_NotAllowedEmail_ReturnsFalse()
    {
        // Arrange
        var service = fixture.GenereteAspNetUser();
        var context = fixture.GenereteHttpContext(true);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Setup(x => x.HttpContext)
            .Returns(context);

        fixture.Mocker
            .GetMock<IConfiguration>()
            .Setup(x => x.GetSection(It.IsAny<string>()))
            .Returns(fixture.GenereteConfigurationSection());

        // Act
        var result = service.CanRegisterTenant();

        // Assert
        Assert.False(result);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Verify(x => x.HttpContext, Times.Once);

        fixture.Mocker
            .GetMock<IConfiguration>()
            .Verify(x => x.GetSection(It.IsAny<string>()), Times.Once);
    }

    [Trait("Category", "Services")]
    [Fact(DisplayName = "When get user e-mail from identity claim, returns string empty")]
    public void CanRegisterTenant_AllowedEmail_ReturnsTrue()
    {
        // Arrange
        var service = fixture.GenereteAspNetUser();
        var context = fixture.GenereteHttpContext(true);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Setup(x => x.HttpContext)
            .Returns(context);

        fixture.Mocker
            .GetMock<IConfiguration>()
            .Setup(x => x.GetSection(It.IsAny<string>()))
            .Returns(fixture.GenereteConfigurationSection(true));

        // Act
        var result = service.CanRegisterTenant();

        // Assert
        Assert.True(result);

        fixture.Mocker
            .GetMock<IHttpContextAccessor>()
            .Verify(x => x.HttpContext, Times.Once);

        fixture.Mocker
            .GetMock<IConfiguration>()
            .Verify(x => x.GetSection(It.IsAny<string>()), Times.Once);
    }
}
