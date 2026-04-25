namespace ApiProcessamento.Config
{
    /// <summary>
    /// Classe de configuracao para a API, contendo propriedades que podem ser definidas
    /// no arquivo appsettings.json ou em variaveis de ambiente. Neste exemplo, temos a
    /// propriedade MaxTemperatura, que pode ser usada para definir um limite maximo de
    /// temperatura para os dados recebidos pela API.
    /// tambem foi adicionado limites de pressao e vibracao
    /// </summary>
    public class ApiConfig
    {
        public double MaxTemperatura { get; set; }
        public double MaxPressao { get; set; }
        public double MaxVibracao { get; set; }
    }
}
