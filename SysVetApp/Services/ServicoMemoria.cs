namespace SysVetApp.Services;

/// <summary>
/// Base dos nossos "bancos de dados em memória".
///
/// Este projeto NÃO usa banco de dados: os registros ficam numa lista que vive
/// enquanto o app estiver aberto. Ao fechar, tudo volta ao estado inicial.
///
/// A ideia é que, quando o professor pedir SQLite, apenas as classes de
/// <c>Services</c> mudem (trocando a lista por comandos de banco).
/// Nenhuma tela precisa ser tocada — é a vantagem da separação em camadas.
/// </summary>
/// <typeparam name="T">O model que este serviço guarda (Especie, Animal, Cliente...).</typeparam>
public abstract class ServicoMemoria<T> where T : class
{
    /// <summary>A "tabela" em memória.</summary>
    protected readonly List<T> Itens = new();

    /// <summary>Equivale ao AUTO INCREMENT do banco: o próximo id a ser entregue.</summary>
    protected int ProximoId = 1;

    /// <summary>Lê a chave primária do registro.</summary>
    protected abstract int ObterId(T item);

    /// <summary>Grava a chave primária no registro.</summary>
    protected abstract void DefinirId(T item, int id);

    /// <summary>Nome usado nas mensagens de erro (ex.: "espécie").</summary>
    protected abstract string NomeDoRegistro { get; }

    /// <summary>Lista síncrona — usada internamente por outros serviços.</summary>
    public IReadOnlyList<T> Listar() => Itens;

    /// <summary>Lista completa (o "R" de Read). Devolve uma cópia, como faria uma consulta.</summary>
    public Task<List<T>> ListarAsync() => Task.FromResult(Itens.ToList());

    /// <summary>Busca um registro pela chave primária.</summary>
    public Task<T?> ObterAsync(int id) =>
        Task.FromResult(Itens.FirstOrDefault(item => ObterId(item) == id));

    /// <summary>
    /// Grava o registro. Funciona como INSERT e UPDATE ao mesmo tempo:
    /// id zerado = registro novo (INSERT); id preenchido = existente (UPDATE).
    /// </summary>
    public Task SalvarAsync(T item)
    {
        int id = ObterId(item);

        // ---------- INSERT ----------
        if (id == 0)
        {
            DefinirId(item, ProximoId);
            ProximoId++;
            Itens.Add(item);
            return Task.CompletedTask;
        }

        // ---------- UPDATE ----------
        int posicao = Itens.FindIndex(existente => ObterId(existente) == id);

        if (posicao < 0)
            throw new InvalidOperationException(
                $"O registro que você tentou alterar não existe mais. Volte para a lista e tente de novo.");

        Itens[posicao] = item;
        return Task.CompletedTask;
    }

    /// <summary>Apaga o registro (o "D" de Delete).</summary>
    public Task ExcluirAsync(T item)
    {
        Itens.RemoveAll(existente => ObterId(existente) == ObterId(item));
        return Task.CompletedTask;
    }
}
