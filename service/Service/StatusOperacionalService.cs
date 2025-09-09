using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;

namespace service.Service
{
    public class StatusOperacionalService
    {
        private readonly AppDbContext _context;

        public StatusOperacionalService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<StatusOperacional> GetAll()
        {
            return _context.StatusOperacional.ToList();
        }

        public StatusOperacional? GetById(Guid id)
        {
            return _context.StatusOperacional.Find(id);
        }

        public StatusOperacional Create(StatusOperacional status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            // Se houver propriedades obrigatórias públicas (ex.: Descricao),
            // valide aqui. No seu modelo atual, ela está privada, então seguimos simples.

            _context.StatusOperacional.Add(status);
            _context.SaveChanges();
            return status;
        }

        public bool Update(Guid id, StatusOperacional updatedStatus)
        {
            var existing = GetById(id);
            if (existing == null)
                return false;

            // Garante consistência do Id
            updatedStatus.Id = id;

            // Se tiver campos de auditoria públicos, ajuste aqui (DtAtualizacao, etc).

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
