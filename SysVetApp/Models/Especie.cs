namespace SysVetApp.Models;

/// <summary>
/// Espécie de animal. Espelha a tabela <c>tblspecies</c> do diagrama do professor.
/// </summary>
public class Especie
{
    /// <summary>Chave primária — coluna <c>espid</c>.</summary>
    public int EspId { get; set; }

    /// <summary>Coluna <c>espnome varchar(50) NOT NULL</c>.</summary>
    public string EspNome { get; set; } = string.Empty;

    /// <summary>
    /// Campo SÓ DE EXIBIÇÃO (não existe no diagrama).
    /// A tela de Espécies preenche com a quantidade de animais que usam esta
    /// espécie — serve para mostrar no card da lista e para bloquear a exclusão
    /// de uma espécie que ainda tem animais vinculados.
    /// </summary>
    public int AnimaisVinculados { get; set; }

    /// <summary>Texto pronto para o card da lista.</summary>
    public string AnimaisTexto =>
        AnimaisVinculados switch
        {
            0 => "Nenhum animal vinculado",
            1 => "1 animal vinculado",
            _ => $"{AnimaisVinculados} animais vinculados"
        };

    public override string ToString() => EspNome;
}
