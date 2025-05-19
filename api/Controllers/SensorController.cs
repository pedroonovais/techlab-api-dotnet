using Microsoft.AspNetCore.Mvc;
using service.Service;
using library.Model;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly SensorService _service;

        public SensorController(SensorService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os sensores cadastrados.
        /// </summary>
        /// <response code="200">Lista de sensores retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var sensores = _service.GetAll();
            return Ok(sensores);
        }

        /// <summary>
        /// Retorna um sensor específico por ID.
        /// </summary>
        /// <param name="id">ID do sensor.</param>
        /// <response code="200">Sensor encontrado.</response>
        /// <response code="404">Sensor não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var sensor = _service.GetById(id);
            if (sensor == null)
                return NotFound();

            return Ok(sensor);
        }

        /// <summary>
        /// Cadastra um novo sensor.
        /// </summary>
        /// <param name="sensor">Dados do sensor a ser cadastrado.</param>
        /// <response code="201">Sensor criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Sensor sensor)
        {
            if (sensor == null || string.IsNullOrWhiteSpace(sensor.tipo) || string.IsNullOrWhiteSpace(sensor.localizacaoFisica))
            {
                return BadRequest("Dados inválidos.");
            }

            var newSensor = _service.Create(sensor);
            return CreatedAtAction(nameof(GetById), new { id = newSensor.id }, newSensor);
        }

        /// <summary>
        /// Atualiza os dados de um sensor existente.
        /// </summary>
        /// <param name="id">ID do sensor a ser atualizado.</param>
        /// <param name="sensor">Dados atualizados do sensor.</param>
        /// <response code="200">Sensor atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Sensor não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] Sensor sensor)
        {
            if (sensor == null || id == Guid.Empty || sensor.id != id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, sensor);
            if (!updated)
                return NotFound();

            return Ok(sensor);
        }

        /// <summary>
        /// Remove um sensor pelo ID.
        /// </summary>
        /// <param name="id">ID do sensor.</param>
        /// <response code="204">Sensor removido com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Sensor não encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("ID inválido.");

            var deleted = _service.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
