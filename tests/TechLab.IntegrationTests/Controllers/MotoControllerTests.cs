using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TechLab.IntegrationTests.Helpers;
using library.Model;
using System.Text.Json;
using Xunit;

namespace TechLab.IntegrationTests.Controllers
{
    /// <summary>
    /// Testes de integração para o MotoController
    /// Valida endpoints HTTP em ambiente de teste com autenticação falsa
    /// </summary>
    public class MotoControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public MotoControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        /// <summary>
        /// Teste de integração: GET /api/v1/Moto deve retornar 200 OK quando autenticado
        /// Valida que o endpoint está acessível e retorna estrutura de paginação correta
        /// </summary>
        [Fact]
        public async Task Get_DeveRetornar200OK_QuandoAutenticado()
        {
            // Arrange
            var endpoint = "/api/v1/Moto";
            Console.WriteLine($"[Teste] Executando GET para: {endpoint}");

            // Act - Executar requisição HTTP
            var response = await _client.GetAsync(endpoint);
            Console.WriteLine($"[Teste] Status Code recebido: {response.StatusCode}");

            // Assert - Validar resposta HTTP
            response.StatusCode.Should().Be(HttpStatusCode.OK,
                "endpoint deve retornar 200 OK quando usuário está autenticado");

            // Validar que conteúdo é JSON
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json",
                "resposta deve ser em formato JSON");
        }

        /// <summary>
        /// Teste de integração: GET /api/v1/Moto deve retornar estrutura de paginação válida
        /// Valida que a resposta contém todos os campos esperados do PagedResource
        /// </summary>
        [Fact]
        public async Task Get_DeveRetornarEstruturaPaginacaoValida()
        {
            // Arrange
            var endpoint = "/api/v1/Moto";

            // Act
            var response = await _client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Teste] Resposta recebida: {content}");

            // Deserializar como JsonDocument para validar estrutura
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            // Assert - Validar estrutura do PagedResource
            root.TryGetProperty("items", out var items).Should().BeTrue(
                "resposta deve conter propriedade 'items'");
            root.TryGetProperty("pageNumber", out _).Should().BeTrue(
                "resposta deve conter propriedade 'pageNumber'");
            root.TryGetProperty("pageSize", out _).Should().BeTrue(
                "resposta deve conter propriedade 'pageSize'");
            root.TryGetProperty("totalItems", out _).Should().BeTrue(
                "resposta deve conter propriedade 'totalItems'");
            root.TryGetProperty("totalPages", out _).Should().BeTrue(
                "resposta deve conter propriedade 'totalPages'");
            root.TryGetProperty("links", out _).Should().BeTrue(
                "resposta deve conter propriedade 'links' para HATEOAS");

            // Validar que items é um array
            items.ValueKind.Should().Be(JsonValueKind.Array,
                "items deve ser um array de recursos");
        }

        /// <summary>
        /// Teste de integração: GET /api/v1/Moto com parâmetros de paginação
        /// Valida que o endpoint respeita os parâmetros pageNumber e pageSize
        /// </summary>
        [Fact]
        public async Task Get_DeveRespeitarParametrosDePaginacao()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 5;
            var endpoint = $"/api/v1/Moto?pageNumber={pageNumber}&pageSize={pageSize}";
            Console.WriteLine($"[Teste] Executando GET com paginação: {endpoint}");

            // Act
            var response = await _client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            // Validar parâmetros de paginação retornados
            root.GetProperty("pageNumber").GetInt32().Should().Be(pageNumber,
                "pageNumber retornado deve corresponder ao solicitado");
            root.GetProperty("pageSize").GetInt32().Should().Be(pageSize,
                "pageSize retornado deve corresponder ao solicitado");

            Console.WriteLine($"[Teste] Paginação validada: Page {pageNumber}, Size {pageSize}");
        }

        /// <summary>
        /// Teste de integração: POST /api/v1/Moto deve criar nova moto
        /// Valida que é possível criar recurso via API
        /// </summary>
        [Fact]
        public async Task Post_DeveCriarNovaMoto_QuandoDadosValidos()
        {
            // Arrange - Preparar dados da nova moto
            var novaMoto = new Moto
            {
                Marca = "Honda",
                Modelo = "CG 160 Titan",
                Placa = "TEST-1234",
                Chassi = "9BWZZZ377VT999999",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            var endpoint = "/api/v1/Moto";
            Console.WriteLine($"[Teste] Criando moto: {novaMoto.Marca} {novaMoto.Modelo}");

            // Act - Enviar POST request
            var response = await _client.PostAsJsonAsync(endpoint, novaMoto);
            Console.WriteLine($"[Teste] Status Code recebido: {response.StatusCode}");

            // Assert - Validar resposta
            response.StatusCode.Should().Be(HttpStatusCode.Created,
                "deve retornar 201 Created ao criar moto com sucesso");

            // Validar que o header Location está presente
            response.Headers.Location.Should().NotBeNull(
                "header Location deve estar presente com URL do recurso criado");

            // Validar conteúdo da resposta
            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            root.TryGetProperty("data", out var data).Should().BeTrue(
                "resposta deve conter propriedade 'data' com o recurso criado");

            var motoData = data.GetProperty("marca").GetString();
            motoData.Should().Be("Honda", "marca da moto criada deve corresponder ao enviado");

            Console.WriteLine($"[Teste] Moto criada com sucesso: ID presente na resposta");
        }

        /// <summary>
        /// Teste de integração: POST /api/v1/Moto deve retornar BadRequest com dados inválidos
        /// Valida que a API rejeita requisições com dados incompletos ou inválidos
        /// </summary>
        [Fact]
        public async Task Post_DeveRetornarBadRequest_QuandoDadosInvalidos()
        {
            // Arrange - Moto sem marca (campo obrigatório)
            var motoInvalida = new
            {
                Marca = "",  // Campo vazio (inválido)
                Modelo = "Test Model",
                Placa = "INVALID",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            var endpoint = "/api/v1/Moto";
            Console.WriteLine("[Teste] Tentando criar moto com dados inválidos");

            // Act
            var response = await _client.PostAsJsonAsync(endpoint, motoInvalida);
            Console.WriteLine($"[Teste] Status Code recebido: {response.StatusCode}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest,
                "deve retornar 400 BadRequest quando dados são inválidos");

            Console.WriteLine("[Teste] Validação correta: BadRequest retornado para dados inválidos");
        }

        /// <summary>
        /// Teste de integração: GET /api/v1/Moto/{id} inexistente deve retornar NotFound
        /// Valida tratamento de recursos não encontrados
        /// </summary>
        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoIdNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();
            var endpoint = $"/api/v1/Moto/{idInexistente}";
            Console.WriteLine($"[Teste] Buscando moto inexistente: {idInexistente}");

            // Act
            var response = await _client.GetAsync(endpoint);
            Console.WriteLine($"[Teste] Status Code recebido: {response.StatusCode}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound,
                "deve retornar 404 NotFound quando moto não existe");

            Console.WriteLine("[Teste] NotFound retornado corretamente para ID inexistente");
        }

        /// <summary>
        /// Teste de integração: Validar links HATEOAS na resposta
        /// Garante que a API segue princípios REST com hypermedia
        /// </summary>
        [Fact]
        public async Task Get_DeveIncluirLinksHATEOAS_NaResposta()
        {
            // Arrange
            var endpoint = "/api/v1/Moto";

            // Act
            var response = await _client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            // Validar que links HATEOAS estão presentes
            root.TryGetProperty("links", out var links).Should().BeTrue(
                "resposta deve incluir links HATEOAS");

            // Validar que pelo menos o link 'self' existe
            links.TryGetProperty("self", out var selfLink).Should().BeTrue(
                "links devem incluir pelo menos o link 'self'");

            Console.WriteLine("[Teste] Links HATEOAS validados com sucesso");
        }
    }
}

