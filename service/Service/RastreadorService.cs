using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;

namespace service.Service
{
    public class RastreadorService
    {
        private readonly AppDbContext _context;

        public RastreadorService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Rastreador> GetAll()
        {
            return _context.Rastreador.ToList();
        }

        public Rastreador? GetById(Guid id)
        {
            return _context.Rastreador.Find(id);
        }

        public Rastreador Create(Rastreador rastreador)
        {
            if (rastreador == null)
                throw new ArgumentNullException(nameof(rastreador));

            // Se você tiver campos de auditoria públicos (ex.: DtCadastro/DtAtualizacao),
            // ajuste aqui como no PerfilService. Como o modelo expõe só Id publicamente,
            // manteremos o insert simples.
            _context.Rastreador.Add(rastreador);
            _context.SaveChanges();
            return rastreador;
        }

        public bool Update(Guid id, Rastreador updatedRastreador)
        {
            var existing = GetById(id);
            if (existing == null)
                return false;

            // Garante consistência do Id
            updatedRastreador.Id = id;

            // Se você tiver campos de auditoria públicos, ajuste aqui (DtAtualizacao, etc).

            // Desanexa o existente e atualiza com o objeto recebido
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
