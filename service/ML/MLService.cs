using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using service.ML.Models;
using data.Context;
using Microsoft.EntityFrameworkCore;

namespace service.ML
{
    /// <summary>
    /// Serviço de Machine Learning para previsão de manutenção de motos
    /// Utiliza ML.NET para treinar e executar modelo de classificação binária
    /// </summary>
    public class MLService
    {
        private readonly MLContext _mlContext;
        private readonly IMemoryCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;
        private const string MODEL_CACHE_KEY = "MaintenanceModel";
        private const string METRICS_CACHE_KEY = "ModelMetrics";

        public MLService(IMemoryCache cache, IServiceScopeFactory scopeFactory)
        {
            _mlContext = new MLContext(seed: 42); // Seed para reprodutibilidade
            _cache = cache;
            _scopeFactory = scopeFactory;
            Console.WriteLine("[MLService] Serviço de ML inicializado");
        }

        /// <summary>
        /// Treina o modelo de previsão de manutenção usando dados sintéticos
        /// O modelo treinado é armazenado em cache para uso posterior
        /// </summary>
        public async Task TrainModelAsync()
        {
            Console.WriteLine("[MLService] Iniciando treinamento do modelo...");
            var inicio = DateTime.Now;

            try
            {
                // Gerar dados sintéticos para treinamento
                var dadosTreinamento = DataGenerator.GerarDadosSinteticos(150);
                
                // Converter para IDataView (formato do ML.NET)
                var dataView = _mlContext.Data.LoadFromEnumerable(dadosTreinamento);

                // Dividir em treino (80%) e teste (20%)
                var divisao = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

                // Definir pipeline de treinamento
                var pipeline = _mlContext.Transforms.Concatenate("Features",
                        nameof(MotoManutencaoData.IdadeMeses),
                        nameof(MotoManutencaoData.NumeroMovimentacoes),
                        nameof(MotoManutencaoData.DiasDesdeUltimaManutencao),
                        nameof(MotoManutencaoData.TempoMedioPermanencia))
                    .Append(_mlContext.BinaryClassification.Trainers.FastTree(
                        numberOfLeaves: 20,
                        numberOfTrees: 100,
                        minimumExampleCountPerLeaf: 10,
                        learningRate: 0.2));

                Console.WriteLine("[MLService] Treinando modelo FastTree...");
                
                // Treinar o modelo
                var modelo = pipeline.Fit(divisao.TrainSet);

                // Avaliar o modelo com dados de teste
                Console.WriteLine("[MLService] Avaliando modelo com dados de teste...");
                var predicoes = modelo.Transform(divisao.TestSet);
                var metricas = _mlContext.BinaryClassification.Evaluate(predicoes);

                // Armazenar modelo e métricas em cache (não expira)
                _cache.Set(MODEL_CACHE_KEY, modelo);
                _cache.Set(METRICS_CACHE_KEY, new ModelMetrics
                {
                    Acuracia = metricas.Accuracy,
                    PrecisaoPositiva = metricas.PositivePrecision,
                    RecallPositivo = metricas.PositiveRecall,
                    F1Score = metricas.F1Score,
                    AUC = metricas.AreaUnderRocCurve,
                    QuantidadeDadosTreinamento = dadosTreinamento.Count,
                    DataTreinamento = DateTime.Now
                });

                var duracao = (DateTime.Now - inicio).TotalSeconds;

                Console.WriteLine($"[MLService] Modelo treinado com sucesso em {duracao:F2}s");
                Console.WriteLine($"[MLService] Métricas do modelo:");
                Console.WriteLine($"  - Acurácia: {metricas.Accuracy:P2}");
                Console.WriteLine($"  - Precisão Positiva: {metricas.PositivePrecision:P2}");
                Console.WriteLine($"  - Recall Positivo: {metricas.PositiveRecall:P2}");
                Console.WriteLine($"  - F1 Score: {metricas.F1Score:P2}");
                Console.WriteLine($"  - AUC: {metricas.AreaUnderRocCurve:P2}");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MLService] ERRO no treinamento: {ex.Message}");
                Console.WriteLine($"[MLService] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Faz previsão de manutenção para uma moto específica
        /// </summary>
        /// <param name="motoId">ID da moto para análise</param>
        /// <returns>Resultado da previsão ou null se moto não encontrada</returns>
        public async Task<(ManutencaoPrediction? Prediction, MotoManutencaoData? InputData)?> PredictMaintenanceAsync(Guid motoId)
        {
            Console.WriteLine($"[MLService] Fazendo previsão para moto {motoId}");

            // Verificar se modelo está treinado
            if (!_cache.TryGetValue<ITransformer>(MODEL_CACHE_KEY, out var modelo))
            {
                Console.WriteLine("[MLService] ERRO: Modelo não está treinado");
                throw new InvalidOperationException("O modelo de ML ainda não foi treinado. Aguarde a inicialização da API.");
            }

            // Criar scope para acessar o banco de dados
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Buscar dados da moto no banco
            var moto = await context.Moto
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == motoId);

            if (moto == null)
            {
                Console.WriteLine($"[MLService] Moto {motoId} não encontrada");
                return null;
            }

            // Calcular features baseadas nos dados da moto
            var inputData = CalcularFeatures(moto);

            // Criar prediction engine
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MotoManutencaoData, ManutencaoPrediction>(modelo!);

            // Fazer previsão
            var prediction = predictionEngine.Predict(inputData);

            Console.WriteLine($"[MLService] Previsão: PrecisaManutencao={prediction.PrecisaManutencao}, Probabilidade={prediction.Probabilidade:P2}");

            return (prediction, inputData);
        }

        /// <summary>
        /// Calcula as features de ML baseadas nos dados da moto
        /// Como não temos histórico real, simulamos valores baseados na idade
        /// </summary>
        private MotoManutencaoData CalcularFeatures(library.Model.Moto moto)
        {
            // Calcular idade em meses
            var idadeMeses = (float)(DateTime.Now - moto.DtCadastro).TotalDays / 30;

            // Simular movimentações baseadas na idade (em produção, viria do histórico real)
            // Motos mais antigas tendem a ter mais movimentações
            var movimentacoes = idadeMeses * 8 + (float)(new Random().NextDouble() * 20);

            // Simular dias desde última manutenção
            // Usamos uma estimativa baseada na idade
            var diasUltimaManu = idadeMeses * 4 + (float)(new Random().NextDouble() * 30);

            // Simular tempo médio de permanência
            // Motos ativas (Ativo=true) tendem a ter menor tempo de permanência
            var tempoPermanencia = moto.Ativo ? 
                (float)(15 + new Random().NextDouble() * 30) : // Motos ativas: 15-45h
                (float)(40 + new Random().NextDouble() * 40); // Motos inativas: 40-80h

            Console.WriteLine($"[MLService] Features calculadas: IdadeMeses={idadeMeses:F1}, Movimentacoes={movimentacoes:F0}, DiasUltimaManu={diasUltimaManu:F0}, TempoPermanencia={tempoPermanencia:F1}");

            return new MotoManutencaoData
            {
                IdadeMeses = idadeMeses,
                NumeroMovimentacoes = movimentacoes,
                DiasDesdeUltimaManutencao = diasUltimaManu,
                TempoMedioPermanencia = tempoPermanencia,
                PrecisaManutencao = false // Não usado em previsão
            };
        }

        /// <summary>
        /// Obtém as métricas do modelo treinado
        /// </summary>
        /// <returns>Métricas do modelo ou null se não treinado</returns>
        public ModelMetrics? GetModelMetrics()
        {
            if (_cache.TryGetValue<ModelMetrics>(METRICS_CACHE_KEY, out var metricas))
            {
                Console.WriteLine("[MLService] Métricas recuperadas do cache");
                return metricas;
            }

            Console.WriteLine("[MLService] Métricas não disponíveis - modelo não treinado");
            return null;
        }

        /// <summary>
        /// Verifica se o modelo foi treinado e está disponível
        /// </summary>
        public bool IsModelTrained()
        {
            return _cache.TryGetValue<ITransformer>(MODEL_CACHE_KEY, out _);
        }
    }
}

