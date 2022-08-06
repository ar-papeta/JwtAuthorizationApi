using BLL.Services.Interfaces;
using DAL.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace BLL.Services.Cached;

internal class SensorsDataServiceCached : ISensorsDataService
{
    private readonly ISensorsDataService _sensorDataService;
    private readonly IMemoryCache _cache;
    public SensorsDataServiceCached(ISensorsDataService sensorDataService, IMemoryCache cache)
    {
        _sensorDataService = sensorDataService;
        _cache = cache;
    }

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, string? period)
    {
        return _cache.GetOrCreate<IEnumerable<SensorData>>(sensorId, 
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
               return  _sensorDataService.GetSensorDataFromPeriod(sensorId, period);
            }
                
        ); 
    }

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, DateTime from, DateTime to, int dataCount)
    {
        return _sensorDataService.GetSensorDataFromPeriod(sensorId, from, to, dataCount);
    }

    public void InsertSensorData(SensorData sensorData, string sensorId)
    {
        _sensorDataService.InsertSensorData(sensorData, sensorId);
    }
}
