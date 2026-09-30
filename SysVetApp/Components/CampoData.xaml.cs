namespace SysVetApp.Components;

/// <summary>
/// Componente reutiliz�vel de data (r�tulo + seletor de data).
/// Usado na data de nascimento do animal.
/// </summary>
public partial class CampoData : ContentView
{
    public CampoData()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TituloProperty =
        BindableProperty.Create(nameof(Titulo), typeof(string), typeof(CampoData), string.Empty);

    /// <summary>R�tulo exibido acima do seletor.</summary>
    public string Titulo
    {
        get => (string)GetValue(TituloProperty);
        set => SetValue(TituloProperty, value);
    }

    public static readonly BindableProperty DataProperty =
        BindableProperty.Create(nameof(Data), typeof(DateTime), typeof(CampoData), DateTime.Today,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>A data escolhida. TwoWay = o valor volta para o c�digo.</summary>
    public DateTime Data
    {
        get => (DateTime)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public static readonly BindableProperty DataMinimaProperty =
        BindableProperty.Create(nameof(DataMinima), typeof(DateTime), typeof(CampoData), new DateTime(1900, 1, 1),
            propertyChanged: AoMudarLimites);

    /// <summary>Data mais antiga que o seletor aceita.</summary>
    public DateTime DataMinima
    {
        get => (DateTime)GetValue(DataMinimaProperty);
        set => SetValue(DataMinimaProperty, value);
    }

    public static readonly BindableProperty DataMaximaProperty =
        BindableProperty.Create(nameof(DataMaxima), typeof(DateTime), typeof(CampoData), new DateTime(2100, 12, 31),
            propertyChanged: AoMudarLimites);

    /// <summary>
    /// Data mais recente que o seletor aceita.
    /// Para data de nascimento, use o padr�o "Hoje" (veja <see cref="PermitirFuturo"/>).
    /// </summary>
    public DateTime DataMaxima
    {
        get => (DateTime)GetValue(DataMaximaProperty);
        set => SetValue(DataMaximaProperty, value);
    }

    public static readonly BindableProperty PermitirFuturoProperty =
        BindableProperty.Create(nameof(PermitirFuturo), typeof(bool), typeof(CampoData), true,
            propertyChanged: AoMudarPermitirFuturo);

    /// <summary>
    /// False impede escolher uma data no futuro � o caso da data de nascimento.
    /// </summary>
    public bool PermitirFuturo
    {
        get => (bool)GetValue(PermitirFuturoProperty);
        set => SetValue(PermitirFuturoProperty, value);
    }

    private static void AoMudarPermitirFuturo(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CampoData campo) return;

        if (newValue is false)
            campo.DataMaxima = DateTime.Today;

        AoMudarLimites(bindable, oldValue, newValue);
    }

    private static void AoMudarLimites(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CampoData campo) return;

        // O DatePicker do MAUI quebra se o m�nimo ficar maior que o m�ximo,
        // ent�o garantimos a ordem antes de aplicar.
        if (campo.DataMinima > campo.DataMaxima)
            return;

        campo.seletorData.MinimumDate = campo.DataMinima;
        campo.seletorData.MaximumDate = campo.DataMaxima;
    }
}
