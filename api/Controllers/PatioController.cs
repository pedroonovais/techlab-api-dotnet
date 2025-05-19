using library.Model;
using Microsoft.AspNetCore.Mvc;
using service.Service;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatioController : ControllerBase
    {
        private readonly PatioService _service;

        public PatioController(PatioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os pátios cadastrados.
        /// </summary>
        /// <response code="200">Lista de pátios retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var patios = _service.GetAll();
            return Ok(patios);
        }

        /// <summary>
        /// Retorna um pátio específico por ID.
        /// </summary>
        /// <param name="id">ID do pátio.</param>
        /// <response code="200">Pátio encontrado.</response>
        /// <response code="404">Pátio não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var patio = _service.GetById(id);
            if (patio == null)
                return NotFound();

            return Ok(patio);
        }

        /// <summary>
        /// Cadastra um novo pátio.
        /// </summary>
        /// <param name="patio">Dados do pátio a ser cadastrado.</param>
        /// <response code="201">Pátio criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Patio patio)
        {
            if (patio == null || string.IsNullOrWhiteSpace(patio.nome) || string.IsNullOrWhiteSpace(patio.localizacao))
                return BadRequest("Dados inválidos.");

            var newPatio = _service.Create(patio);
            return CreatedAtAction(nameof(GetById), new { id = newPatio.id }, newPatio);
        }

        /// <summary>
        /// Atualiza os dados de um pátio existente.
        /// </summary>
        /// <param name="id">ID do pátio a ser atualizado.</param>
        /// <param name="patio">Dados atualizados do pátio.</param>
        /// <response code="200">Pátio atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Pátio não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] Patio patio)
        {
            if (patio == null || patio.id != id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, patio);
            if (!updated)
                return NotFound();

            return Ok(patio);
        }

        /// <summary>
        /// Remove um pátio pelo ID.
        /// </summary>
        /// <param name="id">ID do pátio.</param>
        /// <response code="204">Pátio removido com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Pátio não encontrado.</response>
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