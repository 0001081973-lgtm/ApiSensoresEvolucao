using Microsoft.Data.Sqlite;
using SensorInterface.Command;
using SensorInterface.ViewModels;
using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SensorInterface.Model
{
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Atributos gerais de binding
        /// </summary>
        public ObservableCollection<SensorData> Sensores { get; set; }

        private string _status = "";
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        /// <summary>
        /// Comandos
        /// </summary>
        public ICommand CarregarSensoresCommand { get; }

        public MainViewModel()
        {
            Sensores = new ObservableCollection<SensorData>();
            //comandos
            CarregarSensoresCommand = new RelayCommand(CarregarSensores);

            //cria o banco local da interface
            CriarBanco();
        }

        private async void CarregarSensores()
        {
            Status = "Carregando...";
            var http = new HttpClient();
            var dados = await http.GetFromJsonAsync<List<SensorData>>(
                "https://localhost:64813/api/v1/sensores");

            Sensores.Clear();
            foreach (var sensor in dados)
            {
                Sensores.Add(sensor);
                SalvarLocal(sensor);
            }

            Status = $"Total carregado: {dados.Count} registros";
        }

        private void CriarBanco()
        {
            using var conn = new SqliteConnection("Data Source=interface_log.db");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SensorData (
                Id INTEGER PRIMARY KEY,
                Temperatura REAL,
                Pressao REAL,
                Umidade REAL,
                Vibracao REAL,
                Origem TEXT,
                Timestamp TEXT
            )";
            cmd.ExecuteNonQuery();
        }

        private void SalvarLocal(SensorData sensor)
        {
            using var conn = new SqliteConnection("Data Source=interface_log.db");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT OR IGNORE INTO SensorData (Id, Temperatura, Pressao, Umidade, Vibracao, Origem, Timestamp)
                                VALUES ($id, $temp, $pressao, $umidade, $vibracao, $origem, $ts)";
            cmd.Parameters.AddWithValue("$id", sensor.Id);
            cmd.Parameters.AddWithValue("$temp", sensor.Temperatura);
            cmd.Parameters.AddWithValue("$pressao", sensor.Pressao);
            cmd.Parameters.AddWithValue("$umidade", sensor.Umidade);
            cmd.Parameters.AddWithValue("$vibracao", sensor.Vibracao);
            cmd.Parameters.AddWithValue("$origem", sensor.Origem);
            cmd.Parameters.AddWithValue("$ts", sensor.Timestamp.ToString());
            cmd.ExecuteNonQuery();
        }
    }
}
