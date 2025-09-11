using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;
using service.Common;

namespace service.Service
{
    public class PerfilService
    {
        private readonly AppDbContext _context;

        public PerfilService(AppDbContext context)
        {
            _context = context;
        }

        // --- Paginação ---
        public async Task<PagedResult<Perfil>> GetPagedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sort = "-DtCadastro",
            CancellationToken ct = default)
        {
            var query = _context.Perfil.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Nome.Contains(search));
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

            return new PagedResult<Perfil>(items, total, pageNumber, pageSize);
        }

        public IEnumerable<Perfil> GetAll()
        {
            return _context.Perfil.AsNoTracking().ToList();
        }

        public Perfil? GetById(Guid id)
        {
            return _context.Perfil.Find(id);
        }

        public Perfil Create(Perfil perfil)
        {
            if (perfil == null)
                throw new ArgumentNullException(nameof(perfil));

            if (string.IsNullOrWhiteSpace(perfil.Nome) || perfil.NivelAcesso == 0)
                throw new ArgumentException("Nome e Nivel de acesso são obrigatórios.");

            perfil.DtCadastro = DateTime.UtcNow;
            perfil.DtAtualizacao = DateTime.UtcNow;

            _context.Perfil.Add(perfil);
            _context.SaveChanges();
            return perfil;
        }

        public bool Update(Guid id, Perfil updatedPerfil)
        {
            var existingPerfil = GetById(id);
            if (existingPerfil == null)
                return false;

            if (string.IsNullOrWhiteSpace(updatedPerfil.Nome) || updatedPerfil.NivelAcesso == 0)
                throw new ArgumentException("Nome e Nível de acesso são obrigatórios.");

            updatedPerfil.DtAtualizacao = DateTime.UtcNow;

            _context.Entry(existingPerfil).State = EntityState.Detached;
            _context.Perfil.Update(updatedPerfil);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var perfil = GetById(id);
            if (perfil == null)
                return false;

            _context.Perfil.Remove(perfil);
            _context.SaveChanges();
            return true;
        }
    }
}
