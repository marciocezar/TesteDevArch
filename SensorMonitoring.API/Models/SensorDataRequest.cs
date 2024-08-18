namespace SensorMonitoring.Models
{
    public class SensorDataRequest
    {
        public string Codigo { get; set; }
        public DateTimeOffset DataHoraMedicao { get; set; }
        public decimal Medicao { get; set; }
        public int SensorId { get; set; }
        public SensorRequest Sensor { get; set; }
    }

    public class SensorRequest
    {
        public string Codigo { get; set; }
        public int SetorEquipamentoId { get; set; }
        public SetorEquipamentoRequest SetorEquipamento { get; set; }
    }

    public class SetorEquipamentoRequest
    {
        public string Nome { get; set; }
    }
}
