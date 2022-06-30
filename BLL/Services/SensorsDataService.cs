using DAL.Entities;
using DAL.Repositories;
using MongoDB.Bson;

namespace BLL.Services;

public class SensorsDataService
{
    private readonly IMongoRepository<SensorData> _db;

    public SensorsDataService(IMongoRepository<SensorData> db)
    {
        _db = db;
    }

    public void InsertSensorData(SensorData sensorData, string sensorId)
    {
        sensorData.Id = ObjectId.GenerateNewId().ToString();

        _db.UseCollection(sensorId).InsertOne(sensorData);
    }

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, string period)
    {
        return _db.UseCollection(sensorId).FilterBy();
    }
}
