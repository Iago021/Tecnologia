# Adaptação de ProjetoEscola.zip

Todos os recursos do projeto de referência foram encaixados na assistência técnica. O ZIP continha duas cópias de parte do projeto; os recursos foram consolidados em uma solução.

| Base enviada | Aplicação no Tecnologia |
|---|---|
| `Form1.cs` e menu de cadastros | `PrincipalForm`: menu, áreas do sistema, contadores e troca de usuário |
| `Aluno_cadastrar.cs` | Inclusão de clientes em `ClientesForm` |
| `Aluno_alterar.cs` / `alunoAlterar.cs` | Edição do cliente selecionado em `ClientesForm` |
| `Aluno_visualizar.cs` | Listagem, pesquisa por campo e exportação dos clientes |
| `Professor_cadastrar.cs` | Cadastro de usuário com perfil Técnico |
| `profAlterar.cs` | Alteração do técnico selecionado em `UsuariosForm` |
| `Professor_visualizar.cs` | Listagem, pesquisa e exportação da equipe; filtro por perfil Técnico |
| `Funcionario_cadastrar.cs` | Cadastro de usuário com perfil Atendente |
| `funcionarioAlterar.cs` | Alteração do atendente selecionado em `UsuariosForm` |
| `Funcionario_visualizar.cs` | Listagem, pesquisa e exportação da equipe; filtro por perfil Atendente |
| Exclusão pelo menu e código | Exclusão da linha selecionada, com confirmação e proteção de vínculos |
| Nome, e-mail, telefone, RG, CPF, cidade e nascimento | Campos preservados nos cadastros correspondentes; documentos e nascimento opcionais |
| RA escolar | Código interno do cliente, gerado automaticamente pelo banco |
| `DateTimePicker` | Nascimento e previsão de entrega |
| `DataGridView` | Listas de clientes, equipe, aparelhos, peças, ordens e históricos |
| Pesquisa com `ComboBox` | Seleção de coluna permitida e pesquisa com parâmetros |
| Microsoft Office Interop | `Exportar.cs`, exportando todas as linhas e cabeçalhos visíveis |
| `MySqlConnection` e `MySqlCommand` | `Banco.cs`, eventos dos formulários e operações da ordem |
| `App.config` | Conexão única configurável, sem repetir dados da conexão em cada tela |
| `Program.cs` | Inicialização do aplicativo, login e saída da conta |
| `.csproj`, `.Designer.cs`, `.resx` e recursos | Projeto .NET Framework e formulários editáveis no Designer |
| `packages.config` e pacotes do ZIP | Referências e dependências necessárias, restauradas pelo NuGet; conector MySQL ajustado para o MariaDB |
| `aluno.sql` | `Banco/tecnologia.sql`, com todas as nove tabelas da assistência técnica |

Os diretórios `.vs`, `bin`, `obj`, os executáveis já compilados e os caches não são código-fonte necessário. O Visual Studio os recria. Os pacotes NuGet são restaurados pelas referências do projeto, sem duplicar bibliotecas no GitHub. Os arquivos vazios de configuração e recursos sem uso não viraram telas ou funções extras.

## Ajustes no código de referência

- A concatenação de texto digitado nos comandos SQL foi substituída por parâmetros.
- Conexões, comandos e leitores são liberados com `using`.
- A seleção de código é feita pela tabela, evitando conversões inválidas de campos de texto e exclusão usando o controle em vez de seu valor.
- Os campos RG e RA que apareciam trocados na alteração da versão interna do ZIP foram ajustados para os dados correspondentes do cliente.
- O Excel não é criado junto com o formulário. Ele abre ao exportar, e a última linha real também é incluída.
- Atendentes e técnicos usam a mesma tabela com perfis diferentes, evitando dois cadastros de login para a mesma pessoa.
- Cadastro e alteração compartilham o formulário para reduzir repetição; o método de uso continua sendo selecionar, editar e salvar.

As telas de aparelhos, estoque, login, conta e ordens completam o escopo que já estava no README do Tecnologia. Não foram mantidas telas de escola dentro do sistema de assistência técnica.

## Conector de banco

O `MySql.Data 26.7.0` do ZIP apresentou erro ao ler as collations do MariaDB 10.11 durante a verificação. Foi substituído por **MySqlConnector 2.3.7**, mantendo `MySqlConnection`, `MySqlCommand`, parâmetros e consultas SQL. A diferença visível no código é `using MySqlConnector;`. O Excel Interop foi mantido. As bibliotecas usadas apenas pelo conector anterior foram removidas das referências.
