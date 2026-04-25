using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class SensorData
    {
        public int Id { get; set; }
        public double Temperatura { get; set; }
        public double Pressao { get; set; }
        public double Umidade { get; set; }
        // vibração em m/s² - adicionado para monitorar maquinas rotativas
        public double Vibracao { get; set; }
        public string Origem { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
