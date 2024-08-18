using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Data;
using SensorMonitoring.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorDataController : ControllerBase
    {
        private readonly SensorDataContext _context;

        public SensorDataController(SensorDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Recebe dados do sensor.
        /// </summary>
        /// <param name="sensorDataRequests">Lista de dados do sensor.</param>
        /// <returns>Confirmação de recebimento.</returns>
        [HttpPost]
        [Route("sensor-data")]
        public async Task<IActionResult> PostSensorData([FromBody] List<SensorDataRequest> sensorDataRequests)
        {
            if (sensorDataRequests == null || !sensorDataRequests.Any())
            {
                return BadRequest("A lista de dados do sensor é obrigatória.");
            }

            foreach (var request in sensorDataRequests)
            {
                var sensorData = new SensorData
                {
                    Codigo = request.Codigo,
                    DataHoraMedicao = request.DataHoraMedicao,
                    Medicao = request.Medicao,
                    SensorId = request.SensorId,
                    Sensor = new Sensor
                    {
                        Codigo = request.Sensor.Codigo,
                        SetorEquipamentoId = request.Sensor.SetorEquipamentoId,
                        SetorEquipamento = new SetorEquipamento
                        {
                            Nome = request.Sensor.SetorEquipamento.Nome
                        }
                    }
                };

                _context.SensorData.Add(sensorData);
            }

            await _context.SaveChangesAsync();

            return Ok("Dados do sensor recebidos com sucesso.");
        }

        /// <summary>
        /// Lista todos os dados do sensor.
        /// </summary>
        /// <returns>Lista de dados do sensor.</returns>
        [HttpGet]
        [Route("sensor-data")]
        public async Task<ActionResult<IEnumerable<SensorData>>> GetSensorData()
        {
            var sensorDataList = await _context.SensorData
                .Include(sd => sd.Sensor)
                .ThenInclude(s => s.SetorEquipamento)
                .ToListAsync();

            if (sensorDataList == null || !sensorDataList.Any())
            {
                return NotFound("Nenhum dado de sensor encontrado.");
            }

            return Ok(sensorDataList);
        }
    }
}
