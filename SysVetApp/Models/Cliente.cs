namespace SysVetApp.Models;

/// <summary>
/// Cliente (o tutor do animal). Espelha a tabela <c>tblclientes</c> do diagrama.
/// </summary>
public class Cliente
{
    /// <summary>Chave primária — coluna <c>cliid</c>.</summary>
    public int CliId { get; set; }

    /// <summary>Coluna <c>clinome varchar(50) NOT NULL</c>.</summary>
    public string CliNome { get; set; } = string.Empty;

    /// <summary>
    /// Coluna <c>clicpf</c>.
    /// No diagrama está como <c>decimal(11)</c>, mas aqui é <b>string</b> de
    /// propósito: como número, um CPF que começa com zero (ex.: 012.345.678-90)
    /// perderia o primeiro dígito. Guardar como texto preserva os 11 dígitos.
    /// </summary>
    public string CliCpf { get; set; } = string.Empty;

    /// <summary>Coluna <c>cliemail varchar(100) NOT NULL</c>.</summary>
    public string CliEmail { get; set; } = string.Empty;

    /// <summary>Coluna <c>clidatacadastro date</c>. Preenchida sozinha ao salvar.</summary>
    public DateTime CliDataCadastro { get; set; } = DateTime.Today;

    /// <summary>Campo só de exibição, preenchido pela tela de Clientes.</summary>
    public int AnimaisVinculados { get; set; }

    /// <summary>CPF com pontos e traço, para mostrar na lista.</summary>
    public string CpfFormatado => FormatarCpf(CliCpf);

    /// <summary>Data no padrão brasileiro.</summary>
    public string DataCadastroTexto => CliDataCadastro.ToString("dd/MM/yyyy");

    /// <summary>Texto pronto para o card da lista.</summary>
    public string AnimaisTexto =>
        AnimaisVinculados switch
        {
            0 => "Nenhum animal vinculado",
            1 => "1 animal vinculado",
            _ => $"{AnimaisVinculados} animais vinculados"
        };

    /// <summary>
    /// Devolve o CPF no formato 000.000.000-00.
    /// Se não tiver exatamente 11 dígitos, devolve o texto como está
    /// (assim a lista nunca mostra um CPF "quebrado").
    /// </summary>
    public static string FormatarCpf(string? cpf)
    {
        var digitos = new string((cpf ?? string.Empty).Where(char.IsDigit).ToArray());

        if (digitos.Length != 11)
            return cpf ?? string.Empty;

        return $"{digitos[..3]}.{digitos[3..6]}.{digitos[6..9]}-{digitos[9..]}";
    }

    public override string ToString() => CliNome;
}
