using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.Service;
using library.Model;
using api.Resources;
using Asp.Versioning;

namespace api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class MotoController : ControllerBase
    {
        private readonly MotoService _service;

        public MotoController(MotoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todas as motos cadastradas com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página (default = 1)</param>
        /// <param name="pageSize">Tamanho da página (default = 10)</param>
        /// <param name="search">Filtro por marca/modelo (opcional)</param>
        /// <param name="sort">Ordenação (ex: marca, -marca, dtcadastro, -dtcadastro)</param>
        /// <response code="200">Lista paginada de motos retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
                                             [FromQuery] string? search = null, [FromQuery] string? sort = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search, sort ?? "-DtCadastro");
            
            var items = result.Items.Select(moto =>
            {
                var res = new Resource<Moto> { Data = moto };
                res.Links.Add("self", new Link($"/api/v1/Moto/{moto.Id}", "GET"));
                res.Links.Add("update", new Link($"/api/v1/Moto/{moto.Id}", "PUT"));
                res.Links.Add("delete", new Link($"/api/v1/Moto/{moto.Id}", "DELETE"));
                return res;
            }).ToList();

            var page = new PagedResource<Moto>
            {
                Items = items,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };

            page.Links.Add("self", new Link(PaginationLinkBuilder.BuildUrl(Request, result.PageNumber, result.PageSize), "GET"));
            if (result.PageNumber > 1)
                page.Links.Add("prev", new Link(PaginationLinkBuilder.BuildUrl(Request, result.PageNumber - 1, result.PageSize), "GET"));
            if (result.PageNumber < result.TotalPages)
                page.Links.Add("next", new Link(PaginationLinkBuilder.BuildUrl(Request, result.PageNumber + 1, result.PageSize), "GET"));
            if (result.TotalPages > 0)
            {
                page.Links.Add("first", new Link(PaginationLinkBuilder.BuildUrl(Request, 1, result.PageSize), "GET"));
                page.Links.Add("last", new Link(PaginationLinkBuilder.BuildUrl(Request, result.TotalPages, result.PageSize), "GET"));
            }

            return Ok(page);
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

            resource.Links.Add("self", new Link($"/api/v1/Moto/{id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Moto/{id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Moto/{id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Moto", "GET"));

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

            resource.Links.Add("self", new Link($"/api/v1/Moto/{newMoto.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Moto/{newMoto.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Moto/{newMoto.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Moto", "GET"));

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

            resource.Links.Add("self", new Link($"/api/v1/Moto/{moto.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Moto/{moto.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Moto/{moto.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Moto", "GET"));

            return Ok(resource);
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
