using Xunit;
using SensorMonitoring.Services;
using SensorMonitoring.Models;
using System.Collections.Generic;
using System.Linq;

public class AlertServiceTests
{
    [Fact]
    public void Teste_5_Medicoes_Consecutivas_Fora_Do_Limite()
    {
        // Arrange
        var sensor = new Sensor
        {
            Codigo = "S1",
            SensorData = new List<SensorData>
            {
                new SensorData { Medicao = 0 },
                new SensorData { Medicao = 0.5m },
                new SensorData { Medicao = 51 },
                new SensorData { Medicao = 52 },
                new SensorData { Medicao = 60 }
            }
        };

        var service = new AlertService(null);

        // Act
        var result = service.ShouldSendAlert(sensor.SensorData.ToList());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Teste_Media_Das_Ultimas_50_Medicoes_Na_Margem_De_Erro()
    {
        // Arrange
        var sensor = new Sensor
        {
            Codigo = "S2",
            SensorData = Enumerable.Repeat(new SensorData { Medicao = 2.5m }, 50).ToList()
        };

        var service = new AlertService(null);

        // Act
        var result = service.ShouldSendAttentionAlert(sensor.SensorData.ToList());

        // Assert
        Assert.True(result);
    }
}
