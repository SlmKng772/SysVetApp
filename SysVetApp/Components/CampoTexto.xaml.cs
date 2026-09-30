namespace SysVetApp.Components;

/// <summary>
/// Componente reutiliz�vel de entrada de texto (r�tulo + caixa de digita��o).
/// Usado nos formul�rios de Esp�cies, Animais e Clientes.
/// </summary>
public partial class CampoTexto : ContentView
{
    public CampoTexto()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------------
    // BindableProperty � o que permite usar o componente assim no XAML:
    //     <c:CampoTexto Titulo="Nome" Texto="{Binding ...}" />
    // Uma propriedade C# comum n�o funcionaria com Binding.
    // ---------------------------------------------------------------

    public static readonly BindableProperty TituloProperty =
        BindableProperty.Create(nameof(Titulo), typeof(string), typeof(CampoTexto), string.Empty);

    /// <summary>R�tulo exibido acima da caixa de texto.</summary>
    public string Titulo
    {
        get => (string)GetValue(TituloProperty);
        set => SetValue(TituloProperty, value);
    }

    public static readonly BindableProperty TextoProperty =
        BindableProperty.Create(nameof(Texto), typeof(string), typeof(CampoTexto), string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Conte�do digitado. TwoWay = o que o usu�rio digita volta para o c�digo.</summary>
    public string Texto
    {
        get => (string)GetValue(TextoProperty);
        set => SetValue(TextoProperty, value);
    }

    public static readonly BindableProperty DicaProperty =
        BindableProperty.Create(nameof(Dica), typeof(string), typeof(CampoTexto), string.Empty);

    /// <summary>Texto cinza que aparece quando o campo est� vazio (placeholder).</summary>
    public string Dica
    {
        get => (string)GetValue(DicaProperty);
        set => SetValue(DicaProperty, value);
    }

    public static readonly BindableProperty TamanhoMaximoProperty =
        BindableProperty.Create(nameof(TamanhoMaximo), typeof(int), typeof(CampoTexto), 500);

    /// <summary>Limite de caracteres � espelha o varchar(n) do diagrama.</summary>
    public int TamanhoMaximo
    {
        get => (int)GetValue(TamanhoMaximoProperty);
        set => SetValue(TamanhoMaximoProperty, value);
    }

    public static readonly BindableProperty HabilitadoProperty =
        BindableProperty.Create(nameof(Habilitado), typeof(bool), typeof(CampoTexto), true);

    /// <summary>Permite desligar a digita��o de um campo espec�fico.</summary>
    public bool Habilitado
    {
        get => (bool)GetValue(HabilitadoProperty);
        set => SetValue(HabilitadoProperty, value);
    }

    public static readonly BindableProperty MultilinhaProperty =
        BindableProperty.Create(nameof(Multilinha), typeof(bool), typeof(CampoTexto), false,
            propertyChanged: AoMudarMultilinha);

    /// <summary>
    /// True troca a caixa de uma linha por uma de v�rias linhas.
    /// � o caso do campo de observa��es (varchar(500)).
    /// </summary>
    public bool Multilinha
    {
        get => (bool)GetValue(MultilinhaProperty);
        set => SetValue(MultilinhaProperty, value);
    }

    private static void AoMudarMultilinha(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CampoTexto campo) return;

        bool multilinha = newValue is true;

        campo.entrada.IsVisible = !multilinha;
        campo.entradaLonga.IsVisible = multilinha;
    }

    public static readonly BindableProperty TipoTecladoProperty =
        BindableProperty.Create(nameof(TipoTeclado), typeof(string), typeof(CampoTexto), "Texto",
            propertyChanged: AoMudarTeclado);

    /// <summary>
    /// "Texto", "Numerico" ou "Email" � define o teclado que o celular abre.
    /// </summary>
    public string TipoTeclado
    {
        get => (string)GetValue(TipoTecladoProperty);
        set => SetValue(TipoTecladoProperty, value);
    }

    private static void AoMudarTeclado(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CampoTexto campo) return;

        var teclado = (newValue as string)?.ToLowerInvariant() switch
        {
            "numerico" => Keyboard.Numeric,
            "email" => Keyboard.Email,
            "telefone" => Keyboard.Telephone,
            _ => Keyboard.Default
        };

        campo.entrada.Keyboard = teclado;
        campo.entradaLonga.Keyboard = teclado;
    }
}
