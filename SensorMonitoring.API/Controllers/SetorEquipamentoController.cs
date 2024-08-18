using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Data;
using SensorMonitoring.Models;

namespace SensorMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetorEquipamentoController : ControllerBase
    {
        private readonly SensorDataContext _context;

        public SetorEquipamentoController(SensorDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Vincula um sensor a um Setor/Equipamento.
        /// </summary>
        /// <param name="request">Objeto contendo o ID do Setor/Equipamento e o Código do Sensor.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        [Route("vincular-sensor")]
        public async Task<IActionResult> VincularSensor([FromBody] VinculacaoRequest request)
        {
            var setorEquipamento = await _context.SetoresEquipamentos
                .Include(se => se.Sensores)
                .FirstOrDefaultAsync(se => se.Id == request.SetorEquipamentoId);

            if (setorEquipamento == null)
            {
                return NotFound("Setor/Equipamento não encontrado.");
            }

            var sensor = new Sensor
            {
                Codigo = request.CodigoSensor,
                SetorEquipamentoId = request.SetorEquipamentoId
            };

            setorEquipamento.Sensores.Add(sensor);
            await _context.SaveChangesAsync();

            return Ok("Sensor vinculado com sucesso.");
        }

        /// <summary>
        /// Retorna as últimas 10 medições de cada sensor vinculado a um Setor/Equipamento.
        /// </summary>
        /// <param name="id">ID do Setor/Equipamento.</param>
        /// <returns>Lista de sensores e suas últimas 10 medições.</returns>
        [HttpGet("{id}/ultimas-medicoes")]
        public async Task<IActionResult> GetUltimasMedicoes(int id)
        {
            var setorEquipamento = await _context.SetoresEquipamentos
                .Include(se => se.Sensores)
                .ThenInclude(sensor => sensor.SensorData)
                .FirstOrDefaultAsync(se => se.Id == id);

            if (setorEquipamento == null)
            {
                return NotFound("Setor/Equipamento não encontrado.");
            }

            var resultado = setorEquipamento.Sensores.Select(sensor => new
            {
                SensorCodigo = sensor.Codigo,
                UltimasMedicoes = sensor.SensorData
                    .OrderByDescending(sd => sd.DataHoraMedicao)
                    .Take(10)
                    .ToList()
            });

            return Ok(resultado);
        }
    }

    public class VinculacaoRequest
    {
        public int SetorEquipamentoId { get; set; }
        public string CodigoSensor { get; set; }
    }
}
