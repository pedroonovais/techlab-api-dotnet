using Microsoft.AspNetCore.Mvc;
using service.Service;
using library.Model;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RastreadorController : ControllerBase
    {
        private readonly RastreadorService _service;

        public RastreadorController(RastreadorService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os rastreadores cadastrados.
        /// </summary>
        /// <response code="200">Lista de rastreadores retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var itens = _service.GetAll();
            return Ok(itens);
        }

        /// <summary>
        /// Retorna um rastreador específico por ID.
        /// </summary>
        /// <param name="id">ID do rastreador.</param>
        /// <response code="200">Rastreador encontrado.</response>
        /// <response code="404">Rastreador não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var item = _service.GetById(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// Cadastra um novo rastreador.
        /// </summary>
        /// <param name="rastreador">Dados do rastreador a ser cadastrado.</param>
        /// <response code="201">Rastreador criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Rastreador rastreador)
        {
            if (rastreador == null)
                return BadRequest("Dados inválidos.");

            var novo = _service.Create(rastreador);
            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
        }

        /// <summary>
        /// Atualiza os dados de um rastreador existente.
        /// </summary>
        /// <param name="id">ID do rastreador.</param>
        /// <param name="rastreador">Dados atualizados.</param>
        /// <response code="200">Rastreador atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Rastreador não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] Rastreador rastreador)
        {
            if (rastreador == null || id == Guid.Empty || rastreador.Id != id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, rastreador);
            if (!updated)
                return NotFound();

            return Ok(rastreador);
        }

        /// <summary>
        /// Remove um rastreador pelo ID.
        /// </summary>
        /// <param name="id">ID do rastreador.</param>
        /// <response code="204">Rastreador removido com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Rastreador não encontrado.</response>
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
