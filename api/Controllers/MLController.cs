using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.ML;
using api.DTOs.ML;
using Asp.Versioning;

namespace api.Controllers
{
    /// <summary>
    /// Controller de Machine Learning para previsão de manutenção de motos
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class MLController : ControllerBase
    {
        private readonly MLService _mlService;

        /// <summary>
        /// Construtor do MLController
        /// </summary>
        /// <param name="mlService">Serviço de Machine Learning</param>
        public MLController(MLService mlService)
        {
            _mlService = mlService;
        }

        /// <summary>
        /// Prevê se uma moto precisa de manutenção usando Machine Learning
        /// </summary>
        /// <param name="request">Dados da requisição com ID da moto</param>
        /// <returns>Previsão de manutenção com probabilidade e recomendações</returns>
        /// <response code="200">Previsão realizada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="404">Moto não encontrada</response>
        /// <response code="503">Modelo não treinado ainda</response>
        [HttpPost("prever-manutencao")]
        [ProducesResponseType(typeof(PreverManutencaoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> PreverManutencao([FromBody] PreverManutencaoRequest request)
        {
            Console.WriteLine($"[MLController] Recebida requisição de previsão para moto {request.MotoId}");

            // Validar modelo
            if (!ModelState.IsValid)
            {
                Console.WriteLine("[MLController] Modelo inválido");
                return BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            // Verificar se o modelo está treinado
            if (!_mlService.IsModelTrained())
            {
                Console.WriteLine("[MLController] Modelo não está treinado");
                return StatusCode(503, new
                {
                    message = "O modelo de ML ainda está sendo treinado. Por favor, tente novamente em alguns segundos.",
                    status = "ModelNotReady"
                });
            }

            try
            {
                // Fazer previsão
                var resultado = await _mlService.PredictMaintenanceAsync(request.MotoId);

                if (resultado == null)
                {
                    Console.WriteLine($"[MLController] Moto {request.MotoId} não encontrada");
                    return NotFound(new
                    {
                        message = $"Moto com ID {request.MotoId} não encontrada",
                        motoId = request.MotoId
                    });
                }

                var (prediction, inputData) = resultado.Value;

                // Calcular nível de confiança
                string confianca;
                if (prediction!.Probabilidade >= 0.75f || prediction.Probabilidade <= 0.25f)
                    confianca = "Alta";
                else if (prediction.Probabilidade >= 0.60f || prediction.Probabilidade <= 0.40f)
                    confianca = "Média";
                else
                    confianca = "Baixa";

                // Calcular dias estimados até manutenção
                int diasEstimados;
                if (prediction.PrecisaManutencao)
                {
                    // Se precisa, estimar baseado na probabilidade
                    diasEstimados = prediction.Probabilidade >= 0.80f ? 7 : 30;
                }
                else
                {
                    // Se não precisa, estimar baseado nos dias desde última manutenção
                    diasEstimados = Math.Max(60, (int)(180 - inputData!.DiasDesdeUltimaManutencao));
                }

                // Gerar recomendação
                string recomendacao = GerarRecomendacao(prediction.PrecisaManutencao, prediction.Probabilidade, inputData!);

                var response = new PreverManutencaoResponse
                {
                    MotoId = request.MotoId,
                    PrecisaManutencao = prediction.PrecisaManutencao,
                    Probabilidade = prediction.Probabilidade * 100, // Converter para porcentagem
                    Confianca = confianca,
                    DiasEstimadosAteManutencao = diasEstimados,
                    Recomendacao = recomendacao,
                    DadosUtilizados = new DadosAnalise
                    {
                        IdadeMeses = inputData!.IdadeMeses,
                        NumeroMovimentacoes = inputData.NumeroMovimentacoes,
                        DiasDesdeUltimaManutencao = inputData.DiasDesdeUltimaManutencao,
                        TempoMedioPermanencia = inputData.TempoMedioPermanencia
                    }
                };

                Console.WriteLine($"[MLController] Previsão concluída: PrecisaManutencao={response.PrecisaManutencao}, Probabilidade={response.Probabilidade:F1}%");

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MLController] Erro ao fazer previsão: {ex.Message}");
                return StatusCode(500, new
                {
                    message = "Erro ao processar previsão",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Gera recomendação textual baseada na previsão
        /// </summary>
        private string GerarRecomendacao(bool precisaManu, float probabilidade, service.ML.Models.MotoManutencaoData dados)
        {
            if (precisaManu)
            {
                if (probabilidade >= 0.80f)
                {
                    return "URGENTE: A moto necessita de manutenção imediata. Agende uma revisão completa o mais rápido possível.";
                }
                else if (probabilidade >= 0.65f)
                {
                    return "ATENÇÃO: A moto provavelmente precisa de manutenção em breve. Recomendamos agendar uma revisão nas próximas semanas.";
                }
                else
                {
                    return "A moto pode necessitar de manutenção preventiva. Considere agendar uma inspeção nas próximas semanas.";
                }
            }
            else
            {
                if (dados.IdadeMeses > 36)
                {
                    return "A moto está em boas condições, mas por ser antiga, recomendamos manutenção preventiva regular.";
                }
                else if (dados.DiasDesdeUltimaManutencao > 150)
                {
                    return "A moto não necessita manutenção urgente, mas já faz bastante tempo desde a última. Considere agendar em breve.";
                }
                else
                {
                    return "A moto está em boas condições operacionais. Continue com a manutenção preventiva regular.";
                }
            }
        }
    }
}

