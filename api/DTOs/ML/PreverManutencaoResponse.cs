namespace api.DTOs.ML
{
    /// <summary>
    /// DTO para resposta de previsão de manutenção
    /// </summary>
    public class PreverManutencaoResponse
    {
        /// <summary>
        /// ID da moto analisada
        /// </summary>
        public Guid MotoId { get; set; }

        /// <summary>
        /// Indica se a moto precisa de manutenção
        /// </summary>
        public bool PrecisaManutencao { get; set; }

        /// <summary>
        /// Probabilidade da previsão em porcentagem (0-100)
        /// </summary>
        public float Probabilidade { get; set; }

        /// <summary>
        /// Nível de confiança da previsão: "Alta", "Média" ou "Baixa"
        /// </summary>
        public required string Confianca { get; set; }

        /// <summary>
        /// Estimativa de dias até a próxima manutenção
        /// </summary>
        public int DiasEstimadosAteManutencao { get; set; }

        /// <summary>
        /// Recomendação textual baseada na análise
        /// </summary>
        public required string Recomendacao { get; set; }

        /// <summary>
        /// Dados utilizados na análise (para transparência)
        /// </summary>
        public required DadosAnalise DadosUtilizados { get; set; }
    }

    /// <summary>
    /// Dados que foram analisados pelo modelo
    /// </summary>
    public class DadosAnalise
    {
        /// <summary>
        /// Idade da moto em meses
        /// </summary>
        public float IdadeMeses { get; set; }

        /// <summary>
        /// Número de movimentações registradas
        /// </summary>
        public float NumeroMovimentacoes { get; set; }

        /// <summary>
        /// Dias desde a última manutenção
        /// </summary>
        public float DiasDesdeUltimaManutencao { get; set; }

        /// <summary>
        /// Tempo médio de permanência no pátio em horas
        /// </summary>
        public float TempoMedioPermanencia { get; set; }
    }
}

