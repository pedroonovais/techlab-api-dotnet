namespace api.DTOs.ML
{
    /// <summary>
    /// DTO para resposta com métricas do modelo de ML
    /// </summary>
    public class ModelMetricsResponse
    {
        /// <summary>
        /// Acurácia do modelo (0-100%)
        /// Percentual de previsões corretas sobre o total
        /// </summary>
        public float Acuracia { get; set; }

        /// <summary>
        /// Precisão positiva (0-100%)
        /// Das previsões positivas, quantas estavam corretas
        /// </summary>
        public float PrecisaoPositiva { get; set; }

        /// <summary>
        /// Recall positivo (0-100%)
        /// Dos casos positivos reais, quantos foram identificados
        /// </summary>
        public float RecallPositivo { get; set; }

        /// <summary>
        /// F1 Score (0-100%)
        /// Média harmônica entre precisão e recall
        /// </summary>
        public float F1Score { get; set; }

        /// <summary>
        /// AUC - Area Under Curve (0-100%)
        /// Medida da capacidade do modelo de distinguir entre classes
        /// </summary>
        public float AUC { get; set; }

        /// <summary>
        /// Quantidade de dados usados no treinamento
        /// </summary>
        public int QuantidadeDadosTreinamento { get; set; }

        /// <summary>
        /// Data e hora do treinamento do modelo
        /// </summary>
        public DateTime DataTreinamento { get; set; }

        /// <summary>
        /// Status do modelo
        /// </summary>
        public required string Status { get; set; }
    }
}

