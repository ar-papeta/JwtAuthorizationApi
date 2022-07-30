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

    public PeriodDataService(IMongoRepository<SensorData> db, IOptions<MongoDbConfig> dbOptions)
    {
        _dbOptions = dbOptions;
        _db = db;
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

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, DateTime from, DateTime to, int dataCount)
    {
        _periodData = _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(from) >= 0 && t.Time.CompareTo(to) < 0).ToList();

        
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

        SensorData? firstData = default;
        if(!TrySetFirstData(from, to, out firstData))
        {
            if(firstData is null)
            {
                return new List<SensorData>();
            }

            var lastData = SetZeroDataWithTime(firstData, to); //TO DO check if work wrong (ref copy)

            _optimizedData.Add(firstData);
            _optimizedData.Add(lastData);
            return _optimizedData;
        }

        _optimizedData.Add(firstData);
        for (int i = 1; i < dataCount; i++)
        {
            if ()
            {

            }
            _optimizedData[i] = _periodData.OrderBy(x => x.Time - (_optimizedData[i - 1].Time + interval * i)).First();
        }

        return _optimizedData;
    }

    private bool TrySetFirstData(DateTime from, DateTime to, out SensorData? firstData)
    {
        firstData = default;

        if (!_periodData.Any(t => t.Time.CompareTo(from) >= 0 && t.Time.CompareTo(to) < 0))
        {
            firstData = _periodData.FirstOrDefault();

            if (firstData is not null)
            {
                firstData = SetZeroDataWithTime(firstData, from);
            }
            return false;
        }

        firstData = FindNearestDataTo(from);

        if(firstData.Time > from.Add(_sensorDataIntervalDefault))
        {
            firstData = SetZeroDataWithTime(firstData, from);
        }

        return true;

        

    }

    private SensorData FindNearestDataTo(DateTime time)
    {
        return _periodData.MinBy(x => x.Time - time) 
            ?? throw new Exception("No content for this date period (can not find nearest data)");
    }

    private static SensorData SetZeroDataWithTime(SensorData dataTemplate, DateTime time)
    {
        SensorData zeroClone = (SensorData)dataTemplate.Clone();

        zeroClone.Time = time;

        foreach (var data in zeroClone.Measurements)
        {
            data.Value = 0;
        }
        return zeroClone;
    }
}
