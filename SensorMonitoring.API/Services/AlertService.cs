using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SensorMonitoring.Data;
using SensorMonitoring.Models;

namespace SensorMonitoring.Services
{
    public class AlertService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private const int LimiteInferior = 1;
        private const int LimiteSuperior = 50;
        private const int MargemErro = 2;

        public AlertService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SensorDataContext>();
                    var setoresEquipamentos = await context.SetoresEquipamentos
                        .Include(se => se.Sensores)
                        .ThenInclude(sensor => sensor.SensorData)
                        .ToListAsync();

                    foreach (var setorEquipamento in setoresEquipamentos)
                    {
                        foreach (var sensor in setorEquipamento.Sensores)
                        {
                            var mediçõesRecentes = sensor.SensorData
                                .OrderByDescending(sd => sd.DataHoraMedicao)
                                .Take(5)
                                .ToList();

                            if (mediçõesRecentes.Count >= 5 &&
                                mediçõesRecentes.All(m => m.Medicao < LimiteInferior || m.Medicao > LimiteSuperior))
                            {
                                await EnviarEmailAlerta(setorEquipamento, sensor, "Medições fora dos limites");
                            }

                            // Regra 2: Verificar se a média das últimas 50 medições se encaixa na margem de erro
                            var ultimas50Medicoes = sensor.SensorData
                                .OrderByDescending(sd => sd.DataHoraMedicao)
                                .Take(50)
                                .ToList();

                            if (ultimas50Medicoes.Count >= 50)
                            {
                                var media = ultimas50Medicoes.Average(m => m.Medicao);
                                if (media >= (LimiteInferior - MargemErro) && media <= (LimiteInferior + MargemErro) ||
                                    media >= (LimiteSuperior - MargemErro) && media <= (LimiteSuperior + MargemErro))
                                {
                                    await EnviarEmailAlerta(setorEquipamento, sensor, "Média das medições na margem de erro");
                                }
                            }
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private Task EnviarEmailAlerta(SetorEquipamento setorEquipamento, Sensor sensor, string mensagem)
        {
            var mailMessage = new MailMessage("noreply@sensormonitoring.com", "cliente@exemplo.com")
            {
                Subject = "Alerta de Sensor",
                Body = $"Alerta para o Setor/Equipamento: {setorEquipamento.Nome}, Sensor: {sensor.Codigo}.\n\n{mensagem}."
            };

            using (var smtpClient = new SmtpClient("smtp.seuprovedor.com"))
            {
                smtpClient.Send(mailMessage);
            }

            return Task.CompletedTask;
        }

        public bool ShouldSendAlert(List<SensorData> sensorData)
        {
            return sensorData.Count >= 5 && sensorData.All(m => m.Medicao < LimiteInferior || m.Medicao > LimiteSuperior);
        }

        public bool ShouldSendAttentionAlert(List<SensorData> sensorData)
        {
            if (sensorData.Count >= 50)
            {
                var media = sensorData.Average(m => m.Medicao);
                return (media >= (LimiteInferior - MargemErro) && media <= (LimiteInferior + MargemErro)) ||
                       (media >= (LimiteSuperior - MargemErro) && media <= (LimiteSuperior + MargemErro));
            }
            return false;
        }
    }
}