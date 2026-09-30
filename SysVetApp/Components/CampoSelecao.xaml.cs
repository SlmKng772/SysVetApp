using System.Collections;

namespace SysVetApp.Components;

/// <summary>
/// Componente reutiliz�vel de sele��o (r�tulo + lista suspensa).
/// Serve para escolher registros relacionados � as chaves estrangeiras do diagrama
/// (o animal escolhe a sua esp�cie e o seu cliente).
/// </summary>
public partial class CampoSelecao : ContentView
{
    public CampoSelecao()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TituloProperty =
        BindableProperty.Create(nameof(Titulo), typeof(string), typeof(CampoSelecao), string.Empty);

    /// <summary>R�tulo exibido acima da lista.</summary>
    public string Titulo
    {
        get => (string)GetValue(TituloProperty);
        set => SetValue(TituloProperty, value);
    }

    public static readonly BindableProperty DicaProperty =
        BindableProperty.Create(nameof(Dica), typeof(string), typeof(CampoSelecao), "Selecione...");

    /// <summary>Texto mostrado enquanto nada foi escolhido.</summary>
    public string Dica
    {
        get => (string)GetValue(DicaProperty);
        set => SetValue(DicaProperty, value);
    }

    public static readonly BindableProperty ItensProperty =
        BindableProperty.Create(nameof(Itens), typeof(IList), typeof(CampoSelecao));

    /// <summary>Lista que preenche as op��es (ex.: todas as esp�cies cadastradas).</summary>
    public IList? Itens
    {
        get => (IList?)GetValue(ItensProperty);
        set => SetValue(ItensProperty, value);
    }

    public static readonly BindableProperty ItemSelecionadoProperty =
        BindableProperty.Create(nameof(ItemSelecionado), typeof(object), typeof(CampoSelecao), null,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>O objeto escolhido pelo usu�rio.</summary>
    public object? ItemSelecionado
    {
        get => GetValue(ItemSelecionadoProperty);
        set => SetValue(ItemSelecionadoProperty, value);
    }

    public static readonly BindableProperty CampoExibicaoProperty =
        BindableProperty.Create(nameof(CampoExibicao), typeof(string), typeof(CampoSelecao), string.Empty,
            propertyChanged: AoMudarCampoExibicao);

    /// <summary>
    /// Nome da propriedade que deve aparecer na lista.
    /// Ex.: CampoExibicao="EspNome" faz a lista mostrar o nome da esp�cie,
    /// mesmo o que fica guardado sendo o objeto Esp�cie inteiro.
    /// </summary>
    public string CampoExibicao
    {
        get => (string)GetValue(CampoExibicaoProperty);
        set => SetValue(CampoExibicaoProperty, value);
    }

    private static void AoMudarCampoExibicao(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CampoSelecao campo) return;

        var propriedade = newValue as string;

        if (!string.IsNullOrWhiteSpace(propriedade))
            campo.seletor.ItemDisplayBinding = new Binding(propriedade);
    }
}
