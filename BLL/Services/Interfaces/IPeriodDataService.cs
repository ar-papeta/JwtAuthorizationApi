using DAL.Entities;

namespace BLL.Services.Interfaces;

public interface IPeriodDataService
{
    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, string period);

    public IEnumerable<SensorData> GetSensorDataFromPeriod(string sensorId, DateTime from, DateTime to, int dataCount);
}
