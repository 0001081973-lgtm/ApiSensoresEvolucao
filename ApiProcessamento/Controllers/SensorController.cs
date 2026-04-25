using ApiProcessamento.Config;
using ApiProcessamento.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;

namespace ApiProcessamento.Controllers
{
    /// <summary>
    /// Controller responsável pelo processamento e gerenciamento dos dados dos sensores
    /// </summary>
    [ApiController]
    [Route("api/v1/sensores")]
    public class SensorController : ControllerBase
    {
        private readonly SensorDbContext _db;
        private readonly IOptions<ApiConfig> _config;

        /// <summary>
        /// Construtor da controller
        /// </summary>
        /// <param name="db">Contexto do banco de dados</param>
        /// <param name="config">Configurações da API (limites de sensores)</param>
        public SensorController(SensorDbContext db, IOptions<ApiConfig> config)
        {
            _db = db;
            _config = config;
        }

        /// <summary>
        /// Recebe dados de um sensor e salva no banco de dados
        /// </summary>
        /// <remarks>
        /// Este endpoint valida os dados recebidos com base nos limites configurados.
        /// 
        /// Possíveis erros:
        /// - Temperatura acima do limite permitido → 400
        /// - Pressão acima do limite permitido → 400
        /// - Vibração acima do limite permitido → 400
        /// 
        /// Caso os dados estejam válidos, o registro será salvo com timestamp atual.
        /// </remarks>
        /// <param name="sensor">Objeto contendo os dados do sensor</param>
        /// <returns>Dados salvos ou erro de validação</returns>
        [HttpPost]
        public async Task<IActionResult> Receber(SensorData sensor)
        {
            if (sensor.Temperatura > _config.Value.MaxTemperatura)
            {
                return BadRequest("Temperatura acima do limite permitido!");
            }

            if (sensor.Pressao > _config.Value.MaxPressao)
            {
                return BadRequest("Pressao acima do limite permitido!");
            }

            if (sensor.Vibracao > _config.Value.MaxVibracao)
            {
                return BadRequest("Vibracao acima do limite permitido!");
            }

            sensor.Timestamp = DateTime.Now;
            _db.Sensores.Add(sensor);
            await _db.SaveChangesAsync();

            return Ok(sensor);
        }

        /// <summary>
        /// Lista todos os dados de sensores cadastrados
        /// </summary>
        /// <remarks>
        /// Retorna todos os registros armazenados no banco de dados.
        /// </remarks>
        /// <returns>Lista de sensores</returns>
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var dados = await _db.Sensores.ToListAsync();
            return Ok(dados);
        }

        /// <summary>
        /// Busca um sensor pelo ID
        /// </summary>
        /// <remarks>
        /// Retorna um sensor específico com base no ID informado.
        /// Caso o sensor não seja encontrado, retorna erro 404.
        /// </remarks>
        /// <param name="id">ID do sensor</param>
        /// <returns>Sensor encontrado ou erro 404</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var sensor = await _db.Sensores.FindAsync(id);
            if (sensor == null)
            {
                return NotFound("Sensor nao encontrado!");
            }
            return Ok(sensor);
        }

        /// <summary>
        /// Busca sensores por origem
        /// </summary>
        /// <remarks>
        /// Filtra os sensores com base na origem informada.
        /// Exemplos de origem:
        /// - Simulator
        /// - Interface
        /// </remarks>
        /// <param name="origem">Origem do sensor</param>
        /// <returns>Lista de sensores filtrados</returns>
        [HttpGet("origem/{origem}")]
        public async Task<IActionResult> BuscarPorOrigem(string origem)
        {
            var dados = await _db.Sensores
                .Where(s => s.Origem == origem)
                .ToListAsync();

            return Ok(dados);
        }

        /// <summary>
        /// Retorna o último registro de sensor
        /// </summary>
        /// <remarks>
        /// Busca o registro mais recente com base no timestamp.
        /// Caso não existam dados, retorna erro 404.
        /// </remarks>
        /// <returns>Último registro ou erro 404</returns>
        [HttpGet("ultimo")]
        public async Task<IActionResult> UltimoRegistro()
        {
            var sensor = await _db.Sensores
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefaultAsync();

            if (sensor == null)
            {
                return NotFound("Nenhum dado encontrado!");
            }
            return Ok(sensor);
        }

        /// <summary>
        /// Deleta um sensor pelo ID
        /// </summary>
        /// <remarks>
        /// Remove um registro do banco de dados com base no ID informado.
        /// 
        /// Possíveis erros:
        /// - Sensor não encontrado → 404
        /// </remarks>
        /// <param name="id">ID do sensor</param>
        /// <returns>Mensagem de sucesso ou erro 404</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var sensor = await _db.Sensores.FindAsync(id);
            if (sensor == null)
            {
                return NotFound("Sensor nao encontrado!");
            }

            _db.Sensores.Remove(sensor);
            await _db.SaveChangesAsync();
            return Ok("Deletado com sucesso!");
        }
    }
}
