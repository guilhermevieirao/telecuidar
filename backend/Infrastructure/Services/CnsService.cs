using Application.DTOs.Cns;
using Application.Interfaces;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de validação de CNS (Cartão Nacional de Saúde)
/// </summary>
public class CnsService : ICnsService
{
    public ValidarCnsResponseDto ValidarCns(string cns)
    {
        if (string.IsNullOrEmpty(cns) || cns.Length != 15)
        {
            return new ValidarCnsResponseDto
            {
                Valido = false,
                Mensagem = "CNS deve ter 15 dígitos."
            };
        }

        // Remove caracteres não numéricos
        cns = new string(cns.Where(char.IsDigit).ToArray());

        if (cns.Length != 15)
        {
            return new ValidarCnsResponseDto
            {
                Valido = false,
                Mensagem = "CNS deve conter apenas números."
            };
        }

        // Validação do CNS
        var primeiroDigito = cns[0];
        bool valido;
        string tipoCns;

        if (primeiroDigito == '1' || primeiroDigito == '2')
        {
            // CNS definitivo
            valido = ValidarCnsDefinitivo(cns);
            tipoCns = "Definitivo";
        }
        else if (primeiroDigito == '7' || primeiroDigito == '8' || primeiroDigito == '9')
        {
            // CNS provisório
            valido = ValidarCnsProvisorio(cns);
            tipoCns = "Provisório";
        }
        else
        {
            return new ValidarCnsResponseDto
            {
                Valido = false,
                Mensagem = "CNS inválido. O primeiro dígito deve ser 1, 2, 7, 8 ou 9."
            };
        }

        return new ValidarCnsResponseDto
        {
            Valido = valido,
            Mensagem = valido ? "CNS válido." : "CNS inválido.",
            TipoCns = valido ? tipoCns : null
        };
    }

    public Task<InfoCnsResponseDto?> BuscarInfoCnsAsync(string cns)
    {
        // Em uma implementação real, aqui seria feita uma consulta à API do DataSUS
        // Por enquanto, retornamos uma resposta indicando que não foi encontrado
        return Task.FromResult<InfoCnsResponseDto?>(new InfoCnsResponseDto
        {
            Encontrado = false,
            Mensagem = "Consulta ao DataSUS não implementada."
        });
    }

    private static bool ValidarCnsDefinitivo(string cns)
    {
        // Algoritmo de validação para CNS definitivo (inicia com 1 ou 2)
        var soma = 0;
        for (var i = 0; i < 11; i++)
        {
            soma += int.Parse(cns[i].ToString()) * (15 - i);
        }

        var resto = soma % 11;
        var dv = 11 - resto;

        if (dv == 11)
        {
            dv = 0;
        }
        else if (dv == 10)
        {
            // Recalcula com ponderação adicional
            soma += 2;
            resto = soma % 11;
            dv = 11 - resto;
            if (dv == 11) dv = 0;
        }

        var cnsSemDv = cns.Substring(0, 11) + dv.ToString("D4");
        return cns == cnsSemDv;
    }

    private static bool ValidarCnsProvisorio(string cns)
    {
        // Algoritmo de validação para CNS provisório (inicia com 7, 8 ou 9)
        var soma = 0;
        for (var i = 0; i < 15; i++)
        {
            soma += int.Parse(cns[i].ToString()) * (15 - i);
        }

        return soma % 11 == 0;
    }
}
