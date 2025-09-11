using Microsoft.AspNetCore.Mvc;
using service.Service;
using library.Model;
using api.Resources;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os usuários cadastrados.
        /// </summary>
        /// <response code="200">Lista de usuários retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var usuarios = _service.GetAll();

            var resources = usuarios.Select(u =>
            {
                var resource = new Resource<Usuario>
                {
                    Data = u
                };

                resource.Links.Add("self", new Link($"/api/Usuario/{u.Id}", "GET"));
                resource.Links.Add("update", new Link($"/api/Usuario/{u.Id}", "PUT"));
                resource.Links.Add("delete", new Link($"/api/Usuario/{u.Id}", "DELETE"));

                return resource;
            }).ToList();

            return Ok(resources);
        }

        /// <summary>
        /// Retorna um usuário específico por ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <response code="200">Usuário encontrado.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var usuario = _service.GetById(id);
            if (usuario == null)
                return NotFound();

            var resource = new Resource<Usuario>
            {
                Data = usuario
            };

            resource.Links.Add("self", new Link($"/api/Usuario/{id}", "GET"));
            resource.Links.Add("update", new Link($"/api/Usuario/{id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/Usuario/{id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/Usuario", "GET"));

            return Ok(resource);
        }

        /// <summary>
        /// Cadastra um novo usuário.
        /// </summary>
        /// <param name="usuario">Dados do usuário a ser cadastrado.</param>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Usuario usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Nome) || string.IsNullOrWhiteSpace(usuario.Email))
                return BadRequest("Dados inválidos.");

            var newUsuario = _service.Create(usuario);

            var resource = new Resource<Usuario>
            {
                Data = newUsuario
            };

            resource.Links.Add("self", new Link($"/api/Usuario/{newUsuario.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/Usuario/{newUsuario.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/Usuario/{newUsuario.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/Usuario", "GET"));

            return CreatedAtAction(nameof(GetById), new { id = newUsuario.Id }, resource);
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário a ser atualizado.</param>
        /// <param name="usuario">Dados atualizados do usuário.</param>
        /// <response code="200">Usuário atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] Usuario usuario)
        {
            if (usuario == null || id != usuario.Id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, usuario);
            if (!updated)
                return NotFound();

            var resource = new Resource<Usuario>
            {
                Data = usuario
            };

            resource.Links.Add("self", new Link($"/api/Usuario/{usuario.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/Usuario/{usuario.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/Usuario/{usuario.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/Usuario", "GET"));

            return Ok(resource);
        }

        /// <summary>
        /// Remove um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <response code="204">Usuário removido com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Usuário não encontrado.</response>
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
