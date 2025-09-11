using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;
using service.Common;

namespace service.Service
{
    public class PatioService
    {
        private readonly AppDbContext _context;

        public PatioService(AppDbContext context)
        {
            _context = context;
        }

        // --- Paginação ---
        public async Task<PagedResult<Patio>> GetPagedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sort = "-DtCadastro",
            CancellationToken ct = default)
        {
            var query = _context.Patio.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Nome.Contains(search) || p.Localizacao.Contains(search));
            }

            switch (sort?.ToLowerInvariant())
            {
                case "nome":
                    query = query.OrderBy(p => p.Nome);
                    break;
                case "-nome":
                    query = query.OrderByDescending(p => p.Nome);
                    break;
                case "dtcadastro":
                    query = query.OrderBy(p => p.DtCadastro);
                    break;
                case "-dtcadastro":
                default:
                    query = query.OrderByDescending(p => p.DtCadastro);
                    break;
            }

            var total = await query.CountAsync(ct);
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync(ct);

            return new PagedResult<Patio>(items, total, pageNumber, pageSize);
        }

        public IEnumerable<Patio> GetAll()
        {
            return _context.Patio.AsNoTracking().ToList();
        }

        public Patio? GetById(Guid id)
        {
            return _context.Patio.Find(id);
        }

        public Patio Create(Patio patio)
        {
            if (patio == null)
                throw new ArgumentNullException(nameof(patio));

            if (string.IsNullOrWhiteSpace(patio.Nome) || string.IsNullOrWhiteSpace(patio.Localizacao))
                throw new ArgumentException("Nome e localização são obrigatórios.");

            patio.DtCadastro = DateTime.UtcNow;
            patio.DtAtualizacao = DateTime.UtcNow;

            _context.Patio.Add(patio);
            _context.SaveChanges();
            return patio;
        }

        public bool Update(Guid id, Patio updatedPatio)
        {
            var existingPatio = GetById(id);
            if (existingPatio == null)
                return false;

            if (string.IsNullOrWhiteSpace(updatedPatio.Nome) || string.IsNullOrWhiteSpace(updatedPatio.Localizacao))
                throw new ArgumentException("Nome e localização são obrigatórios.");

            updatedPatio.DtAtualizacao = DateTime.UtcNow;

            _context.Entry(existingPatio).State = EntityState.Detached;
            _context.Patio.Update(updatedPatio);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var patio = GetById(id);
            if (patio == null)
                return false;

            _context.Patio.Remove(patio);
            _context.SaveChanges();
            return true;
        }
    }
}
