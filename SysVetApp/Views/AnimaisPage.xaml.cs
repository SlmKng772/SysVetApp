using SysVetApp.Models;
using SysVetApp.Services;

namespace SysVetApp.Views;

/// <summary>
/// Tela PRINCIPAL da funcionalidade Animais.
/// É a tabela do meio do diagrama: cada animal aponta para uma espécie e um cliente.
/// </summary>
public partial class AnimaisPage : ContentPage
{
    private readonly AnimalService _animais = AnimalService.Instancia;

    private Animal? _selecionado;

    public AnimaisPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await CarregarAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao carregar", ex.Message, "OK");
        }
    }

    private async Task CarregarAsync()
    {
        // ListarComNomesAsync já devolve a espécie e o tutor de cada animal.
        var lista = await _animais.ListarComNomesAsync();

        listaAnimais.ItemsSource = lista;
        listaAnimais.SelectedItem = null;
        _selecionado = null;

        lblSituacao.Text = lista.Count == 0
            ? "Nenhum registro. Toque em INSERIR."
            : $"{lista.Count} registro(s). Toque em um para selecionar.";
    }

    private void OnSelecaoAlterada(object? sender, SelectionChangedEventArgs e)
    {
        _selecionado = e.CurrentSelection.FirstOrDefault() as Animal;

        if (_selecionado is not null)
            lblSituacao.Text = $"Selecionado: {_selecionado.AniNome}";
    }

    // ============================ CREATE ============================
    private async void OnInserirClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("animalform");

    // ============================ UPDATE ============================
    private async void OnAlterarClicked(object? sender, EventArgs e)
    {
        if (_selecionado is null)
        {
            await DisplayAlertAsync("Atenção", "Selecione um animal na lista primeiro.", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"animalform?id={_selecionado.AniId}");
    }

    // ============================ DELETE ============================
    private async void OnExcluirClicked(object? sender, EventArgs e)
    {
        if (_selecionado is null)
        {
            await DisplayAlertAsync("Atenção", "Selecione um animal na lista primeiro.", "OK");
            return;
        }

        try
        {
            // O animal é o "lado muitos" do relacionamento, então ele PODE ser
            // excluído: não existe nada apontando para ele.
            bool confirma = await DisplayAlertAsync("Confirmar exclusão",
                $"Excluir o animal \"{_selecionado.AniNome}\"?", "Excluir", "Cancelar");

            if (!confirma)
                return;

            await _animais.ExcluirAsync(_selecionado);
            await CarregarAsync();

            await DisplayAlertAsync("Sucesso", "Animal excluído com sucesso.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao excluir", ex.Message, "OK");
        }
    }
}
