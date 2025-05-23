

using System.Text.RegularExpressions;

namespace Fiap.Cloud.Games.Domain.ValueObjects
{
    public class Nome
    {
        public string? Valor { get; private set; }

        protected Nome() { }

        public Nome(string valor)
        {
            try
            {
                // Se for nulo, vazio ou só espaços, é inválido
                if (string.IsNullOrWhiteSpace(valor))
                    throw new ArgumentException("Nome inválido.", nameof(valor));

                Valor = valor;
            }
            catch
            {
                throw new ArgumentException("Nome inválido.");
            }

            Valor = valor;
        }

        public override string ToString() => Valor!;

        public static implicit operator string(Nome nome) => nome.Valor!;
    }
}
