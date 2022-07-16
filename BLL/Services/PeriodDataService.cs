using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    internal class PeriodDataService
    {
        private readonly IMongoRepository<SensorData> _db;
        private const int _limit = 1000;

        public PeriodDataService(IMongoRepository<SensorData> db)
        {
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

        public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, DateTime from, DateTime to)
        {
            List<SensorData> periodData = _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(from) >= 0 && t.Time.CompareTo(to) < 0).ToList();

            return periodData;
        }
    }
}
