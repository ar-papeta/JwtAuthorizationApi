using BLL.Services.Interfaces;
using DAL.Entities;
using DAL.Repositories;
using MongoDB.Bson;

namespace BLL.Services;

public class SensorsDataService : ISensorsDataService
{
    private readonly IMongoRepository<SensorData> _db;
    private readonly IPeriodDataService _periodDataService;

    public SensorsDataService(IMongoRepository<SensorData> db, IPeriodDataService periodDataService)
    {
        _db = db;
        _periodDataService = periodDataService;
    }

    public void InsertSensorData(SensorData sensorData, string sensorId)
    {
        sensorData.Id = ObjectId.GenerateNewId().ToString();

        _db.UseCollection(sensorId).InsertOne(sensorData);
    }

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, string? period)
    {
        var p = period?.ToLower() ?? string.Empty;
        return _periodDataService.GetSensorDataFromPeriod(sensorId, p);
    }

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, DateTime from, DateTime to, int dataCount)
    {
        return _periodDataService.GetSensorDataFromPeriod(sensorId, from, to, dataCount);
    }

}
