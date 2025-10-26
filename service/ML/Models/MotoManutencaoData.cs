using Microsoft.ML.Data;

namespace service.ML.Models
{
    /// <summary>
    /// Modelo de dados para treinamento de previsão de manutenção de motos
    /// Representa as características de uma moto que influenciam na necessidade de manutenção
    /// </summary>
    public class MotoManutencaoData
    {
        /// <summary>
        /// Idade da moto em meses desde o cadastro
        /// </summary>
        [LoadColumn(0)]
        public float IdadeMeses { get; set; }

        /// <summary>
        /// Número total de movimentações registradas para a moto
        /// Movimentações frequentes podem indicar maior desgaste
        /// </summary>
        [LoadColumn(1)]
        public float NumeroMovimentacoes { get; set; }

        /// <summary>
        /// Dias desde a última manutenção registrada
        /// Maior tempo sem manutenção aumenta a probabilidade de precisar
        /// </summary>
        [LoadColumn(2)]
        public float DiasDesdeUltimaManutencao { get; set; }

        /// <summary>
        /// Tempo médio de permanência da moto no pátio em horas
        /// Permanência baixa pode indicar uso intensivo
        /// </summary>
        [LoadColumn(3)]
        public float TempoMedioPermanencia { get; set; }

        /// <summary>
        /// Label: Indica se a moto precisa de manutenção (true) ou não (false)
        /// Esta é a variável alvo para o modelo de classificação binária
        /// </summary>
        [LoadColumn(4)]
        [ColumnName("Label")]
        public bool PrecisaManutencao { get; set; }
    }
}

