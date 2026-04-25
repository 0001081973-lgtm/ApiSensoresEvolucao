# Documentação da API - SIMI

Documentação gerada com auxílio de prompt para IA, descrevendo os endpoints, parâmetros e exemplos de uso da API do sistema SIMI.

---

## Prompt utilizado

```
Você é um especialista em APIs REST. Com base no código abaixo de um controller ASP.NET Core,
gere a documentação completa da API em português, descrevendo cada endpoint, os parâmetros
esperados, os tipos de resposta e exemplos de requisição e resposta em JSON.

[código do SensorController colado aqui]
```

---

## Sobre a API

A API do SIMI é responsável por receber os dados enviados pelo simulador de sensores, validar se estão dentro dos limites configurados e salvar no banco de dados SQLite. Ela também disponibiliza endpoints para consulta e exclusão dos registros.

**URL base:** `https://localhost:7205`  
**Formato:** JSON  
**Documentação interativa:** `https://localhost:7205/swagger`

---

## Modelo de dados - SensorData

Representa um registro de leitura de sensor.

| Campo | Tipo | Descrição |
|---|---|---|
| id | int | identificador do registro (gerado automaticamente) |
| temperatura | double | temperatura em graus Celsius |
| pressao | double | pressão em bar |
| umidade | double | umidade relativa em % |
| vibracao | double | vibração em m/s² |
| origem | string | de onde veio o dado (ex: "Simulator" ou "Interface") |
| timestamp | datetime | data e hora do registro |

**Exemplo:**
```json
{
  "id": 1,
  "temperatura": 65.5,
  "pressao": 4.2,
  "umidade": 70.0,
  "vibracao": 2.8,
  "origem": "Simulator",
  "timestamp": "2026-04-24T14:30:00"
}
```

---

## Endpoints

### POST /api/v1/sensores

Recebe um novo dado de sensor e salva no banco.

Antes de salvar, a API valida se os valores estão dentro dos limites definidos no `appsettings.json`. Se algum valor estiver acima do limite, retorna erro 400.

**Corpo da requisição:**
```json
{
  "temperatura": 65.5,
  "pressao": 4.2,
  "umidade": 70.0,
  "vibracao": 2.8,
  "origem": "Simulator"
}
```

**Resposta de sucesso - 200 OK:**
```json
{
  "id": 1,
  "temperatura": 65.5,
  "pressao": 4.2,
  "umidade": 70.0,
  "vibracao": 2.8,
  "origem": "Simulator",
  "timestamp": "2026-04-24T14:30:00"
}
```

**Resposta de erro - 400 Bad Request:**
```
"Temperatura acima do limite permitido!"
```

---

### GET /api/v1/sensores

Retorna todos os registros salvos no banco.

**Resposta - 200 OK:**
```json
[
  {
    "id": 1,
    "temperatura": 65.5,
    "pressao": 4.2,
    "umidade": 70.0,
    "vibracao": 2.8,
    "origem": "Simulator",
    "timestamp": "2026-04-24T14:30:00"
  },
  {
    "id": 2,
    "temperatura": 42.0,
    "pressao": 3.1,
    "umidade": 55.0,
    "vibracao": 1.2,
    "origem": "Simulator",
    "timestamp": "2026-04-24T14:30:02"
  }
]
```

---

### GET /api/v1/sensores/{id}

Busca um registro específico pelo id.

**Parâmetro:** `id` (int) - id do registro

**Resposta - 200 OK:**
```json
{
  "id": 1,
  "temperatura": 65.5,
  "pressao": 4.2,
  "umidade": 70.0,
  "vibracao": 2.8,
  "origem": "Simulator",
  "timestamp": "2026-04-24T14:30:00"
}
```

**Resposta - 404 Not Found:**
```
"Sensor nao encontrado!"
```

---

### GET /api/v1/sensores/origem/{origem}

Filtra os registros por origem. Útil para separar os dados que vieram do simulador dos que foram inseridos pela interface.

**Parâmetro:** `origem` (string) - ex: `Simulator` ou `Interface`

**Exemplo de requisição:**
```
GET /api/v1/sensores/origem/Simulator
```

**Resposta - 200 OK:**
```json
[
  {
    "id": 1,
    "temperatura": 65.5,
    "pressao": 4.2,
    "umidade": 70.0,
    "vibracao": 2.8,
    "origem": "Simulator",
    "timestamp": "2026-04-24T14:30:00"
  }
]
```

---

### GET /api/v1/sensores/ultimo

Retorna o registro mais recente salvo no banco. Pode ser usado para monitoramento em tempo real.

**Resposta - 200 OK:**
```json
{
  "id": 10,
  "temperatura": 58.3,
  "pressao": 5.0,
  "umidade": 63.0,
  "vibracao": 3.5,
  "origem": "Simulator",
  "timestamp": "2026-04-24T14:30:18"
}
```

**Resposta - 404 Not Found:**
```
"Nenhum dado encontrado!"
```

---

### DELETE /api/v1/sensores/{id}

Deleta um registro pelo id.

**Parâmetro:** `id` (int) - id do registro a ser deletado

**Resposta - 200 OK:**
```
"Deletado com sucesso!"
```

**Resposta - 404 Not Found:**
```
"Sensor nao encontrado!"
```

---

## Códigos de resposta utilizados

| Código | Significado |
|---|---|
| 200 | requisição bem sucedida |
| 400 | dado inválido (acima do limite) |
| 404 | registro não encontrado |

---

## Persistência de dados

A API usa o Entity Framework Core com SQLite para salvar os dados. O banco `simi_sensores.db` é criado automaticamente quando a API é iniciada pela primeira vez.

O SensorSimulator também tem seu próprio banco local (`simulator_log.db`) onde guarda cada leitura gerada, mesmo antes de enviar para a API. Isso serve pra ter um histórico mesmo se a API estiver fora do ar.

A SensorInterface faz o mesmo com `interface_log.db`, salvando localmente tudo que busca na API.
