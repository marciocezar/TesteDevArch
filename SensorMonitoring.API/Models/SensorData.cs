using SensorMonitoring.Models;

namespace SensorMonitoring.Models
{
    public class SensorData
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public DateTimeOffset DataHoraMedicao { get; set; }
        public decimal Medicao { get; set; }
        public int? SensorId { get; set; }
        public Sensor Sensor { get; set; } 
    }
}
