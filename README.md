# Tecnologia

Sistema de assistência técnica para celulares, notebooks e computadores, feito em **C# com Windows Forms**, **.NET Framework 4.7.2** e **MySQL/MariaDB**, com o conector **MySqlConnector**. A solução abre no Visual Studio e segue a base do `ProjetoEscola.zip`: formulários, eventos de botões, `DataGridView`, consultas SQL e exportação para Excel.

## Abrir e executar

O visual foi adaptado às capturas do Figma: logo original, fundo branco com degradê verde suave, cartões cinza e botões verdes arredondados. O login e o dashboard foram reorganizados; os outros formulários usam o mesmo tema, mantendo todos os campos e ações. Os detalhes e a conferência no Windows estão em [docs/VISUAL.md](docs/VISUAL.md).

**Verificação desta atualização:** análise de sintaxe e preservação das regras concluída; a nova interface ainda precisa ser compilada e conferida no Windows/Visual Studio. Os testes da versão anterior não validam este ajuste visual.

1. No Windows, instale o Visual Studio com a carga **Desenvolvimento para desktop com .NET** e o **Developer Pack/Targeting Pack do .NET Framework 4.7.2**.
2. Inicie o **MySQL** no XAMPP, ou use uma instalação do MySQL/MariaDB.
3. No phpMyAdmin ou MySQL Workbench, importe **[Banco/tecnologia.sql](Banco/tecnologia.sql)**. Ele cria o banco `tecnologia` e todas as tabelas. Não é necessário importar o SQL da escola.
4. Abra **`Tecnologia.sln`** no Visual Studio. Não é preciso criar um projeto vazio nem copiar os formulários manualmente.
5. Clique com o botão direito na solução e escolha **Restaurar Pacotes NuGet**. A primeira restauração precisa de internet. As versões estão em `Tecnologia/packages.config`.
6. Confira a conexão em **`Tecnologia/App.config`**. O padrão é `127.0.0.1`, porta `3306`, banco `tecnologia`, usuário `root` e senha vazia, como na base do ZIP. Ajuste conforme sua instalação.
7. Se necessário, defina **Tecnologia** como projeto de inicialização e pressione **F5**.
8. Na primeira execução, preencha nome, e-mail e senha e clique em **Criar primeiro atendente**. Depois clique em **Entrar**. A senha deve ter de 8 a 128 caracteres.
9. Em **Atendentes e técnicos**, cadastre a conta do técnico. Use **Sair da conta** para entrar com o outro perfil.

O sistema inicia sem usuários, clientes, peças ou ordens de exemplo. A criação do primeiro atendente só fica disponível enquanto não existir nenhum usuário.

O Microsoft Excel para desktop é necessário **somente para exportar**. As outras funções não dependem dele. Não há necessidade de instalar PHP ou um servidor web para executar os formulários; o XAMPP é usado apenas para disponibilizar o banco e facilitar a importação pelo phpMyAdmin.

## Perfis e funções

| Área | Atendente | Técnico |
|---|---|---|
| Clientes | Cadastrar, pesquisar, alterar, excluir e desativar | Consulta os dados do cliente na ordem |
| Aparelhos | Cadastrar, pesquisar, alterar e excluir | Consulta o aparelho e suas condições na ordem |
| Equipe | Cadastrar e gerenciar atendentes e técnicos | Altera os próprios dados em Minha conta |
| Ordens | Abrir, pesquisar, acompanhar e entregar | Pesquisar, assumir, diagnosticar e concluir |
| Peças | Consulta as peças utilizadas na ordem | Cadastrar, pesquisar, alterar, excluir e movimentar estoque |
| Minha conta | Nome, e-mail, telefone e senha | Nome, e-mail, telefone e senha |
| Exportação | Listas acessíveis ao perfil | Listas acessíveis ao perfil |

Os cadastros têm **Novo / limpar**, **Salvar**, **Editar selecionado**, **Excluir selecionado**, pesquisa por campo e **Exportar Excel**. Para alterar, selecione a linha, clique em **Editar selecionado**, mude os campos e salve. Na equipe, a senha vazia durante a edição mantém a senha existente.

## Fluxo de atendimento

1. O atendente cadastra o cliente e seu aparelho, com tipo, marca, modelo, série/IMEI, cor, acessórios e estado físico.
2. Abre uma ordem com o problema relatado e, se desejar, a previsão de entrega e as observações.
3. O técnico abre a ordem e salva o diagnóstico e o serviço necessário. Isso o define como responsável.
4. Se precisar de peças, cadastra os componentes em **Peças e estoque** e registra a entrada em **Movimentar estoque**.
5. Na ordem, seleciona uma peça compatível e informa a quantidade utilizada. A saída é registrada e o saldo é atualizado automaticamente.
6. Define mão de obra e desconto, atualiza o andamento e, ao terminar, salva o status **Concluída**.
7. O atendente acessa **Aguardando entrega** e registra a entrega ao cliente.

| Status | Significado |
|---|---|
| Aberta | Equipamento recebido, sem diagnóstico salvo |
| Em manutenção | Reparo em andamento |
| Aguardando peça | Serviço aguardando componente |
| Concluída | Conserto finalizado e equipamento pronto para entrega |
| Entregue | Equipamento devolvido ao cliente |

O painel possui atalhos para ordens abertas, serviços em andamento, concluídos e aguardando entrega. **Concluídas** inclui as ordens já entregues; **Em manutenção** inclui as que aguardam peças. O histórico registra cada mudança de status com usuário e data.

## Peças e valores

- A compatibilidade compara **tipo, marca e modelo** da peça com o aparelho. Use `*` na marca ou no modelo para indicar compatibilidade com todos daquele campo. Cada cadastro representa uma combinação; não é uma pesquisa automática em lojas ou na internet.
- A quantidade inicial é zero. Registre uma **Entrada** para adicionar saldo, com quantidade e motivo/fornecedor.
- A **Saída** na tela de estoque atende a ajustes e retiradas avulsas. Peças usadas no reparo devem ser adicionadas pela própria ordem, para ficarem vinculadas ao serviço.
- **Devolver peça selecionada** remove o item da ordem e devolve a quantidade ao estoque, mantendo a movimentação no histórico.
- O estoque não pode ficar negativo. Operações da ordem, movimentações e saldo são salvos juntos por transação.
- O preço unitário fica registrado no momento de uso da peça. Alterar o preço no cadastro não altera serviços anteriores.
- **Total = mão de obra + peças utilizadas − desconto.** O desconto é em reais e não pode superar o total.

## Regras de cadastro e acesso

- O cliente possui nome, telefone, e-mail, CPF, RG, cidade, endereço, nascimento e situação ativa. CPF, RG e nascimento são opcionais. O CPF informado deve ter 11 números; essa validação de formato não consulta a Receita Federal.
- As senhas de usuários são armazenadas com **PBKDF2-SHA256 e salt individual**, nunca em texto normal.
- Campos obrigatórios, e-mail, datas, quantidades e valores são conferidos antes da gravação. Os valores digitados são enviados como parâmetros SQL.
- Somente o técnico responsável pode alterar o reparo. Outro técnico pode consultar a ordem.
- Um aparelho não pode ter duas ordens ainda não entregues.
- Ordens concluídas e entregues ficam bloqueadas para alteração. Apenas o atendente registra a entrega, e somente após a conclusão.
- Registros já utilizados não são excluídos quando isso quebraria o histórico. Clientes, peças e contas podem ser desativados.
- Os dados do aparelho ficam bloqueados após sua primeira ordem para preservar a identificação do equipamento no histórico.
- A própria conta é alterada por **Minha conta**, com confirmação da senha atual. Não é possível excluir ou desativar a própria conta pela gestão da equipe.
- Um técnico com ordens em andamento deve finalizar seus serviços antes de ser desativado ou mudar de perfil.

## Organização do código

| Arquivo ou pasta | Função |
|---|---|
| `Tecnologia.sln` | Solução para abrir no Visual Studio |
| `Tecnologia/Tecnologia.csproj` | Projeto Windows Forms e referências |
| `Tecnologia/Forms/` | Telas, eventos de botões, arquivos `.Designer.cs` e `.resx` |
| `Tecnologia/Program.cs` | Início do sistema e troca de usuário |
| `Tecnologia/Banco.cs` | Conexão, consultas e comandos MySQL |
| `Tecnologia/OperacoesOrdem.cs` | Abertura, diagnóstico, peças, estoque e entrega |
| `Tecnologia/Sessao.cs` | Usuário conectado e verificação do perfil |
| `Tecnologia/Senha.cs` | Proteção e conferência das senhas |
| `Tecnologia/Tela.cs` | Validações e mensagens comuns |
| `Tecnologia/Exportar.cs` | Exportação de tabelas para o Excel |
| `Tecnologia/App.config` | Configuração da conexão |
| `Banco/tecnologia.sql` | Criação do banco completo em um único arquivo |
| [docs/ADAPTACAO.md](docs/ADAPTACAO.md) | Correspondência entre o ZIP e o Tecnologia |
| [docs/VERIFICACAO.md](docs/VERIFICACAO.md) | Verificações feitas e roteiro de uso |

O código usa classes simples, `if`, laços, eventos de botão, `DataTable`, `MySqlConnection` e `MySqlCommand`. Não usa Entity Framework, API, injeção de dependência ou arquitetura em várias camadas. O cadastro e a alteração compartilham o mesmo formulário: código zero insere; um código selecionado atualiza.

Para editar a interface, abra um formulário em **Exibir Designer**. Os construtores sem parâmetros apenas inicializam os controles; a consulta ao banco acontece no evento `Load`.

## Banco e requisitos

As nove tabelas são `usuarios`, `clientes`, `aparelhos`, `ordens_servico`, `diagnosticos`, `pecas`, `ordem_pecas`, `movimentacoes_estoque` e `historico_status`. O SQL é a definição utilizada pelo programa.

O [diagrama original](assets/diagrama-banco.png) foi preservado como referência. Na implementação, o cliente da ordem é obtido pelo aparelho, o próprio ID identifica a ordem e os totais são calculados a partir dos itens. Isso evita manter valores duplicados. Não há armazenamento de senha de desbloqueio dos aparelhos.

| Requisito original | Onde está implementado |
|---|---|
| RF01 — Acesso com e-mail e senha | LoginForm |
| RF02 — Clientes | ClientesForm |
| RF03 — Aparelhos vinculados ao cliente | AparelhosForm |
| RF04 — Abertura com o problema informado | AbrirOrdemForm |
| RF05 — Consulta das ordens | OrdensForm e OrdemForm |
| RF06 — Diagnóstico | OrdemForm |
| RF07 — Peças compatíveis | Lista de peças da OrdemForm |
| RF08 — Peças utilizadas | OrdemForm e tabela ordem_pecas |
| RF09 — Entradas e saídas | MovimentoForm e histórico de estoque |
| RF10 — Andamento do reparo | OrdemForm e histórico de status |
| RF11 — Conclusão | Status Concluída na OrdemForm |
| RF12 — Entrega | Botão Registrar entrega |
| RF13 — Resumo das ordens | PrincipalForm |
| RF14 — Dados e senha do usuário | PerfilForm |

Os requisitos não funcionais são atendidos pela aplicação em C#, banco SQL, proteção das senhas, permissões por perfil, controles Windows Forms e validação antes de salvar.

## Problemas comuns

- **Não conecta:** confira se o MySQL está iniciado e se servidor, porta, usuário e senha em `App.config` correspondem à sua instalação.
- **Tabela ou banco não encontrado:** importe `Banco/tecnologia.sql`. Ele pode ser importado novamente sem apagar registros; não é um script de migração de bancos antigos com outra estrutura.
- **Referências ausentes:** restaure os pacotes NuGet na solução e confira se o .NET Framework 4.7.2 Developer Pack está instalado.
- **Lista de aparelhos vazia:** cadastre o cliente e o aparelho antes de abrir a ordem.
- **Peça não aparece na ordem:** confira tipo, marca, modelo, situação ativa e saldo maior que zero.
- **Exportação não abre:** é necessário Microsoft Excel instalado no Windows; ele só é iniciado ao exportar.

Referências: [Windows Forms e Designer do Visual Studio](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/) e [compatibilidade do MySqlConnector com a API usada na base](https://mysqlconnector.net/tutorials/migrating-from-connector-net/).
