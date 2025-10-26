using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace data.Seed
{
    /// <summary>
    /// Classe responsável por popular o banco de dados com dados iniciais
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Popula o banco de dados com dados iniciais se estiver vazio
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public static async Task SeedAsync(AppDbContext context)
        {
            Console.WriteLine("[DataSeeder] Verificando se existem dados no banco...");

            // Verifica se já existem dados nas tabelas principais
            var hasData = await context.Perfil.AnyAsync() ||
                         await context.StatusOperacional.AnyAsync() ||
                         await context.Rastreador.AnyAsync() ||
                         await context.Patio.AnyAsync() ||
                         await context.Usuario.AnyAsync() ||
                         await context.Moto.AnyAsync();

            if (hasData)
            {
                Console.WriteLine("[DataSeeder] Banco já contém dados. Seed não será executado.");
                return;
            }

            Console.WriteLine("[DataSeeder] Banco vazio detectado. Iniciando seed de dados...");

            // Início de transação para garantir consistência
            using var transaction = await context.Database.BeginTransactionAsync();
            
            try
            {
                // 1. Criar Perfis
                var perfis = await SeedPerfisAsync(context);
                Console.WriteLine($"[DataSeeder] {perfis.Count} perfis criados");

                // 2. Criar Status Operacionais
                var statusOperacionais = await SeedStatusOperacionaisAsync(context);
                Console.WriteLine($"[DataSeeder] {statusOperacionais.Count} status operacionais criados");

                // 3. Criar Rastreadores
                var rastreadores = await SeedRastreadoresAsync(context);
                Console.WriteLine($"[DataSeeder] {rastreadores.Count} rastreadores criados");

                // 4. Criar Pátios
                var patios = await SeedPatiosAsync(context);
                Console.WriteLine($"[DataSeeder] {patios.Count} pátios criados");

                // 5. Criar Usuários
                var usuarios = await SeedUsuariosAsync(context, perfis);
                Console.WriteLine($"[DataSeeder] {usuarios.Count} usuários criados");

                // 6. Criar Motos
                var motos = await SeedMotosAsync(context, statusOperacionais, rastreadores);
                Console.WriteLine($"[DataSeeder] {motos.Count} motos criadas");

                // Commit da transação
                await transaction.CommitAsync();
                
                Console.WriteLine("[DataSeeder] ✅ Seed concluído com sucesso!");
                Console.WriteLine("[DataSeeder] Credenciais padrão:");
                Console.WriteLine("[DataSeeder]   Admin: admin@techlab.com / Admin@123");
                Console.WriteLine("[DataSeeder]   Gerente: gerente@techlab.com / Gerente@123");
                Console.WriteLine("[DataSeeder]   Usuário: usuario@techlab.com / Usuario@123");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[DataSeeder] ❌ Erro ao executar seed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cria perfis de acesso padrão
        /// </summary>
        private static async Task<List<Perfil>> SeedPerfisAsync(AppDbContext context)
        {
            var perfis = new List<Perfil>
            {
                new Perfil
                {
                    Id = Guid.NewGuid(),
                    Nome = "Administrador",
                    NivelAcesso = 3,
                    DtCadastro = DateTime.UtcNow,
                    DtAtualizacao = DateTime.UtcNow,
                    Ativo = true
                },
                new Perfil
                {
                    Id = Guid.NewGuid(),
                    Nome = "Gerente",
                    NivelAcesso = 2,
                    DtCadastro = DateTime.UtcNow,
                    DtAtualizacao = DateTime.UtcNow,
                    Ativo = true
                },
                new Perfil
                {
                    Id = Guid.NewGuid(),
                    Nome = "Usuário Padrão",
                    NivelAcesso = 1,
                    DtCadastro = DateTime.UtcNow,
                    DtAtualizacao = DateTime.UtcNow,
                    Ativo = true
                }
            };

            await context.Perfil.AddRangeAsync(perfis);
            await context.SaveChangesAsync();
            
            return perfis;
        }

        /// <summary>
        /// Cria status operacionais padrão para as motos
        /// </summary>
        private static async Task<List<StatusOperacional>> SeedStatusOperacionaisAsync(AppDbContext context)
        {
            var statusOperacionais = new List<StatusOperacional>
            {
                new StatusOperacional
                {
                    Id = Guid.NewGuid(),
                    Descricao = "Disponível"
                },
                new StatusOperacional
                {
                    Id = Guid.NewGuid(),
                    Descricao = "Em Uso"
                },
                new StatusOperacional
                {
                    Id = Guid.NewGuid(),
                    Descricao = "Manutenção"
                },
                new StatusOperacional
                {
                    Id = Guid.NewGuid(),
                    Descricao = "Indisponível"
                },
                new StatusOperacional
                {
                    Id = Guid.NewGuid(),
                    Descricao = "Reservada"
                }
            };

            await context.StatusOperacional.AddRangeAsync(statusOperacionais);
            await context.SaveChangesAsync();
            
            return statusOperacionais;
        }

        /// <summary>
        /// Cria rastreadores de exemplo
        /// </summary>
        private static async Task<List<Rastreador>> SeedRastreadoresAsync(AppDbContext context)
        {
            var rastreadores = new List<Rastreador>();
            var modelos = new[] { "GPS-2000", "GPS-3000", "GPS-4000", "GPS-5000" };

            for (int i = 1; i <= 20; i++)
            {
                rastreadores.Add(new Rastreador
                {
                    Id = Guid.NewGuid(),
                    NumeroSerie = $"TRACK{i:D6}",
                    Modelo = modelos[(i - 1) % modelos.Length],
                    DtCadastro = DateTime.UtcNow.AddDays(-30),
                    DtAtualizacao = DateTime.UtcNow.AddDays(-30),
                    Ativo = true
                });
            }

            await context.Rastreador.AddRangeAsync(rastreadores);
            await context.SaveChangesAsync();
            
            return rastreadores;
        }

        /// <summary>
        /// Cria pátios de exemplo
        /// </summary>
        private static async Task<List<Patio>> SeedPatiosAsync(AppDbContext context)
        {
            var patios = new List<Patio>
            {
                new Patio
                {
                    Id = Guid.NewGuid(),
                    Nome = "Pátio Centro",
                    Localizacao = "Av. Paulista, 1000 - São Paulo/SP",
                    DtCadastro = DateTime.UtcNow.AddMonths(-6),
                    DtAtualizacao = DateTime.UtcNow.AddMonths(-6)
                },
                new Patio
                {
                    Id = Guid.NewGuid(),
                    Nome = "Pátio Norte",
                    Localizacao = "Rua das Flores, 500 - São Paulo/SP",
                    DtCadastro = DateTime.UtcNow.AddMonths(-5),
                    DtAtualizacao = DateTime.UtcNow.AddMonths(-5)
                },
                new Patio
                {
                    Id = Guid.NewGuid(),
                    Nome = "Pátio Sul",
                    Localizacao = "Av. dos Estados, 2000 - São Paulo/SP",
                    DtCadastro = DateTime.UtcNow.AddMonths(-4),
                    DtAtualizacao = DateTime.UtcNow.AddMonths(-4)
                },
                new Patio
                {
                    Id = Guid.NewGuid(),
                    Nome = "Pátio Leste",
                    Localizacao = "Rua do Comércio, 750 - São Paulo/SP",
                    DtCadastro = DateTime.UtcNow.AddMonths(-3),
                    DtAtualizacao = DateTime.UtcNow.AddMonths(-3)
                },
                new Patio
                {
                    Id = Guid.NewGuid(),
                    Nome = "Pátio Oeste",
                    Localizacao = "Av. Industrial, 1500 - São Paulo/SP",
                    DtCadastro = DateTime.UtcNow.AddMonths(-2),
                    DtAtualizacao = DateTime.UtcNow.AddMonths(-2)
                }
            };

            await context.Patio.AddRangeAsync(patios);
            await context.SaveChangesAsync();
            
            return patios;
        }

        /// <summary>
        /// Cria usuários de exemplo com senhas criptografadas
        /// </summary>
        private static async Task<List<Usuario>> SeedUsuariosAsync(AppDbContext context, List<Perfil> perfis)
        {
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = "Administrador do Sistema",
                    Email = "admin@techlab.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Perfil = perfis.First(p => p.Nome == "Administrador").Id,
                    DtCriacao = DateTime.UtcNow.AddMonths(-6),
                    DtAlteracao = DateTime.UtcNow.AddMonths(-6),
                    Ativo = true
                },
                new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = "Gerente de Operações",
                    Email = "gerente@techlab.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("Gerente@123"),
                    Perfil = perfis.First(p => p.Nome == "Gerente").Id,
                    DtCriacao = DateTime.UtcNow.AddMonths(-5),
                    DtAlteracao = DateTime.UtcNow.AddMonths(-5),
                    Ativo = true
                },
                new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = "Usuário Teste",
                    Email = "usuario@techlab.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("Usuario@123"),
                    Perfil = perfis.First(p => p.Nome == "Usuário Padrão").Id,
                    DtCriacao = DateTime.UtcNow.AddMonths(-4),
                    DtAlteracao = DateTime.UtcNow.AddMonths(-4),
                    Ativo = true
                },
                new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = "Pedro Novais",
                    Email = "pedro.novais@techlab.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("Pedro@123"),
                    Perfil = perfis.First(p => p.Nome == "Administrador").Id,
                    DtCriacao = DateTime.UtcNow.AddMonths(-6),
                    DtAlteracao = DateTime.UtcNow.AddMonths(-6),
                    Ativo = true
                },
                new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = "Maria Silva",
                    Email = "maria.silva@techlab.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("Maria@123"),
                    Perfil = perfis.First(p => p.Nome == "Gerente").Id,
                    DtCriacao = DateTime.UtcNow.AddMonths(-3),
                    DtAlteracao = DateTime.UtcNow.AddMonths(-3),
                    Ativo = true
                }
            };

            await context.Usuario.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();
            
            return usuarios;
        }

        /// <summary>
        /// Cria motos de exemplo
        /// </summary>
        private static async Task<List<Moto>> SeedMotosAsync(
            AppDbContext context, 
            List<StatusOperacional> statusOperacionais, 
            List<Rastreador> rastreadores)
        {
            var motos = new List<Moto>();
            var marcas = new[] { "Honda", "Yamaha", "Suzuki", "Kawasaki", "BMW" };
            var modelos = new Dictionary<string, string[]>
            {
                { "Honda", new[] { "CG 160", "CB 500", "CB 650", "PCX 150", "ADV 150" } },
                { "Yamaha", new[] { "Factor 150", "MT-03", "MT-07", "NMAX", "XTZ 150" } },
                { "Suzuki", new[] { "GSX-S 750", "V-Strom 650", "Burgman 125", "Hayabusa", "Boulevard" } },
                { "Kawasaki", new[] { "Ninja 400", "Z900", "Versys 650", "Z400", "Ninja ZX-10R" } },
                { "BMW", new[] { "G 310 R", "F 850 GS", "R 1250 GS", "S 1000 RR", "F 900 R" } }
            };

            var random = new Random(42); // Seed fixo para resultados consistentes
            
            for (int i = 0; i < rastreadores.Count; i++)
            {
                var marca = marcas[i % marcas.Length];
                var modelosArray = modelos[marca];
                var modelo = modelosArray[random.Next(modelosArray.Length)];
                
                // Gera placas no formato brasileiro (ABC1D23)
                var placa = $"{(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}{random.Next(10)}{(char)('A' + random.Next(26))}{random.Next(10)}{random.Next(10)}";
                
                // Gera chassi (17 caracteres alfanuméricos)
                var chassi = $"9BW{marca.Substring(0, 2).ToUpper()}{random.Next(10)}{random.Next(10)}{random.Next(10)}{random.Next(10)}{random.Next(10)}{random.Next(10)}{random.Next(10)}{random.Next(10)}{random.Next(10)}{random.Next(10)}{random.Next(10)}";
                
                // Distribui os status de forma variada
                var statusIndex = i % statusOperacionais.Count;
                
                motos.Add(new Moto
                {
                    Id = Guid.NewGuid(),
                    Marca = marca,
                    Modelo = modelo,
                    Placa = placa,
                    Chassi = chassi,
                    IdStatusOperacional = statusOperacionais[statusIndex].Id,
                    IdRastreador = rastreadores[i].Id,
                    DtCadastro = DateTime.UtcNow.AddDays(-random.Next(1, 180)),
                    DtAtualizacao = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    Ativo = true
                });
            }

            await context.Moto.AddRangeAsync(motos);
            await context.SaveChangesAsync();
            
            return motos;
        }
    }
}

