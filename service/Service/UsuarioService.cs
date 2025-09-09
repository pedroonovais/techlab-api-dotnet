using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;

namespace service.Service
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Usuario> GetAll()
        {
            return _context.Usuario.ToList();
        }

        public Usuario? GetById(Guid id)
        {
            return _context.Usuario.Find(id);
        }

        public Usuario Create(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (string.IsNullOrWhiteSpace(usuario.Nome) || string.IsNullOrWhiteSpace(usuario.Email))
                throw new ArgumentException("Nome e e-mail são obrigatórios.");

            usuario.DtCriacao = DateTime.UtcNow;
            usuario.DtAlteracao = DateTime.UtcNow;
            usuario.Id = Guid.NewGuid();

            _context.Usuario.Add(usuario);
            _context.SaveChanges();
            return usuario;
        }

        public bool Update(Guid id, Usuario updatedUsuario)
        {
            var existingUsuario = GetById(id);
            if (existingUsuario == null)
                return false;

            if (string.IsNullOrWhiteSpace(updatedUsuario.Nome) || string.IsNullOrWhiteSpace(updatedUsuario.Email))
                throw new ArgumentException("Nome e e-mail são obrigatórios.");

            updatedUsuario.DtAlteracao = DateTime.UtcNow;

            _context.Entry(existingUsuario).State = EntityState.Detached;
            _context.Usuario.Update(updatedUsuario);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var usuario = GetById(id);
            if (usuario == null)
                return false;

            _context.Usuario.Remove(usuario);
            _context.SaveChanges();
            return true;
        }
    }
}
