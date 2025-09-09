# TechLab Api DotNet

**TechLab Api DotNet** é uma API desenvolvida em .NET para o sistema de **gerenciamento de pátios**, com foco no controle de localização de motos utilizando sensores e tecnologia RFID.  
A solução é modularizada em camadas e utiliza **Entity Framework Core com PostgreSQL** rodando em **Docker Compose**. Conta também com documentação interativa via Swagger.

---

## 📌 Funcionalidades

- Gerenciamento de **usuários**
- Registro e controle de **motos** associadas a usuários
- Cadastro e monitoramento de **sensores** posicionados no pátio
- Registro de **leituras RFID** para rastrear a movimentação das motos
- Administração de **pátios**, com possibilidade de ativar/desativar unidades
- API RESTful com respostas em JSON
- Documentação interativa via Swagger
- Banco de dados PostgreSQL em container
- Migrações automáticas com EF Core

---

# 👩‍💻 Participantes

- Pedro Henrique Mendonça de Novais - RM555276
- Davi Alves de Lima - RM556008
- Rodrigo Alcides Bohac Ríos - RM554826

---

## 🏗 Estrutura do Projeto

- **api**: Camada de apresentação (controllers, Swagger, configurações iniciais)
- **service**: Camada de regras de negócio (serviços e lógica da aplicação)
- **data**: Acesso a dados e contexto do banco (AppDbContext, migrations)
- **library**: Camada de domínio (entidades e modelos do sistema)

---

## 💻 Tecnologias Utilizadas

- .NET 8 / .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL (via Npgsql)
- Docker / Docker Compose
- Swagger / Swashbuckle
- C#

---

## 🚀 Como Executar o Projeto

Clone o repositório:

```bash
git clone https://github.com/pedroonovais/techlab-api-dotnet
cd techlab-api-dotnet
```

### 1. Subir containers
```bash
docker compose up --build
```

Isso irá:
- Criar o container do **PostgreSQL**
- Criar o container da **API**
- Aplicar automaticamente as migrations no banco (`db.Database.Migrate()`)

### 2. Acessar a API
Abra no navegador:
```
http://localhost:5000/swagger
```

---

## 🛠 Migrações (EF Core)

As migrations são aplicadas automaticamente na inicialização da API.  
Mas se você precisar **criar novas migrations** (quando alterar entidades):

```bash
dotnet ef migrations add NomeDaMigration -p data -s api -c data.Context.AppDbContext -o Migrations
```

---

## 📬 Endpoints da API

### 🔹 Usuários (`/api/Usuario`)

| Método | Rota                         | Descrição                   |
|--------|------------------------------|-----------------------------|
| GET    | `/api/Usuario`               | Lista todos os usuários     |
| GET    | `/api/Usuario/{id}`          | Retorna um usuário por ID   |
| POST   | `/api/Usuario`               | Cria um novo usuário        |
| PUT    | `/api/Usuario/{id}`          | Atualiza um usuário         |
| DELETE | `/api/Usuario/{id}`          | Remove um usuário           |

---

### 🔹 Motos (`/api/Moto`)

| Método | Rota                         | Descrição                   |
|--------|------------------------------|-----------------------------|
| GET    | `/api/Moto`                  | Lista todas as motos        |
| GET    | `/api/Moto/{id}`             | Retorna uma moto por ID     |
| POST   | `/api/Moto`                  | Cadastra uma nova moto      |
| PUT    | `/api/Moto/{id}`             | Atualiza os dados da moto   |
| DELETE | `/api/Moto/{id}`             | Remove uma moto             |

---

### 🔹 Sensores (`/api/Sensor`)

| Método | Rota                         | Descrição                         |
|--------|------------------------------|-----------------------------------|
| GET    | `/api/Sensor`                | Lista todos os sensores           |
| GET    | `/api/Sensor/{id}`           | Retorna um sensor por ID          |
| POST   | `/api/Sensor`                | Cadastra um novo sensor           |
| PUT    | `/api/Sensor/{id}`           | Atualiza um sensor existente      |
| DELETE | `/api/Sensor/{id}`           | Remove um sensor                  |

---

### 🔹 Leituras RFID (`/api/leituraRfid`)

| Método | Rota                                 | Descrição                             |
|--------|--------------------------------------|---------------------------------------|
| GET    | `/api/leituraRfid`                   | Lista todas as leituras RFID          |
| GET    | `/api/leituraRfid/{id}`              | Retorna uma leitura por ID            |
| POST   | `/api/leituraRfid`                   | Registra uma nova leitura RFID        |
| PUT    | `/api/leituraRfid/{id}`              | Atualiza uma leitura RFID             |
| DELETE | `/api/leituraRfid/{id}`              | Remove uma leitura RFID               |

---

### 🔹 Pátios (`/api/Patio`)

| Método | Rota                         | Descrição                           |
|--------|------------------------------|-------------------------------------|
| GET    | `/api/Patio`                 | Lista todos os pátios               |
| GET    | `/api/Patio/{id}`            | Retorna um pátio por ID             |
| POST   | `/api/Patio`                 | Cadastra um novo pátio              |
| PUT    | `/api/Patio/{id}`            | Atualiza os dados de um pátio       |
| DELETE | `/api/Patio/{id}`            | Remove um pátio                     |
