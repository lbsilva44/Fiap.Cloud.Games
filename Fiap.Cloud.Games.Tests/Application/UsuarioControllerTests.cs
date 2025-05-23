using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
// Ajuste esses namespaces conforme seu projeto:
using Fiap.Cloud.Games.API.Controllers;       // seu UsuarioController
using Fiap.Cloud.Games.Application.DTOs;      // RegistroUsuarioDto, LoginUsuarioDto
using Fiap.Cloud.Games.Application.Interfaces;// IUsuarioService  
using Fiap.Cloud.Games.Domain.Entities;
using Fiap.Cloud.Games.Application.DTOs.Usuario;

namespace Fiap.Cloud.Games.Tests.Application
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioService> _serviceMock;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _serviceMock = new Mock<IUsuarioService>();
            _controller = new UsuarioController(_serviceMock.Object);
        }

        [Fact]
        public async Task Registrar_Deve_retornar_Ok_com_mensagem_sucesso()
        {
            // Arrange
            var dto = new RegistroUsuarioDto
            {
                Nome = "Teste Usuário",
                Email = "teste@exemplo.com",
                Senha = "Senha@123",
                // demais campos se houver
            };

            _serviceMock
                .Setup(s => s.RegistrarUsuario(dto))
                .ReturnsAsync(new Usuario(dto.Nome, dto.Email, dto.Senha));

            // Act
            var result = await _controller.Registrar(dto);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(new { Mensagem = "Usuário cadastrado com sucesso." });

            _serviceMock.Verify(s => s.RegistrarUsuario(dto), Times.Once);
        }

        [Theory]
        [InlineData("lbsilva44@gmail.com", "Le@156487812335")]
        public async Task Login_Deve_retornar_Ok_com_token(string email, string senha)
        {
            // Arrange
            var dto = new LoginUsuarioDto { Email = email, Senha = senha };
            var fakeToken = "jwt-token-123";

            _serviceMock
                .Setup(s => s.LoginUsuario(dto))
                .ReturnsAsync(fakeToken);

            // Act
            var result = await _controller.Login(dto);

            // Assert  
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(new
            {
                Mensagem = "Usuário logado com sucesso.",
                Token = fakeToken
            });

            _serviceMock.Verify(s => s.LoginUsuario(dto), Times.Once);
        }
    }
}
