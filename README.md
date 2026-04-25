# SIMI — Sistema Integrado de Monitoramento Industrial

> **Atividade:** Documentação, Persistência e Publicação de API de Sensores  
> **Tecnologias:** .NET 8 · ASP.NET Core Web API · Entity Framework Core · SQLite · WPF · Swagger/OpenAPI

---

## Visão Geral

O SIMI é um sistema distribuído para monitoramento de sensores industriais em tempo real. O ecossistema é composto por quatro componentes principais que garantem a coleta, validação, persistência e visualização de dados críticos de telemetria.

| Componente | Tipo | Descrição |
|---|---|---|
| `Shared` | Class Library | Modelo de dados compartilhado (`SensorData`) entre todos os projetos. |
| `ApiProcessamento` | ASP.NET Web API | Core do sistema: valida limites técnicos, atribui timestamps e persiste dados. |
| `SensorSimulator` | Console App | Simula o hardware industrial, enviando dados para a API a cada 2 segundos. |
| `SensorInterface` | WPF Desktop App | Dashboard para visualização e consulta histórica dos dados processados. |

---

## Arquitetura e Fluxo de Dados

O sistema utiliza uma arquitetura cliente-servidor onde emissores alimentam uma base de dados centralizada, enquanto a interface de usuário consome esses dados para monitoramento.

### Fluxo de Comunicação e Persistência

```text
┌─────────────────┐         POST /api/v1/sensores        ┌─────────────────────┐
│  SensorSimulator │ ──────────────────────────────────► │   ApiProcessamento  │
│  (Console App)   │                                     │   (ASP.NET Web API) │
│                  │  Persiste localmente em:            │                     │
│  simulator_log.db│  simulator_log.db                   │   simi_sensores.db  │
└─────────────────┘                                      │   (EF Core + SQLite) │
                                                         └──────────┬──────────┘
┌─────────────────┐          GET /api/v1/sensores           │
│  SensorInterface │ ◄──────────────────────────────────────┘
│  (WPF Desktop)   │
│                  │  Persiste cache em:
│  interface_log.db│  interface_log.db
└─────────────────┘
```
---

## Sinal Industrial Adicionado: Vibração (m/s²)

A inclusão do sinal de **Vibração** é fundamental para a estratégia de **Manutenção Preditiva**. Em máquinas rotativas (motores, bombas e ventiladores), o aumento da vibração é o primeiro indicador de falhas mecânicas.

* **Finalidade:** Identificar desbalanceamento, desalinhamento de eixos e desgaste de rolamentos.
* **Limite Máximo:** 50 m/s² (Configurável via `appsettings.json`).
* **Justificativa:** Prevenção de paradas não programadas.

| Sinal | Unidade | Limite Máximo |
|---|---|---|
| Temperatura | °C | 80 |
| Pressão | bar | 10 |
| Umidade | % | - |
| Vibração | m/s² | 50 |

---

## Configuração e Execução

### Pré-requisitos
* [.NET 8 SDK]
* Visual Studio 2022

### Passo a Passo

1.  **Clone o repositório:**
    ```bash
    git clone (https://github.com/0001081973-lgtm/ApiSensorIOT)
    cd SIMI
    ```

2.  **Inicie a API (Terminal 1):**
    ```bash
    cd ApiProcessamento
    dotnet run
    ```
    *A API criará o banco `simi_sensores.db` automaticamente. 

3.  **Inicie o Simulador (Terminal 2):**
    ```bash
    cd SensorSimulator
    dotnet run
    ```

4.  **Inicie a Interface WPF (Terminal 3):**
    ```bash
    cd SensorInterface
    dotnet run
    ```

---

## Documentação da API (Endpoints)

Base URL: `https://localhost:64813/api/v1/sensores`

| Método | Rota | Descrição |
|---|---|---|
| **POST** | `/` | Recebe e valida dados. Salva com Timestamp atual. |
| **GET** | `/` | Lista todos os registros do banco de dados. |
| **GET** | `/{id}` | Busca um registro específico por ID. |
| **GET** | `/origem/{origem}` | Filtra registros por origem (Ex: `Simulator`). |
| **GET** | `/ultimo` | Retorna o registro mais recente (maior timestamp). |
| **DELETE** | `/{id}` | Remove um registro permanentemente. |

---

##  Evidências de Persistência (Método POST)

Nesta seção, apresentamos a validação do fluxo completo de persistência, desde o envio dos dados até a gravação no banco SQLite.

### 1. Chamada do Endpoint via Swagger/Postman
*(Requisição POST retornando status 200 OK)*

> ** <img width="1382" height="841" alt="POST_EVIDENCIA" src="https://github.com/user-attachments/assets/fb42058e-7431-4131-b75d-d2de4f70a990" /> **


### 2. Persistência no Banco de Dados SQLite
Abaixo, a evidência dos dados salvos na tabela `Sensores` após o processamento da API.

> ** <img width="955" height="647" alt="PERCISTENCIA_EMBANCO" src="https://github.com/user-attachments/assets/74f4a5f7-f4ca-41f4-bde8-51a74828106d" /> **

---

## Configurações (`appsettings.json`)

Os limites de validação podem ser alterados sem recompilação:

```json
{
  "ApiConfig": {
    "MaxTemperatura": 80,
    "MaxPressao": 10,
    "MaxVibracao": 50
  }
}



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

### Análise dos Endpoints

#### 1. Receber Dado de Sensor

**`POST /api/v1/sensores`**

**Finalidade:** Ponto de entrada principal da API. Recebe um pacote de leitura de sensor,
executa validação de limites e persiste o registro no banco se aprovado.

**Lógica de validação aplicada:**
- Se `temperatura > MaxTemperatura` (padrão: 80°C) → rejeita com `400`
- Se `pressao > MaxPressao` (padrão: 10 bar) → rejeita com `400`
- Se `vibracao > MaxVibracao` (padrão: 50 m/s²) → rejeita com `400`
- Se aprovado → persiste e retorna `201 Created` com o objeto salvo (incluindo ID gerado)

**Caso de uso:** O `SensorSimulator` chama este endpoint a cada 2 segundos com uma nova leitura gerada aleatoriamente.

**Corpo da requisição:**
```json
{
  "temperatura": 55.40,
  "pressao": 3.80,
  "umidade": 72.00,
  "vibracao": 1.95,
  "origem": "Simulator"
}
```

**Resposta de sucesso (`201 Created`):**
```json
{
  "id": 15,
  "temperatura": 55.40,
  "pressao": 3.80,
  "umidade": 72.00,
  "vibracao": 1.95,
  "origem": "Simulator",
  "timestamp": "2026-04-24T14:30:00.000Z"
}
```

**Resposta de erro (`400 Bad Request`):**
```
"Temperatura 92.5°C acima do limite de 80°C."
```

---

#### 2. Listar Todos os Registros

**`GET /api/v1/sensores`**

**Finalidade:** Retorna o histórico completo de leituras, ordenadas da mais recente para
a mais antiga. Consumido pela `SensorInterface` (WPF) para exibir os dados em tabela.

**Caso de uso:** A interface WPF chama este endpoint quando o usuário clica em "Carregar Dados".

**Resposta (`200 OK`):**
```json
[
  {
    "id": 15,
    "temperatura": 55.40,
    "pressao": 3.80,
    "umidade": 72.00,
    "vibracao": 1.95,
    "origem": "Simulator",
    "timestamp": "2026-04-24T14:32:00.000Z"
  },
  {
    "id": 14,
    "temperatura": 61.20,
    "pressao": 4.10,
    "umidade": 68.50,
    "vibracao": 2.40,
    "origem": "Simulator",
    "timestamp": "2026-04-24T14:30:00.000Z"
  }
]
```

---

#### 3. Buscar por ID

**`GET /api/v1/sensores/{id}`**

**Finalidade:** Recupera um registro específico. Útil para auditorias, rastreabilidade
e integração com sistemas externos que precisam verificar um dado específico.

**Caso de uso:** Um sistema de BI externo precisa verificar a leitura com ID 42 para
cruzar com um evento de alarme registrado no mesmo horário.

**Exemplo de requisição:**
```
GET /api/v1/sensores/42
```

**Resposta (`200 OK`):**
```json
{
  "id": 42,
  "temperatura": 78.90,
  "pressao": 9.20,
  "umidade": 40.00,
  "vibracao": 12.50,
  "origem": "Simulator",
  "timestamp": "2026-04-24T10:15:00.000Z"
}
```

**Resposta (`404 Not Found`):**
```
"Registro com ID 42 não encontrado."
```

---

#### 4. Filtrar por Origem

**`GET /api/v1/sensores/por-origem/{origem}`**

**Finalidade:** Separa as leituras por fonte de dados. Permite analisar individualmente
os dados gerados pelo simulador versus os inseridos manualmente pela interface, facilitando
comparações e auditoria de qualidade.

**Caso de uso:** Equipe de QA quer verificar apenas os dados inseridos manualmente pela
interface para validar a consistência das entradas humanas.

**Exemplo de requisição:**
```
GET /api/v1/sensores/por-origem/Interface
```

**Resposta (`200 OK`):**
```json
[
  {
    "id": 7,
    "temperatura": 30.00,
    "pressao": 2.00,
    "umidade": 55.00,
    "vibracao": 0.80,
    "origem": "Interface",
    "timestamp": "2026-04-24T09:00:00.000Z"
  }
]
```

---

#### 5. Último Registro

**`GET /api/v1/sensores/ultimo`**

**Finalidade:** Fornece acesso rápido à leitura mais recente sem carregar todo o histórico.
Ideal para dashboards em tempo real e sistemas de alarme que precisam checar o estado atual.

**Caso de uso:** Um painel de monitoramento faz polling a cada 5 segundos neste endpoint
para exibir os valores atuais dos sensores em tempo real.

**Resposta (`200 OK`):**
```json
{
  "id": 120,
  "temperatura": 42.10,
  "pressao": 3.50,
  "umidade": 65.00,
  "vibracao": 2.10,
  "origem": "Simulator",
  "timestamp": "2026-04-24T14:59:58.000Z"
}
```

**Resposta (`404 Not Found`):**
```
"Nenhum dado de sensor registrado."
```

---

#### 6. Remover Registro

**`DELETE /api/v1/sensores/{id}`**

**Finalidade:** Remove permanentemente um registro do banco. Destinado a operações
administrativas, correção de dados inválidos ou conformidade com políticas de retenção.

**Caso de uso:** Um operador identificou que o registro ID 5 foi inserido com valores
incorretos devido a um sensor defeituoso e precisa removê-lo do histórico.

**Exemplo de requisição:**
```
DELETE /api/v1/sensores/5
```

**Resposta (`204 No Content`):** corpo vazio — remoção bem-sucedida.

**Resposta (`404 Not Found`):**
```
"Registro com ID 5 não encontrado."
```

---

### Diagrama de Fluxo de Dados

```
SensorSimulator                     ApiProcessamento                   SensorInterface
      │                                    │                                  │
      │── POST /api/v1/sensores ──────────►│                                  │
      │                                    │── Valida limites                 │
      │                                    │── Persiste no SQLite             │
      │◄── 201 Created ───────────────────│                                  │
      │                                    │                                  │
      │                                    │◄── GET /api/v1/sensores ─────────│
      │                                    │── Consulta SQLite                │
      │                                    │── 200 OK + lista ───────────────►│
      │                                    │                                  │── Persiste local
      │                                    │                                  │── Exibe na UI
```

---

### Resumo dos Códigos HTTP

| Código | Significado | Quando ocorre |
|---|---|---|
| `200 OK` | Sucesso | GET com resultado |
| `201 Created` | Criado | POST bem-sucedido |
| `204 No Content` | Sem conteúdo | DELETE bem-sucedido |
| `400 Bad Request` | Requisição inválida | Dado fora dos limites |
| `404 Not Found` | Não encontrado | ID inexistente ou banco vazio |
