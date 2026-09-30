using SysVetApp.Models;
using SysVetApp.Services;

namespace SysVetApp.Views;

/// <summary>
/// Formulário de Inserir/Alterar Cliente.
/// </summary>
[QueryProperty(nameof(RegistroId), "id")]
public partial class ClienteFormPage : ContentPage
{
    private readonly ClienteService _clientes = ClienteService.Instancia;

    private Cliente _cliente = new();
    private bool _jaCarregou;

    public ClienteFormPage()
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
            if (int.TryParse(_registroId, out int id) && id > 0)
                await CarregarAsync(id);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao carregar", ex.Message, "OK");
        }
    }

    private async Task CarregarAsync(int id)
    {
        var encontrado = await _clientes.ObterAsync(id);

        if (encontrado is null)
        {
            await DisplayAlertAsync("Registro não encontrado",
                "Este cliente não existe mais. Volte para a lista.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        _cliente = encontrado;

        lblTitulo.Text = "Alterar Cliente";
        Title = "Alterar Cliente";

        campoNome.Texto = _cliente.CliNome;
        campoCpf.Texto = _cliente.CliCpf;
        campoEmail.Texto = _cliente.CliEmail;

        lblDataCadastro.Text = $"Data de cadastro: {_cliente.CliDataCadastro:dd/MM/yyyy} " +
                               "(preenchida automaticamente pelo sistema)";
    }

    private async void OnSalvarClicked(object? sender, EventArgs e)
    {
        try
        {
            // ---------- Validação ----------
            var nome = (campoNome.Texto ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nome))
            {
                await DisplayAlertAsync("Campo obrigatório", "Informe o nome do cliente.", "OK");
                return;
            }

            if (nome.Length > 50)
            {
                await DisplayAlertAsync("Nome muito longo",
                    "O nome do cliente deve ter no máximo 50 caracteres (clinome varchar(50)).", "OK");
                return;
            }

            var cpf = (campoCpf.Texto ?? string.Empty).Trim();
            var cpfNumeros = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpfNumeros.Length == 0)
            {
                await DisplayAlertAsync("Campo obrigatório", "Informe o CPF do cliente.", "OK");
                return;
            }

            if (cpfNumeros.Length != 11)
            {
                await DisplayAlertAsync("CPF inválido",
                    $"O CPF precisa ter 11 números (você digitou {cpfNumeros.Length}).\n\n" +
                    "Exemplo: 123.456.789-01", "OK");
                return;
            }

            if (_clientes.CpfJaExiste(cpfNumeros, _cliente.CliId))
            {
                await DisplayAlertAsync("CPF repetido",
                    "Já existe um cliente cadastrado com este CPF.", "OK");
                return;
            }

            var email = (campoEmail.Texto ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                await DisplayAlertAsync("Campo obrigatório", "Informe o e-mail do cliente.", "OK");
                return;
            }

            if (email.Length > 100)
            {
                await DisplayAlertAsync("E-mail muito longo",
                    "O e-mail deve ter no máximo 100 caracteres (cliemail varchar(100)).", "OK");
                return;
            }

            if (!EmailValido(email))
            {
                await DisplayAlertAsync("E-mail inválido",
                    "Digite um e-mail no formato nome@dominio.com.", "OK");
                return;
            }

            // ---------- Move os dados da tela para o objeto ----------
            _cliente.CliNome = nome;
            _cliente.CliCpf = cpfNumeros;      // guardamos só os números
            _cliente.CliEmail = email;

            // clidatacadastro é preenchida pelo sistema, nunca pelo usuário.
            if (_cliente.CliDataCadastro == default)
                _cliente.CliDataCadastro = DateTime.Today;

            // ---------- Grava (INSERT ou UPDATE) ----------
            await _clientes.SalvarAsync(_cliente);

            await DisplayAlertAsync("Sucesso", "Cliente salvo com sucesso.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao salvar", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Validação simples de e-mail: precisa ter algo antes do @, um domínio
    /// depois e um ponto separando o domínio da extensão.
    /// (Não usamos expressão regular para o código ficar fácil de explicar.)
    /// </summary>
    private static bool EmailValido(string email)
    {
        int posicaoArroba = email.IndexOf('@');

        if (posicaoArroba <= 0 || posicaoArroba == email.Length - 1)
            return false;

        var dominio = email[(posicaoArroba + 1)..];

        return dominio.Contains('.')
               && !dominio.StartsWith('.')
               && !dominio.EndsWith('.')
               && !email.Contains(' ');
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
