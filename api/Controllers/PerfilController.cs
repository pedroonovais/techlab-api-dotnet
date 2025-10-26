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
    public class PerfilController : ControllerBase
    {
        private readonly PerfilService _service;

        public PerfilController(PerfilService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os perfis cadastrados com paginação.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
                                             [FromQuery] string? search = null, [FromQuery] string? sort = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search, sort ?? "-DtCadastro");

            var items = result.Items.Select(perfil =>
            {
                var res = new Resource<Perfil> { Data = perfil };
                res.Links.Add("self", new Link($"/api/v1/Perfil/{perfil.Id}", "GET"));
                res.Links.Add("update", new Link($"/api/v1/Perfil/{perfil.Id}", "PUT"));
                res.Links.Add("delete", new Link($"/api/v1/Perfil/{perfil.Id}", "DELETE"));
                return res;
            }).ToList();

            var page = new PagedResource<Perfil>
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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var perfil = _service.GetById(id);
            if (perfil == null)
                return NotFound();

            var resource = new Resource<Perfil> { Data = perfil };
            resource.Links.Add("self", new Link($"/api/v1/Perfil/{id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Perfil/{id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Perfil/{id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Perfil", "GET"));

            return Ok(resource);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Perfil perfil)
        {
            if (perfil == null || string.IsNullOrWhiteSpace(perfil.Nome) || perfil.NivelAcesso == 0)
                return BadRequest("Nome e Nível de acesso são obrigatórios.");

            var novo = _service.Create(perfil);

            var resource = new Resource<Perfil> { Data = novo };
            resource.Links.Add("self", new Link($"/api/v1/Perfil/{novo.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Perfil/{novo.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Perfil/{novo.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Perfil", "GET"));

            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, resource);
        }

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

            var resource = new Resource<Perfil> { Data = perfil };
            resource.Links.Add("self", new Link($"/api/v1/Perfil/{perfil.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Perfil/{perfil.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Perfil/{perfil.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Perfil", "GET"));

            return Ok(resource);
        }

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
