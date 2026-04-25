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
