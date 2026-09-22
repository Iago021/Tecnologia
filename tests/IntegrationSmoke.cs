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
        using (var db = new MySqlConnection(builder.ConnectionString))
        {
            db.Open();
            string schema = File.ReadAllText(args[0]).Replace("tecnologia", "tecnologia_verificacao");
            using (var cmd = new MySqlCommand(schema, db)) cmd.ExecuteNonQuery();
        }
        Check(Contas.Criar("Atendente", "admin@example.com", "admin@example.com", Password, Password), "Primeira conta ativa");
        Check(!Contas.Criar("Técnico", "tech@example.com", "tech@example.com", Password, Password), "Segunda conta pendente");
        int admin = Id("admin@example.com"), tech = Id("tech@example.com");
        Check(!Convert.ToBoolean(Banco.Valor("SELECT ativo FROM usuarios WHERE id=@id", Banco.P("@id", tech))), "Sem ativação pública");
        Fails(() => Contas.Criar("Duplicado", "admin@example.com", "admin@example.com", Password, Password), "Cadastro duplicado", "Duplicate");
        Check(Senha.Conferir(Password, Hash(admin)), "Senha armazenada válida");
        Check(!Senha.Conferir("errada", Hash(admin)), "Senha errada negada");
        Login(tech); Fails(() => Sessao.Exigir(), "Conta inativa negada", "inativa");
        Banco.Executar("UPDATE usuarios SET ativo=1,perfil='Técnico',telefone='11999999999' WHERE id=@id", Banco.P("@id", tech));

        Token(admin, false);
        Fails(() => Contas.Redefinir("admin@example.com", "999999", Password, Password), "Código incorreto", "Código inválido");
        Check(Convert.ToInt32(Banco.Valor("SELECT tentativas FROM recuperacao_senha WHERE usuario_id=@id", Banco.P("@id", admin))) == 1, "Tentativa incorreta registrada");
        for (int i = 0; i < 4; i++) Fails(() => Contas.Redefinir("admin@example.com", "999999", Password, Password), "Limite de tentativas", "Código inválido");
        Fails(() => Contas.Redefinir("admin@example.com", "123456", Password, Password), "Código bloqueado após cinco erros", "Código inválido");
        Token(admin, true);
        Fails(() => Contas.Redefinir("admin@example.com", "123456", Password, Password), "Código expirado", "Código inválido");
        Token(admin, false);
        Contas.Redefinir("admin@example.com", "123456", "Nova-123456", "Nova-123456");
        Check(Senha.Conferir("Nova-123456", Hash(admin)) && !Senha.Conferir(Password, Hash(admin)), "Redefinição altera a senha");
        Fails(() => Contas.Redefinir("admin@example.com", "123456", Password, Password), "Código de uso único", "Código inválido");
        Login(admin); Token(admin, false);
        Fails(() => Contas.AtualizarPerfil("Admin", "admin@example.com", "11999999999", "incorreta", Password, Password), "Perfil exige senha atual", "Senha atual incorreta");
        Contas.AtualizarPerfil("Admin", "admin@example.com", "11999999999", "Nova-123456", Password, Password);
        Check(Senha.Conferir(Password, Hash(admin)), "Senha alterada pelo perfil");
        Fails(() => Contas.Redefinir("admin@example.com", "123456", Password, Password), "Troca de senha invalida código antigo", "Código inválido");
        Token(tech, false);
        string oldHash = Hash(tech);
        Contas.SalvarUsuario("UPDATE usuarios SET nome='Técnico atualizado',senha_hash=IF(@senha_hash='',senha_hash,@senha_hash) WHERE id=@id", Banco.P("@id", tech), Banco.P("@senha_hash", ""));
        Check(Hash(tech) == oldHash, "Edição sem senha preserva o hash");
        Check(Convert.ToInt32(Banco.Valor("SELECT COUNT(*) FROM recuperacao_senha WHERE usuario_id=@id", Banco.P("@id", tech))) == 0, "Edição administrativa invalida recuperação");
        Environment.SetEnvironmentVariable("TECNOLOGIA_SMTPHOST", "127.0.0.1");
        Environment.SetEnvironmentVariable("TECNOLOGIA_SMTPPORT", "1");
        Environment.SetEnvironmentVariable("TECNOLOGIA_SMTPFROM", "teste@example.com");
        Fails(() => Contas.SolicitarCodigo("admin@example.com"), "Falha SMTP tratada", "Não foi possível enviar");
        Check(Convert.ToInt32(Banco.Valor("SELECT COUNT(*) FROM recuperacao_senha WHERE usuario_id=@id", Banco.P("@id", admin))) == 0, "Código não enviado removido");

        Banco.Executar("INSERT INTO clientes(nome,telefone) VALUES('Cliente teste','11999999999')");
        int client = Convert.ToInt32(Banco.Valor("SELECT MAX(id) FROM clientes"));
        Banco.Executar("INSERT INTO aparelhos(cliente_id,tipo,marca,modelo) VALUES(@id,'Celular','Teste','Modelo')", Banco.P("@id", client));
        int device = Convert.ToInt32(Banco.Valor("SELECT MAX(id) FROM aparelhos"));
        Banco.Executar("INSERT INTO pecas(codigo,nome,tipo,marca,modelo_compativel,valor_venda) VALUES('P1','Peça teste','Celular','*','*',50)");
        int part = Convert.ToInt32(Banco.Valor("SELECT MAX(id) FROM pecas"));
        int order = OperacoesOrdem.Abrir(device, "Falha de teste", null, "");
        Fails(() => OperacoesOrdem.Abrir(device, "Duplicada", null, ""), "Ordem duplicada negada", "já possui");
        Fails(() => OperacoesOrdem.Diagnosticar(order, "D", "S", "Em manutenção", 100, 0), "Atendente não altera reparo", "Técnico");
        Login(tech);
        OperacoesOrdem.Movimentar(part, "Entrada", 3, "Teste");
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
        Login(admin); OperacoesOrdem.Entregar(order);
        Check(Convert.ToString(Banco.Valor("SELECT status FROM ordens_servico WHERE id=@id", Banco.P("@id", order))) == "Entregue", "Entrega concluída");
        Fails(() => OperacoesOrdem.Entregar(order), "Entrega repetida negada", "Apenas serviços concluídos");

        using (var users = new UsuariosForm())
        {
            users.Show(); Application.DoEvents();
            Field<TextBox>(users, "camposenha").Text = "NaoReaproveitar";
            Invoke(users, "btnEditar_Click");
            Check(Field<TextBox>(users, "camposenha").Text == "", "Editar limpa senha anterior");
            Check(!AlteracoesFormulario.TemAlteracoes(users), "Registro carregado não é alteração pendente");
            Field<TextBox>(users, "txtBusca").Text = "pesquisa";
            Check(!AlteracoesFormulario.TemAlteracoes(users), "Pesquisa não causa alerta de perda de dados");
            Field<TextBox>(users, "camponome").Text += " alterado";
            Check(AlteracoesFormulario.TemAlteracoes(users), "Alteração real detectada");
            Invoke(users, "btnNovo_Click");
            Check(!AlteracoesFormulario.TemAlteracoes(users), "Limpar reinicia controle de alterações");
        }
        using (var orders = new OrdensForm())
        {
            orders.Show(); Application.DoEvents();
            Field<ComboBox>(orders, "cmbStatus").SelectedIndex = 1;
            Check(!AlteracoesFormulario.TemAlteracoes(orders), "Filtro não é edição de ordem");
        }
        Console.WriteLine("PASS INTEGRATION: " + checks + " verificações com MySQL isolado; nenhum e-mail externo enviado.");
        return 0;
    }
    private static T Field<T>(object obj, string name) { return (T)obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj); }
    private static void Invoke(object obj, string name) { obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, new object[] { obj, EventArgs.Empty }); }
    private static void Login(int id) { Sessao.Id = id; }
    private static int Id(string email) { return Convert.ToInt32(Banco.Valor("SELECT id FROM usuarios WHERE email=@email", Banco.P("@email", email))); }
    private static string Hash(int id) { return Convert.ToString(Banco.Valor("SELECT senha_hash FROM usuarios WHERE id=@id", Banco.P("@id", id))); }
    private static int Stock(int id) { return Convert.ToInt32(Banco.Valor("SELECT quantidade FROM pecas WHERE id=@id", Banco.P("@id", id))); }
    private static void Token(int id, bool expired)
    {
        string salt = new string('A', 32);
        string hash = (string)typeof(Contas).GetMethod("HashCodigo", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { salt, "123456" });
        Banco.Executar("REPLACE INTO recuperacao_senha(usuario_id,codigo_hash,salt,expira_em,tentativas,enviado_em) VALUES(@id,@hash,@salt,UTC_TIMESTAMP()+INTERVAL " + (expired ? "-1" : "10") + " MINUTE,0,UTC_TIMESTAMP())", Banco.P("@id", id), Banco.P("@hash", hash), Banco.P("@salt", salt));
    }
    private static void Check(bool condition, string name) { if (!condition) throw new Exception("FAIL: " + name); checks++; Console.WriteLine("PASS " + name); }
    private static void Fails(Action action, string name, string expected)
    {
        try { action(); } catch (Exception ex) { Check(ex.Message.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0, name + " (erro esperado)"); return; }
        throw new Exception("FAIL: deveria impedir " + name);
    }
}
