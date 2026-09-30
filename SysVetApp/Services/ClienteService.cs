using SysVetApp.Models;

namespace SysVetApp.Services;

/// <summary>
/// "Tabela" de Clientes (tblclientes) em memória.
/// </summary>
public class ClienteService : ServicoMemoria<Cliente>
{
    /// <summary>Instância única compartilhada por todas as telas.</summary>
    public static ClienteService Instancia { get; } = new();

    private ClienteService()
    {
        // ---------- Registro de exemplo ----------
        Itens.Add(new Cliente
        {
            CliId = ProximoId++,
            CliNome = "Maria Souza",
            CliCpf = "12345678901",
            CliEmail = "maria.souza@email.com",
            CliDataCadastro = DateTime.Today.AddDays(-30)
        });
    }

    protected override int ObterId(Cliente item) => item.CliId;

    protected override void DefinirId(Cliente item, int id) => item.CliId = id;

    protected override string NomeDoRegistro => "cliente";

    /// <summary>
    /// Confere se o CPF já está cadastrado em outro cliente.
    /// Comparamos apenas os números, então "123.456.789-01" e "12345678901"
    /// são tratados como o mesmo CPF.
    /// </summary>
    public bool CpfJaExiste(string cpf, int idIgnorado = 0)
    {
        var procurado = SomenteDigitos(cpf);

        if (procurado.Length == 0)
            return false;

        return Itens.Any(cliente =>
            cliente.CliId != idIgnorado &&
            SomenteDigitos(cliente.CliCpf) == procurado);
    }

    /// <summary>Quantos animais pertencem a este cliente (bloqueia a exclusão).</summary>
    public Task<int> ContarAnimaisAsync(int cliId) =>
        Task.FromResult(AnimalService.Instancia.ContarPorCliente(cliId));

    private static string SomenteDigitos(string? texto) =>
        new((texto ?? string.Empty).Where(char.IsDigit).ToArray());
}
