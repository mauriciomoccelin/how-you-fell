namespace HowYouFell.Test.Unit;

public class AppControllerTestFixture : IDisposable
{
    public Faker Faker { get; set; }
    public AutoMocker Mocker { get; set; }

    public AppControllerTestFixture()
    {
        Faker = new Faker();
        Mocker = new AutoMocker();
    }

    public void Dispose()
    {
    }

    public AppController GenereteAppController()
    {
        Mocker = new AutoMocker();

        var controller = Mocker.CreateInstance<AppController>();
        return controller;
    }

    public IMongoCollection<T> GenereteMongoCollection<T>(IAsyncCursor<T> cursor)
    {
        var collection = new Mock<IMongoCollection<T>>();

        collection
            .Setup(
                x => x.FindAsync<T>(
                    It.IsAny<FilterDefinition<T>>(),
                    It.IsAny<FindOptions<T, T>>(),
                    default(CancellationToken)
                )
            )
            .ReturnsAsync(cursor);

        collection
            .Setup(
                x => x.InsertOneAsync(
                    It.IsAny<T>(),
                    It.IsAny<InsertOneOptions>(),
                    default(CancellationToken)
                )
            )
            .Returns(Task.CompletedTask);

        collection
            .Setup(
                x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<T>>(),
                    It.IsAny<UpdateDefinition<T>>(),
                    null,
                    default(CancellationToken)
                )
            )
            .Returns(Task.FromResult(default(UpdateResult)));

        return collection.Object;
    }

    public IFindFluent<T, T> GenerateFindFluent<T>(IEnumerable<T> result)
    {
        var findFluent = new Mock<IFindFluent<T, T>>();

        findFluent
            .Setup(x => x.Project<T>(It.IsAny<ProjectionDefinition<T>>()))
            .Returns(findFluent.Object);

        findFluent
            .Setup(x => x.FirstOrDefaultAsync<T>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result.First());

        return findFluent.Object;
    }

    public IAsyncCursor<T> GenerateAsyncCursor<T>(IEnumerable<T> tenats)
    {
        var cursor = new Mock<IAsyncCursor<T>>();

        cursor
            .Setup(x => x.Current)
            .Returns(tenats);

        cursor
            .SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        cursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        return cursor.Object;
    }

    public IEnumerable<Person> GeneretePersons()
    {
        var faker = new Faker<Person>()
            .CustomInstantiator(
                fake => Person.Factory
                    .Create(fake.Company.CompanyName())
            );

        return faker.Generate(1);
    }

    public IEnumerable<Tenant> GenereteTenants()
    {
        var faker = new Faker<Tenant>()
            .CustomInstantiator(fake => Tenant.Factory.Create(fake.Company.CompanyName()));

        return faker.Generate(1);
    }

    public IEnumerable<TenantEquip> GenereteTenantEquips()
    {
        var faker = new Faker<TenantEquip>()
            .CustomInstantiator(fake => TenantEquip.Factory.Create(fake.Lorem.Word()));

        return faker.Generate(1);
    }

    public IEnumerable<TenantThread> GenereteTenantThreads()
    {
        var faker = new Faker<TenantThread>()
            .CustomInstantiator(fake => TenantThread.Factory.Create(fake.Lorem.Word()));

        return faker.Generate(1);
    }
}