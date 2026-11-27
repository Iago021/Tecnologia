using System;
using System.Data;
using MySqlConnector;

namespace Tecnologia
{
    internal static class Contas
    {
        public static DataRow Autenticar(string email, string senha)
        {
            email = email.Trim();
            Tela.Email(email, true);
            DataTable contas = Banco.Consultar(
                "SELECT id,nome,perfil,senha_hash,ativo FROM usuarios WHERE email=@email", Banco.P("@email", email));

            if (contas.Rows.Count == 0 || !Senha.Conferir(senha, contas.Rows[0]["senha_hash"].ToString()))
                throw new InvalidOperationException("E-mail ou senha incorretos.");

            DataRow conta = contas.Rows[0];
            if (!Convert.ToBoolean(conta["ativo"]))
                throw new InvalidOperationException("Sua conta está inativa. Peça a um atendente para ativá-la em Cadastros > Equipe.");

            return conta;
        }

        public static bool Criar(string nome, string email, string confirmacaoEmail, string senha, string confirmacaoSenha)
        {
            nome = nome.Trim();
            email = email.Trim();
            ValidarDados(nome, email, "");
            if (!email.Equals(confirmacaoEmail.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Os e-mails não conferem.");
            if (senha != confirmacaoSenha)
                throw new InvalidOperationException("As senhas não conferem.");

            string hash = Senha.Criar(senha);
            const string trava = "CONCAT(LEFT(DATABASE(), 40), ':primeiro_acesso')";
            using (MySqlConnection conexao = Banco.Abrir())
            {
                if (Convert.ToInt32(Banco.Valor(conexao, null, "SELECT GET_LOCK(" + trava + ", 10)")) != 1)
                    throw new InvalidOperationException("Tente novamente em alguns segundos.");

                try
                {
                    bool primeira = Banco.Valor(conexao, null, "SELECT id FROM usuarios LIMIT 1") == null;
                    Banco.Executar(conexao, null,
                        "INSERT INTO usuarios (nome,email,senha_hash,perfil,ativo) VALUES (@nome,@email,@senha,'Atendente',@ativo)",
                        Banco.P("@nome", nome), Banco.P("@email", email), Banco.P("@senha", hash), Banco.P("@ativo", primeira));
                    return primeira;
                }
                finally
                {
                    if (conexao.State == ConnectionState.Open)
                        Banco.Valor(conexao, null, "SELECT RELEASE_LOCK(" + trava + ")");
                }
            }
        }

        public static void AtualizarPerfil(string nome, string email, string telefone, string atual, string nova, string confirmacao)
        {
            nome = nome.Trim();
            email = email.Trim();
            telefone = telefone.Trim();
            ValidarDados(nome, email, telefone);
            if (nova != confirmacao)
                throw new InvalidOperationException("A confirmação da nova senha está diferente.");

            string novoHash = nova.Length == 0 ? null : Senha.Criar(nova);
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                string hashAnterior = Banco.Valor(conexao, transacao,
                    "SELECT senha_hash FROM usuarios WHERE id=@id AND ativo=1 FOR UPDATE", Banco.P("@id", Sessao.Id)) as string;
                if (hashAnterior == null)
                    throw new InvalidOperationException("Sua conta está inativa. Entre novamente.");
                if (!Senha.Conferir(atual, hashAnterior))
                    throw new InvalidOperationException("Senha atual incorreta.");

                Banco.Executar(conexao, transacao,
                    "UPDATE usuarios SET nome=@nome,email=@email,telefone=@telefone,senha_hash=@senha WHERE id=@id",
                    Banco.P("@nome", nome), Banco.P("@email", email), Banco.P("@telefone", telefone),
                    Banco.P("@senha", novoHash ?? hashAnterior), Banco.P("@id", Sessao.Id));

                RecuperacaoSenha.InvalidarCodigo(conexao, transacao, Sessao.Id);
                transacao.Commit();
            }
            Sessao.Nome = nome;
        }

        public static void SalvarUsuario(int id, string nome, string email, string telefone, string cpf, string rg,
            string cidade, DateTime? nascimento, string perfil, bool ativo, string senha)
        {
            if (id != 0 && id == Sessao.Id)
                throw new InvalidOperationException("Use Minha conta para alterar seus próprios dados.");

            nome = nome.Trim();
            email = email.Trim();
            telefone = telefone.Trim();
            ValidarDados(nome, email, telefone);
            if (perfil != "Atendente" && perfil != "Técnico")
                throw new InvalidOperationException("Selecione um perfil.");

            string hash = id == 0 || senha != "" ? Senha.Criar(senha) : null;
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
                if (id != 0 && (!ativo || perfil != "Técnico"))
                    ConferirOrdensDoTecnico(conexao, transacao, id);

                string sql = id == 0
                    ? @"INSERT INTO usuarios (nome,email,telefone,cpf,rg,cidade,data_nascimento,perfil,ativo,senha_hash)
                          VALUES (@nome,@email,@telefone,@cpf,@rg,@cidade,@nascimento,@perfil,@ativo,@senha)"
                    : @"UPDATE usuarios SET nome=@nome,email=@email,telefone=@telefone,cpf=@cpf,rg=@rg,cidade=@cidade,
                          data_nascimento=@nascimento,perfil=@perfil,ativo=@ativo,senha_hash=COALESCE(@senha,senha_hash)
                          WHERE id=@id";

                int alterados = Banco.Executar(conexao, transacao, sql,
                    Banco.P("@nome", nome), Banco.P("@email", email), Banco.P("@telefone", telefone), Banco.P("@cpf", cpf),
                    Banco.P("@rg", rg.Trim()), Banco.P("@cidade", cidade.Trim()), Banco.P("@nascimento", nascimento),
                    Banco.P("@perfil", perfil), Banco.P("@ativo", ativo), Banco.P("@senha", hash), Banco.P("@id", id));

                if (alterados != 1)
                    throw new InvalidOperationException("Conta não encontrada. Atualize a lista.");
                if (id != 0)
                    RecuperacaoSenha.InvalidarCodigo(conexao, transacao, id);

                transacao.Commit();
            }
        }

        public static void TrocarPerfil(int id, string perfilEsperado)
        {
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
                string atual = Banco.Valor(conexao, transacao,
                    "SELECT perfil FROM usuarios WHERE id=@id FOR UPDATE", Banco.P("@id", id)) as string;
                if (atual != perfilEsperado || (atual != "Atendente" && atual != "Técnico"))
                    throw new InvalidOperationException("A conta ou o perfil mudou. Atualize a lista e tente novamente.");

                if (id == Sessao.Id && Banco.Valor(conexao, transacao,
                    "SELECT id FROM usuarios WHERE perfil='Atendente' AND ativo=1 AND id<>@id LIMIT 1 FOR UPDATE",
                    Banco.P("@id", id)) == null)
                    throw new InvalidOperationException("Cadastre ou ative outro atendente antes de mudar o perfil da sua conta.");

                if (atual == "Técnico") ConferirOrdensDoTecnico(conexao, transacao, id);
                string novoPerfil = atual == "Atendente" ? "Técnico" : "Atendente";
                Banco.Executar(conexao, transacao, "UPDATE usuarios SET perfil=@perfil WHERE id=@id",
                    Banco.P("@perfil", novoPerfil), Banco.P("@id", id));
                RecuperacaoSenha.InvalidarCodigo(conexao, transacao, id);
                transacao.Commit();
            }
        }

        public static void ExcluirUsuario(int id)
        {
            if (id == Sessao.Id)
                throw new InvalidOperationException("Você não pode excluir sua própria conta.");

            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
                if (Banco.Executar(conexao, transacao, "DELETE FROM usuarios WHERE id=@id", Banco.P("@id", id)) == 0)
                    throw new InvalidOperationException("Conta não encontrada. Atualize a lista.");

                transacao.Commit();
            }
        }

        private static void ConferirOrdensDoTecnico(MySqlConnection conexao, MySqlTransaction transacao, int id)
        {
            Banco.Valor(conexao, transacao, "SELECT id FROM usuarios WHERE id=@id FOR UPDATE", Banco.P("@id", id));
            object ordem = Banco.Valor(conexao, transacao,
                "SELECT id FROM ordens_servico WHERE tecnico_id=@id AND status NOT IN ('Concluída','Entregue','Cancelada') LIMIT 1",
                Banco.P("@id", id));
            if (ordem != null)
                throw new InvalidOperationException("Este técnico possui ordens em andamento. Conclua os serviços ou remova a atribuição das ordens abertas.");
        }

        private static void ValidarDados(string nome, string email, string telefone)
        {
            Tela.Obrigatorio(nome, "Nome");
            Tela.Email(email, true);
            if (nome.Length > 100 || email.Length > 100)
                throw new InvalidOperationException("Nome e e-mail devem ter até 100 caracteres.");
            if (telefone.Length > 20)
                throw new InvalidOperationException("O telefone deve ter até 20 caracteres.");
        }
    }
}
