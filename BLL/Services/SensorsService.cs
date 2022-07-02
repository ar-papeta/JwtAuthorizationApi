using DAL.Entities;
using DAL.Extensions;
using DAL.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services;

public class SensorsService
{
    private readonly IMongoRepository<Sensor> _db;

    public SensorsService(IMongoRepository<Sensor> db, IOptions<MongoDbConfig> dbOptions)
    {
        _db = db;
        _db.UseCollection(dbOptions.Value.SensorsCollectionName);
    }

    public void CreateSensor(Sensor sensorDto)
    {
        _db.InsertOne(sensorDto);
    }

    public Sensor DeleteSensor(string sensorId)
    {
        _db.DropCollection(sensorId);
        return _db.DeleteById(sensorId);
    }

    public Sensor UpdateSensor(Sensor sensor)
    {
        return _db.ReplaceOne(sensor);
    }

    public Sensor GetSensorById(string id)
    {
        return _db.FindById(id);
    }

    public IEnumerable<Sensor> GetUserSensors(string userId)
    {
        return _db.FilterBy(x => x.UserId.Equals(userId));
    }
}
