using SysVetApp.Models;
using SysVetApp.Services;

namespace SysVetApp.Views;

/// <summary>
/// Formulário de Inserir/Alterar Espécie.
/// Uma tela só atende as duas operações: se receber um id, está alterando;
/// se não receber, está inserindo.
/// </summary>
[QueryProperty(nameof(RegistroId), "id")]   // captura o "?id=" passado na navegação
public partial class EspecieFormPage : ContentPage
{
    private readonly EspecieService _especies = EspecieService.Instancia;

    private Especie _especie = new();
    private bool _jaCarregou;

    public EspecieFormPage()
    {
        InitializeComponent();
    }

    // ATENÇÃO ao nome desta propriedade: NÃO pode se chamar "Id".
    // Toda página do MAUI já herda uma propriedade "Id" (um Guid interno do
    // framework). Se criarmos outra com o mesmo nome, o Shell se confunde na
    // hora de entregar o "?id=" e o app fecha sozinho.
    //
    // Aqui o valor é apenas GUARDADO. O carregamento fica no OnAppearing,
    // quando a tela já está montada.
    private string _registroId = "0";
    public string RegistroId
    {
        get => _registroId;
        set => _registroId = value;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_jaCarregou) return;   // evita recarregar se a tela reaparecer
        _jaCarregou = true;

        try
        {
            if (int.TryParse(_registroId, out int id) && id > 0)
                await CarregarAsync(id);
        }
        catch (Exception ex)
        {
            // Sem este try/catch, qualquer erro aqui fecharia o app sem explicação.
            await DisplayAlertAsync("Erro ao carregar", ex.Message, "OK");
        }
    }

    private async Task CarregarAsync(int id)
    {
        var encontrada = await _especies.ObterAsync(id);

        if (encontrada is null)
        {
            await DisplayAlertAsync("Registro não encontrado",
                "Esta espécie não existe mais. Volte para a lista.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        _especie = encontrada;

        lblTitulo.Text = "Alterar Espécie";
        Title = "Alterar Espécie";

        campoNome.Texto = _especie.EspNome;
    }

    private async void OnSalvarClicked(object? sender, EventArgs e)
    {
        try
        {
            // ---------- Validação ----------
            var nome = (campoNome.Texto ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nome))
            {
                await DisplayAlertAsync("Campo obrigatório", "Informe o nome da espécie.", "OK");
                return;
            }

            if (nome.Length > 50)
            {
                await DisplayAlertAsync("Nome muito longo",
                    "O nome da espécie deve ter no máximo 50 caracteres (espnome varchar(50)).", "OK");
                return;
            }

            if (_especies.NomeJaExiste(nome, _especie.EspId))
            {
                await DisplayAlertAsync("Espécie repetida",
                    $"Já existe uma espécie cadastrada com o nome \"{nome}\".", "OK");
                return;
            }

            // ---------- Move os dados da tela para o objeto ----------
            _especie.EspNome = nome;

            // ---------- Grava (INSERT ou UPDATE, decidido pelo service) ----------
            await _especies.SalvarAsync(_especie);

            await DisplayAlertAsync("Sucesso", "Espécie salva com sucesso.", "OK");
            await Shell.Current.GoToAsync("..");   // ".." = voltar para a tela anterior
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro ao salvar", ex.Message, "OK");
        }
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
