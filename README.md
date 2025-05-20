# TechLab Api DotNet

**TechLab Api DotNet** é uma API desenvolvida em .NET com o objetivo de simular o funcionamento básico de uma rede social, permitindo o gerenciamento de usuários e publicações (posts). O projeto adota uma arquitetura em camadas com uso de Entity Framework Core conectado a um banco de dados Oracle, e fornece documentação interativa via Swagger.

---

## 📌 Funcionalidades

- Cadastro, visualização, edição e exclusão de **usuários**
- Criação, exibição, atualização e remoção de **posts**
- API RESTful com retorno em JSON
- Documentação interativa com Swagger UI
- Integração com Oracle via Entity Framework Core
- Migrações de banco de dados com EF Core

---

## 🏗 Estrutura do Projeto

- **api**: Camada de apresentação (controllers, Swagger, configurações iniciais)
- **service**: Camada de regras de negócio (serviços)
- **data**: Contexto de banco de dados (AppDbContext, migrations)
- **library**: Modelos de domínio (User, Post, etc.)

---

## 💻 Tecnologias Utilizadas

- .NET 8 / .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- Oracle.EntityFrameworkCore
- Swagger / Swashbuckle
- C#

---

## 🚀 Como Executar o Projeto

Clone o repositório:

git clone https://github.com/pedroonovais/techlab-api-dotnet
cd techlab-api-dotnet

Restaure os pacotes:

dotnet restore

Aplique as migrations:

dotnet ef database update --project ./data --startup-project ./api

Execute a aplicação:

cd api
dotnet run

Acesse a documentação Swagger:

https://localhost:{porta}/swagger

---

## 📬 Endpoints da API - TechLab

### 🔹 Sensores (`/api/Sensor`)

| Método | Rota                   | Descrição                                | Corpo da Requisição                                           |
|--------|------------------------|------------------------------------------|----------------------------------------------------------------|
| GET    | `/api/Sensor`          | Lista todos os sensores                   | -                                                              |
| GET    | `/api/Sensor/{id}`     | Retorna um sensor por ID                  | -                                                              |
| POST   | `/api/Sensor`          | Cria um novo sensor                       | `{ "codigo": "", "descricao": "", "localizacao": "" }`         |
| PUT    | `/api/Sensor/{id}`     | Atualiza os dados de um sensor            | `{ "id": 0, "codigo": "", "descricao": "", "localizacao": "" }`|
| DELETE | `/api/Sensor/{id}`     | Remove um sensor pelo ID                  | -                                                              |

---

### 🔹 Leituras RFID (`/api/LeituraRfid`)

| Método | Rota                          | Descrição                                         | Corpo da Requisição                                                                 |
|--------|-------------------------------|---------------------------------------------------|--------------------------------------------------------------------------------------|
| GET    | `/api/LeituraRfid`           | Lista todas as leituras RFID                      | -                                                                                    |
| GET    | `/api/LeituraRfid/{id}`      | Retorna uma leitura RFID por ID                   | -                                                                                    |
| POST   | `/api/LeituraRfid`           | Registra uma nova leitura RFID                   | `{ "codigoRfid": "", "dataLeitura": "2025-05-19T13:00:00", "sensorId": 1 }`         |
| DELETE | `/api/LeituraRfid/{id}`      | Remove uma leitura RFID pelo ID                   | -                                                                                    |

---

### 🔹 Pátios (`/api/Patio`)

| Método | Rota                | Descrição                              | Corpo da Requisição                                                   |
|--------|---------------------|----------------------------------------|------------------------------------------------------------------------|
| GET    | `/api/Patio`        | Lista todos os pátios cadastrados      | -                                                                      |
| GET    | `/api/Patio/{id}`   | Retorna os dados de um pátio específico| -                                                                      |
| POST   | `/api/Patio`        | Cria um novo pátio                     | `{ "nome": "", "localizacao": "", "ativo": true }`                     |
| PUT    | `/api/Patio/{id}`   | Atualiza um pátio                      | `{ "id": 0, "nome": "", "localizacao": "", "ativo": true }`            |
| DELETE | `/api/Patio/{id}`   | Remove um pátio pelo ID                | -                                                                      |




