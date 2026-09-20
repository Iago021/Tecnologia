# Visual baseado no Figma

Referência: as seis capturas fornecidas pelo usuário, do arquivo `HlYJGnhEg0f9xVCCszhFSr`. A adaptação continua em C# Windows Forms e .NET Framework 4.7.2. Não foi criado um site nem adicionada biblioteca de interface.

## O que mudou

- Login centralizado, logo, cartão cinza, campos arredondados e botão verde.
- O formulário de primeiro acesso continua disponível somente quando não há usuários. O campo de nome e a opção de mostrar senha foram preservados.
- Dashboard com logo, boas-vindas e a fotografia da referência. A moldura do notebook e as barras do navegador não fazem parte do aplicativo.
- Navegação superior: Dashboard (atualiza os dados), Manutenção (lista as ordens), Peças e Conta / Perfil.
- Os quatro indicadores continuam abrindo suas listas. Clientes, Aparelhos, Atendentes e técnicos, Abrir ordem e Sair da conta continuam disponíveis.
- Tema comum nos 12 formulários: verde, cinza, cartões arredondados, tabelas com cabeçalho verde e foco de teclado visível. A rolagem mantém os campos acessíveis em janelas menores.
- Os textos de campos e botões têm contraste mais forte que nas capturas, para facilitar a leitura.

As telas com mais informações mantêm os campos e tabelas existentes; não foram reduzidas ao formulário simplificado da referência. As permissões por perfil, o banco, os filtros, o estoque e a exportação não foram alterados.

## Limites do escopo

As capturas mostram recuperação de senha por e-mail, código de verificação e cadastro público. A versão atual não tem esses fluxos. Esta alteração visual não implementa envio de e-mail, 2FA ou abertura pública de contas, nem coloca links sem funcionamento. A troca de senha existente permanece em Conta / Perfil, e o cadastro de novos usuários permanece na área de equipe.

## Arquivos

- `Tecnologia/Tema.cs`: cores, cartões, campos, botões, tabelas e carregamento das imagens.
- `Tecnologia/Forms/LoginForm.Visual.cs`: disposição visual do login e primeiro acesso.
- `Tecnologia/Forms/PrincipalForm.Visual.cs`: disposição visual do painel.
- `Tecnologia/Imagens/`: recortes do logo e da foto das imagens fornecidas; incorporados ao executável pelo `.csproj`. Não dependem de URLs ou arquivos externos na instalação.

Os arquivos `.Designer.cs` e seus eventos foram preservados. O tema e os layouts adicionais são aplicados após `InitializeComponent()`. Para ver a apresentação final, execute o projeto; o designer do Visual Studio continua mostrando o layout-base dos controles.

## Verificação realizada nesta alteração

- Análise de sintaxe dos 35 arquivos C# sem erros de parser. Isso não substitui a compilação.
- Referências dos novos arquivos e recursos no `.csproj` conferidas; imagens PNG válidas.
- Comparação de 84 métodos existentes: conteúdo preservado. As mudanças adicionais nos métodos antigos são a montagem visual do menu principal e a visibilidade do cartão de primeiro acesso.
- Arquivos de banco, SQL, senha, sessão, regras de ordens, exportação, inicialização, configuração e dependências sem alterações.
- Eventos dos arquivos Designer preservados e limites dos cartões conferidos estaticamente.
- `git diff --check` sem problemas.

**Pendente:** compilação e execução no Windows. O ambiente usado para esta atualização não dispõe do runtime/compilador Windows Forms; não foi possível abrir as janelas nem realizar comparação visual do resultado executado. Não há alegação de teste funcional ou visual completo desta versão.

## Conferir no Visual Studio

1. Abra `Tecnologia.sln`, restaure os pacotes NuGet e compile a solução.
2. Em um banco de teste vazio, confira o primeiro acesso, o campo Nome, o botão de criação e o login. Em um banco com usuários, confira que o primeiro acesso não aparece.
3. Entre como atendente e como técnico. Verifique os atalhos permitidos e bloqueados, os quatro filtros do dashboard e Sair da conta.
4. Abra todos os cadastros, ordens, movimentações, histórico e perfil. Teste os botões, pesquisa, edição e exportação disponíveis ao perfil.
5. Confira tabulação, Enter no login, foco de teclado e campos somente leitura.
6. Confira janelas menores e escala do Windows em 100%, 125% e 150%, verificando se a rolagem permite alcançar todos os campos.

Não reimporte o SQL nem apague os dados de uma instalação existente apenas para atualizar o visual.
