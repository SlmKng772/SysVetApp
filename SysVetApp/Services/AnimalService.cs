using SysVetApp.Models;

namespace SysVetApp.Services;

/// <summary>
/// "Tabela" de Animais (tblanimais) em memória.
/// É a tabela do meio do diagrama: tem chave estrangeira para Espécie e para Cliente.
/// </summary>
public class AnimalService : ServicoMemoria<Animal>
{
    /// <summary>Instância única compartilhada por todas as telas.</summary>
    public static AnimalService Instancia { get; } = new();

    private AnimalService()
    {
        // ---------- Registros de exemplo ----------
        // Os animais são amarrados à espécie e ao cliente já cadastrados acima,
        // exatamente como as FKs do diagrama.
        var especie = EspecieService.Instancia.Listar().FirstOrDefault();
        var cliente = ClienteService.Instancia.Listar().FirstOrDefault();

        Itens.Add(new Animal
        {
            AniId = ProximoId++,
            AniNome = "Rex",
            AniApelido = "Rexinho",
            AniDataNasc = DateTime.Today.AddYears(-3).AddMonths(-2),
            AnObservacoes = "Vacinas em dia. Castrado. Alergia a dipirona.",
            EspId = especie?.EspId ?? 0,
            CliId = cliente?.CliId ?? 0
        });

        Itens.Add(new Animal
        {
            AniId = ProximoId++,
            AniNome = "Mimi",
            AniApelido = "Mimizinha",
            AniDataNasc = DateTime.Today.AddMonths(-8),
            AnObservacoes = "Gata resgatada. Precisa de retorno em 30 dias.",
            EspId = EspecieService.Instancia.Listar().Skip(1).FirstOrDefault()?.EspId ?? 0,
            CliId = cliente?.CliId ?? 0
        });
    }

    protected override int ObterId(Animal item) => item.AniId;

    protected override void DefinirId(Animal item, int id) => item.AniId = id;

    protected override string NomeDoRegistro => "animal";

    /// <summary>
    /// Lista os animais já com o NOME da espécie e do tutor preenchidos.
    /// Como esses dois campos são só de exibição, alguém precisa preenchê-los —
    /// é o mesmo papel do atributo [Ignore] no projeto de referência.
    /// </summary>
    public async Task<List<Animal>> ListarComNomesAsync()
    {
        var animais = await ListarAsync();
        var especies = await EspecieService.Instancia.ListarAsync();
        var clientes = await ClienteService.Instancia.ListarAsync();

        foreach (var animal in animais)
        {
            animal.EspecieNome = especies.FirstOrDefault(especie => especie.EspId == animal.EspId)?.EspNome
                                 ?? "(espécie não encontrada)";

            animal.ClienteNome = clientes.FirstOrDefault(cliente => cliente.CliId == animal.CliId)?.CliNome
                                 ?? "(cliente não encontrado)";
        }

        return animais;
    }

    /// <summary>Quantos animais estão ligados a esta espécie (usado pela tela de Espécies).</summary>
    public int ContarPorEspecie(int espId) => Itens.Count(animal => animal.EspId == espId);

    /// <summary>Quantos animais estão ligados a este cliente (usado pela tela de Clientes).</summary>
    public int ContarPorCliente(int cliId) => Itens.Count(animal => animal.CliId == cliId);
}
