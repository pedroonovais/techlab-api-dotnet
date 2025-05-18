using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;

namespace service.Service
{
    public class PatioService
    {
        private readonly AppDbContext _context;

        public PatioService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Patio> GetAll()
        {
            return _context.Patio.ToList();
        }

        public Patio? GetById(Guid id)
        {
            return _context.Patio.Find(id);
        }

        public Patio Create(Patio patio)
        {
            if (patio == null)
                throw new ArgumentNullException(nameof(patio));

            if (string.IsNullOrWhiteSpace(patio.nome) || string.IsNullOrWhiteSpace(patio.localizacao))
                throw new ArgumentException("Nome e localização são obrigatórios.");

            patio.dataCadastro = DateTime.UtcNow;
            patio.dataAtualizacao = DateTime.UtcNow;

            _context.Patio.Add(patio);
            _context.SaveChanges();
            return patio;
        }

        public bool Update(Guid id, Patio updatedPatio)
        {
            var existingPatio = GetById(id);
            if (existingPatio == null)
                return false;

            if (string.IsNullOrWhiteSpace(updatedPatio.nome) || string.IsNullOrWhiteSpace(updatedPatio.localizacao))
                throw new ArgumentException("Nome e localização são obrigatórios.");

            updatedPatio.dataAtualizacao = DateTime.UtcNow;

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
