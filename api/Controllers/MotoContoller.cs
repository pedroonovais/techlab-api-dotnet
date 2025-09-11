using Microsoft.AspNetCore.Mvc;
using service.Service;
using library.Model;
using api.Resources;

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

            var resources = motos.Select(moto =>
            {
                var resource = new Resource<Moto>
                {
                    Data = moto
                };
                resource.Links.Add("self", new Link($"/api/Moto/{moto.Id}", "GET"));
                resource.Links.Add("update", new Link($"/api/Moto/{moto.Id}", "PUT"));
                resource.Links.Add("delete", new Link($"/api/Moto/{moto.Id}", "DELETE"));
                return resource;
            }).ToList();

            return Ok(resources);
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
        public IActionResult GetById(Guid id)
        {
            var moto = _service.GetById(id);
            if (moto == null)
                return NotFound();

            var resource = new Resource<Moto>
            {
                Data = moto
            };

            resource.Links.Add("self", new Link($"/api/Moto/{id}", "GET"));
            resource.Links.Add("update", new Link($"/api/Moto/{id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/Moto/{id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/Moto", "GET"));

            return Ok(resource);
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
            if (moto == null || string.IsNullOrWhiteSpace(moto.Marca) || string.IsNullOrWhiteSpace(moto.Modelo))
            {
                return BadRequest("Dados inválidos.");
            }

            var newMoto = _service.Create(moto);

            var resource = new Resource<Moto>
            {
                Data = newMoto
            };

            resource.Links.Add("self", new Link($"/api/Moto/{newMoto.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/Moto/{newMoto.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/Moto/{newMoto.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/Moto", "GET"));

            return CreatedAtAction(nameof(GetById), new { id = newMoto.Id }, resource);
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
        public IActionResult Put(Guid id, [FromBody] Moto moto)
        {
            if (moto == null || id == Guid.Empty || moto.Id != id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, moto);
            
            if (!updated)
                return NotFound();

            var resource = new Resource<Moto>
            {
                Data = moto
            };

            resource.Links.Add("self", new Link($"/api/Moto/{moto.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/Moto/{moto.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/Moto/{moto.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/Moto", "GET"));

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
