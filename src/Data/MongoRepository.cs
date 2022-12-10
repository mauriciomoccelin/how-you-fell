using MongoDB.Driver;

namespace HowYouFell.Api.Data;

public class MongoRepository : IMongoRepository
{
    private readonly IConfiguration configuration;
    
    public MongoRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public IMongoCollection<T> GetCollection<T>() where T: class
    {
        var connectionString = configuration
            .GetSection("Mongo:ConnectionString")
            .Get<string>();

        var database = configuration
            .GetSection("Mongo:DatabaseName")
            .Get<string>();

        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase(database);
        
        return mongoDatabase.GetCollection<T>(typeof(T).Name);
    }
}
