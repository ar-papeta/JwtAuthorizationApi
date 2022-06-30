using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Entities;

public class SensorData : IDocument
{
    [JsonIgnore]
    public string? Id { get; set; } = null!; //TO DO: Add Dto for avoid this!!!
    public List<SensorMeasurement> Measurements { get; set; } = null!;
    public DateTime Time { get; set; }

}
