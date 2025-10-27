using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;
using service.Service;
using FluentAssertions;
using Xunit;

namespace TechLab.UnitTests.Services
{
    /// <summary>
    /// Testes unitários para o serviço de Moto
    /// Utiliza EF Core InMemory para garantir isolamento e determinismo
    /// </summary>
    public class MotoServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MotoService _service;

        public MotoServiceTests()
        {
            // Configuração: Criar contexto InMemory para cada teste
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new MotoService(_context);
        }

        /// <summary>
        /// Regra de negócio: Ao criar uma moto, DtCadastro deve ser preenchido automaticamente
        /// </summary>
        [Fact]
        public void Create_DevePreencherDtCadastroAutomaticamente()
        {
            // Arrange - Preparar dados de teste
            var moto = new Moto
            {
                Marca = "Honda",
                Modelo = "CG 160",
                Placa = "ABC-1234",
                Chassi = "9BWZZZ377VT004251",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            var dataAntesCriacao = DateTime.UtcNow;

            // Act - Executar ação
            var resultado = _service.Create(moto);

            // Assert - Verificar resultado
            resultado.Should().NotBeNull("a moto deve ter sido criada");
            resultado.DtCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5),
                "DtCadastro deve ser preenchido com a data/hora atual");
            resultado.DtCadastro.Should().BeOnOrAfter(dataAntesCriacao,
                "DtCadastro não pode ser anterior ao momento da criação");
        }

        /// <summary>
        /// Regra de negócio: Ao criar uma moto, DtAtualizacao deve ser preenchido automaticamente
        /// </summary>
        [Fact]
        public void Create_DevePreencherDtAtualizacaoAutomaticamente()
        {
            // Arrange
            var moto = new Moto
            {
                Marca = "Yamaha",
                Modelo = "Factor 150",
                Placa = "DEF-5678",
                Chassi = "9BWZZZ377VT004252",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            // Act
            var resultado = _service.Create(moto);

            // Assert
            resultado.DtAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5),
                "DtAtualizacao deve ser preenchido na criação");
        }

        /// <summary>
        /// Regra de negócio: Não é possível criar moto sem marca
        /// </summary>
        [Fact]
        public void Create_DeveLancarExcecao_QuandoMarcaVazia()
        {
            // Arrange
            var moto = new Moto
            {
                Marca = "",
                Modelo = "CG 160",
                Placa = "ABC-1234",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            // Act & Assert
            var acao = () => _service.Create(moto);
            acao.Should().Throw<ArgumentException>()
                .WithMessage("*marca*", "deve validar que marca é obrigatória");
        }

        /// <summary>
        /// Regra de negócio: Não é possível criar moto sem modelo
        /// </summary>
        [Fact]
        public void Create_DeveLancarExcecao_QuandoModeloVazio()
        {
            // Arrange
            var moto = new Moto
            {
                Marca = "Honda",
                Modelo = "",
                Placa = "ABC-1234",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            // Act & Assert
            var acao = () => _service.Create(moto);
            acao.Should().Throw<ArgumentException>()
                .WithMessage("*modelo*", "deve validar que modelo é obrigatório");
        }

        /// <summary>
        /// Regra de negócio: Atualização deve retornar false quando moto não existe
        /// </summary>
        [Fact]
        public void Update_DeveRetornarFalse_QuandoMotoNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();
            var motoAtualizada = new Moto
            {
                Id = idInexistente,
                Marca = "Suzuki",
                Modelo = "Burgman",
                Placa = "XYZ-9999",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            // Act
            var resultado = _service.Update(idInexistente, motoAtualizada);

            // Assert
            resultado.Should().BeFalse("deve retornar false quando tenta atualizar moto inexistente");
        }

        /// <summary>
        /// Regra de negócio: Ao atualizar uma moto, DtAtualizacao deve ser atualizado automaticamente
        /// </summary>
        [Fact]
        public void Update_DeveAtualizarDtAtualizacao_QuandoMotoExiste()
        {
            // Arrange - Criar moto primeiro
            var moto = new Moto
            {
                Marca = "Kawasaki",
                Modelo = "Ninja 400",
                Placa = "KAW-1000",
                Chassi = "9BWZZZ377VT004253",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            var motoCriada = _service.Create(moto);
            var dtAtualizacaoOriginal = motoCriada.DtAtualizacao;

            // Aguardar um pouco para garantir diferença no timestamp
            Thread.Sleep(100);

            // Preparar atualização
            motoCriada.Modelo = "Ninja 650";

            // Act
            var resultado = _service.Update(motoCriada.Id, motoCriada);

            // Assert
            resultado.Should().BeTrue("deve retornar true quando atualização é bem-sucedida");
            
            // Buscar moto atualizada do banco
            var motoAtualizada = _service.GetById(motoCriada.Id);
            motoAtualizada.Should().NotBeNull();
            motoAtualizada!.DtAtualizacao.Should().BeAfter(dtAtualizacaoOriginal,
                "DtAtualizacao deve ser atualizado para data/hora mais recente");
        }

        /// <summary>
        /// Regra de negócio: Não é possível atualizar moto com marca vazia
        /// </summary>
        [Fact]
        public void Update_DeveLancarExcecao_QuandoMarcaVazia()
        {
            // Arrange - Criar moto primeiro
            var moto = new Moto
            {
                Marca = "BMW",
                Modelo = "G 310 R",
                Placa = "BMW-5000",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            var motoCriada = _service.Create(moto);
            
            // Tentar atualizar com marca vazia
            motoCriada.Marca = "";

            // Act & Assert
            var acao = () => _service.Update(motoCriada.Id, motoCriada);
            acao.Should().Throw<ArgumentException>()
                .WithMessage("*marca*", "deve validar marca mesmo na atualização");
        }

        /// <summary>
        /// Regra de negócio: Delete deve retornar false quando moto não existe
        /// </summary>
        [Fact]
        public void Delete_DeveRetornarFalse_QuandoMotoNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();

            // Act
            var resultado = _service.Delete(idInexistente);

            // Assert
            resultado.Should().BeFalse("deve retornar false ao tentar deletar moto inexistente");
        }

        /// <summary>
        /// Regra de negócio: Delete deve remover moto quando existe
        /// </summary>
        [Fact]
        public void Delete_DeveRemoverMoto_QuandoExiste()
        {
            // Arrange - Criar moto primeiro
            var moto = new Moto
            {
                Marca = "Honda",
                Modelo = "PCX",
                Placa = "PCX-1234",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            var motoCriada = _service.Create(moto);

            // Act
            var resultado = _service.Delete(motoCriada.Id);

            // Assert
            resultado.Should().BeTrue("deve retornar true quando deleta moto existente");
            
            // Verificar que moto foi realmente removida
            var motoDeletada = _service.GetById(motoCriada.Id);
            motoDeletada.Should().BeNull("moto não deve mais existir após deleção");
        }

        /// <summary>
        /// Regra de negócio: GetById deve retornar null quando ID não existe
        /// </summary>
        [Fact]
        public void GetById_DeveRetornarNull_QuandoIdNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();

            // Act
            var resultado = _service.GetById(idInexistente);

            // Assert
            resultado.Should().BeNull("deve retornar null para ID inexistente");
        }

        /// <summary>
        /// Regra de negócio: GetById deve retornar moto quando ID existe
        /// </summary>
        [Fact]
        public void GetById_DeveRetornarMoto_QuandoIdExiste()
        {
            // Arrange - Criar moto primeiro
            var moto = new Moto
            {
                Marca = "Yamaha",
                Modelo = "MT-03",
                Placa = "MT-9999",
                IdStatusOperacional = Guid.NewGuid(),
                IdRastreador = Guid.NewGuid(),
                Ativo = true
            };

            var motoCriada = _service.Create(moto);

            // Act
            var resultado = _service.GetById(motoCriada.Id);

            // Assert
            resultado.Should().NotBeNull("deve encontrar moto criada");
            resultado!.Id.Should().Be(motoCriada.Id, "deve retornar moto com ID correto");
            resultado.Marca.Should().Be("Yamaha", "marca deve ser preservada");
            resultado.Modelo.Should().Be("MT-03", "modelo deve ser preservado");
        }

        /// <summary>
        /// Limpeza: Liberar recursos após cada teste
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

