using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;

namespace service.Service
{
    public class MotoService
    {
        private readonly AppDbContext _context;

        public MotoService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Moto> GetAll()
        {
            return _context.Moto.ToList();
        }

        public Moto? GetById(Guid id)
        {
            return _context.Moto.Find(id);
        }

        public Moto Create(Moto moto)
        {
            if (moto == null)
                throw new ArgumentNullException(nameof(moto));

            if (string.IsNullOrWhiteSpace(moto.marca) || string.IsNullOrWhiteSpace(moto.modelo))
                throw new ArgumentException("Marca e modelo são obrigatórios.");

            moto.GetType()
                .GetProperty("dataCadastro")?
                .SetValue(moto, DateTime.UtcNow);

            moto.GetType()
                .GetProperty("dataAtualizacao")?
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

            if (string.IsNullOrWhiteSpace(updatedMoto.marca) || string.IsNullOrWhiteSpace(updatedMoto.modelo))
                throw new ArgumentException("Marca e modelo são obrigatórios.");

            updatedMoto.GetType()
                .GetProperty("dataAtualizacao")?
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
