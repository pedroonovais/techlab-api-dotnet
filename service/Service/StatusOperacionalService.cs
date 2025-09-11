using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;
using service.Common;

namespace service.Service
{
    public class StatusOperacionalService
    {
        private readonly AppDbContext _context;

        public StatusOperacionalService(AppDbContext context)
        {
            _context = context;
        }

        // --- Paginação ---
        public async Task<PagedResult<StatusOperacional>> GetPagedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? sort = "Id",
            CancellationToken ct = default)
        {
            var query = _context.StatusOperacional.AsNoTracking().AsQueryable();

            switch (sort?.ToLowerInvariant())
            {
                case "id":
                default:
                    query = query.OrderBy(s => s.Id);
                    break;
                case "-id":
                    query = query.OrderByDescending(s => s.Id);
                    break;
            }

            var total = await query.CountAsync(ct);
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync(ct);

            return new PagedResult<StatusOperacional>(items, total, pageNumber, pageSize);
        }

        public IEnumerable<StatusOperacional> GetAll()
        {
            return _context.StatusOperacional.AsNoTracking().ToList();
        }

        public StatusOperacional? GetById(Guid id)
        {
            return _context.StatusOperacional.Find(id);
        }

        public StatusOperacional Create(StatusOperacional status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            _context.StatusOperacional.Add(status);
            _context.SaveChanges();
            return status;
        }

        public bool Update(Guid id, StatusOperacional updatedStatus)
        {
            var existing = GetById(id);
            if (existing == null)
                return false;

            updatedStatus.Id = id;

            _context.Entry(existing).State = EntityState.Detached;
            _context.StatusOperacional.Update(updatedStatus);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var entity = GetById(id);
            if (entity == null)
                return false;

            _context.StatusOperacional.Remove(entity);
            _context.SaveChanges();
            return true;
        }
    }
}
