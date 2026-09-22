# Interface e contas

A interface continua em C# Windows Forms (.NET Framework 4.7.2), usando o logo, a foto, os cartões cinza, o fundo claro e os botões verdes das imagens fornecidas.

## Organização das telas

| Área | Organização |
|---|---|
| Login | Entrar, mostrar senha, criar conta e recuperar senha |
| Criar conta | Nome, e-mail e confirmação, senha e confirmação |
| Esqueci a senha | Solicitação de código e acesso à redefinição |
| Redefinir senha | Código recebido, nova senha e confirmação |
| Dashboard | Boas-vindas, nova ordem e quatro indicadores |
| Cadastros | Submenu de clientes, aparelhos e equipe |
| Clientes / aparelhos / equipe / peças | Abas Consultar e Cadastrar / editar; campos agrupados por assunto |
| Manutenção | Pesquisa, filtros e tabela de ordens |
| Ordem | Recebimento; diagnóstico e serviço; peças utilizadas; conclusão e histórico |
| Nova ordem | Cliente e aparelho; solicitação de manutenção |
| Movimentação | Peça, tipo de movimento, quantidade e motivo |
| Histórico | Tabela e exportação |
| Conta / Perfil | Dados pessoais e segurança |

As páginas principais abrem dentro da janela central. As janelas de operações pontuais (nova ordem, detalhe da ordem, movimentação e histórico) continuam como diálogos. Ao trocar de página, alterações nos campos geram uma confirmação para evitar a perda acidental de dados ainda não salvos.

Nenhum campo dos cadastros existentes foi removido. Os eventos de negócio e as permissões de atendente/técnico continuam ativos. A alteração de senha atual continua disponível no perfil.

## Criação de conta

A primeira conta criada é um atendente ativo, como no primeiro acesso da versão anterior. Quando já existe qualquer usuário, o cadastro público gera uma conta inativa. Um atendente deve abrir Cadastros > Equipe, editar a conta, escolher o perfil adequado e ativá-la. O formulário público não permite escolher privilégios nem ativar a própria conta.

## Recuperação de senha

Reimporte o mesmo arquivo Banco/tecnologia.sql para adicionar a tabela recuperacao_senha. O script usa CREATE TABLE IF NOT EXISTS e não apaga registros. Nenhuma credencial SMTP foi incluída.

Configure SmtpHost, SmtpPort (padrão 587 com STARTTLS), SmtpFrom e SmtpUser em Tecnologia/App.config. Coloque a senha do provedor na variável de ambiente TECNOLOGIA_SMTPPASSWORD, no Windows que executa o aplicativo. Reinicie o Visual Studio/aplicativo após definir a variável. Use a senha de aplicativo exigida pelo provedor, quando aplicável. Não envie essa senha para o GitHub.

As variáveis TECNOLOGIA_SMTPHOST, TECNOLOGIA_SMTPPORT, TECNOLOGIA_SMTPFROM e TECNOLOGIA_SMTPUSER também substituem os valores do arquivo. O aplicativo não oferece um envio de e-mail próprio: precisa de um servidor SMTP configurado.

O código tem seis dígitos aleatórios, expira em dez minutos, admite cinco tentativas e é consumido em uma transação junto com a alteração da senha. Somente o hash com salt é guardado. Um novo envio invalida o código anterior; o intervalo mínimo é de 60 segundos. A mensagem normal de envio não revela se o e-mail pertence a uma conta ativa. A redefinição não ativa contas inativas.

## Código e edição tela por tela

Cada uma das 15 telas tem seu próprio `NomeDaTela.Designer.cs`, `NomeDaTela.resx` e `NomeDaTela.Eventos.cs`. Cores, fontes, tamanhos, posições, campos, botões, painéis e abas estão declarados no Designer da própria tela. As imagens e o fundo estão no respectivo arquivo de recursos.

No Visual Studio, abra `Tecnologia.sln`, expanda `Forms`, clique com o botão direito em um formulário e escolha **Exibir Designer** (Shift+F7). Selecione um controle e use **Propriedades** (F4) para editar. Faça alterações de aparência pelo Designer, que atualiza o código gerado.

Os arquivos `.Eventos.cs` contêm apenas os eventos locais, como arredondamento e adaptação à largura da janela. A navegação fica em `PrincipalForm.Navegacao.cs` e `LoginForm.Navegacao.cs`. Os arquivos `Tema.cs`, `LayoutTelas.cs` e `AcessoLayout.cs` foram removidos: o programa não aplica um tema global nem recria a interface depois de `InitializeComponent`.

`AlteracoesFormulario.cs` apenas acompanha alterações não salvas; não cria controles e não define cores ou posições. `Contas.cs` continua responsável pelo cadastro e pela recuperação de senha via banco/SMTP.

## Verificações

O workflow .github/workflows/windows.yml restaura os pacotes, compila no Windows e executa tests/VisualSmoke.cs. O teste constrói 15 formulários, confere a conexão dos controles, alterna as abas, verifica campos após redimensionamento e gera imagens no artefato verificacao-windows. Também abre cada formulário no host de design do Windows Forms e verifica formato, salt e comparação dos códigos. Consulte o resultado da execução no GitHub Actions.

O teste não chama os eventos Load que acessam o banco e não envia e-mails. Portanto, ele não substitui a conferência funcional com MySQL e SMTP nem a inspeção visual das capturas.

A etapa adicional `tests/IntegrationSmoke.cs` executa 42 verificações com um MySQL temporário separado, incluindo contas, recuperação, ordens, estoque e regressões de edição. Veja os resultados e limites em [VERIFICACAO.md](VERIFICACAO.md). O envio para um provedor SMTP real e o Excel continuam fora dessa automação.

Antes de usar em produção, confira com um banco de teste: primeira conta, conta pendente e ativação, login de ambos os perfis, código correto/incorreto/expirado/reutilizado, reenvio, troca de senha, cadastros, manutenção, estoque, exportação e escala do Windows em 100%, 125% e 150%.
