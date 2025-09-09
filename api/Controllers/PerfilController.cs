using Microsoft.AspNetCore.Mvc;
using service.Service;
using library.Model;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilController : ControllerBase
    {
        private readonly PerfilService _service;

        public PerfilController(PerfilService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os perfis cadastrados.
        /// </summary>
        /// <response code="200">Lista de perfis retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var perfis = _service.GetAll();
            return Ok(perfis);
        }

        /// <summary>
        /// Retorna um perfil específico por ID.
        /// </summary>
        /// <param name="id">ID do perfil.</param>
        /// <response code="200">Perfil encontrado.</response>
        /// <response code="404">Perfil não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var perfil = _service.GetById(id);
            if (perfil == null)
                return NotFound();

            return Ok(perfil);
        }

        /// <summary>
        /// Cadastra um novo perfil.
        /// </summary>
        /// <param name="perfil">Dados do perfil a ser cadastrado.</param>
        /// <response code="201">Perfil criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Perfil perfil)
        {
            if (perfil == null || string.IsNullOrWhiteSpace(perfil.Nome) || perfil.NivelAcesso == 0)
                return BadRequest("Nome e Nível de acesso são obrigatórios.");

            var novo = _service.Create(perfil);
            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
        }

        /// <summary>
        /// Atualiza os dados de um perfil existente.
        /// </summary>
        /// <param name="id">ID do perfil.</param>
        /// <param name="perfil">Dados atualizados do perfil.</param>
        /// <response code="200">Perfil atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Perfil não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] Perfil perfil)
        {
            if (perfil == null || id == Guid.Empty || perfil.Id != id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, perfil);
            if (!updated)
                return NotFound();

            return Ok(perfil);
        }

        /// <summary>
        /// Remove um perfil pelo ID.
        /// </summary>
        /// <param name="id">ID do perfil.</param>
        /// <response code="204">Perfil removido com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Perfil não encontrado.</response>
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
