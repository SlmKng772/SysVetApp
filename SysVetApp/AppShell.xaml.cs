using SysVetApp.Views;

namespace SysVetApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // As telas de formulário (Inserir/Alterar) NÃO ficam no menu nem na TabBar:
        // elas são abertas de dentro da tela principal de cada funcionalidade.
        // Por isso precisam ser registradas aqui como rotas de navegação.
        Routing.RegisterRoute("especieform", typeof(EspecieFormPage));
        Routing.RegisterRoute("animalform", typeof(AnimalFormPage));
        Routing.RegisterRoute("clienteform", typeof(ClienteFormPage));
    }

    private async void OnSobreClicked(object? sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;

        var pagina = Current.CurrentPage;
        if (pagina is null) return;

        await pagina.DisplayAlertAsync(
            "Sobre o sistema",
            "Gabriel Florencio de Souza - RA: 2507896\n\n" +
            "SysVetApp — Sistema Veterinário\n\n" +
            "Aplicativo desenvolvido em .NET MAUI (C#) para a disciplina de " +
            "Computação para Dispositivos Móveis.\n\n" +
            "Funcionalidades: cadastro de Espécies, Animais e Clientes, com as " +
            "operações de Inserir, Alterar e Excluir.\n\n" +
            "Nesta versão os dados ficam em memória (sem banco de dados).",
            "OK");
    }

    private async void OnSairClicked(object? sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;

        var pagina = Current.CurrentPage;
        if (pagina is null) return;

        bool confirma = await pagina.DisplayAlertAsync(
            "Sair", "Deseja realmente encerrar o aplicativo?", "Sim", "Não");

        if (confirma)
            Application.Current?.Quit();
    }
}
