namespace SensorMonitoring.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int SetorEquipamentoId { get; set; }
        public SetorEquipamento SetorEquipamento { get; set; }
        public ICollection<SensorData> SensorData { get; set; } = new List<SensorData>();
    }
}
