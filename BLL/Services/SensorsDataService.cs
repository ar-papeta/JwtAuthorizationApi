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

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, string? period)
    {
        var p = period?.ToLower() ?? string.Empty;      
        return p switch
        {
            "" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-1)) >= 0),
            "yesterday" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-2)) >= 0 && t.Time.CompareTo(DateTime.Now.AddDays(-1)) < 0),
            "today" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-1)) >= 0),
            "week" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-7)) >= 0),
            "month" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-1)) >= 0),
            "quarter" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-3)) >= 0),
            "half-year" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-6)) >= 0),
            "year" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddYears(-1)) >= 0),
            "all" => _db.UseCollection(sensorId).FilterBy(),
            _ => throw new Exception($"Wrong period: {p}"),
        };
    }
}
