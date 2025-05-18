using Microsoft.AspNetCore.Mvc;
using service.Service;
using library.Model;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotoController : ControllerBase
    {
        private readonly MotoService _service;

        public MotoController(MotoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todas as motos cadastradas.
        /// </summary>
        /// <response code="200">Lista de motos retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var motos = _service.GetAll();
            return Ok(motos);
        }

        /// <summary>
        /// Retorna uma moto específica por ID.
        /// </summary>
        /// <param name="id">ID da moto.</param>
        /// <response code="200">Moto encontrada.</response>
        /// <response code="404">Moto não encontrada.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var moto = _service.GetById(id);
            if (moto == null)
                return NotFound();

            return Ok(moto);
        }

        /// <summary>
        /// Cadastra uma nova moto.
        /// </summary>
        /// <param name="moto">Dados da moto a ser cadastrada.</param>
        /// <response code="201">Moto criada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Moto moto)
        {
            if (moto == null || string.IsNullOrWhiteSpace(moto.marca) || string.IsNullOrWhiteSpace(moto.modelo))
            {
                return BadRequest("Dados inválidos.");
            }

            var newMoto = _service.Create(moto);
            return CreatedAtAction(nameof(GetById), new { id = newMoto.id }, newMoto);
        }

        /// <summary>
        /// Atualiza os dados de uma moto existente.
        /// </summary>
        /// <param name="id">ID da moto a ser atualizada.</param>
        /// <param name="moto">Dados atualizados da moto.</param>
        /// <response code="200">Moto atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Moto não encontrada.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(int id, [FromBody] Moto moto)
        {
            if (moto == null || id <= 0 || moto.id != id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, moto);
            if (!updated)
                return NotFound();

            return Ok(moto);
        }

        /// <summary>
        /// Remove uma moto pelo ID.
        /// </summary>
        /// <param name="id">ID da moto.</param>
        /// <response code="204">Moto removida com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Moto não encontrada.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var deleted = _service.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
