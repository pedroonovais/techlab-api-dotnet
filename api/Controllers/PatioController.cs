using library.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.Service;
using api.Resources;
using Asp.Versioning;

namespace api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class PatioController : ControllerBase
    {
        private readonly PatioService _service;

        public PatioController(PatioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os pátios cadastrados com paginação.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
                                             [FromQuery] string? search = null, [FromQuery] string? sort = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search, sort ?? "-DtCadastro");

            var items = result.Items.Select(patio =>
            {
                var res = new Resource<Patio> { Data = patio };
                res.Links.Add("self", new Link($"/api/v1/Patio/{patio.Id}", "GET"));
                res.Links.Add("update", new Link($"/api/v1/Patio/{patio.Id}", "PUT"));
                res.Links.Add("delete", new Link($"/api/v1/Patio/{patio.Id}", "DELETE"));
                return res;
            }).ToList();

            var page = new PagedResource<Patio>
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
            var patio = _service.GetById(id);
            if (patio == null)
                return NotFound();

            var resource = new Resource<Patio> { Data = patio };
            resource.Links.Add("self", new Link($"/api/v1/Patio/{id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Patio/{id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Patio/{id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Patio", "GET"));

            return Ok(resource);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Patio patio)
        {
            if (patio == null || string.IsNullOrWhiteSpace(patio.Nome) || string.IsNullOrWhiteSpace(patio.Localizacao))
                return BadRequest("Dados inválidos.");

            var newPatio = _service.Create(patio);

            var resource = new Resource<Patio> { Data = newPatio };
            resource.Links.Add("self", new Link($"/api/v1/Patio/{newPatio.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Patio/{newPatio.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Patio/{newPatio.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Patio", "GET"));

            return CreatedAtAction(nameof(GetById), new { id = newPatio.Id }, resource);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] Patio patio)
        {
            if (patio == null || patio.Id != id)
                return BadRequest("Dados inválidos.");

            var updated = _service.Update(id, patio);
            if (!updated)
                return NotFound();

            var resource = new Resource<Patio> { Data = patio };
            resource.Links.Add("self", new Link($"/api/v1/Patio/{patio.Id}", "GET"));
            resource.Links.Add("update", new Link($"/api/v1/Patio/{patio.Id}", "PUT"));
            resource.Links.Add("delete", new Link($"/api/v1/Patio/{patio.Id}", "DELETE"));
            resource.Links.Add("all", new Link("/api/v1/Patio", "GET"));

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
