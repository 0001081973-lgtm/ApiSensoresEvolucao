# SIMI - Sistema Integrado de Monitoramento Industrial

Projeto desenvolvido para a disciplina de sistemas distribuídos.
O sistema é composto por uma API, um simulador de sensores e uma interface desktop para visualização dos dados.

## Tecnologias utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core com SQLite
- WPF (Windows Presentation Foundation)
- Swagger para documentação da API

## Estrutura do projeto

```
SIMI/
├── Shared/              -> modelo de dados compartilhado entre os projetos
├── ApiProcessamento/    -> API que recebe e salva os dados dos sensores
├── SensorSimulator/     -> simula sensores enviando dados para a API
└── SensorInterface/     -> interface WPF para visualizar os dados
```

## Como executar

**1. Iniciar a API**
```
cd ApiProcessamento
dotnet run
```
O banco de dados SQLite (`simi_sensores.db`) é criado automaticamente na primeira execução.
O Swagger fica disponível em: `https://localhost:7205/swagger`

**2. Iniciar o Simulador** (em outro terminal)
```
cd SensorSimulator
dotnet run
```
Os dados ficam salvos também em `simulator_log.db` localmente.

**3. Abrir a Interface** (Windows)
```
cd SensorInterface
dotnet run
```
Clique em "Atualizar" para carregar os dados da API. Os dados são salvos em `interface_log.db`.

## Sinal industrial adicionado

Foi adicionado o sinal de **Vibração (m/s²)** ao conjunto de sensores.

A vibração é importante para monitorar maquinas rotativas como motores e bombas. Com ela é possivel identificar problemas como desbalanceamento ou desgaste antes que causem falha no equipamento. O limite configurado é de 50 m/s².

## Sinais monitorados

| Sinal | Unidade | Limite máximo |
|---|---|---|
| Temperatura | °C | 80 |
| Pressão | bar | 10 |
| Umidade | % | - |
| Vibração | m/s² | 50 |

## Endpoints da API

| Método | Rota | Descrição |
|---|---|---|
| POST | /api/v1/sensores | envia um novo dado de sensor |
| GET | /api/v1/sensores | lista todos os dados |
| GET | /api/v1/sensores/{id} | busca por id |
| GET | /api/v1/sensores/origem/{origem} | filtra por origem |
| GET | /api/v1/sensores/ultimo | retorna o ultimo registro |
| DELETE | /api/v1/sensores/{id} | deleta um registro |
