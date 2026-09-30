# SysVetApp — Sistema Veterinário (.NET MAUI / C#)

Projeto da disciplina **Computação para Dispositivos Móveis**.

**Aluno:** Gabriel Florencio de Souza - **RA:** 2507896

Aplicativo **Android + Windows** com menu lateral (Flyout), barra de abas (TabBar)
e CRUD completo das três tabelas do diagrama:

| Diagrama | Funcionalidade no app |
|---|---|
| `tblspecies` | **Espécies** |
| `tblanimais` | **Animais** (aponta para uma espécie e um cliente) |
| `tblclientes` | **Clientes** (os tutores) |

> **Nesta versão não existe banco de dados.** Os registros ficam em memória,
> com **1 exemplo já cadastrado em cada funcionalidade**. Ao fechar o app, tudo
> volta ao estado inicial. Veja o item 4 para entender (e para trocar por SQLite depois).

---

## 1. Como abrir e rodar

### Passo 1 — Abrir o projeto

1. **Visual Studio 2022** (atualizado) ou **Visual Studio 2026**, com a carga de trabalho
   **Desenvolvimento em .NET Multi-platform App UI** instalada.
   (Se a opção .NET MAUI não aparecer: abra o *Visual Studio Installer* → *Modificar* → marque essa carga de trabalho.)
2. Abra o arquivo **`SysVetApp.slnx`** (duplo clique, ou *Arquivo → Abrir → Projeto/Solução*).
3. **Não é preciso instalar nenhum pacote NuGet** — o projeto não usa banco de dados.

### Passo 2 — Escolher o destino e rodar

Na barra superior do Visual Studio, escolha o destino:

| Destino | Como preparar |
|---|---|
| **Android Emulator** | Já vem com o Visual Studio. A primeira execução demora alguns minutos. |
| **Celular Android** | No celular: *Configurações → Sobre o telefone → toque 7 vezes em "Número da versão"* → volte e ative a **Depuração USB**. Conecte o cabo e escolha o aparelho na lista. |
| **Windows Machine** | Roda como aplicativo de desktop. Ótimo para testar rápido. |

Depois é só apertar **F5**.

> Diferença visual entre as plataformas: no **Android** a barra de abas fica
> **embaixo** e no **Windows** ela aparece **no topo**, logo abaixo do título.
> É o comportamento padrão do Shell do MAUI, não é erro.

### Passo 3 — Colocar seu nome

Abra **`AppShell.xaml`**, procure o `<Shell.FlyoutFooter>` e troque
`SEU NOME AQUI` pelo seu nome (aparece no rodapé do menu lateral).

---

## 2. Como cada exigência do enunciado foi atendida

| Exigência | Onde está no código |
|---|---|
| Tela inicial com Flyout ativado | `AppShell.xaml` → `FlyoutBehavior="Flyout"` |
| Botões de navegação (TabBar) | `AppShell.xaml` → as quatro `<Tab>` dentro do `<FlyoutItem>` |
| Menu com cabeçalho (header) | `AppShell.xaml` → `<Shell.FlyoutHeader>` |
| Menu com rodapé (footer) | `AppShell.xaml` → `<Shell.FlyoutFooter>` |
| Cada opção acessa a tela principal da funcionalidade | `Views/EspeciesPage`, `Views/AnimaisPage`, `Views/ClientesPage` |
| Da tela principal: Inserir, Alterar e Excluir | Os três botões no rodapé de cada tela de listagem |
| Componentização | `Components/CampoTexto`, `CampoSelecao` e `CampoData`, reaproveitados nos três formulários |
| CRUD em cada funcionalidade | `Services/EspecieService`, `AnimalService`, `ClienteService` |
| Relacionamento entre tabelas (FK) | Formulário de Animal escolhe **Espécie** e **Cliente** em listas suspensas |
| Mensagens de sucesso e de erro | `DisplayAlertAsync` em todas as ações (veja o item 6) |

---

## 3. O diagrama e os models

Cada classe em `Models/` é uma tabela do diagrama, com os **mesmos nomes de coluna**
(adaptados para PascalCase):

| Tabela do diagrama | Model | Campos |
|---|---|---|
| `tblspecies` | `Especie` | `EspId` (PK, `espid`), `EspNome` (`espnome varchar(50)`) |
| `tblanimais` | `Animal` | `AniId` (PK, `anid`), `AniNome` (`aninome varchar(50)`), `AniApelido` (`aniapelido varchar(25)`), `AniDataNasc` (`anidatanasc date`), `AnObservacoes` (`anobservacoes varchar(500)`), **`EspId`** (FK `espid`), **`CliId`** (FK `cliid`) |
| `tblclientes` | `Cliente` | `CliId` (PK, `cliid`), `CliNome` (`clinome varchar(50)`), `CliCpf` (`clicpf`), `CliEmail` (`cliemail varchar(100)`), `CliDataCadastro` (`clidatacadastro date`) |

### Duas decisões que valem explicação

**1. O CPF é `string`, não `decimal`.**
O diagrama traz `clicpf decimal(11)`. No app ele é guardado como **texto**, porque
como número um CPF que começa com zero (`012.345.678-90`) perderia o primeiro dígito.
Guardando como texto os 11 dígitos ficam preservados, e o app mostra o CPF
formatado (`123.456.789-01`) na lista.

**2. Campos que existem só para exibir.**
Em `Animal`, as propriedades `EspecieNome` e `ClienteNome` guardam o **nome** da
espécie e do tutor. Em `Especie` e `Cliente`, a propriedade `AnimaisVinculados`
guarda a quantidade de animais ligados. Nenhuma delas é "coluna" do diagrama:
servem para montar os cards da lista e para bloquear exclusões indevidas
(é o mesmo papel do `[Ignore]` num projeto com banco).

`CliDataCadastro` **não é digitada**: o sistema preenche com a data de hoje na
primeira vez que o cliente é salvo.

---

## 4. Sem banco de dados: como o CRUD funciona

Todo o acesso a dados está em `Services/`, e as telas nunca falam com "o banco"
diretamente — elas chamam o service.

```
Models/    → o formato dos dados (espelha as tabelas do diagrama)
Services/  → guarda e devolve os registros (todo o CRUD acontece aqui)
Views/     → as telas (só cuidam do visual e chamam o Service)
Components/→ pedaços de tela reutilizáveis
```

`Services/ServicoMemoria.cs` é a base compartilhada: ela guarda uma `List<T>` e já
implementa `ListarAsync`, `ObterAsync`, `SalvarAsync` e `ExcluirAsync`. Os três
services herdam dela e só dizem qual é a chave primária (`EspId`, `AniId`, `CliId`).

**Como o INSERT e o UPDATE ficam no mesmo método:**

```csharp
int id = ObterId(item);

if (id == 0)                          // registro novo  → INSERT
{
    DefinirId(item, ProximoId);
    ProximoId++;
    Itens.Add(item);
    return Task.CompletedTask;
}

int posicao = Itens.FindIndex(existente => ObterId(existente) == id);
if (posicao < 0) throw new InvalidOperationException("O registro que você tentou alterar não existe mais...");

Itens[posicao] = item;                // registro existente → UPDATE
```

| Operação | Como acontece no app |
|---|---|
| **C**reate | Botão INSERIR → formulário em branco → SALVAR |
| **R**ead | A própria listagem, recarregada no `OnAppearing()` |
| **U**pdate | Seleciona na lista → ALTERAR → formulário preenchido → SALVAR |
| **D**elete | Seleciona na lista → EXCLUIR → confirmação → apaga |

**Dados de exemplo** (criados no construtor de cada service, para as telas nunca
abrirem vazias):

- Espécies: **Canina** e **Felina**
- Clientes: **Maria Souza** — CPF 123.456.789-01
- Animais: **Rex** (Canina, tutor Maria) e **Mimi** (Felina, tutor Maria)

### Quando o professor pedir SQLite

Só a pasta `Services/` muda: troca-se a `List<T>` por `sqlite-net-pcl`
(como no projeto de referência, `DatabaseService`). Nenhuma tela, componente ou
model precisa ser tocado — é exatamente a vantagem da separação em camadas.

---

## 5. Os três componentes reutilizáveis

Em vez de repetir `Label + Entry + Border` em cada campo de cada formulário
(seriam mais de 30 blocos iguais), existe **um** componente para cada tipo de campo.
Isso é feito com `BindableProperty`, que é o que permite usar as propriedades como
atributos no XAML.

```xml
<!-- texto de uma linha -->
<c:CampoTexto x:Name="campoNome"
              Titulo="Nome do animal *"
              Dica="Ex.: Rex"
              TamanhoMaximo="50" />

<!-- texto de várias linhas (observações) -->
<c:CampoTexto x:Name="campoObservacoes"
              Titulo="Observações"
              Dica="Vacinas, alergias, retornos..."
              TamanhoMaximo="500"
              Multilinha="True" />

<!-- lista suspensa (chave estrangeira) -->
<c:CampoSelecao x:Name="campoEspecie"
                Titulo="Espécie *"
                CampoExibicao="EspNome" />

<!-- data -->
<c:CampoData x:Name="campoDataNasc"
             Titulo="Data de nascimento *"
             PermitirFuturo="False" />
```

`CampoData` é o componente que **não existia** no projeto de referência: ele foi
criado porque o diagrama da veterinária tem datas (`anidatanasc` e `clidatacadastro`).
O `PermitirFuturo="False"` impede escolher uma data de nascimento no futuro.

---

## 6. Mensagens de sucesso e de erro

Toda ação responde com uma mensagem. Isso é o que o app mostra em cada situação:

| Quando | Título | Mensagem |
|---|---|---|
| Salvar espécie/cliente/animal com campo vazio | Campo obrigatório | "Informe o nome da espécie/cliente/animal." |
| Texto maior que o `varchar` do diagrama | Nome muito longo | "…no máximo 50 caracteres (espnome varchar(50))." |
| Espécie com nome repetido | Espécie repetida | "Já existe uma espécie cadastrada com o nome "X"." |
| CPF sem 11 números | CPF inválido | "O CPF precisa ter 11 números (você digitou N)." |
| CPF de outro cliente | CPF repetido | "Já existe um cliente cadastrado com este CPF." |
| E-mail fora do formato | E-mail inválido | "Digite um e-mail no formato nome@dominio.com." |
| Data de nascimento no futuro | Data inválida | "A data de nascimento não pode estar no futuro." |
| Animal sem espécie ou tutor escolhido | Campo obrigatório | "Escolha a espécie do animal." / "Escolha o cliente (tutor) do animal." |
| Nada selecionado e tocou em ALTERAR/EXCLUIR | Atenção | "Selecione um(a) … na lista primeiro." |
| Excluir espécie que tem animais | Não é possível excluir | "A espécie "Canina" possui 1 animal(is) cadastrado(s). Exclua ou altere esses animais antes." |
| Excluir cliente que tem animais | Não é possível excluir | "O cliente "Maria Souza" possui 2 animal(is) cadastrado(s)…" |
| Excluir (com confirmação) | Confirmar exclusão | "Excluir a espécie "Ave"?" — botões **Excluir** / **Cancelar** |
| Salvar com sucesso | Sucesso | "Espécie salva com sucesso." / "Animal salvo com sucesso." / "Cliente salvo com sucesso." |
| Excluir com sucesso | Sucesso | "Espécie excluída com sucesso." |
| Cadastrar animal sem ter espécie ou cliente | Falta cadastro | "Cadastre pelo menos uma ESPÉCIE antes de cadastrar um animal." |
| Erro inesperado | Erro ao carregar / Erro ao salvar / Erro ao excluir | A mensagem técnica do erro (com `try/catch`, para o app não fechar sozinho) |

---

## 7. Regras de validação e de integridade

1. **Campos obrigatórios** — os que têm `*` no rótulo.
2. **Limite de caracteres** — cada campo respeita o `varchar(n)` do diagrama
   (o `MaxLength` do componente já bloqueia a digitação, e o service confere de novo).
3. **CPF** — precisa ter 11 números e ser único entre os clientes
   (a comparação ignora pontos e traço).
4. **E-mail** — precisa ter algo antes do `@`, um domínio com ponto depois.
5. **Data de nascimento** — não pode estar no futuro.
6. **Nome de espécie** — não pode repetir.
7. **Integridade referencial** — o app **não deixa** excluir uma espécie ou um
   cliente que ainda tenha animal vinculado, senão sobrariam animais apontando
   para um registro que não existe mais. O animal, por ser o lado "muitos" do
   relacionamento, pode ser excluído normalmente.

---

## 8. Estrutura de pastas

```
SysVetApp.slnx                        ← solução (abra este arquivo)
LEIA-ME.md                            ← este arquivo
.gitignore
SysVetApp/
├─ AppShell.xaml(.cs)                 ← Flyout (header + footer) + TabBar + rotas
├─ App.xaml(.cs)                      ← ponto de entrada do app
├─ MauiProgram.cs                     ← configuração do MAUI e das fontes
├─ SysVetApp.csproj                   ← Android + Windows
├─ Models/
│  ├─ Especie.cs
│  ├─ Animal.cs
│  └─ Cliente.cs
├─ Services/
│  ├─ ServicoMemoria.cs               ← base do "banco em memória"
│  ├─ EspecieService.cs
│  ├─ AnimalService.cs
│  └─ ClienteService.cs
├─ Components/
│  ├─ CampoTexto.xaml(.cs)            ← rótulo + caixa de texto (1 ou várias linhas)
│  ├─ CampoSelecao.xaml(.cs)          ← rótulo + lista suspensa
│  └─ CampoData.xaml(.cs)             ← rótulo + seletor de data
├─ Views/
│  ├─ HomePage.xaml(.cs)              ← tela inicial com os 3 atalhos
│  ├─ EspeciesPage / EspecieFormPage
│  ├─ AnimaisPage / AnimalFormPage
│  └─ ClientesPage / ClienteFormPage
├─ Resources/Styles/Colors.xaml       ← A PALETA DE CORES (mude aqui e o app todo muda)
├─ Resources/                         ← ícone, splash, fontes e estilos
└─ Platforms/                         ← Android e Windows
```

### A paleta de cores

Todas as cores do sistema estão em **`Resources/Styles/Colors.xaml`**. As telas não
têm código de cor nenhum: usam `{StaticResource CorXxx}`. Para trocar o tema do app
inteiro, mexe-se em **um arquivo só**.

| Chave | Onde aparece | Valor |
|---|---|---|
| `CorPrimaria` | títulos, rótulos de campo e botão ALTERAR | `#0E6E62` |
| `CorPrimariaEscura` | cabeçalho do menu lateral | `#0A544A` |
| `CorSucesso` | botões INSERIR e SALVAR | `#149E7C` |
| `CorPerigo` | botão EXCLUIR | `#CC4B57` |
| `CorCancelar` | botão CANCELAR | `#8A95A5` |
| `CorFundo` / `CorCard` | fundo das telas e cartões das listas | `#F2F7F5` / `#FFFFFF` |
| `CorCampoFundo` / `CorBordaCampo` / `CorBorda` | caixas de texto e bordas | `#FBFDFC` / `#C6D8D3` / `#D9E5E1` |
| `CorTexto` / `CorTextoMedio` / `CorTextoSecundario` / `CorTextoFraco` | textos | `#16241F` / `#4E6A63` / `#758D86` / `#A2B4AF` |
| `CorMenuRodape` / `CorMenuTextoSuave` | rodapé e subtítulo do menu lateral | `#EAF2EF` / `#BFE0D8` |

> No Android existe ainda `Platforms/Android/Resources/values/colors.xml`, que define
> a cor da barra do sistema. Os dois arquivos estão com a mesma paleta.

---

## 9. O que já foi verificado

Tudo abaixo foi testado de verdade, rodando o app:

- **Build Windows**: `net10.0-windows10.0.19041.0` → **0 erros, 0 avisos**
- **Build Android**: `net10.0-android` → **0 erros, 0 avisos**
- Navegação: Flyout abre com cabeçalho e rodapé, as 4 abas funcionam, os 3 atalhos da tela inicial funcionam
- Abas **Espécies**, **Animais** e **Clientes** listando os registros de exemplo com os relacionamentos resolvidos (o card do animal mostra o nome da espécie e do tutor)
- Fluxo completo de **INSERIR** → validação de campo obrigatório → sucesso → lista atualizada
- **ALTERAR** sem seleção → mensagem de atenção
- **EXCLUIR** espécie com animal vinculado → mensagem de bloqueio

---

## 10. Problemas comuns

| Situação | Solução |
|---|---|
| `The type or namespace 'SysVetApp' could not be found` | O projeto foi renomeado. O `RootNamespace` e os `namespace` de todos os arquivos precisam ser `SysVetApp` |
| A carga de trabalho .NET MAUI não aparece no Visual Studio | *Visual Studio Installer* → *Modificar* → marque **Desenvolvimento em .NET Multi-platform App UI** |
| Erro de Android SDK / JDK ao compilar para Android | No Visual Studio: *Ferramentas → Opções → Xamarin → Android Settings*, ou instale o Android SDK pelo próprio instalador do VS |
| Emulador muito lento | Ative a virtualização (Hyper-V / WHPX) na BIOS, ou rode direto no celular por USB |
| Quero começar do zero, sem os dados de exemplo | Apague os `Itens.Add(...)` nos construtores de `Services/EspecieService.cs`, `ClienteService.cs` e `AnimalService.cs` |
| A lista ficou vazia e não volta | Feche e abra o app: os dados são reiniciados (é o comportamento esperado, não há banco) |
| `error XARDF7024: Access to the path '...\obj\Debug\net10.0-android\...' is denied` | Sobrou uma pasta com atributo somente-leitura (junction criada pelo SDK do Android) na pasta `obj`. Feche o Visual Studio, **apague a pasta `obj` do projeto** e compile de novo. No Visual Studio também resolve com *Build → Clean Solution* |
| O app não compila porque "o arquivo está em uso" | Feche o app que está rodando (ou o emulador) antes de compilar |

---

## 11. Próximos passos (quando o professor pedir)

1. **SQLite** — trocar a `List<T>` de `ServicoMemoria` por `sqlite-net-pcl`
   (`DatabaseService`, como no projeto de referência). Só a pasta `Services/` muda.
2. **Padrão MVVM** — levar a lógica que está no code-behind das Views para ViewModels.
3. **Busca e filtros** nas listagens.
4. **Máscaras de entrada** para o CPF e validação de dígitos verificadores.
5. **Consumo de API REST** em vez de dados locais.
