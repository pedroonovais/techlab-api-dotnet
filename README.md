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

## 🔄 Versionamento da API

A API utiliza versionamento por URL Path para garantir compatibilidade e evolução controlada:

- **Versão Atual:** v1
- **Formato:** `/api/v{version}/[controller]`
- **Exemplo:** `/api/v1/Usuario`, `/api/v1/Moto`

### Como funciona:
- Todos os endpoints atuais estão na versão 1
- Futuras versões (v2, v3, etc.) podem coexistir
- Headers de resposta incluem `api-supported-versions: 1.0`
- Swagger documenta a versão ativa

---

## 🏥 Health Checks

A API possui endpoints de Health Checks para monitoramento de saúde e disponibilidade:

### Endpoints Disponíveis:

- **`/health`** - Status completo da API e dependências
  - Verifica: API + Banco de dados PostgreSQL
  - Retorna: JSON detalhado com status de cada componente

- **`/health/live`** - Liveness probe
  - Verifica: Se a API está respondendo
  - Uso: Kubernetes liveness probe

- **`/health/ready`** - Readiness probe  
  - Verifica: Se a API está pronta (DB conectado)
  - Uso: Kubernetes readiness probe

- **`/health-ui`** - Interface visual de monitoramento
  - Dashboard interativo com histórico
  - Atualização automática a cada 30 segundos

### Exemplos de Uso:

```bash
# Verificar saúde completa
curl http://localhost:8080/health

# Verificar apenas se API está UP
curl http://localhost:8080/health/live

# Verificar se API está pronta para receber requisições
curl http://localhost:8080/health/ready
```

### Resposta Exemplo:

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "API Health": {
      "status": "Healthy",
      "description": "API está respondendo"
    },
    "PostgreSQL Database": {
      "status": "Healthy",
      "description": "Connection successful"
    }
  }
}
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

**Versão Atual:** v1  
**URLs:** `/api/v1/[controller]`

### 🔹 Usuario (`/api/v1/Usuario`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Usuario` | Retorna todos os usuários cadastrados com paginação. |
| POST | `/api/v1/Usuario` | Cadastra um novo usuário. |
| DELETE | `/api/v1/Usuario/{id}` | Remove um usuário pelo ID. |
| GET | `/api/v1/Usuario/{id}` | Retorna um usuário específico por ID. |
| PUT | `/api/v1/Usuario/{id}` | Atualiza os dados de um usuário existente. |

---

### 🔹 Moto (`/api/v1/Moto`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Moto` | Retorna todas as motos cadastradas com paginação. |
| POST | `/api/v1/Moto` | Cadastra uma nova moto. |
| DELETE | `/api/v1/Moto/{id}` | Remove uma moto pelo ID. |
| GET | `/api/v1/Moto/{id}` | Retorna uma moto específica por ID. |
| PUT | `/api/v1/Moto/{id}` | Atualiza os dados de uma moto existente. |

---

### 🔹 Patio (`/api/v1/Patio`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Patio` | Retorna todos os pátios cadastrados com paginação. |
| POST | `/api/v1/Patio` | Cadastra um novo pátio. |
| DELETE | `/api/v1/Patio/{id}` | Remove um pátio pelo ID. |
| GET | `/api/v1/Patio/{id}` | Retorna um pátio específico por ID. |
| PUT | `/api/v1/Patio/{id}` | Atualiza os dados de um pátio existente. |

---

### 🔹 Perfil (`/api/v1/Perfil`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Perfil` | Retorna todos os perfis cadastrados com paginação. |
| POST | `/api/v1/Perfil` | Cadastra um novo perfil. |
| DELETE | `/api/v1/Perfil/{id}` | Remove um perfil pelo ID. |
| GET | `/api/v1/Perfil/{id}` | Retorna um perfil específico por ID. |
| PUT | `/api/v1/Perfil/{id}` | Atualiza os dados de um perfil existente. |

---

### 🔹 Rastreador (`/api/v1/Rastreador`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Rastreador` | Retorna todos os rastreadores cadastrados com paginação. |
| POST | `/api/v1/Rastreador` | Cadastra um novo rastreador. |
| DELETE | `/api/v1/Rastreador/{id}` | Remove um rastreador pelo ID. |
| GET | `/api/v1/Rastreador/{id}` | Retorna um rastreador específico por ID. |
| PUT | `/api/v1/Rastreador/{id}` | Atualiza os dados de um rastreador existente. |

---

### 🔹 StatusOperacional (`/api/v1/StatusOperacional`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/StatusOperacional` | Retorna todos os status operacionais cadastrados com paginação. |
| POST | `/api/v1/StatusOperacional` | Cadastra um novo status operacional. |
| DELETE | `/api/v1/StatusOperacional/{id}` | Remove um status operacional pelo ID. |
| GET | `/api/v1/StatusOperacional/{id}` | Retorna um status operacional específico por ID. |
| PUT | `/api/v1/StatusOperacional/{id}` | Atualiza os dados de um status operacional existente. |

---
