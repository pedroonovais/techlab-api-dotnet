namespace service.ML.Models
{
    /// <summary>
    /// Modelo de métricas do modelo de Machine Learning treinado
    /// Contém todas as métricas de avaliação do modelo de classificação binária
    /// </summary>
    public class ModelMetrics
    {
        /// <summary>
        /// Acurácia do modelo (0.0 a 1.0)
        /// Percentual de previsões corretas sobre o total
        /// </summary>
        public double Acuracia { get; set; }

        /// <summary>
        /// Precisão positiva (0.0 a 1.0)
        /// Das previsões positivas, quantas estavam corretas
        /// </summary>
        public double PrecisaoPositiva { get; set; }

        /// <summary>
        /// Recall positivo (0.0 a 1.0)
        /// Dos casos positivos reais, quantos foram identificados
        /// </summary>
        public double RecallPositivo { get; set; }

        /// <summary>
        /// F1 Score (0.0 a 1.0)
        /// Média harmônica entre precisão e recall
        /// </summary>
        public double F1Score { get; set; }

        /// <summary>
        /// AUC - Area Under Curve (0.0 a 1.0)
        /// Medida da capacidade do modelo de distinguir entre classes
        /// </summary>
        public double AUC { get; set; }

        /// <summary>
        /// Quantidade de dados usados no treinamento
        /// </summary>
        public int QuantidadeDadosTreinamento { get; set; }

        /// <summary>
        /// Data e hora do treinamento do modelo
        /// </summary>
        public DateTime DataTreinamento { get; set; }
    }
}

