using SysVetApp.Models;
using SysVetApp.Services;

namespace SysVetApp.Views;

/// <summary>
/// Tela PRINCIPAL da funcionalidade Clientes (os tutores dos animais).
/// </summary>
public partial class ClientesPage : ContentPage
{
    private readonly ClienteService _clientes = ClienteService.Instancia;

    private Cliente? _selecionado;

    public ClientesPage()
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
        var lista = await _clientes.ListarAsync();

        // Quantos animais cada cliente tem — mostrado no card e usado
        // para impedir a exclusão de um cliente que ainda tem animal.
        foreach (var cliente in lista)
            cliente.AnimaisVinculados = await _clientes.ContarAnimaisAsync(cliente.CliId);

        listaClientes.ItemsSource = lista;
        listaClientes.SelectedItem = null;
        _selecionado = null;

        lblSituacao.Text = lista.Count == 0
            ? "Nenhum registro. Toque em INSERIR."
            : $"{lista.Count} registro(s). Toque em um para selecionar.";
    }

    private void OnSelecaoAlterada(object? sender, SelectionChangedEventArgs e)
    {
        _selecionado = e.CurrentSelection.FirstOrDefault() as Cliente;

        if (_selecionado is not null)
            lblSituacao.Text = $"Selecionado: {_selecionado.CliNome}";
    }

    // ============================ CREATE ============================
    private async void OnInserirClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("clienteform");

    // ============================ UPDATE ============================
    private async void OnAlterarClicked(object? sender, EventArgs e)
    {
        if (_selecionado is null)
        {
            await DisplayAlertAsync("Atenção", "Selecione um cliente na lista primeiro.", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"clienteform?id={_selecionado.CliId}");
    }

    // ============================ DELETE ============================
    private async void OnExcluirClicked(object? sender, EventArgs e)
    {
        if (_selecionado is null)
        {
            await DisplayAlertAsync("Atenção", "Selecione um cliente na lista primeiro.", "OK");
            return;
        }

        try
        {
            // Regra de integridade: cliente que ainda tem animal não pode sair.
            int animaisVinculados = await _clientes.ContarAnimaisAsync(_selecionado.CliId);

            if (animaisVinculados > 0)
            {
                await DisplayAlertAsync("Não é possível excluir",
                    $"O cliente \"{_selecionado.CliNome}\" possui {animaisVinculados} animal(is) cadastrado(s). " +
                    "Exclua ou altere esses animais antes.", "OK");
                return;
            }

            bool confirma = await DisplayAlertAsync("Confirmar exclusão",
                $"Excluir o cliente \"{_selecionado.CliNome}\"?", "Excluir", "Cancelar");

            if (!confirma)
                return;

            await _clientes.ExcluirAsync(_selecionado);
            await CarregarAsync();

            await DisplayAlertAsync("Sucesso", "Cliente excluído com sucesso.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao excluir", ex.Message, "OK");
        }
    }
}
