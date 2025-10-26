# TechLab Api DotNet

**TechLab Api DotNet** é uma API desenvolvida em .NET para o sistema de **gerenciamento de pátios**, com foco no controle de localização de motos utilizando sensores e tecnologia RFID.  
A solução é modularizada em camadas e utiliza **Entity Framework Core com PostgreSQL** rodando em **Docker Compose**. Conta também com documentação interativa via Swagger.

---

## 📌 Funcionalidades

- 🔐 **Autenticação JWT** com registro e login de usuários
- 🔒 **Segurança** com hash BCrypt e proteção de rotas
- Gerenciamento de **usuários** com perfis e permissões
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
- **JWT (JSON Web Tokens)** para autenticação
- **BCrypt.Net** para hash de senhas
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

## 🔐 Autenticação JWT

A API utiliza **JSON Web Tokens (JWT)** para autenticação e autorização. Todos os endpoints principais estão protegidos e requerem um token válido.

### 🚀 Como Começar

#### 1️⃣ Registrar um Novo Usuário

```bash
POST /api/v1/Auth/register
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao@techlab.com",
  "senha": "senha123",
  "confirmacaoSenha": "senha123"
}
```

**Nota:** O campo `perfilId` é **opcional**. Se não fornecido, será criado automaticamente um perfil padrão "Usuário Padrão".

**Resposta (201 Created):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 28800,
  "usuarioId": "guid-do-usuario",
  "nome": "João Silva",
  "email": "joao@techlab.com",
  "perfilId": "guid-do-perfil"
}
```

#### 2️⃣ Fazer Login

```bash
POST /api/v1/Auth/login
Content-Type: application/json

{
  "email": "joao@techlab.com",
  "senha": "senha123"
}
```

**Resposta (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 28800,
  "usuarioId": "guid-do-usuario",
  "nome": "João Silva",
  "email": "joao@techlab.com",
  "perfilId": "guid-do-perfil"
}
```

#### 3️⃣ Usar o Token em Requisições

Após obter o token, inclua-o no header `Authorization` de todas as requisições:

```bash
GET /api/v1/Usuario
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 🔑 Endpoints de Autenticação

| Método | Rota | Descrição | Autenticação |
|--------|------|-----------|--------------|
| POST | `/api/v1/Auth/register` | Registra um novo usuário e retorna token JWT | ❌ Não requer |
| POST | `/api/v1/Auth/login` | Autentica um usuário e retorna token JWT | ❌ Não requer |
| GET | `/api/v1/Auth/check-email?email={email}` | Verifica se um e-mail já está cadastrado | ❌ Não requer |
| GET | `/api/v1/Auth/me` | Retorna informações do usuário autenticado | ✅ Requer token |

### 🧪 Testando com Swagger

1. Acesse o Swagger em `http://localhost:5000/swagger`
2. Registre-se ou faça login usando os endpoints de Auth
3. Copie o token retornado
4. Clique no botão **"Authorize"** 🔒 no canto superior direito
5. Digite: `Bearer {seu-token}` (substitua `{seu-token}` pelo token copiado)
6. Clique em **"Authorize"** e depois **"Close"**
7. Agora você pode testar todos os endpoints protegidos! ✅

### 📋 Exemplos Práticos

#### Exemplo com cURL:

```bash
# 1. Fazer login
TOKEN=$(curl -X POST http://localhost:5000/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"joao@techlab.com","senha":"senha123"}' \
  | jq -r '.token')

# 2. Usar o token para acessar endpoint protegido
curl http://localhost:5000/api/v1/Usuario \
  -H "Authorization: Bearer $TOKEN"
```

#### Exemplo com JavaScript/Fetch:

```javascript
// 1. Fazer login
const response = await fetch('http://localhost:5000/api/v1/Auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'joao@techlab.com',
    senha: 'senha123'
  })
});

const { token } = await response.json();

// 2. Usar o token
const usuarios = await fetch('http://localhost:5000/api/v1/Usuario', {
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### 🔒 Segurança

- ✅ Senhas criptografadas com **BCrypt** (impossível reverter)
- ✅ Tokens JWT assinados digitalmente (HMAC-SHA256)
- ✅ Tokens válidos por **8 horas** (configurável)
- ✅ Validação automática em todas as requisições
- ✅ Todos os endpoints principais protegidos com `[Authorize]`

### 📚 Documentação Completa

Para mais detalhes sobre autenticação, consulte:
- **[AUTENTICACAO_JWT.md](AUTENTICACAO_JWT.md)** - Guia completo de uso
- **[CHANGELOG_JWT.md](CHANGELOG_JWT.md)** - Histórico de alterações
- **[api/auth-examples.http](api/auth-examples.http)** - Exemplos de requisições HTTP

---

## 📬 Endpoints da API

**Versão Atual:** v1  
**URLs:** `/api/v1/[controller]`

⚠️ **Atenção:** Todos os endpoints abaixo **requerem autenticação JWT** (exceto endpoints de Auth). Inclua o token no header: `Authorization: Bearer {token}`

### 🔐 Auth (`/api/v1/Auth`)

| Método | Rota | Descrição | Autenticação |
|--------|------|-----------|--------------|
| POST | `/api/v1/Auth/register` | Registra um novo usuário e retorna token JWT. | ❌ Não requer |
| POST | `/api/v1/Auth/login` | Autentica um usuário e retorna token JWT. | ❌ Não requer |
| GET | `/api/v1/Auth/check-email` | Verifica se um e-mail já está cadastrado. | ❌ Não requer |
| GET | `/api/v1/Auth/me` | Retorna informações do usuário autenticado. | ✅ Requer token |

---

### 🔹 Usuario (`/api/v1/Usuario`) 🔒

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Usuario` | Retorna todos os usuários cadastrados com paginação. |
| POST | `/api/v1/Usuario` | Cadastra um novo usuário. |
| DELETE | `/api/v1/Usuario/{id}` | Remove um usuário pelo ID. |
| GET | `/api/v1/Usuario/{id}` | Retorna um usuário específico por ID. |
| PUT | `/api/v1/Usuario/{id}` | Atualiza os dados de um usuário existente. |

---

### 🔹 Moto (`/api/v1/Moto`) 🔒

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Moto` | Retorna todas as motos cadastradas com paginação. |
| POST | `/api/v1/Moto` | Cadastra uma nova moto. |
| DELETE | `/api/v1/Moto/{id}` | Remove uma moto pelo ID. |
| GET | `/api/v1/Moto/{id}` | Retorna uma moto específica por ID. |
| PUT | `/api/v1/Moto/{id}` | Atualiza os dados de uma moto existente. |

---

### 🔹 Patio (`/api/v1/Patio`) 🔒

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Patio` | Retorna todos os pátios cadastrados com paginação. |
| POST | `/api/v1/Patio` | Cadastra um novo pátio. |
| DELETE | `/api/v1/Patio/{id}` | Remove um pátio pelo ID. |
| GET | `/api/v1/Patio/{id}` | Retorna um pátio específico por ID. |
| PUT | `/api/v1/Patio/{id}` | Atualiza os dados de um pátio existente. |

---

### 🔹 Perfil (`/api/v1/Perfil`) 🔒

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Perfil` | Retorna todos os perfis cadastrados com paginação. |
| POST | `/api/v1/Perfil` | Cadastra um novo perfil. |
| DELETE | `/api/v1/Perfil/{id}` | Remove um perfil pelo ID. |
| GET | `/api/v1/Perfil/{id}` | Retorna um perfil específico por ID. |
| PUT | `/api/v1/Perfil/{id}` | Atualiza os dados de um perfil existente. |

---

### 🔹 Rastreador (`/api/v1/Rastreador`) 🔒

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/Rastreador` | Retorna todos os rastreadores cadastrados com paginação. |
| POST | `/api/v1/Rastreador` | Cadastra um novo rastreador. |
| DELETE | `/api/v1/Rastreador/{id}` | Remove um rastreador pelo ID. |
| GET | `/api/v1/Rastreador/{id}` | Retorna um rastreador específico por ID. |
| PUT | `/api/v1/Rastreador/{id}` | Atualiza os dados de um rastreador existente. |

---

### 🔹 StatusOperacional (`/api/v1/StatusOperacional`) 🔒

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/StatusOperacional` | Retorna todos os status operacionais cadastrados com paginação. |
| POST | `/api/v1/StatusOperacional` | Cadastra um novo status operacional. |
| DELETE | `/api/v1/StatusOperacional/{id}` | Remove um status operacional pelo ID. |
| GET | `/api/v1/StatusOperacional/{id}` | Retorna um status operacional específico por ID. |
| PUT | `/api/v1/StatusOperacional/{id}` | Atualiza os dados de um status operacional existente. |

---

## 📝 Notas

### 🆕 Novidades Recentes

- **Autenticação JWT implementada** (Outubro 2025)
  - ✅ Registro e login de usuários
  - ✅ Tokens JWT com expiração de 8 horas
  - ✅ Senhas criptografadas com BCrypt
  - ✅ Todos os endpoints protegidos
  - ✅ Campo `perfilId` opcional no registro (perfil padrão automático)

### 🔗 Links Úteis

- **[AUTENTICACAO_JWT.md](AUTENTICACAO_JWT.md)** - Documentação completa sobre autenticação
- **[CHANGELOG_JWT.md](CHANGELOG_JWT.md)** - Histórico de mudanças na autenticação
- **[api/auth-examples.http](api/auth-examples.http)** - Exemplos práticos de requisições HTTP
- **Swagger UI**: `http://localhost:5000/swagger`
- **Health Check**: `http://localhost:5000/health`
- **Health UI**: `http://localhost:5000/health-ui`

---
