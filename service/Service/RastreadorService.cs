using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;
using service.Common;

namespace service.Service
{
    public class RastreadorService
    {
        private readonly AppDbContext _context;

        public RastreadorService(AppDbContext context)
        {
            _context = context;
        }

        // --- Paginação ---
        public async Task<PagedResult<Rastreador>> GetPagedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? sort = "Id",
            CancellationToken ct = default)
        {
            var query = _context.Rastreador.AsNoTracking().AsQueryable();

            switch (sort?.ToLowerInvariant())
            {
                case "id":
                default:
                    query = query.OrderBy(r => r.Id);
                    break;
                case "-id":
                    query = query.OrderByDescending(r => r.Id);
                    break;
            }

            var total = await query.CountAsync(ct);
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync(ct);

            return new PagedResult<Rastreador>(items, total, pageNumber, pageSize);
        }

        public IEnumerable<Rastreador> GetAll()
        {
            return _context.Rastreador.AsNoTracking().ToList();
        }

        public Rastreador? GetById(Guid id)
        {
            return _context.Rastreador.Find(id);
        }

        public Rastreador Create(Rastreador rastreador)
        {
            if (rastreador == null)
                throw new ArgumentNullException(nameof(rastreador));

            _context.Rastreador.Add(rastreador);
            _context.SaveChanges();
            return rastreador;
        }

        public bool Update(Guid id, Rastreador updatedRastreador)
        {
            var existing = GetById(id);
            if (existing == null)
                return false;

            updatedRastreador.Id = id;

            _context.Entry(existing).State = EntityState.Detached;
            _context.Rastreador.Update(updatedRastreador);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var entity = GetById(id);
            if (entity == null)
                return false;

            _context.Rastreador.Remove(entity);
            _context.SaveChanges();
            return true;
        }
    }
}
