using Microsoft.Data.Sqlite;
using Shared;
using System.Net.Http.Json;

var http = new HttpClient();
int index = 0;

//cria o banco local do simulador se nao existir
CriarBanco();

while (true)
{
    var sensor = new SensorData
    {
        Id = index,
        Temperatura = new Random().Next(20, 100),
        Pressao = Math.Round(new Random().NextDouble() * 9 + 0.5, 2),
        Umidade = new Random().Next(20, 90),
        Vibracao = Math.Round(new Random().NextDouble() * 44 + 0.1, 2),
        Origem = "Simulator",
        Timestamp = DateTime.Now
    };

    // salva localmente antes de enviar
    SalvarLocal(sensor);

    var response = await http.PostAsJsonAsync(
        "https://localhost:64813/api/v1/sensores", sensor);

    if (!response.IsSuccessStatusCode)
    {
        var erro = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Erro: {response.StatusCode} - {erro}");
    }
    else
    {
        Console.WriteLine($"Enviado: Temp={sensor.Temperatura} | Pressao={sensor.Pressao} | Umidade={sensor.Umidade} | Vibracao={sensor.Vibracao}");
    }

    await Task.Delay(2000);
    index++;
}

void CriarBanco()
{
    using var conn = new SqliteConnection("Data Source=simulator_log.db");
    conn.Open();
    var cmd = conn.CreateCommand();
    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SensorData (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Temperatura REAL,
        Pressao REAL,
        Umidade REAL,
        Vibracao REAL,
        Origem TEXT,
        Timestamp TEXT
    )";
    cmd.ExecuteNonQuery();
}

void SalvarLocal(SensorData sensor)
{
    using var conn = new SqliteConnection("Data Source=simulator_log.db");
    conn.Open();
    var cmd = conn.CreateCommand();
    cmd.CommandText = @"INSERT INTO SensorData (Temperatura, Pressao, Umidade, Vibracao, Origem, Timestamp)
                        VALUES ($temp, $pressao, $umidade, $vibracao, $origem, $ts)";
    cmd.Parameters.AddWithValue("$temp", sensor.Temperatura);
    cmd.Parameters.AddWithValue("$pressao", sensor.Pressao);
    cmd.Parameters.AddWithValue("$umidade", sensor.Umidade);
    cmd.Parameters.AddWithValue("$vibracao", sensor.Vibracao);
    cmd.Parameters.AddWithValue("$origem", sensor.Origem);
    cmd.Parameters.AddWithValue("$ts", sensor.Timestamp.ToString());
    cmd.ExecuteNonQuery();
}
