using SysVetApp.Models;

namespace SysVetApp.Services;

/// <summary>
/// "Tabela" de Espécies (tblspecies) em memória.
/// Instância única (<see cref="Instancia"/>) compartilhada por todas as telas.
/// </summary>
public class EspecieService : ServicoMemoria<Especie>
{
    /// <summary>Instância única, do mesmo jeito que o DatabaseService do projeto de referência.</summary>
    public static EspecieService Instancia { get; } = new();

    private EspecieService()
    {
        // ---------- Registro de exemplo ----------
        // Sem banco de dados, o app começaria com as listas vazias.
        // Este registro inicial deixa as telas com conteúdo desde o primeiro uso.
        Itens.Add(new Especie
        {
            EspId = ProximoId++,
            EspNome = "Canina"
        });

        Itens.Add(new Especie
        {
            EspId = ProximoId++,
            EspNome = "Felina"
        });
    }

    protected override int ObterId(Especie item) => item.EspId;

    protected override void DefinirId(Especie item, int id) => item.EspId = id;

    protected override string NomeDoRegistro => "espécie";

    /// <summary>
    /// Verifica se já existe outra espécie com o mesmo nome.
    /// <paramref name="idIgnorado"/> serve para a alteração não brigar consigo mesma.
    /// </summary>
    public bool NomeJaExiste(string nome, int idIgnorado = 0)
    {
        var procurado = (nome ?? string.Empty).Trim();

        return Itens.Any(especie =>
            especie.EspId != idIgnorado &&
            string.Equals(especie.EspNome.Trim(), procurado, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Quantos animais usam esta espécie (é o que bloqueia a exclusão).</summary>
    public Task<int> ContarAnimaisAsync(int espId) =>
        Task.FromResult(AnimalService.Instancia.ContarPorEspecie(espId));
}
