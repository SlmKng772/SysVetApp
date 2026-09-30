namespace SysVetApp.Views;

/// <summary>
/// Tela inicial. Só apresenta o sistema e leva para as três funcionalidades.
/// </summary>
public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    // "//rota" navega direto para uma aba da TabBar (rota absoluta).
    private async void OnEspeciesClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//especies");

    private async void OnAnimaisClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//animais");

    private async void OnClientesClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//clientes");
}
