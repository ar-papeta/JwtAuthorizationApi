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
        private const int _maxValues = 1000;

        public PeriodDataService(IMongoRepository<SensorData> db)
        {
            _db = db;
        }

        public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, string period)
        {
            return period.ToLower() switch
            {
                "" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-1)) >= 0),
                "yersterday" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-2)) >= 0 && t.Time.CompareTo(DateTime.Now.AddDays(-1)) < 0),
                "today" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-1)) >= 0),
                "week" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddDays(-7)) >= 0),
                "month" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-1)) >= 0),
                "quarter" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-3)) >= 0),
                "half-year" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddMonths(-6)) >= 0),
                "year" => _db.UseCollection(sensorId).FilterBy(t => t.Time.CompareTo(DateTime.Now.AddYears(-1)) >= 0),
                "all" => _db.UseCollection(sensorId).FilterBy(),
                _ => throw new Exception("Wrong period"),
            };
        }

        public IEnumerable<SensorData> GetSensorDataYerstarday(string sensorId)
        {
            return _db.UseCollection(sensorId).FilterBy(t => t.Time.AddDays(1) - t.Time < TimeSpan.Zero);
        }
    }
}
