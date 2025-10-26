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
    public class RastreadorController : ControllerBase
    {
        private readonly RastreadorService _service;

        public RastreadorController(RastreadorService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os rastreadores cadastrados com paginação.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
                                             [FromQuery] string? sort = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, sort ?? "Id");

            var items = result.Items.Select(r =>
            {
                var res = new Resource<Rastreador> { Data = r };
                res.Links.Add("self", new Link($"/api/v1/Rastreador/{r.Id}", "GET"));
                res.Links.Add("update", new Link($"/api/v1/Rastreador/{r.Id}", "PUT"));
                res.Links.Add("delete", new Link($"/api/v1/Rastreador/{r.Id}", "DELETE"));
                return res;
            }).ToList();

            var page = new PagedResource<Rastreador>
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
            var item = _service.GetById(id);
            if (item == null)
                return NotFound();

            var resource = new Resource<Rastreador> { Data = item };
            resource.Links.Add("self", new Link($"/api/v1/Rastreador/{id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Rastreador/{id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Rastreador/{id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Rastreador", "GET"));

            return Ok(resource);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Rastreador rastreador)
        {
            if (rastreador == null)
                return BadRequest("Dados inválidos.");

            var novo = _service.Create(rastreador);

            var resource = new Resource<Rastreador> { Data = novo };
            resource.Links.Add("self", new Link($"/api/v1/Rastreador/{novo.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Rastreador/{novo.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Rastreador/{novo.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Rastreador", "GET"));

            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, resource);
        }

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

            var resource = new Resource<Rastreador> { Data = rastreador };
            resource.Links.Add("self", new Link($"/api/v1/Rastreador/{rastreador.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Rastreador/{rastreador.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Rastreador/{rastreador.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Rastreador", "GET"));

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
