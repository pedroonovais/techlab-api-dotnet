using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;
using service.Common;

namespace service.Service
{
    public class MotoService
    {
        private readonly AppDbContext _context;

        public MotoService(AppDbContext context)
        {
            _context = context;
        }

        // --- Paginação ---
        public async Task<PagedResult<Moto>> GetPagedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sort = "-DtCadastro",
            CancellationToken ct = default)
        {
            var query = _context.Moto.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.Marca.Contains(search) || m.Modelo.Contains(search));
            }

            switch (sort?.ToLowerInvariant())
            {
                case "marca":
                    query = query.OrderBy(m => m.Marca);
                    break;
                case "-marca":
                    query = query.OrderByDescending(m => m.Marca);
                    break;
                case "dtcadastro":
                    query = query.OrderBy(m => EF.Property<DateTime?>(m, "DtCadastro"));
                    break;
                case "-dtcadastro":
                default:
                    query = query.OrderByDescending(m => EF.Property<DateTime?>(m, "DtCadastro"));
                    break;
            }

            var total = await query.CountAsync(ct);
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync(ct);

            return new PagedResult<Moto>(items, total, pageNumber, pageSize);
        }

        public IEnumerable<Moto> GetAll()
        {
            return _context.Moto.AsNoTracking().ToList();
        }

        public Moto? GetById(Guid id)
        {
            return _context.Moto.Find(id);
        }

        public Moto Create(Moto moto)
        {
            if (moto == null)
                throw new ArgumentNullException(nameof(moto));

            if (string.IsNullOrWhiteSpace(moto.Marca) || string.IsNullOrWhiteSpace(moto.Modelo))
                throw new ArgumentException("Marca e modelo são obrigatórios.");

            moto.GetType()
                .GetProperty("DtCadastro")?
                .SetValue(moto, DateTime.UtcNow);

            moto.GetType()
                .GetProperty("DtAtualizacao")?
                .SetValue(moto, DateTime.UtcNow);

            _context.Moto.Add(moto);
            _context.SaveChanges();
            return moto;
        }

        public bool Update(Guid id, Moto updatedMoto)
        {
            var existingMoto = GetById(id);
            if (existingMoto == null)
                return false;

            if (string.IsNullOrWhiteSpace(updatedMoto.Marca) || string.IsNullOrWhiteSpace(updatedMoto.Modelo))
                throw new ArgumentException("Marca e modelo são obrigatórios.");

            updatedMoto.GetType()
                .GetProperty("DtAtualizacao")?
                .SetValue(updatedMoto, DateTime.UtcNow);

            _context.Entry(existingMoto).State = EntityState.Detached;
            _context.Moto.Update(updatedMoto);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var moto = GetById(id);
            if (moto == null)
                return false;

            _context.Moto.Remove(moto);
            _context.SaveChanges();
            return true;
        }
    }
}
