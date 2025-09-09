using Microsoft.AspNetCore.Mvc;
using service.Service;
using library.Model;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusOperacionalController : ControllerBase
    {
        private readonly StatusOperacionalService _service;

        public StatusOperacionalController(StatusOperacionalService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os status operacionais cadastrados.
        /// </summary>
        /// <response code="200">Lista de status retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var itens = _service.GetAll();
            return Ok(itens);
        }

        /// <summary>
        /// Retorna um status operacional específico por ID.
        /// </summary>
        /// <param name="id">ID do status operacional.</param>
        /// <response code="200">Status encontrado.</response>
        /// <response code="404">Status não encontrado.</response>
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
        /// Cadastra um novo status operacional.
        /// </summary>
        /// <param name="status">Dados do status a ser cadastrado.</param>
        /// <response code="201">Status criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] StatusOperacional status)
        {
            if (status == null)
                return BadRequest("Dados inválidos.");

            var novo = _service.Create(status);
            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
        }

        /// <summary>
        /// Atualiza um status operacional existente.
        /// </summary>
        /// <param name="id">ID do status operacional.</param>
        /// <param name="status">Dados atualizados.</param>
        /// <response code="200">Status atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Status não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] StatusOperacional status)
        {
            if (status == null || id == Guid.Empty || status.Id != id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, status);
            if (!updated)
                return NotFound();

            return Ok(status);
        }

        /// <summary>
        /// Remove um status operacional pelo ID.
        /// </summary>
        /// <param name="id">ID do status.</param>
        /// <response code="204">Status removido com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Status não encontrado.</response>
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
