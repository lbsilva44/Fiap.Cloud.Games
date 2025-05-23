# Fiap.Cloud.Games

Plataforma de venda de jogos digitais e gestão de biblioteca de usuários — MVP da Fase 1 do Tech Challenge FIAP Cloud Games.

---

## 📋 Sumário

- [Descrição](#descrição)  
- [Arquitetura](#arquitetura)  
- [Pré-requisitos](#pré-requisitos)  
- [Instalação & Configuração](#instalação--configuração)  
- [Migrações & Banco de Dados](#migrações--banco-de-dados)  
- [Como Executar](#como-executar)  
- [Endpoints Principais](#endpoints-principais)  
- [Documentação API](#documentação-api)  
- [Seed de Usuários](#seed-de-usuários)  
- [Testes](#testes)  
- [Links Úteis](#links-úteis)  

---

## 📝 Descrição

Este projeto implementa:

- **Cadastro** de usuários com validações de e-mail e senha segura  
- **Autenticação** e **autorização** via JWT com perfis **Usuário** e **Administrador**  
- **CRUD** de jogos e promoções  
- **Biblioteca** de jogos adquiridos, com débito em carteira  
- **DDD**: Entidades, Value Objects, Domain Events e Policies  
- **Testes** unitários e de controller (xUnit + FluentAssertions + Moq)  
- **Swagger** + **ReDoc** para documentação interativa  

---

## 🏗️ Arquitetura

Fiap.Cloud.Games.sln
├─ Fiap.Cloud.Games.API            ← API / Presentation  
│  ├─ Controllers  
│  └─ Extensions, Middlewares  
├─ Fiap.Cloud.Games.Application    ← Casos de uso, DTOs, Services  
├─ Fiap.Cloud.Games.Domain         ← Entidades, Value Objects, Eventos, Policies  
├─ Fiap.Cloud.Games.Infrastructure ← EF Core DbContext, Migrations, Mappings  
└─ Fiap.Cloud.Games.Tests          ← Testes unitários e de controller  



---

## ⚙️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)  
- SQL Server (ou LocalDB)  
- (Opcional) Docker para container SQL Server  

---

## 🚀 Instalação & Configuração

1. Clone o repositório  
   ```bash
   git clone https://github.com/lbsilva44/Fiap.Cloud.Games.git
   cd Fiap.Cloud.Games

2. Ajuste a connection string e JWT em Fiap.Cloud.Games.API/appsettings.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FiapCloudGamesDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "SuaChaveSuperSecretaAqui",
    "Issuer": "Fiap.Cloud.Games",
    "Audience": "Fiap.Cloud.Games.API",
    "ExpiresInHours": 2
  }
}


 Migrações & Banco de Dados

 1.Aplique as migrations:
 cd Fiap.Cloud.Games.Infrastructure
dotnet ef database update --startup-project ../Fiap.Cloud.Games.API

2.O banco será criado com as tabelas de Identity, Jogos, Promoções, Biblioteca e Logs.

▶️ Como Executar
cd Fiap.Cloud.Games.API
dotnet run
API disponível em https://localhost:7026 (ver launchSettings.json).

📡 Endpoints Principais
Usuário & Auth
| Método | Rota                           | Descrição                       | Permissão        |
|--------|--------------------------------|---------------------------------|------------------|
| POST   | `/api/Usuario/Registrar-se`    | Cadastrar usuário               | Anônimo          |
| POST   | `/api/Usuario/Login`           | Autenticar e gerar JWT          | Anônimo          |
| PUT    | `/api/Usuario/Alterar-acesso/{id}` | Alterar role (Admin)        | Bearer(Admin)    |
| GET    | `/api/Usuario/Tipo-acessos`    | Listar perfis existentes        | Bearer(Admin)    |
| GET    | `/api/Usuario/Lista-Usuarios`  | Listar todos os usuários        | Bearer(Admin)    |
| PUT    | `/api/Usuario/Desativar/{id}`  | Desativar usuário               | Bearer(Admin)    |
| PUT    | `/api/Usuario/Ativar/{id}`     | Reativar usuário                | Bearer(Admin)    |

Jogos
Método	Rota	Descrição	Permissão
POST	/api/Jogos	Cadastrar jogo	Bearer(Admin)
PUT	/api/Jogos/Publicar/{id}	Publicar jogo	Bearer(Admin)
DELETE	/api/Jogos/{id}	Excluir jogo	Bearer(Admin)
GET	/api/Jogos	Listar jogos	Bearer(User/Admin)

Promoções
Método	Rota	Descrição	Permissão
POST	/api/Promocao	Criar promoção	Bearer(Admin)
PUT	/api/Promocao/Ativar/{id}	Ativar promoção	Bearer(Admin)
DELETE	/api/Promocao/{id}	Excluir promoção	Bearer(Admin)
GET	/api/Promocao	Listar ativas	Bearer(User/Admin)

Biblioteca
Método	Rota	Descrição	Permissão
POST	/api/Biblioteca/Adicionar	Adicionar jogo	Bearer(User)
DELETE	/api/Biblioteca/Remover	Remover jogo	Bearer(User)
GET	/api/Biblioteca	Listar biblioteca do usuário	Bearer(User)

📖 Documentação API
Swagger UI: https://localhost:7026/swagger/index.html

ReDoc: https://localhost:7026/docs/index.html

🔐 Seed de Usuários
No primeiro startup, são criados automaticamente:

Admin

E-mail: admin@fcg.com

Senha: Admin@123!

Usuário

E-mail: user@fcg.com

Senha: Senha@123!

Use essas credenciais em Authorize no Swagger.

✅ Testes
Na raiz da solução:

bash
Copiar
Editar
dotnet test --logger "console;verbosity=detailed"
Cobertura:

Testes de Domínio: validações de Usuario e Promocao

Testes de Controller: endpoints de UsuarioController

📚 Links Úteis
Board Miro (Event Storming & Diagramas):
https://miro.com/app/board/uXjVIFs8CKc=/

Repositório GitHub:
https://github.com/lbsilva44/Fiap.Cloud.Games

Documentação DDD & Diagramas:
(inserir link final)

Autor: Leonardo Silva
Data de Entrega: 03/06/2025
