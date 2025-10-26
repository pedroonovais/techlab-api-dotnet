using Microsoft.ML.Data;

namespace service.ML.Models
{
    /// <summary>
    /// Modelo de resultado da previsão de manutenção
    /// Contém a predição do modelo e métricas associadas
    /// </summary>
    public class ManutencaoPrediction
    {
        /// <summary>
        /// Resultado da previsão: true se a moto precisa de manutenção, false caso contrário
        /// </summary>
        [ColumnName("PredictedLabel")]
        public bool PrecisaManutencao { get; set; }

        /// <summary>
        /// Probabilidade da previsão (0.0 a 1.0)
        /// Valor mais próximo de 1.0 indica maior confiança na previsão positiva
        /// </summary>
        [ColumnName("Probability")]
        public float Probabilidade { get; set; }

        /// <summary>
        /// Score bruto da previsão antes da calibração
        /// Usado internamente pelo modelo para métricas
        /// </summary>
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}

