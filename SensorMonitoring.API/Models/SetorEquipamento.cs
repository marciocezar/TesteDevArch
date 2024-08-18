namespace SensorMonitoring.Models
{
    public class SetorEquipamento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ICollection<Sensor> Sensores { get; set; } = new List<Sensor>();
    }
}
