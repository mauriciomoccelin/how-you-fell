using MongoDB.Driver;

namespace HowYouFell.Api.Data;

public interface IMongoRepository
{
    IMongoCollection<T> GetCollection<T>() where T : class;
}
