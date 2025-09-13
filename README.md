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

- .NET 9
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

### 🔹 Usuario (`/api/Usuario`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/Usuario` | Retorna todos os usuários cadastrados com paginação. |
| POST | `/api/Usuario` | Retorna todos os usuários cadastrados com paginação. |
| DELETE | `/api/Usuario/{id}` | Retorna todos os usuários cadastrados com paginação. |
| GET | `/api/Usuario/{id}` | Retorna todos os usuários cadastrados com paginação. |
| PUT | `/api/Usuario/{id}` | Retorna todos os usuários cadastrados com paginação. |

---

### 🔹 Moto (`/api/Moto`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/Moto` | Retorna todas as motos cadastradas com paginação. |
| POST | `/api/Moto` | Cadastra uma nova moto. |
| DELETE | `/api/Moto/{id}` | Remove uma moto pelo ID. |
| GET | `/api/Moto/{id}` | Retorna uma moto específica por ID. |
| PUT | `/api/Moto/{id}` | Atualiza os dados de uma moto existente. |

---

### 🔹 Patio (`/api/Patio`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/Patio` | Retorna todos os pátios cadastrados com paginação. |
| POST | `/api/Patio` | Retorna todos os pátios cadastrados com paginação. |
| DELETE | `/api/Patio/{id}` | Retorna todos os pátios cadastrados com paginação. |
| GET | `/api/Patio/{id}` | Retorna todos os pátios cadastrados com paginação. |
| PUT | `/api/Patio/{id}` | Retorna todos os pátios cadastrados com paginação. |

---

### 🔹 Perfil (`/api/Perfil`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/Perfil` | Retorna todos os perfis cadastrados com paginação. |
| POST | `/api/Perfil` | Retorna todos os perfis cadastrados com paginação. |
| DELETE | `/api/Perfil/{id}` | Retorna todos os perfis cadastrados com paginação. |
| GET | `/api/Perfil/{id}` | Retorna todos os perfis cadastrados com paginação. |
| PUT | `/api/Perfil/{id}` | Retorna todos os perfis cadastrados com paginação. |

---

### 🔹 Rastreador (`/api/Rastreador`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/Rastreador` | Retorna todos os rastreadores cadastrados com paginação. |
| POST | `/api/Rastreador` | Retorna todos os rastreadores cadastrados com paginação. |
| DELETE | `/api/Rastreador/{id}` | Retorna todos os rastreadores cadastrados com paginação. |
| GET | `/api/Rastreador/{id}` | Retorna todos os rastreadores cadastrados com paginação. |
| PUT | `/api/Rastreador/{id}` | Retorna todos os rastreadores cadastrados com paginação. |

---

### 🔹 StatusOperacional (`/api/StatusOperacional`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/StatusOperacional` | Retorna todos os status operacionais cadastrados com paginação. |
| POST | `/api/StatusOperacional` | Retorna todos os status operacionais cadastrados com paginação. |
| DELETE | `/api/StatusOperacional/{id}` | Retorna todos os status operacionais cadastrados com paginação. |
| GET | `/api/StatusOperacional/{id}` | Retorna todos os status operacionais cadastrados com paginação. |
| PUT | `/api/StatusOperacional/{id}` | Retorna todos os status operacionais cadastrados com paginação. |

---
