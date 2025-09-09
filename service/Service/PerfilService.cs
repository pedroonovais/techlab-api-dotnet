using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;

namespace service.Service
{
    public class PerfilService
    {
        private readonly AppDbContext _context;

        public PerfilService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Perfil> GetAll()
        {
            return _context.Perfil.ToList();
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
