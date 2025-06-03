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
- (Opcional) Docker para container de SQL Server 

---

## 🚀 Instalação & Configuração

1. Clone o repositório

git clone https://github.com/lbsilva44/Fiap.Cloud.Games.git

3. Configure o appsettings.json em Fiap.Cloud.Games.API:

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

---

 Migrações & Banco de Dados

 1.Aplique as migrations:
 
 cd Fiap.Cloud.Games.Infrastructure
 
dotnet ef database update --startup-project ../Fiap.Cloud.Games.API

2.O banco será criado com as tabelas de Identity, Jogos, Promoções, Biblioteca e Logs.

▶️ Como Executar

cd Fiap.Cloud.Games.API

dotnet run

API disponível em https://localhost:7026 (ver launchSettings.json).

---

📡 Endpoints Principais

Usuário & Auth
| Método | Rota                 | Descrição                       | Permissão         |
|--------|----------------------|---------------------------------|-------------------|
| POST   | `/Registrar-se`      | Cadastrar usuário               | Anônimo           |
| POST   | `/Login`             | Autenticar e gerar JWT          | Anônimo           |
| PUT    | `/Alterar-acesso/`   | Alterar role (Admin)            | Bearer(Admin)     |
| GET    | `/Tipo-acessos`      | Listar perfis existentes        | Bearer(Admin)     |
| GET    | `/Lista-Usuarios`    | Listar todos os usuários        | Bearer(Admin)     |
| PUT    | `/Desativar-usuario` | Desativar usuário               | Bearer(Admin)     |
| PUT    | `/Ativar-usuario`    | Reativar usuário                | Bearer(Admin)     |
| POST   | `/Depositar-saldo`   | Depositar saldo para usuário    | Bearer(User/Admin)|
| GET    | `/Detalhe-usuario`   | Detalhe  usuário                | Anônimo           |

Jogos
| Método | Rota                  | Descrição                      | Permissão        |
|--------|-----------------------|--------------------------------|------------------|
| POST   | `/Cadastrar-jogo`     | Cadastrar jogo                 | Bearer(Admin)    |
| PUT    | `/Publicar-jogo`      | Publicar jogo                  | Bearer(Admin)    |
| GET    | `/Listar-jogos`       | Listar  jogos                  | Anônimo          |
| POST   | `/Adquirir-jogo`      | Adquirir jogo                  | Bearer(User/Admin)|
| GET    | `/Biblioteca-jogos-usuario`| Listar biblioteca usuário | Bearer(User/Admin)|

Promoções
| Método | Rota                           | Descrição                       | Permissão         |
|--------|--------------------------------|---------------------------------|-------------------|
| POST   | `/Criar-Promocao`              | Criar promoção                  | Bearer(Admin)     |
| POST   | `/Ativar-Promocao`             | Ativar promoção                 | Bearer(Admin)     |
| POST   | `/Adicinar-Jogo-Promocao`      | Adicionar jogo a promoção       | Bearer(Admin)     |
| POST   | `/Excluir-Promocao`            | Excluir promoção                | Bearer(Admin)     |
| GET    | `/Listar-promocao`             | Lista promoção de jogos         | Anônimo           |
| POST   | `/Adquirir-promocao`           | Adquirir promoção               | Bearer(User/Admin)|

---

📖 Documentação API

Swagger UI: https://localhost:7026/swagger/index.html

ReDoc: https://localhost:7026/docs/index.html

---

🔐 Seed de Usuários
No primeiro startup, são criados automaticamente:

Admin

E-mail: admin@fcg.com

Senha: Admin@123!



Usuário

E-mail: user@fcg.com

Senha: Senha@123!

Use essas credenciais em Login para gerar o Token e utilizar no Authorize no Swagger.

---

✅ Testes
Na raiz da solução, execute:

dotnet test --logger "console;verbosity=detailed"

Testes de Domínio: validações de Usuario e Promocao

Testes de Controller: endpoints de UsuarioController

---
📚 Links Úteis

Board Miro (Event Storming & Diagramas):

https://miro.com/app/board/uXjVIFs8CKc=/

Repositório GitHub:

https://github.com/lbsilva44/Fiap.Cloud.Games

---

Autor: Leonardo Silva
Data de Entrega: 03/06/2025

---
