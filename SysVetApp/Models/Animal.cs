namespace SysVetApp.Models;

/// <summary>
/// Animal atendido na veterinária. Espelha a tabela <c>tblanimais</c> do diagrama,
/// incluindo as duas chaves estrangeiras: <c>espid</c> e <c>cliid</c>.
/// </summary>
public class Animal
{
    /// <summary>Chave primária — coluna <c>anid</c>.</summary>
    public int AniId { get; set; }

    /// <summary>Coluna <c>aninome varchar(50) NOT NULL</c>.</summary>
    public string AniNome { get; set; } = string.Empty;

    /// <summary>Coluna <c>aniapelido varchar(25)</c>.</summary>
    public string AniApelido { get; set; } = string.Empty;

    /// <summary>Coluna <c>anidatanasc date</c>.</summary>
    public DateTime AniDataNasc { get; set; } = DateTime.Today;

    /// <summary>Coluna <c>anobservacoes varchar(500)</c>.</summary>
    public string AnObservacoes { get; set; } = string.Empty;

    /// <summary>Chave estrangeira para a espécie — coluna <c>espid</c>.</summary>
    public int EspId { get; set; }

    /// <summary>Chave estrangeira para o cliente/tutor — coluna <c>cliid</c>.</summary>
    public int CliId { get; set; }

    // ------------------------------------------------------------------
    // Campos SÓ DE EXIBIÇÃO (não viram coluna): guardam os NOMES da espécie
    // e do tutor para a lista não precisar ficar buscando registro por registro.
    // ------------------------------------------------------------------

    /// <summary>Nome da espécie, preenchido pela tela de Animais.</summary>
    public string EspecieNome { get; set; } = string.Empty;

    /// <summary>Nome do tutor, preenchido pela tela de Animais.</summary>
    public string ClienteNome { get; set; } = string.Empty;

    /// <summary>Data de nascimento no padrão brasileiro.</summary>
    public string DataNascTexto => AniDataNasc.ToString("dd/MM/yyyy");

    /// <summary>Apelido e espécie juntos, para o subtítulo do card.</summary>
    public string ApelidoEEspecie =>
        string.IsNullOrWhiteSpace(AniApelido) ? EspecieNome : $"{AniApelido} · {EspecieNome}";

    /// <summary>Idade calculada a partir da data de nascimento.</summary>
    public string IdadeTexto
    {
        get
        {
            if (AniDataNasc == default || AniDataNasc.Date > DateTime.Today)
                return "nascimento não informado";

            var hoje = DateTime.Today;

            // Meses completos entre o nascimento e hoje.
            int meses = ((hoje.Year - AniDataNasc.Year) * 12) + hoje.Month - AniDataNasc.Month;
            if (hoje.Day < AniDataNasc.Day)
                meses--;

            if (meses < 0)
                meses = 0;

            int anos = meses / 12;
            int restoMeses = meses % 12;

            if (anos == 0)
                return restoMeses == 1 ? "1 mês" : $"{restoMeses} meses";

            string textoAnos = anos == 1 ? "1 ano" : $"{anos} anos";

            if (restoMeses == 0)
                return textoAnos;

            return restoMeses == 1 ? $"{textoAnos} e 1 mês" : $"{textoAnos} e {restoMeses} meses";
        }
    }

    public override string ToString() => AniNome;
}
