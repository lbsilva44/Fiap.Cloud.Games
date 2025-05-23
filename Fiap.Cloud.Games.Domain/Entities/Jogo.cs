
using Fiap.Cloud.Games.Domain.Enums;

namespace Fiap.Cloud.Games.Domain.Entities;

public class Jogo
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }
    public TipoJogo Tipo { get; private set; }
    public bool Publicado { get; private set; } = false;
    public bool Ativo { get; private set; } = true;
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;

    protected Jogo() { }

    private readonly List<PromocaoJogo> _promocaoJogos = new();
    public IReadOnlyCollection<PromocaoJogo> PromocaoJogos => _promocaoJogos.AsReadOnly();

    public Jogo(string nome, string descricao, decimal preco, TipoJogo tipo)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        Tipo = tipo;
    }

    public void Publicar() => Publicado = true;
    public void Desativar() => Ativo = false;
}
