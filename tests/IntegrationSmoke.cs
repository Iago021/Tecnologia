using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MySqlConnector;
using Tecnologia;

internal static class IntegrationSmoke
{
    private static int checks;
    private const string Password = "Teste-123456";
    [STAThread]
    private static int Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["Tecnologia"].ConnectionString;
        var builder = new MySqlConnectionStringBuilder(connection);
        if (builder.Server != "127.0.0.1" || builder.Port != 3307 || builder.Database != "tecnologia_verificacao")
            throw new Exception("O teste só pode acessar o banco local isolado de verificação.");
        builder.Database = "";
        string schema = File.ReadAllText(args[0]).Replace("tecnologia", "tecnologia_verificacao");
        using (var db = new MySqlConnection(builder.ConnectionString))
        {
            db.Open();
            using (var cmd = new MySqlCommand(schema, db)) cmd.ExecuteNonQuery();
        }
        Check(Contas.Criar("Atendente", "admin@example.com", "admin@example.com", Password, Password), "Primeira conta ativa");
        Check(!Contas.Criar("Técnico", "tech@example.com", "tech@example.com", Password, Password), "Segunda conta pendente");
        int admin = Id("admin@example.com"), tech = Id("tech@example.com");
        Check(!Convert.ToBoolean(Banco.Valor("SELECT ativo FROM usuarios WHERE id=@id", Banco.P("@id", tech))), "Sem ativação pública");
        Fails(() => Contas.Criar("Duplicado", "admin@example.com", "admin@example.com", Password, Password), "Cadastro duplicado", "Duplicate");
        Check(Senha.Conferir(Password, Hash(admin)), "Senha armazenada válida");
        Check(!Senha.Conferir("errada", Hash(admin)), "Senha errada negada");
        Login(tech);
        Fails(() => Sessao.Exigir(), "Conta inativa negada", "inativa");
        Fails(() => Contas.Autenticar("tech@example.com", Password), "Login informa conta pendente", "inativa");
        Fails(() => Contas.Autenticar("tech@example.com", "errada"), "Senha errada preserva o estado da conta", "incorretos");

        Login(admin);
        SalvarTecnico(tech, true);
        Check(Convert.ToInt32(Contas.Autenticar("tech@example.com", Password)["id"]) == tech, "Conta pública ativada sem telefone");
        Fails(() => Contas.ExcluirUsuario(admin), "Protege a própria conta", "própria conta");

        Token(admin, false);
        Fails(() => RecuperacaoSenha.Redefinir("admin@example.com", "999999", Password, Password), "Código incorreto", "Código inválido");
        Check(Convert.ToInt32(Banco.Valor("SELECT tentativas FROM recuperacao_senha WHERE usuario_id=@id", Banco.P("@id", admin))) == 1, "Tentativa incorreta registrada");
        for (int i = 0; i < 4; i++) Fails(() => RecuperacaoSenha.Redefinir("admin@example.com", "999999", Password, Password), "Limite de tentativas", "Código inválido");
        Fails(() => RecuperacaoSenha.Redefinir("admin@example.com", "123456", Password, Password), "Código bloqueado após cinco erros", "Código inválido");
        Token(admin, true);
        Fails(() => RecuperacaoSenha.Redefinir("admin@example.com", "123456", Password, Password), "Código expirado", "Código inválido");
        Token(admin, false);
        RecuperacaoSenha.Redefinir("admin@example.com", "123456", "Nova-123456", "Nova-123456");
        Check(Senha.Conferir("Nova-123456", Hash(admin)) && !Senha.Conferir(Password, Hash(admin)), "Redefinição altera a senha");
        Fails(() => RecuperacaoSenha.Redefinir("admin@example.com", "123456", Password, Password), "Código de uso único", "Código inválido");
        Login(admin); Token(admin, false);
        Fails(() => Contas.AtualizarPerfil("Admin", "admin@example.com", "11999999999", "incorreta", Password, Password), "Perfil exige senha atual", "Senha atual incorreta");
        Contas.AtualizarPerfil("Admin", "admin@example.com", "11999999999", "Nova-123456", Password, Password);
        Check(Senha.Conferir(Password, Hash(admin)), "Senha alterada pelo perfil");
        Fails(() => RecuperacaoSenha.Redefinir("admin@example.com", "123456", Password, Password), "Troca de senha invalida código antigo", "Código inválido");
        Token(tech, false);
        string oldHash = Hash(tech);
        SalvarTecnico(tech, true);
        Check(Hash(tech) == oldHash, "Edição sem senha preserva o hash");
        Check(Convert.ToInt32(Banco.Valor("SELECT COUNT(*) FROM recuperacao_senha WHERE usuario_id=@id", Banco.P("@id", tech))) == 0, "Edição administrativa invalida recuperação");
        Environment.SetEnvironmentVariable("TECNOLOGIA_SMTPHOST", "127.0.0.1");
        Environment.SetEnvironmentVariable("TECNOLOGIA_SMTPPORT", "1");
        Environment.SetEnvironmentVariable("TECNOLOGIA_SMTPFROM", "teste@example.com");
        Fails(() => RecuperacaoSenha.SolicitarCodigo("admin@example.com"), "Falha SMTP tratada", "Não foi possível enviar");
        Check(Convert.ToInt32(Banco.Valor("SELECT COUNT(*) FROM recuperacao_senha WHERE usuario_id=@id", Banco.P("@id", admin))) == 0, "Código não enviado removido");

        Banco.Executar("INSERT INTO clientes(nome,telefone) VALUES('Cliente teste','11999999999')");
        int client = Convert.ToInt32(Banco.Valor("SELECT MAX(id) FROM clientes"));
        Aparelhos.Salvar(0, client, "Celular", "Teste", "Modelo", "", "", "", "", "");
        int device = Convert.ToInt32(Banco.Valor("SELECT MAX(id) FROM aparelhos"));
        Banco.Executar("INSERT INTO pecas(codigo,nome,tipo,marca,modelo_compativel,valor_venda) VALUES('P1','Peça teste','Celular','*','*',50)");
        int part = Convert.ToInt32(Banco.Valor("SELECT MAX(id) FROM pecas"));
        int order = OperacoesOrdem.Abrir(device, "Falha de teste", null, "");
        Fails(() => OperacoesOrdem.Abrir(device, "Duplicada", null, ""), "Ordem duplicada negada", "já possui");
        Fails(() => Aparelhos.Salvar(device, client, "Celular", "Alterada", "Modelo", "", "", "", "", ""),
            "Aparelho com ordem preservado", "fazem parte do histórico");
        Fails(() => OperacoesOrdem.Diagnosticar(order, "D", "S", "Em manutenção", 100, 0), "Atendente não altera reparo", "Técnico");
        Fails(() => OperacoesOrdem.AtribuirTecnico(order, admin), "Atendente não pode ser responsável", "técnico ativo");
        OperacoesOrdem.AtribuirTecnico(order, tech);
        Check(Convert.ToInt32(Banco.Valor("SELECT tecnico_id FROM ordens_servico WHERE id=@id", Banco.P("@id", order))) == tech, "Atribuição mantém técnico na ordem");
        Fails(() => SalvarTecnico(tech, false), "Técnico de ordem aberta não pode ser desativado", "ordens em andamento");
        OperacoesOrdem.AtribuirTecnico(order, 0);
        Check(Banco.Valor("SELECT tecnico_id FROM ordens_servico WHERE id=@id", Banco.P("@id", order)) == DBNull.Value, "Ordem aberta pode ficar sem responsável");
        OperacoesOrdem.AtribuirTecnico(order, tech);
        Login(tech);
        Fails(() => OperacoesOrdem.AtribuirTecnico(order, 0), "Técnico não muda responsável", "Atendente");
        Estoque.Movimentar(part, "Entrada", 3, "Teste");
        Fails(() => OperacoesOrdem.UsarPeca(order, part, 1), "Exige assumir a ordem", "diagnóstico primeiro");
        OperacoesOrdem.Diagnosticar(order, "Diagnóstico teste", "Troca da peça", "Em manutenção", 100, 0);
        OperacoesOrdem.UsarPeca(order, part, 2);
        Check(Stock(part) == 1, "Peças utilizadas baixam estoque");
        Fails(() => OperacoesOrdem.UsarPeca(order, part, 2), "Estoque insuficiente negado", "Estoque insuficiente");
        Check(Stock(part) == 1, "Falha não altera estoque");
        Check(Convert.ToInt32(Banco.Valor("SELECT COUNT(*) FROM ordem_pecas")) == 1, "Falha não cria item parcial");
        int item = Convert.ToInt32(Banco.Valor("SELECT MAX(id) FROM ordem_pecas"));
        OperacoesOrdem.DevolverPeca(order, item);
        Check(Stock(part) == 3, "Devolução recompõe estoque");
        OperacoesOrdem.UsarPeca(order, part, 1);
        Fails(() => OperacoesOrdem.Diagnosticar(order, "D", "S", "Concluída", 100, 151), "Desconto acima do total negado", "desconto");
        OperacoesOrdem.Diagnosticar(order, "D", "S", "Concluída", 100, 10);
        Fails(() => OperacoesOrdem.UsarPeca(order, part, 1), "Ordem concluída bloqueada", "concluída");
        Login(admin);
        Fails(() => OperacoesOrdem.AtribuirTecnico(order, 0), "Ordem concluída bloqueia atribuição", "antes do diagnóstico");
        OperacoesOrdem.Entregar(order);
        Check(Convert.ToString(Banco.Valor("SELECT status FROM ordens_servico WHERE id=@id", Banco.P("@id", order))) == "Entregue", "Entrega concluída");
        Fails(() => OperacoesOrdem.Entregar(order), "Entrega repetida negada", "Apenas serviços concluídos");

        using (var users = new UsuariosForm())
        {
            users.Show(); Application.DoEvents();
            Field<TextBox>(users, "camposenha").Text = "NaoReaproveitar";
            AlteracoesFormulario.MarcarSalvo(users);
            Invoke(users, "btnEditar_Click");
            Check(Field<TextBox>(users, "camposenha").Text == "", "Editar limpa senha anterior");
            Check(!AlteracoesFormulario.TemAlteracoes(users), "Registro carregado não é alteração pendente");
            Field<TextBox>(users, "txtBusca").Text = "pesquisa";
            Check(!AlteracoesFormulario.TemAlteracoes(users), "Pesquisa não causa alerta de perda de dados");
            Field<TextBox>(users, "camponome").Text += " alterado";
            Check(AlteracoesFormulario.TemAlteracoes(users), "Alteração real detectada");
            Field<TextBox>(users, "camponome").Text = Field<TextBox>(users, "camponome").Text.Replace(" alterado", "");
            Invoke(users, "btnNovo_Click");
            Check(!AlteracoesFormulario.TemAlteracoes(users), "Novo cadastro reinicia controle de alterações");
        }
        using (var orders = new OrdensForm())
        {
            orders.Show(); Application.DoEvents();
            Field<ComboBox>(orders, "cmbStatus").SelectedIndex = 1;
            Check(!AlteracoesFormulario.TemAlteracoes(orders), "Filtro não é edição de ordem");
        }
        using (var cadastro = new CriarContaForm())
        {
            cadastro.Show();
            Application.DoEvents();
            Field<CheckBox>(cadastro, "mostrarSenha").Checked = true;
            Check(!Field<TextBox>(cadastro, "senha").UseSystemPasswordChar &&
                !Field<TextBox>(cadastro, "confirmarSenha").UseSystemPasswordChar, "Mostrar senhas controla os dois campos");
            Invoke(cadastro, "Criar_Click");
            DateTime limite = DateTime.UtcNow.AddSeconds(8);
            Label erro = Field<Label>(cadastro, "lblErro");
            while (!erro.Visible && DateTime.UtcNow < limite)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(10);
            }
            Check(erro.Visible && erro.Text.Contains("Nome"), "Cadastro incompleto mostra o erro na tela");
            Check(Field<Button>(cadastro, "criar").Enabled, "Cadastro libera nova tentativa");
        }
        Banco.Executar(schema);
        Check(Id("admin@example.com") == admin &&
            Convert.ToString(Banco.Valor("SELECT status FROM ordens_servico WHERE id=@id", Banco.P("@id", order))) == "Entregue",
            "Reimportação preserva contas e ordens existentes");
        using (var tabela = new DataGridView())
        {
            tabela.Columns.Add("oculto", "Oculto");
            tabela.Columns["oculto"].Visible = false;
            tabela.Columns.Add("nome", "Nome");
            tabela.Columns.Add("nota", "Nota");
            tabela.Rows.Add("segredo", "João", "Texto; com \"aspas\"");
            tabela.Rows.Add("segredo", "=1+1", "Segunda linha");
            var dados = Exportar.LerTabela(tabela);
            string csv = Exportar.GerarCsv(dados);
            Check(dados.Count == 3 && dados[0].Length == 2 && !csv.Contains("segredo"), "Exportação ignora coluna oculta e linha de cadastro");
            Check(csv.Contains("\"João\";\"Texto; com \"\"aspas\"\"\"") && csv.Contains("\"'=1+1\""), "CSV preserva acentos e aspas e protege fórmulas");
            for (int i = 0; i < 55; i++) dados.Add(new[] { "Registro " + i, "Observação" });
            string pdf = System.Text.Encoding.GetEncoding(1252).GetString(ExportacaoPdf.Gerar(dados, "Ordens de serviço"));
            Check(pdf.StartsWith("%PDF-1.4") && pdf.Contains("/Count 3") && pdf.Contains("Registro 54") && pdf.Contains("Ordens de serviço"), "PDF exporta acentos e todas as páginas");
        }
        Console.WriteLine("PASS INTEGRATION: " + checks + " verificações com MySQL isolado; nenhum e-mail externo enviado.");
        return 0;
    }
    private static T Field<T>(object obj, string name) { return (T)obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj); }
    private static void Invoke(object obj, string name) { obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, new object[] { obj, EventArgs.Empty }); }
    private static void SalvarTecnico(int id, bool ativo)
    {
        Contas.SalvarUsuario(id, "Técnico atualizado", "tech@example.com", "", null, "", "", null, "Técnico", ativo, "");
    }
    private static void Login(int id) { Sessao.Id = id; }
    private static int Id(string email) { return Convert.ToInt32(Banco.Valor("SELECT id FROM usuarios WHERE email=@email", Banco.P("@email", email))); }
    private static string Hash(int id) { return Convert.ToString(Banco.Valor("SELECT senha_hash FROM usuarios WHERE id=@id", Banco.P("@id", id))); }
    private static int Stock(int id) { return Convert.ToInt32(Banco.Valor("SELECT quantidade FROM pecas WHERE id=@id", Banco.P("@id", id))); }
    private static void Token(int id, bool expired)
    {
        string salt = new string('A', 32);
        string hash = (string)typeof(RecuperacaoSenha).GetMethod("HashCodigo", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { salt, "123456" });
        Banco.Executar("REPLACE INTO recuperacao_senha(usuario_id,codigo_hash,salt,expira_em,tentativas,enviado_em) VALUES(@id,@hash,@salt,UTC_TIMESTAMP()+INTERVAL " + (expired ? "-1" : "10") + " MINUTE,0,UTC_TIMESTAMP())", Banco.P("@id", id), Banco.P("@hash", hash), Banco.P("@salt", salt));
    }
    private static void Check(bool condition, string name) { if (!condition) throw new Exception("FAIL: " + name); checks++; Console.WriteLine("PASS " + name); }
    private static void Fails(Action action, string name, string expected)
    {
        try { action(); } catch (Exception ex) { Check(ex.Message.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0, name + " (erro esperado)"); return; }
        throw new Exception("FAIL: deveria impedir " + name);
    }
}
