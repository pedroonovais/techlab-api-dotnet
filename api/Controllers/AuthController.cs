using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.Service;
using api.DTOs;
using Asp.Versioning;

namespace api.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação de usuários
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Construtor do AuthController
        /// </summary>
        /// <param name="authService">Serviço de autenticação</param>
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza o login de um usuário e retorna um token JWT
        /// </summary>
        /// <param name="request">Dados de login (e-mail e senha)</param>
        /// <returns>Token JWT e informações do usuário</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"[AuthController] Requisição de login recebida para: {request.Email}");

            // Valida o modelo
            if (!ModelState.IsValid)
            {
                Console.WriteLine("[AuthController] Modelo inválido");
                return BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                // Autentica o usuário
                var usuario = await _authService.AuthenticateAsync(request.Email, request.Senha);

                if (usuario == null)
                {
                    Console.WriteLine($"[AuthController] Falha na autenticação para: {request.Email}");
                    return Unauthorized(new { message = "E-mail ou senha inválidos" });
                }

                // Gera o token JWT
                var token = _authService.GenerateJwtToken(usuario);
                var expirationInSeconds = _authService.GetTokenExpirationInSeconds();

                var response = new LoginResponse
                {
                    Token = token,
                    TokenType = "Bearer",
                    ExpiresIn = expirationInSeconds,
                    UsuarioId = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    PerfilId = usuario.Perfil
                };

                Console.WriteLine($"[AuthController] Login bem-sucedido para: {request.Email}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Erro no login: {ex.Message}");
                return StatusCode(500, new { message = "Erro ao processar login", error = ex.Message });
            }
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="request">Dados do novo usuário</param>
        /// <returns>Token JWT e informações do usuário criado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="409">E-mail já cadastrado</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            Console.WriteLine($"[AuthController] Requisição de registro recebida para: {request.Email}");

            // Valida o modelo
            if (!ModelState.IsValid)
            {
                Console.WriteLine("[AuthController] Modelo inválido");
                return BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                // Verifica se o e-mail já existe
                var emailExists = await _authService.EmailExistsAsync(request.Email);
                if (emailExists)
                {
                    Console.WriteLine($"[AuthController] E-mail já cadastrado: {request.Email}");
                    return Conflict(new { message = "E-mail já cadastrado no sistema" });
                }

                // Registra o novo usuário
                var novoUsuario = await _authService.RegisterAsync(
                    request.Nome,
                    request.Email,
                    request.Senha,
                    request.PerfilId
                );

                // Gera o token JWT automaticamente após o registro
                var token = _authService.GenerateJwtToken(novoUsuario);
                var expirationInSeconds = _authService.GetTokenExpirationInSeconds();

                var response = new LoginResponse
                {
                    Token = token,
                    TokenType = "Bearer",
                    ExpiresIn = expirationInSeconds,
                    UsuarioId = novoUsuario.Id,
                    Nome = novoUsuario.Nome,
                    Email = novoUsuario.Email,
                    PerfilId = novoUsuario.Perfil
                };

                Console.WriteLine($"[AuthController] Usuário registrado com sucesso: {request.Email}");
                return CreatedAtAction(nameof(Login), response);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[AuthController] Erro de validação: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[AuthController] Erro de operação: {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Erro no registro: {ex.Message}");
                return StatusCode(500, new { message = "Erro ao processar registro", error = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se um e-mail já está cadastrado no sistema
        /// </summary>
        /// <param name="email">E-mail a ser verificado</param>
        /// <returns>True se o e-mail existe, False caso contrário</returns>
        /// <response code="200">Verificação realizada com sucesso</response>
        [HttpGet("check-email")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "E-mail é obrigatório" });
            }

            var exists = await _authService.EmailExistsAsync(email);
            return Ok(new { email, exists });
        }

        /// <summary>
        /// Endpoint protegido para verificar se o token JWT é válido
        /// </summary>
        /// <returns>Informações do usuário autenticado</returns>
        /// <response code="200">Token válido</response>
        /// <response code="401">Token inválido ou ausente</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            // Obtém as claims do token JWT do usuário autenticado
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var perfilId = User.FindFirst("PerfilId")?.Value;

            Console.WriteLine($"[AuthController] Informações do usuário autenticado: {userEmail}");

            return Ok(new
            {
                id = userId,
                nome = userName,
                email = userEmail,
                perfilId = perfilId,
                message = "Token válido"
            });
        }
    }
}

