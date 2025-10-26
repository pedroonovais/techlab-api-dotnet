using service.ML.Models;

namespace service.ML
{
    /// <summary>
    /// Gerador de dados sintéticos para treinamento do modelo de manutenção
    /// Cria registros realistas baseados em padrões observados em gestão de frotas
    /// </summary>
    public static class DataGenerator
    {
        private static readonly Random _random = new Random(42); // Seed fixo para reprodutibilidade

        /// <summary>
        /// Gera dados sintéticos de manutenção de motos para treinamento do modelo
        /// </summary>
        /// <param name="quantidade">Quantidade de registros a gerar (padrão: 150)</param>
        /// <returns>Lista de dados sintéticos</returns>
        public static List<MotoManutencaoData> GerarDadosSinteticos(int quantidade = 150)
        {
            var dados = new List<MotoManutencaoData>();

            Console.WriteLine($"[DataGenerator] Gerando {quantidade} registros sintéticos...");

            // Distribuição:
            // 30% motos novas (0-12 meses)
            // 40% motos usadas (12-36 meses)
            // 30% motos antigas (36+ meses)

            int motosNovas = (int)(quantidade * 0.30);
            int motosUsadas = (int)(quantidade * 0.40);
            int motosAntigas = quantidade - motosNovas - motosUsadas;

            // Gerar motos novas
            for (int i = 0; i < motosNovas; i++)
            {
                dados.Add(GerarMotoNova());
            }

            // Gerar motos usadas
            for (int i = 0; i < motosUsadas; i++)
            {
                dados.Add(GerarMotoUsada());
            }

            // Gerar motos antigas
            for (int i = 0; i < motosAntigas; i++)
            {
                dados.Add(GerarMotoAntiga());
            }

            // Embaralhar dados para evitar padrões sequenciais
            dados = dados.OrderBy(x => _random.Next()).ToList();

            Console.WriteLine($"[DataGenerator] {quantidade} registros gerados com sucesso!");
            Console.WriteLine($"[DataGenerator] Distribuição: {motosNovas} novas, {motosUsadas} usadas, {motosAntigas} antigas");

            return dados;
        }

        /// <summary>
        /// Gera dados de uma moto nova (0-12 meses)
        /// Baixa probabilidade de manutenção (~15%)
        /// </summary>
        private static MotoManutencaoData GerarMotoNova()
        {
            var idadeMeses = (float)(_random.NextDouble() * 12); // 0-12 meses
            var movimentacoes = (float)(_random.NextDouble() * 50 + idadeMeses * 3); // Poucas movimentações
            var diasUltimaManu = (float)(_random.NextDouble() * 30 + idadeMeses * 2); // Manutenção recente
            var tempoPermanencia = (float)(_random.NextDouble() * 20 + 20); // 20-40h (uso moderado)

            // Lógica de negócio: Motos novas raramente precisam manutenção
            // Apenas se tiverem muitas movimentações OU muito tempo desde última manutenção
            var precisaManu = false;
            var risco = CalcularRisco(idadeMeses, movimentacoes, diasUltimaManu, tempoPermanencia);
            
            // 15% de chance base para motos novas
            if (_random.NextDouble() < 0.15 || risco > 0.7)
            {
                precisaManu = true;
                // Se precisa, ajustar dias para ser mais alto
                diasUltimaManu = (float)(_random.NextDouble() * 40 + 60);
            }

            return new MotoManutencaoData
            {
                IdadeMeses = idadeMeses,
                NumeroMovimentacoes = movimentacoes,
                DiasDesdeUltimaManutencao = diasUltimaManu,
                TempoMedioPermanencia = tempoPermanencia,
                PrecisaManutencao = precisaManu
            };
        }

        /// <summary>
        /// Gera dados de uma moto usada (12-36 meses)
        /// Probabilidade média de manutenção (~35%)
        /// </summary>
        private static MotoManutencaoData GerarMotoUsada()
        {
            var idadeMeses = (float)(_random.NextDouble() * 24 + 12); // 12-36 meses
            var movimentacoes = (float)(_random.NextDouble() * 150 + idadeMeses * 5); // Movimentações moderadas
            var diasUltimaManu = (float)(_random.NextDouble() * 60 + idadeMeses * 1.5); // Variável
            var tempoPermanencia = (float)(_random.NextDouble() * 30 + 15); // 15-45h

            var precisaManu = false;
            var risco = CalcularRisco(idadeMeses, movimentacoes, diasUltimaManu, tempoPermanencia);
            
            // 35% de chance base para motos usadas
            if (_random.NextDouble() < 0.35 || risco > 0.6)
            {
                precisaManu = true;
                diasUltimaManu = (float)(_random.NextDouble() * 60 + 90);
                movimentacoes += (float)(_random.NextDouble() * 100); // Mais movimentações
            }

            return new MotoManutencaoData
            {
                IdadeMeses = idadeMeses,
                NumeroMovimentacoes = movimentacoes,
                DiasDesdeUltimaManutencao = diasUltimaManu,
                TempoMedioPermanencia = tempoPermanencia,
                PrecisaManutencao = precisaManu
            };
        }

        /// <summary>
        /// Gera dados de uma moto antiga (36+ meses)
        /// Alta probabilidade de manutenção (~65%)
        /// </summary>
        private static MotoManutencaoData GerarMotoAntiga()
        {
            var idadeMeses = (float)(_random.NextDouble() * 36 + 36); // 36-72 meses
            var movimentacoes = (float)(_random.NextDouble() * 300 + idadeMeses * 8); // Muitas movimentações
            var diasUltimaManu = (float)(_random.NextDouble() * 120 + idadeMeses * 2); // Tempo maior
            var tempoPermanencia = (float)(_random.NextDouble() * 50 + 10); // 10-60h (variável)

            var precisaManu = false;
            var risco = CalcularRisco(idadeMeses, movimentacoes, diasUltimaManu, tempoPermanencia);
            
            // 65% de chance base para motos antigas
            if (_random.NextDouble() < 0.65 || risco > 0.5)
            {
                precisaManu = true;
                diasUltimaManu = (float)(_random.NextDouble() * 90 + 120);
                movimentacoes += (float)(_random.NextDouble() * 200);
            }

            return new MotoManutencaoData
            {
                IdadeMeses = idadeMeses,
                NumeroMovimentacoes = movimentacoes,
                DiasDesdeUltimaManutencao = diasUltimaManu,
                TempoMedioPermanencia = tempoPermanencia,
                PrecisaManutencao = precisaManu
            };
        }

        /// <summary>
        /// Calcula risco de manutenção baseado nas features
        /// Retorna valor entre 0 (baixo risco) e 1 (alto risco)
        /// </summary>
        private static float CalcularRisco(float idadeMeses, float movimentacoes, float diasUltimaManu, float tempoPermanencia)
        {
            // Normalizar valores para escala 0-1
            var riscoIdade = Math.Min(idadeMeses / 72f, 1f); // Max 72 meses
            var riscoMovimentacoes = Math.Min(movimentacoes / 600f, 1f); // Max ~600 movimentações
            var riscoDiasManu = Math.Min(diasUltimaManu / 180f, 1f); // Max 180 dias
            var riscoPermanencia = Math.Max(0, (60f - tempoPermanencia) / 60f); // Menos tempo = mais risco

            // Peso: idade (30%), movimentações (25%), dias manutenção (35%), permanência (10%)
            return (riscoIdade * 0.3f) + 
                   (riscoMovimentacoes * 0.25f) + 
                   (riscoDiasManu * 0.35f) + 
                   (riscoPermanencia * 0.1f);
        }
    }
}
