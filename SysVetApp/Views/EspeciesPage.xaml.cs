using SysVetApp.Models;
using SysVetApp.Services;

namespace SysVetApp.Views;

/// <summary>
/// Tela PRINCIPAL da funcionalidade Espécies.
/// Mostra a listagem (o "R" de Read do CRUD) e dá acesso a Inserir, Alterar e Excluir.
/// </summary>
public partial class EspeciesPage : ContentPage
{
    // Instância única do "banco em memória", compartilhada por todas as telas.
    private readonly EspecieService _especies = EspecieService.Instancia;
    private readonly AnimalService _animais = AnimalService.Instancia;

    private Especie? _selecionada;

    public EspeciesPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// OnAppearing roda toda vez que a tela aparece — inclusive quando
    /// voltamos do formulário. É o que faz a lista atualizar sozinha
    /// depois de inserir ou alterar um registro.
    /// </summary>
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
        var lista = await _especies.ListarAsync();

        // Preenche quantos animais cada espécie tem — aparecia no card e é
        // o que impede excluir uma espécie que ainda está em uso.
        foreach (var especie in lista)
            especie.AnimaisVinculados = await _especies.ContarAnimaisAsync(especie.EspId);

        listaEspecies.ItemsSource = lista;
        listaEspecies.SelectedItem = null;
        _selecionada = null;

        lblSituacao.Text = lista.Count == 0
            ? "Nenhum registro. Toque em INSERIR."
            : $"{lista.Count} registro(s). Toque em um para selecionar.";
    }

    private void OnSelecaoAlterada(object? sender, SelectionChangedEventArgs e)
    {
        _selecionada = e.CurrentSelection.FirstOrDefault() as Especie;

        if (_selecionada is not null)
            lblSituacao.Text = $"Selecionado: {_selecionada.EspNome}";
    }

    // ============================ CREATE ============================
    private async void OnInserirClicked(object? sender, EventArgs e)
    {
        // Sem parâmetro de id = formulário em branco, modo "novo registro".
        await Shell.Current.GoToAsync("especieform");
    }

    // ============================ UPDATE ============================
    private async void OnAlterarClicked(object? sender, EventArgs e)
    {
        if (_selecionada is null)
        {
            await DisplayAlertAsync("Atenção", "Selecione uma espécie na lista primeiro.", "OK");
            return;
        }

        // O "?id=" passa a chave primária para o formulário, que carrega o registro.
        await Shell.Current.GoToAsync($"especieform?id={_selecionada.EspId}");
    }

    // ============================ DELETE ============================
    private async void OnExcluirClicked(object? sender, EventArgs e)
    {
        if (_selecionada is null)
        {
            await DisplayAlertAsync("Atenção", "Selecione uma espécie na lista primeiro.", "OK");
            return;
        }

        try
        {
            // Regra de integridade: não deixa apagar espécie que tem animais vinculados.
            int animaisVinculados = await _especies.ContarAnimaisAsync(_selecionada.EspId);

            if (animaisVinculados > 0)
            {
                await DisplayAlertAsync("Não é possível excluir",
                    $"A espécie \"{_selecionada.EspNome}\" possui {animaisVinculados} animal(is) cadastrado(s). " +
                    "Exclua ou altere esses animais antes.", "OK");
                return;
            }

            bool confirma = await DisplayAlertAsync("Confirmar exclusão",
                $"Excluir a espécie \"{_selecionada.EspNome}\"?", "Excluir", "Cancelar");

            if (!confirma)
                return;

            await _especies.ExcluirAsync(_selecionada);
            await CarregarAsync();

            await DisplayAlertAsync("Sucesso", "Espécie excluída com sucesso.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao excluir", ex.Message, "OK");
        }
    }
}
