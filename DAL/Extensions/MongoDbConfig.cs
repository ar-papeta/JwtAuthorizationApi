namespace DAL.Extensions;

public class MongoDbConfig
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string SensorsCollectionName { get; set; } = null!;
    public string UsersCollectionName { get; set; } = null!;
}
