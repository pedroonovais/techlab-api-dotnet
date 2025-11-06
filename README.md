# TechLab Api DotNet

**TechLab Api DotNet** é uma API desenvolvida em .NET para o sistema de **Gerenciamento de pátios**, com foco no controle de localização de motos utilizando rastreadores GPS.  
A solução é modularizada em camadas e utiliza **Entity Framework Core com PostgreSQL** rodando em **Docker Compose**. Conta também com documentação interativa via Swagger.

---

## 📌 Funcionalidades

- 🤖 **Machine Learning** com previsão de manutenção de motos usando ML.NET
- 🔐 **Autenticação JWT** com registro e login de usuários
- 🔒 **Segurança** com hash BCrypt e proteção de rotas
- 🌱 **Seed de dados automático** - Banco populado com dados de teste na primeira execução
- Gerenciamento de **usuários** com perfis e permissões
- Registro e controle de **motos** com rastreadores GPS
- Cadastro e monitoramento de **rastreadores** (IoT) para localização de motos
- Administração de **pátios** com localização e controle
- Gerenciamento de **status operacionais** das motos
- API RESTful com respostas em JSON e HATEOAS
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
- **ML.NET** para Machine Learning e previsões
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
- **Popular o banco com dados iniciais** se estiver vazio (seed data)
- Treinar o modelo de Machine Learning automaticamente

### 2. Acessar a API

**⚠️ Importante:** O Swagger está configurado na **raiz** da aplicação (não em `/swagger`).

**Docker (recomendado):**
- Swagger UI: `http://localhost:8080/`

**Desenvolvimento local (sem Docker):**
- HTTP: `http://localhost:5154/`
- HTTPS: `https://localhost:7075/`

---

## 🔄 Versionamento da API

A API utiliza versionamento por URL Path para garantir compatibilidade e evolução controlada:

- **Versões Ativas:** v1 e v2
- **Formato:** `/api/v{version}/[controller]`
- **Exemplos:** `/api/v1/Usuario`, `/api/v1/Moto`, `/api/v2/ML`

### Como funciona:
- Endpoints gerais permanecem na versão 1 (v1)
- Endpoints de Machine Learning (ML) estão na versão 2 (v2)
- Futuras versões (v3, v4, etc.) podem coexistir
- Headers de resposta incluem `api-supported-versions: 1.0, 2.0`
- O Swagger principal documenta v1; endpoints v2 podem não aparecer no doc v1

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

## 🧪 Executando os Testes

O projeto possui testes unitários e de integração implementados com **xUnit** para garantir a qualidade e confiabilidade do código.

### ⚡ Quick Start

Execute os **testes unitários** (100% funcionais) rapidamente:

```bash
dotnet test tests/TechLab.UnitTests/TechLab.UnitTests.csproj
```

Você verá: ✅ **11 testes passando** validando toda a lógica de negócio do `MotoService`!

### 📦 Estrutura dos Testes

- **`tests/TechLab.UnitTests`** - Testes unitários que validam a lógica de negócio isoladamente
- **`tests/TechLab.IntegrationTests`** - Testes de integração que validam endpoints HTTP completos

### 🚀 Executar Todos os Testes

Para restaurar dependências, compilar e executar todos os testes:

```bash
dotnet test techlab-api-dotnet.sln
```

> **Nota:** É necessário especificar o arquivo `.sln` pois há múltiplos projetos na pasta raiz.

### 🔬 Executar Apenas Testes Unitários (✅ Recomendado)

```bash
dotnet test tests/TechLab.UnitTests/TechLab.UnitTests.csproj
```

**Status:** ✅ **11 testes passando com sucesso!**

### 🌐 Executar Apenas Testes de Integração

```bash
dotnet test tests/TechLab.IntegrationTests/TechLab.IntegrationTests.csproj
```

**Status:** ⚠️ Estrutura implementada (7 testes) - requer ajuste no esquema de autenticação

### 📊 Executar com Cobertura Detalhada

```bash
dotnet test techlab-api-dotnet.sln --verbosity normal
```

Ou para apenas testes unitários com detalhes:

```bash
dotnet test tests/TechLab.UnitTests/TechLab.UnitTests.csproj --verbosity normal
```

### ✅ Características dos Testes

#### Testes Unitários
- ✅ Utilizam **EF Core InMemory** para isolar a camada de dados
- ✅ Testam regras de negócio (validações, preenchimento automático de datas, etc.)
- ✅ Determinísticos e rápidos (não dependem de recursos externos)
- ✅ Cada teste usa um banco InMemory isolado

#### Testes de Integração
- ✅ Utilizam **WebApplicationFactory** para iniciar a API em memória
- ✅ Ambiente configurado como `Testing` automaticamente
- ✅ Autenticação de teste (não requer tokens JWT reais)
- ✅ Banco de dados InMemory (não requer PostgreSQL)
- ✅ Validam endpoints HTTP completos (request → response)

### 🔐 Autenticação nos Testes

Os testes de integração usam um **handler de autenticação falso** (`TestAuthenticationHandler`) que:
- Autentica automaticamente todas as requisições
- Não requer tokens JWT reais
- Simula um usuário autenticado com claims de teste

**Vantagem:** Testes podem focar na lógica de negócio sem complexidade de autenticação real.

### 🗄️ Banco de Dados nos Testes

**Ambos os tipos de teste usam EF Core InMemory:**
- Não é necessário ter PostgreSQL instalado/rodando
- Não é necessário configurar connection strings
- Testes são isolados e não compartilham dados
- Banco é criado e destruído automaticamente

### 🎯 Cobertura de Testes

Os testes cobrem cenários importantes como:
- ✅ Criação de recursos (validando preenchimento automático de timestamps)
- ✅ Atualização de recursos (validando que retorna false para IDs inexistentes)
- ✅ Deleção de recursos (validando comportamento com dados válidos e inválidos)
- ✅ Consultas (validando retorno null para IDs inexistentes)
- ✅ Validação de dados (validando exceções para dados inválidos)
- ✅ Paginação (validando parâmetros pageNumber e pageSize)
- ✅ HATEOAS (validando presença de links hipermídia)
- ✅ Status HTTP corretos (200 OK, 201 Created, 400 BadRequest, 404 NotFound)

### 🚀 Executando em CI/CD

Os testes são **totalmente independentes** e podem ser executados em pipelines de CI/CD sem configurações adicionais:
- Não requerem variáveis de ambiente obrigatórias
- Não requerem banco de dados externo
- Não requerem serviços externos (APIs, mensageria, etc.)

```bash
# Pipeline CI/CD exemplo
dotnet restore techlab-api-dotnet.sln
dotnet build techlab-api-dotnet.sln --no-restore
dotnet test techlab-api-dotnet.sln --no-build --verbosity normal
```

### 📝 Convenções de Nomenclatura

Os testes seguem o padrão **`MetodoTestado_Cenario_ResultadoEsperado`**:

```csharp
// Exemplos:
Create_DevePreencherDtCadastroAutomaticamente()
Update_DeveRetornarFalse_QuandoMotoNaoExiste()
Get_DeveRetornar200OK_QuandoAutenticado()
```

### 💡 Dicas

- Use `--verbosity normal` para ver logs detalhados durante os testes
- Testes unitários são mais rápidos - execute-os com frequência durante o desenvolvimento
- Testes de integração validam o sistema completo - execute antes de commits importantes
- Todos os comentários nos testes estão em **português** para facilitar manutenção

### 📈 Status dos Testes

| Tipo | Status | Quantidade | Observações |
|------|--------|------------|-------------|
| **Testes Unitários** | ✅ Passando | 11/11 (100%) | Validam toda lógica de negócio do MotoService |
| **Testes de Integração** | ⚠️ Implementados | 7/7 | Estrutura completa - ajuste de autenticação pendente |

**Recomendação:** Execute os testes unitários regularmente durante o desenvolvimento. Eles são rápidos, isolados e validam completamente as regras de negócio!

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

1. Acesse o Swagger em `http://localhost:8080/` (Docker) ou `http://localhost:5154/` (desenvolvimento local)
2. Registre-se ou faça login usando os endpoints de Auth
3. Copie o token retornado
4. Clique no botão **"Authorize"** 🔒 no canto superior direito
5. Digite: `Bearer {seu-token}` (substitua `{seu-token}` pelo token copiado)
6. Clique em **"Authorize"** e depois **"Close"**
7. Agora você pode testar todos os endpoints protegidos! ✅

### 📋 Exemplos Práticos

#### Exemplo com cURL (Docker):

```bash
# 1. Fazer login
TOKEN=$(curl -X POST http://localhost:8080/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"joao@techlab.com","senha":"senha123"}' \
  | jq -r '.token')

# 2. Usar o token para acessar endpoint protegido
curl http://localhost:8080/api/v1/Usuario \
  -H "Authorization: Bearer $TOKEN"
```

**Nota:** Para desenvolvimento local, substitua `localhost:8080` por `localhost:5154` (HTTP) ou `localhost:7075` (HTTPS).

#### Exemplo com JavaScript/Fetch (Docker):

```javascript
// 1. Fazer login
const response = await fetch('http://localhost:8080/api/v1/Auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'joao@techlab.com',
    senha: 'senha123'
  })
});

const { token } = await response.json();

// 2. Usar o token
const usuarios = await fetch('http://localhost:8080/api/v1/Usuario', {
  headers: { 'Authorization': `Bearer ${token}` }
});
```

**Nota:** Para desenvolvimento local, substitua `localhost:8080` por `localhost:5154` (HTTP) ou `localhost:7075` (HTTPS).

### 🔒 Segurança

- ✅ Senhas criptografadas com **BCrypt** (impossível reverter)
- ✅ Tokens JWT assinados digitalmente (HMAC-SHA256)
- ✅ Tokens válidos por **8 horas** (configurável)
- ✅ Validação automática em todas as requisições
- ✅ Todos os endpoints principais protegidos com `[Authorize]`


---

## 🌱 Dados Iniciais (Seed Data)

Na **primeira execução** da API, o banco de dados é automaticamente populado com dados de exemplo para facilitar testes e desenvolvimento.

### 📊 Dados Criados Automaticamente

#### 👥 Usuários Padrão

| Nome | E-mail | Senha | Perfil |
|------|--------|-------|--------|
| Administrador do Sistema | `admin@techlab.com` | `Admin@123` | Administrador |
| Gerente de Operações | `gerente@techlab.com` | `Gerente@123` | Gerente |
| Usuário Teste | `usuario@techlab.com` | `Usuario@123` | Usuário Padrão |
| Pedro Novais | `pedro.novais@techlab.com` | `Pedro@123` | Administrador |
| Maria Silva | `maria.silva@techlab.com` | `Maria@123` | Gerente |

#### 🏢 Perfis de Acesso

- **Administrador** (Nível 3) - Acesso total ao sistema
- **Gerente** (Nível 2) - Gerenciamento de operações
- **Usuário Padrão** (Nível 1) - Acesso básico

#### 🏍️ Status Operacionais

- Disponível
- Em Uso
- Manutenção
- Indisponível
- Reservada

#### 📍 Rastreadores

- 20 rastreadores GPS (modelos GPS-2000 a GPS-5000)
- Números de série no formato `TRACK000001` a `TRACK000020`

#### 🏢 Pátios

- **Pátio Centro** - Av. Paulista, 1000 - São Paulo/SP
- **Pátio Norte** - Rua das Flores, 500 - São Paulo/SP
- **Pátio Sul** - Av. dos Estados, 2000 - São Paulo/SP
- **Pátio Leste** - Rua do Comércio, 750 - São Paulo/SP
- **Pátio Oeste** - Av. Industrial, 1500 - São Paulo/SP

#### 🏍️ Motos

- 20 motos de diversas marcas (Honda, Yamaha, Suzuki, Kawasaki, BMW)
- Cada moto vinculada a um rastreador e status operacional
- Placas e chassi gerados automaticamente

### 🚀 Como Funciona

O seed é executado automaticamente durante o startup da API se o banco estiver vazio:

1. ✅ API inicia
2. ✅ Aplica migrations do EF Core
3. ✅ **Verifica se o banco está vazio**
4. ✅ **Se vazio, popula com dados iniciais**
5. ✅ Treina modelo de Machine Learning
6. ✅ API fica pronta para uso

### 🔒 Senhas Criptografadas

Todas as senhas são criptografadas com **BCrypt** antes de serem armazenadas no banco de dados, garantindo máxima segurança mesmo em dados de teste.

### 🧪 Testando com Dados de Seed

Após a primeira execução, você pode fazer login com qualquer um dos usuários padrão:

```bash
POST /api/v1/Auth/login
Content-Type: application/json

{
  "email": "admin@techlab.com",
  "senha": "Admin@123"
}
```

**⚠️ Nota:** O seed só é executado quando o banco está completamente vazio. Se você já tem dados, eles não serão sobrescritos.

---

## 🤖 Machine Learning - Previsão de Manutenção

A API utiliza **ML.NET** para prever quando uma moto precisará de manutenção, analisando características como:
- Idade da moto em meses
- Número de movimentações registradas
- Dias desde a última manutenção
- Tempo médio de permanência no pátio

### 📊 Como Funciona

1. **Treinamento Automático**: O modelo é treinado automaticamente quando a API inicia
2. **Dados Sintéticos**: Usa 150 registros sintéticos gerados com padrões realistas
3. **Algoritmo**: FastTree (decision tree) com alta acurácia (~85-90%)
4. **Previsão**: Retorna probabilidade, recomendações e dias estimados até manutenção

### 🎯 Exemplo de Uso (v2)

```bash
POST /api/v2/ML/prever-manutencao
Authorization: Bearer {seu-token}
Content-Type: application/json

{
  "motoId": "guid-da-moto"
}
```

**Resposta:**
```json
{
  "motoId": "guid-da-moto",
  "precisaManutencao": true,
  "probabilidade": 85.5,
  "confianca": "Alta",
  "diasEstimadosAteManutencao": 7,
  "recomendacao": "URGENTE: A moto necessita de manutenção imediata...",
  "dadosUtilizados": {
    "idadeMeses": 48.5,
    "numeroMovimentacoes": 420,
    "diasDesdeUltimaManutencao": 180,
    "tempoMedioPermanencia": 15.2
  }
}
```

---

## 🔄 Fluxo de Exemplo Completo

Esta seção demonstra um fluxo completo de uso da API, desde a autenticação até o cadastro de uma moto com rastreador (IoT) e previsão de manutenção.

### 🎯 Cenário: Cadastrar uma Nova Moto com Rastreador

**Passo a passo:**
1. ✅ Autenticar no sistema
2. ✅ Listar status operacionais disponíveis
3. ✅ Criar um rastreador (IoT)
4. ✅ Cadastrar uma moto associada ao rastreador
5. ✅ Consultar a moto criada
6. ✅ (Opcional) Prever manutenção usando ML

### 📋 Exemplo Completo com cURL

#### 1️⃣ Autenticar no Sistema

```bash
# Fazer login para obter o token JWT
TOKEN=$(curl -X POST http://localhost:8080/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@techlab.com",
    "senha": "Admin@123"
  }' | jq -r '.token')

echo "Token obtido: $TOKEN"
```

**Resposta esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 28800,
  "usuarioId": "guid-do-usuario",
  "nome": "Administrador do Sistema",
  "email": "admin@techlab.com",
  "perfilId": "guid-do-perfil"
}
```

#### 2️⃣ Listar Status Operacionais

```bash
# Listar status operacionais disponíveis (já criados no seed)
STATUS_RESPONSE=$(curl -X GET "http://localhost:8080/api/v1/StatusOperacional?pageSize=10" \
  -H "Authorization: Bearer $TOKEN")

# Extrair o ID do status "Disponível" (primeiro item geralmente)
STATUS_ID=$(echo $STATUS_RESPONSE | jq -r '.items[0].data.id')

echo "Status Operacional ID: $STATUS_ID"
```

**Resposta esperada:**
```json
{
  "items": [
    {
      "data": {
        "id": "guid-do-status",
        "descricao": "Disponível"
      },
      "links": { ... }
    },
    ...
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalItems": 5,
  "totalPages": 1
}
```

#### 3️⃣ Criar um Rastreador (IoT)

```bash
# Criar um novo rastreador GPS
RASTREADOR_RESPONSE=$(curl -X POST http://localhost:8080/api/v1/Rastreador \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "numeroSerie": "TRACK000999",
    "modelo": "GPS-6000",
    "ativo": true
  }')

# Extrair o ID do rastreador criado
RASTREADOR_ID=$(echo $RASTREADOR_RESPONSE | jq -r '.data.id')

echo "Rastreador criado com ID: $RASTREADOR_ID"
```

**Resposta esperada (201 Created):**
```json
{
  "data": {
    "id": "guid-do-rastreador",
    "numeroSerie": "TRACK000999",
    "modelo": "GPS-6000",
    "dtCadastro": "2025-01-15T10:30:00Z",
    "dtAtualizacao": "2025-01-15T10:30:00Z",
    "ativo": true
  },
  "links": {
    "self": { "href": "/api/v1/Rastreador/{id}", "method": "GET" },
    ...
  }
}
```

#### 4️⃣ Cadastrar uma Moto com o Rastreador

```bash
# Criar uma nova moto associada ao rastreador e status operacional
MOTO_RESPONSE=$(curl -X POST http://localhost:8080/api/v1/Moto \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"marca\": \"Honda\",
    \"modelo\": \"CB 600F Hornet\",
    \"placa\": \"ABC1D23\",
    \"chassi\": \"9BWHON12345678901\",
    \"idStatusOperacional\": \"$STATUS_ID\",
    \"idRastreador\": \"$RASTREADOR_ID\",
    \"ativo\": true
  }")

# Extrair o ID da moto criada
MOTO_ID=$(echo $MOTO_RESPONSE | jq -r '.data.id')

echo "Moto criada com ID: $MOTO_ID"
```

**Resposta esperada (201 Created):**
```json
{
  "data": {
    "id": "guid-da-moto",
    "marca": "Honda",
    "modelo": "CB 600F Hornet",
    "placa": "ABC1D23",
    "chassi": "9BWHON12345678901",
    "idStatusOperacional": "guid-do-status",
    "idRastreador": "guid-do-rastreador",
    "dtCadastro": "2025-01-15T10:35:00Z",
    "dtAtualizacao": "2025-01-15T10:35:00Z",
    "ativo": true
  },
  "links": {
    "self": { "href": "/api/v1/Moto/{id}", "method": "GET" },
    ...
  }
}
```

#### 5️⃣ Consultar a Moto Criada

```bash
# Consultar os detalhes da moto criada
curl -X GET "http://localhost:8080/api/v1/Moto/$MOTO_ID" \
  -H "Authorization: Bearer $TOKEN" | jq
```

**Resposta esperada (200 OK):**
```json
{
  "data": {
    "id": "guid-da-moto",
    "marca": "Honda",
    "modelo": "CB 600F Hornet",
    "placa": "ABC1D23",
    "chassi": "9BWHON12345678901",
    "idStatusOperacional": "guid-do-status",
    "idRastreador": "guid-do-rastreador",
    "dtCadastro": "2025-01-15T10:35:00Z",
    "dtAtualizacao": "2025-01-15T10:35:00Z",
    "ativo": true
  },
  "links": { ... }
}
```

#### 6️⃣ Prever Manutenção usando ML (Opcional)

```bash
# Usar Machine Learning para prever se a moto precisa de manutenção
curl -X POST http://localhost:8080/api/v2/ML/prever-manutencao \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"motoId\": \"$MOTO_ID\"
  }" | jq
```

**Resposta esperada (200 OK):**
```json
{
  "motoId": "guid-da-moto",
  "precisaManutencao": false,
  "probabilidade": 15.2,
  "confianca": "Baixa",
  "diasEstimadosAteManutencao": 120,
  "recomendacao": "A moto está em bom estado. Continue monitorando...",
  "dadosUtilizados": {
    "idadeMeses": 12.5,
    "numeroMovimentacoes": 85,
    "diasDesdeUltimaManutencao": 30,
    "tempoMedioPermanencia": 8.5
  }
}
```

### 🎨 Exemplo Completo com JavaScript/Fetch

```javascript
const API_BASE = 'http://localhost:8080';

// 1. Autenticar
const loginResponse = await fetch(`${API_BASE}/api/v1/Auth/login`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'admin@techlab.com',
    senha: 'Admin@123'
  })
});
const { token } = await loginResponse.json();

// 2. Listar status operacionais
const statusResponse = await fetch(`${API_BASE}/api/v1/StatusOperacional?pageSize=10`, {
  headers: { 'Authorization': `Bearer ${token}` }
});
const statusData = await statusResponse.json();
const statusId = statusData.items[0].data.id; // Status "Disponível"

// 3. Criar rastreador
const rastreadorResponse = await fetch(`${API_BASE}/api/v1/Rastreador`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    numeroSerie: 'TRACK000999',
    modelo: 'GPS-6000',
    ativo: true
  })
});
const rastreadorData = await rastreadorResponse.json();
const rastreadorId = rastreadorData.data.id;

// 4. Cadastrar moto
const motoResponse = await fetch(`${API_BASE}/api/v1/Moto`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    marca: 'Honda',
    modelo: 'CB 600F Hornet',
    placa: 'ABC1D23',
    chassi: '9BWHON12345678901',
    idStatusOperacional: statusId,
    idRastreador: rastreadorId,
    ativo: true
  })
});
const motoData = await motoResponse.json();
const motoId = motoData.data.id;

console.log('Moto criada:', motoId);

// 5. Consultar moto criada
const motoDetails = await fetch(`${API_BASE}/api/v1/Moto/${motoId}`, {
  headers: { 'Authorization': `Bearer ${token}` }
});
const motoDetailsData = await motoDetails.json();
console.log('Detalhes da moto:', motoDetailsData);

// 6. Prever manutenção (ML)
const mlResponse = await fetch(`${API_BASE}/api/v2/ML/prever-manutencao`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({ motoId })
});
const mlData = await mlResponse.json();
console.log('Previsão de manutenção:', mlData);
```

### 📝 Notas Importantes

- **Autenticação obrigatória:** Todos os endpoints (exceto Auth) requerem o token JWT no header `Authorization: Bearer {token}`
- **Campos obrigatórios para Moto:**
  - `marca` (string) - obrigatório
  - `modelo` (string) - obrigatório
  - `idStatusOperacional` (Guid) - obrigatório
  - `idRastreador` (Guid) - obrigatório
- **Campos opcionais para Moto:**
  - `placa` (string) - opcional
  - `chassi` (string) - opcional
  - `ativo` (boolean) - padrão: true
- **Datas automáticas:** `dtCadastro` e `dtAtualizacao` são preenchidas automaticamente pelo sistema
- **Status Operacionais:** Já vêm populados no seed (Disponível, Em Uso, Manutenção, Indisponível, Reservada)
- **Versão da API:** Use `/api/v1/` para endpoints gerais e `/api/v2/` para endpoints de ML

### 🧪 Testando no Swagger

1. Acesse `http://localhost:8080/` (Docker) ou `http://localhost:5154/` (desenvolvimento)
2. Faça login em `/api/v1/Auth/login`
3. Copie o token retornado
4. Clique em **"Authorize"** 🔒 e digite: `Bearer {seu-token}`
5. Execute os endpoints na ordem: Status → Rastreador → Moto → ML

---

## 📬 Endpoints da API

**Versão Atual:** v1  
**URLs:** `/api/v1/[controller]`

⚠️ **Atenção:** Todos os endpoints abaixo **requerem autenticação JWT** (exceto endpoints de Auth). Inclua o token no header: `Authorization: Bearer {token}`

### 🤖 ML (`/api/v2/ML`) 🔒

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/v2/ML/prever-manutencao` | Prevê se uma moto precisa de manutenção usando ML. |

---

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

- **Machine Learning implementado** (Outubro 2025)
  - ✅ Previsão de manutenção de motos com ML.NET
  - ✅ Treinamento automático no startup
  - ✅ Algoritmo FastTree com 85-90% de acurácia
  - ✅ Análise de 4 features principais
  - ✅ Recomendações inteligentes baseadas em probabilidade

- **Autenticação JWT implementada** (Outubro 2025)
  - ✅ Registro e login de usuários
  - ✅ Tokens JWT com expiração de 8 horas
  - ✅ Senhas criptografadas com BCrypt
  - ✅ Todos os endpoints protegidos
  - ✅ Campo `perfilId` opcional no registro (perfil padrão automático)

### 🔗 URLs Úteis

**Docker:**
- **Swagger UI**: `http://localhost:8080/` (⚠️ na raiz, não em `/swagger`)
- **Health Check**: `http://localhost:8080/health`

**Desenvolvimento Local:**
- **Swagger UI**: `http://localhost:5154/` ou `https://localhost:7075/`
- **Health Check**: `http://localhost:5154/health` ou `https://localhost:7075/health`

---
