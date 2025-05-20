using Microsoft.AspNetCore.Mvc;
using service.Service;
using library.Model;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeituraRfidController : ControllerBase
    {
        private readonly LeituraRfidService _service;

        public LeituraRfidController(LeituraRfidService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todas as leituras RFID cadastradas.
        /// </summary>
        /// <response code="200">Lista de leituras retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var leituras = _service.GetAll();
            return Ok(leituras);
        }

        /// <summary>
        /// Retorna uma leitura RFID específica por ID.
        /// </summary>
        /// <param name="id">ID da leitura RFID.</param>
        /// <response code="200">Leitura encontrada.</response>
        /// <response code="404">Leitura não encontrada.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var leitura = _service.GetById(id);
            if (leitura == null)
                return NotFound();

            return Ok(leitura);
        }

        /// <summary>
        /// Cadastra uma nova leitura RFID.
        /// </summary>
        /// <param name="leitura">Dados da leitura RFID a ser cadastrada.</param>
        /// <response code="201">Leitura criada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] LeituraRfid leitura)
        {
            if (leitura == null || leitura.RfidId == Guid.Empty || leitura.SensorId == Guid.Empty)
            {
                return BadRequest("Dados inválidos.");
            }

            var newLeitura = _service.Create(leitura);
            return CreatedAtAction(nameof(GetById), new { id = newLeitura.Id }, newLeitura);
        }

        /// <summary>
        /// Remove uma leitura RFID pelo ID.
        /// </summary>
        /// <param name="id">ID da leitura RFID.</param>
        /// <response code="204">Leitura removida com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Leitura não encontrada.</response>
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

        /// <summary>
        /// Atualiza uma leitura RFID existente.
        /// </summary>
        /// <param name="id">ID da leitura a ser atualizada.</param>
        /// <param name="leituraAtualizada">Dados atualizados da leitura.</param>
        /// <response code="200">Leitura atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Leitura não encontrada.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] LeituraRfid leituraAtualizada)
        {
            if (id == Guid.Empty || leituraAtualizada == null ||
                leituraAtualizada.RfidId == Guid.Empty || leituraAtualizada.SensorId == Guid.Empty)
            {
                return BadRequest("Dados inválidos.");
            }

            var leitura = _service.Update(id, leituraAtualizada);
            if (leitura == null)
                return NotFound();

            return Ok(leitura);
        }
    }
}
