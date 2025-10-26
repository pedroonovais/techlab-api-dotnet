using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace service.Service
{
    /// <summary>
    /// Serviço responsável pela autenticação e geração de tokens JWT
    /// </summary>
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Autentica um usuário com e-mail e senha
        /// </summary>
        /// <param name="email">E-mail do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <returns>Usuário autenticado ou null se as credenciais forem inválidas</returns>
        public async Task<Usuario?> AuthenticateAsync(string email, string senha)
        {
            Console.WriteLine($"[AuthService] Tentativa de autenticação para: {email}");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                Console.WriteLine("[AuthService] E-mail ou senha vazios");
                return null;
            }

            // Busca o usuário pelo e-mail
            var usuario = await _context.Usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (usuario == null)
            {
                Console.WriteLine($"[AuthService] Usuário não encontrado: {email}");
                return null;
            }

            if (!usuario.Ativo)
            {
                Console.WriteLine($"[AuthService] Usuário inativo: {email}");
                return null;
            }

            // Verifica se a senha está correta usando BCrypt
            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);

            if (!senhaCorreta)
            {
                Console.WriteLine($"[AuthService] Senha incorreta para: {email}");
                return null;
            }

            Console.WriteLine($"[AuthService] Autenticação bem-sucedida para: {email}");
            return usuario;
        }

        /// <summary>
        /// Gera um token JWT para o usuário autenticado
        /// </summary>
        /// <param name="usuario">Usuário autenticado</param>
        /// <returns>Token JWT</returns>
        public string GenerateJwtToken(Usuario usuario)
        {
            Console.WriteLine($"[AuthService] Gerando token JWT para usuário: {usuario.Email}");

            // Obtém as configurações JWT do appsettings.json
            var secretKey = _configuration["JwtSettings:SecretKey"] 
                ?? throw new InvalidOperationException("SecretKey não configurada");
            var issuer = _configuration["JwtSettings:Issuer"] 
                ?? throw new InvalidOperationException("Issuer não configurado");
            var audience = _configuration["JwtSettings:Audience"] 
                ?? throw new InvalidOperationException("Audience não configurada");
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "480");

            // Cria as claims (informações) que serão incluídas no token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("PerfilId", usuario.Perfil.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID único do token
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()) // Timestamp de criação
            };

            // Cria a chave de assinatura
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Define a data de expiração do token
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

            // Cria o token JWT
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            // Serializa o token para string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine($"[AuthService] Token JWT gerado com sucesso. Expira em: {expiration}");

            return tokenString;
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="nome">Nome do usuário</param>
        /// <param name="email">E-mail do usuário</param>
        /// <param name="senha">Senha do usuário (será criptografada)</param>
        /// <param name="perfilId">ID do perfil do usuário (opcional)</param>
        /// <returns>Usuário criado</returns>
        public async Task<Usuario> RegisterAsync(string nome, string email, string senha, Guid? perfilId = null)
        {
            Console.WriteLine($"[AuthService] Registrando novo usuário: {email}");

            // Valida os dados de entrada
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("Nome, e-mail e senha são obrigatórios");
            }

            if (senha.Length < 6)
            {
                throw new ArgumentException("A senha deve ter no mínimo 6 caracteres");
            }

            // Verifica se o e-mail já está cadastrado
            var existingUser = await _context.Usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (existingUser != null)
            {
                Console.WriteLine($"[AuthService] E-mail já cadastrado: {email}");
                throw new InvalidOperationException("E-mail já cadastrado no sistema");
            }

            // Se o perfil não foi fornecido, busca ou cria um perfil padrão
            Guid finalPerfilId;
            
            if (perfilId.HasValue && perfilId.Value != Guid.Empty)
            {
                // Verifica se o perfil fornecido existe
                var perfilExists = await _context.Perfil.AnyAsync(p => p.Id == perfilId.Value);
                if (!perfilExists)
                {
                    Console.WriteLine($"[AuthService] Perfil não encontrado: {perfilId}");
                    throw new ArgumentException("Perfil não encontrado");
                }
                finalPerfilId = perfilId.Value;
            }
            else
            {
                // Busca ou cria um perfil padrão
                Console.WriteLine("[AuthService] Perfil não fornecido, buscando perfil padrão");
                var perfilPadrao = await _context.Perfil
                    .FirstOrDefaultAsync(p => p.Nome.ToLower() == "usuário padrão" || p.Nome.ToLower() == "usuario padrao");
                
                if (perfilPadrao == null)
                {
                    // Cria um perfil padrão
                    Console.WriteLine("[AuthService] Criando perfil padrão");
                    perfilPadrao = new library.Model.Perfil
                    {
                        Id = Guid.NewGuid(),
                        Nome = "Usuário Padrão",
                        NivelAcesso = 1,
                        DtCadastro = DateTime.UtcNow,
                        DtAtualizacao = DateTime.UtcNow,
                        Ativo = true
                    };
                    _context.Perfil.Add(perfilPadrao);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[AuthService] Perfil padrão criado com ID: {perfilPadrao.Id}");
                }
                
                finalPerfilId = perfilPadrao.Id;
                Console.WriteLine($"[AuthService] Usando perfil padrão: {finalPerfilId}");
            }

            // Criptografa a senha usando BCrypt
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

            // Cria o novo usuário
            var novoUsuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = nome,
                Email = email.ToLower(),
                Senha = senhaHash,
                Perfil = finalPerfilId,
                DtCriacao = DateTime.UtcNow,
                DtAlteracao = DateTime.UtcNow,
                Ativo = true
            };

            _context.Usuario.Add(novoUsuario);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[AuthService] Usuário registrado com sucesso: {email}");

            return novoUsuario;
        }

        /// <summary>
        /// Verifica se um e-mail já está cadastrado no sistema
        /// </summary>
        /// <param name="email">E-mail a ser verificado</param>
        /// <returns>True se o e-mail já existe, False caso contrário</returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Usuario
                .AsNoTracking()
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        /// <summary>
        /// Obtém o tempo de expiração configurado para os tokens JWT
        /// </summary>
        /// <returns>Tempo de expiração em segundos</returns>
        public int GetTokenExpirationInSeconds()
        {
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "480");
            return expirationMinutes * 60;
        }
    }
}

