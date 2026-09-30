using SysVetApp.Models;
using SysVetApp.Services;

namespace SysVetApp.Views;

/// <summary>
/// Formulário de Inserir/Alterar Animal.
/// É o formulário mais completo: além dos campos de texto e da data,
/// tem as duas listas suspensas das chaves estrangeiras (Espécie e Cliente).
/// </summary>
[QueryProperty(nameof(RegistroId), "id")]
public partial class AnimalFormPage : ContentPage
{
    private readonly AnimalService _animais = AnimalService.Instancia;
    private readonly EspecieService _especies = EspecieService.Instancia;
    private readonly ClienteService _clientes = ClienteService.Instancia;

    private Animal _animal = new();
    private bool _jaCarregou;

    public AnimalFormPage()
    {
        InitializeComponent();
    }

    // Não pode se chamar "Id" (o MAUI já tem uma propriedade com esse nome).
    private string _registroId = "0";
    public string RegistroId
    {
        get => _registroId;
        set => _registroId = value;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_jaCarregou) return;
        _jaCarregou = true;

        try
        {
            // 1) Primeiro as listas suspensas: sem espécie e sem cliente
            //    não há como cadastrar um animal.
            if (!await CarregarCombosAsync())
                return;

            // 2) Depois, se veio um id, carrega o registro para alteração.
            if (int.TryParse(_registroId, out int id) && id > 0)
                await CarregarAsync(id);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao carregar", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Preenche as listas suspensas de Espécie e Cliente.
    /// Devolve false quando falta algum cadastro — nesse caso avisa e volta.
    /// </summary>
    private async Task<bool> CarregarCombosAsync()
    {
        var especies = await _especies.ListarAsync();
        var clientes = await _clientes.ListarAsync();

        if (especies.Count == 0)
        {
            await DisplayAlertAsync("Falta cadastro",
                "Cadastre pelo menos uma ESPÉCIE antes de cadastrar um animal.", "OK");
            await Shell.Current.GoToAsync("..");
            return false;
        }

        if (clientes.Count == 0)
        {
            await DisplayAlertAsync("Falta cadastro",
                "Cadastre pelo menos um CLIENTE antes de cadastrar um animal.", "OK");
            await Shell.Current.GoToAsync("..");
            return false;
        }

        campoEspecie.Itens = especies;
        campoCliente.Itens = clientes;

        return true;
    }

    private async Task CarregarAsync(int id)
    {
        var encontrado = await _animais.ObterAsync(id);

        if (encontrado is null)
        {
            await DisplayAlertAsync("Registro não encontrado",
                "Este animal não existe mais. Volte para a lista.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        _animal = encontrado;

        lblTitulo.Text = "Alterar Animal";
        Title = "Alterar Animal";

        campoNome.Texto = _animal.AniNome;
        campoApelido.Texto = _animal.AniApelido;
        campoDataNasc.Data = _animal.AniDataNasc;
        campoObservacoes.Texto = _animal.AnObservacoes;

        // Seleciona nas listas o registro que já estava gravado (as FKs).
        if (campoEspecie.Itens is IEnumerable<Especie> especies)
            campoEspecie.ItemSelecionado = especies.FirstOrDefault(especie => especie.EspId == _animal.EspId);

        if (campoCliente.Itens is IEnumerable<Cliente> clientes)
            campoCliente.ItemSelecionado = clientes.FirstOrDefault(cliente => cliente.CliId == _animal.CliId);
    }

    private async void OnSalvarClicked(object? sender, EventArgs e)
    {
        try
        {
            // ---------- Validação ----------
            var nome = (campoNome.Texto ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nome))
            {
                await DisplayAlertAsync("Campo obrigatório", "Informe o nome do animal.", "OK");
                return;
            }

            if (nome.Length > 50)
            {
                await DisplayAlertAsync("Nome muito longo",
                    "O nome do animal deve ter no máximo 50 caracteres (aninome varchar(50)).", "OK");
                return;
            }

            var apelido = (campoApelido.Texto ?? string.Empty).Trim();

            if (apelido.Length > 25)
            {
                await DisplayAlertAsync("Apelido muito longo",
                    "O apelido deve ter no máximo 25 caracteres (aniapelido varchar(25)).", "OK");
                return;
            }

            if (campoDataNasc.Data.Date > DateTime.Today)
            {
                await DisplayAlertAsync("Data inválida",
                    "A data de nascimento não pode estar no futuro.", "OK");
                return;
            }

            if (campoEspecie.ItemSelecionado is not Especie especie)
            {
                await DisplayAlertAsync("Campo obrigatório", "Escolha a espécie do animal.", "OK");
                return;
            }

            if (campoCliente.ItemSelecionado is not Cliente cliente)
            {
                await DisplayAlertAsync("Campo obrigatório", "Escolha o cliente (tutor) do animal.", "OK");
                return;
            }

            var observacoes = (campoObservacoes.Texto ?? string.Empty).Trim();

            if (observacoes.Length > 500)
            {
                await DisplayAlertAsync("Observações muito longas",
                    "As observações devem ter no máximo 500 caracteres (anobservacoes varchar(500)).", "OK");
                return;
            }

            // ---------- Move os dados da tela para o objeto ----------
            _animal.AniNome = nome;
            _animal.AniApelido = apelido;
            _animal.AniDataNasc = campoDataNasc.Data.Date;
            _animal.AnObservacoes = observacoes;

            // Guardamos o NÚMERO da chave estrangeira, como no banco relacional.
            _animal.EspId = especie.EspId;
            _animal.CliId = cliente.CliId;

            // ---------- Grava (INSERT ou UPDATE) ----------
            await _animais.SalvarAsync(_animal);

            await DisplayAlertAsync("Sucesso", "Animal salvo com sucesso.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao salvar", ex.Message, "OK");
        }
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
