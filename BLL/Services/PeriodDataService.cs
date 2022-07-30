using DAL.Entities;
using DAL.Extensions;
using DAL.Repositories;
using Microsoft.Extensions.Options;

namespace BLL.Services;

internal class PeriodDataService
{
    private readonly IOptions<MongoDbConfig> _dbOptions;
    private readonly IMongoRepository<SensorData> _db;
    private readonly List<SensorData> _optimizedData = new();
    private List<SensorData> _periodData = new();
    private readonly TimeSpan _sensorDataIntervalDefault = TimeSpan.FromSeconds(30);
    private TimeSpan _optimizedInterval;
    private SensorData _sensorDataTemplate;

    public PeriodDataService(IMongoRepository<SensorData> db, IOptions<MongoDbConfig> dbOptions)
    {
        _dbOptions = dbOptions;
        _db = db;
        _optimizedInterval = _sensorDataIntervalDefault;

    }

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, string period)
    {
        return period.ToLower() switch
        {
            "" => GetSensorDataLive(sensorId),
            "yersterday" => GetSensorDataYerstarday(sensorId),
            "today" => GetSensorDataToday(sensorId),
            "week" => GetSensorDataWeek(sensorId),
            "month" => GetSensorDataMonth(sensorId),
            "quarter" => GetSensorDataQuarter(sensorId),
            "half-year" => GetSensorDataHalfYear(sensorId),
            "year" => GetSensorDataYear(sensorId),
            "all" => GetSensorDataAll(sensorId),
            _ => throw new Exception("Wrong period"),
        };
    }

    public IEnumerable<SensorData> GetSensorDataLive(string sensorId)
    {
        return GetSensorDataToday(sensorId);
    }

    public IEnumerable<SensorData> GetSensorDataYerstarday(string sensorId)
    {
        return _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-2)) >= 0 && t.Time.CompareTo(DateTime.Now.AddDays(-1)) < 0);
    }

    public IEnumerable<SensorData> GetSensorDataToday(string sensorId)
    {
        return _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-1)) >= 0);
    }

    public IEnumerable<SensorData> GetSensorDataWeek(string sensorId)
    {
        return _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-7)) >= 0);
    }

    public IEnumerable<SensorData> GetSensorDataMonth(string sensorId)
    {
        return _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-1)) >= 0);
    }

    public IEnumerable<SensorData> GetSensorDataQuarter(string sensorId)
    {
        return _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-3)) >= 0);
    }

    public IEnumerable<SensorData> GetSensorDataHalfYear(string sensorId)
    {
        return _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-6)) >= 0);
    }

    public IEnumerable<SensorData> GetSensorDataYear(string sensorId)
    {
        return _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddYears(-1)) >= 0);
    }

    public IEnumerable<SensorData> GetSensorDataAll(string sensorId)
    {
        return _db.UseCollection(sensorId).FilterBy();
    }

    private void LoadAllDataForPeriod(DateTime from, DateTime to, string sensorId)
    {
        _periodData = _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(from) >= 0 && t.Time.CompareTo(to) < 0).ToList();
    }

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, DateTime from, DateTime to, int dataCount)
    {
        LoadAllDataForPeriod(from, to, sensorId);

        if (!IsSensorDataForTamplate())
        {
            return Enumerable.Empty<SensorData>();
        }

        var interval = (to - from) / dataCount;

        //TO DO: MIGRATE for this implpementation
        /*
        var sensorUpdatePeriod = _db.UseCollection(_dbOptions.Value.SensorsCollectionName).FindById(sensorId).UpdatePeriod;
        */

        if (_sensorDataIntervalDefault < interval)
        {
            interval = _sensorDataIntervalDefault;
            dataCount = (int)((from - to) / interval); 
        }
        _optimizedInterval = interval;

        for (int i = 0; i < dataCount; i++)
        {
            var localInterval = from + interval * i;
            _optimizedData.Add(GetNextData(localInterval, localInterval.Add(interval)));
        }

        return _optimizedData;
    }

    private bool IsSensorDataForTamplate()
    {
        _sensorDataTemplate = _periodData.FirstOrDefault();
        
        return _sensorDataTemplate is not null;
    }
    private bool IsSensorData(DateTime from, DateTime to)
    {
        return _periodData.Any(x => x.Time < to && x.Time > from);
    }

    private SensorData GetNextData(DateTime from, DateTime to)
    {
        if (!IsSensorData(from, to))
        {
            return SetZeroDataWithTime(from);
        }

        var nextData = FindNearestDataBefore(from, from.Add(_optimizedInterval));

        if(nextData is null)
        {
            nextData = SetZeroDataWithTime(from);
        }

        return nextData;
    }

    private SensorData? FindNearestDataBefore(DateTime from, DateTime to)
    {
        return _periodData.Where(x => x.Time < to && x.Time > from).MinBy(x => x.Time - to);
    }

    private SensorData SetZeroDataWithTime(DateTime time)
    {
        SensorData zeroClone = (SensorData)_sensorDataTemplate.Clone();
        zeroClone.Time = time;

        foreach (var data in zeroClone.Measurements)
        {
            data.Value = 0;
        }
        return zeroClone;
    }
}
