using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;

namespace Tecnologia
{
    internal static class Contas
    {
        public static void AtualizarPerfil(string nome, string email, string telefone, string atual, string nova, string confirmacao)
        {
            Sessao.Exigir(); Tela.Obrigatorio(nome, "nome"); Tela.Email(email.Trim(), true);
            if (nova != confirmacao) throw new Exception("A confirmação da nova senha está diferente.");
            string novoHash = nova == "" ? "" : Senha.Criar(nova);
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                string antiga;
                using (MySqlCommand buscar = Banco.Comando(conexao, transacao,
                    "SELECT senha_hash FROM usuarios WHERE id=@id AND ativo=1 FOR UPDATE", Banco.P("@id", Sessao.Id)))
                    antiga = Convert.ToString(buscar.ExecuteScalar());
                if (!Senha.Conferir(atual, antiga)) throw new Exception("Senha atual incorreta.");
                using (MySqlCommand salvar = Banco.Comando(conexao, transacao,
                    "UPDATE usuarios SET nome=@nome,email=@email,telefone=@telefone,senha_hash=@senha WHERE id=@id",
                    Banco.P("@nome", nome.Trim()), Banco.P("@email", email.Trim()), Banco.P("@telefone", telefone.Trim()),
                    Banco.P("@senha", novoHash == "" ? antiga : novoHash), Banco.P("@id", Sessao.Id))) salvar.ExecuteNonQuery();
                InvalidarCodigo(conexao, transacao, Sessao.Id);
                transacao.Commit();
            }
            Sessao.Nome = nome.Trim();
        }

        public static void SalvarUsuario(string sql, params MySqlParameter[] parametros)
        {
            Sessao.Exigir("Atendente");
            int id = 0;
            foreach (MySqlParameter parametro in parametros) if (parametro.ParameterName == "@id") id = Convert.ToInt32(parametro.Value);
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                using (MySqlCommand salvar = Banco.Comando(conexao, transacao, sql, parametros))
                    if (salvar.ExecuteNonQuery() != 1) throw new Exception("Conta não encontrada. Atualize a lista.");
                if (id != 0) InvalidarCodigo(conexao, transacao, id);
                transacao.Commit();
            }
        }

        private static void InvalidarCodigo(MySqlConnection conexao, MySqlTransaction transacao, int id)
        {
            using (MySqlCommand limpar = Banco.Comando(conexao, transacao,
                "DELETE FROM recuperacao_senha WHERE usuario_id=@id", Banco.P("@id", id))) limpar.ExecuteNonQuery();
        }

        public static bool Criar(string nome, string email, string confirmacaoEmail, string senha, string confirmacaoSenha)
        {
            nome = nome.Trim(); email = email.Trim();
            Tela.Obrigatorio(nome, "nome"); Tela.Email(email, true);
            if (nome.Length > 100 || email.Length > 100) throw new Exception("Nome e e-mail devem ter até 100 caracteres.");
            if (!email.Equals(confirmacaoEmail.Trim(), StringComparison.OrdinalIgnoreCase)) throw new Exception("Os e-mails não conferem.");
            if (senha != confirmacaoSenha) throw new Exception("As senhas não conferem.");
            string hash = Senha.Criar(senha);
            using (MySqlConnection conexao = Banco.Abrir())
            {
                using (MySqlCommand trava = Banco.Comando(conexao, null, "SELECT GET_LOCK('tecnologia_primeiro_acesso',10)"))
                    if (Convert.ToInt32(trava.ExecuteScalar()) != 1) throw new Exception("Tente novamente em alguns segundos.");
                try
                {
                    bool primeira;
                    using (MySqlCommand contar = Banco.Comando(conexao, null, "SELECT COUNT(*) FROM usuarios"))
                        primeira = Convert.ToInt32(contar.ExecuteScalar()) == 0;
                    // Uma conta pública não pode conceder a si mesma acesso aos dados da equipe.
                    using (MySqlCommand inserir = Banco.Comando(conexao, null,
                        "INSERT INTO usuarios(nome,email,senha_hash,perfil,ativo) VALUES(@nome,@email,@senha,'Atendente',@ativo)",
                        Banco.P("@nome", nome), Banco.P("@email", email), Banco.P("@senha", hash), Banco.P("@ativo", primeira)))
                        inserir.ExecuteNonQuery();
                    return primeira;
                }
                finally
                {
                    using (MySqlCommand liberar = Banco.Comando(conexao, null, "SELECT RELEASE_LOCK('tecnologia_primeiro_acesso')")) liberar.ExecuteScalar();
                }
            }
        }

        private static string Config(string chave)
        {
            return Environment.GetEnvironmentVariable("TECNOLOGIA_" + chave.ToUpperInvariant())
                ?? ConfigurationManager.AppSettings[chave] ?? "";
        }

        public static void SolicitarCodigo(string email)
        {
            email = email.Trim(); Tela.Email(email, true);
            string servidor = Config("SmtpHost"), remetente = Config("SmtpFrom");
            if (string.IsNullOrWhiteSpace(servidor) || string.IsNullOrWhiteSpace(remetente))
                throw new Exception("O envio de e-mail ainda não foi configurado. Peça à equipe para configurar o SMTP conforme o README.");
            int porta;
            if (!int.TryParse(Config("SmtpPort"), out porta) || porta < 1 || porta > 65535) porta = 587;
            string codigo = GerarCodigo();
            byte[] bytes = new byte[16];
            using (RandomNumberGenerator gerador = RandomNumberGenerator.Create()) gerador.GetBytes(bytes);
            string salt = BitConverter.ToString(bytes).Replace("-", "");
            string hash = HashCodigo(salt, codigo);
            int usuario;
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                using (MySqlCommand buscar = Banco.Comando(conexao, transacao,
                    "SELECT id FROM usuarios WHERE email=@email AND ativo=1 FOR UPDATE", Banco.P("@email", email)))
                {
                    object id = buscar.ExecuteScalar();
                    if (id == null) { transacao.Commit(); return; }
                    usuario = Convert.ToInt32(id);
                }
                using (MySqlCommand recente = Banco.Comando(conexao, transacao,
                    "SELECT COUNT(*) FROM recuperacao_senha WHERE usuario_id=@id AND enviado_em > UTC_TIMESTAMP() - INTERVAL 60 SECOND", Banco.P("@id", usuario)))
                    if (Convert.ToInt32(recente.ExecuteScalar()) > 0) { transacao.Commit(); return; }
                using (MySqlCommand salvar = Banco.Comando(conexao, transacao, @"INSERT INTO recuperacao_senha
                    (usuario_id,codigo_hash,salt,expira_em,tentativas,enviado_em)
                    VALUES(@id,@hash,@salt,UTC_TIMESTAMP()+INTERVAL 10 MINUTE,0,UTC_TIMESTAMP())
                    ON DUPLICATE KEY UPDATE codigo_hash=@hash,salt=@salt,expira_em=UTC_TIMESTAMP()+INTERVAL 10 MINUTE,tentativas=0,enviado_em=UTC_TIMESTAMP()",
                    Banco.P("@id", usuario), Banco.P("@hash", hash), Banco.P("@salt", salt))) salvar.ExecuteNonQuery();
                transacao.Commit();
            }
            try
            {
                using (SmtpClient smtp = new SmtpClient(servidor, porta))
                using (MailMessage mensagem = new MailMessage(remetente, email))
                {
                    smtp.EnableSsl = true;
                    smtp.Timeout = 15000;
                    smtp.UseDefaultCredentials = false;
                    string login = Config("SmtpUser");
                    if (!string.IsNullOrWhiteSpace(login)) smtp.Credentials = new NetworkCredential(login, Config("SmtpPassword"));
                    mensagem.Subject = "Tecnologia — código para redefinir sua senha";
                    mensagem.Body = "Seu código de verificação é: " + codigo + "\r\n\r\nEle expira em 10 minutos e só pode ser utilizado uma vez.\r\nSe você não solicitou a alteração, ignore este e-mail.";
                    mensagem.BodyEncoding = Encoding.UTF8;
                    smtp.Send(mensagem);
                }
            }
            catch
            {
                // Não deixa válido um código cujo envio falhou; não remove um código mais novo.
                Banco.Executar("DELETE FROM recuperacao_senha WHERE usuario_id=@id AND codigo_hash=@hash", Banco.P("@id", usuario), Banco.P("@hash", hash));
                throw new Exception("Não foi possível enviar o e-mail. Confira a configuração de envio com a equipe e tente novamente.");
            }
        }

        public static void Redefinir(string email, string codigo, string senha, string confirmacao)
        {
            email = email.Trim(); codigo = codigo.Trim(); Tela.Email(email, true);
            if (senha != confirmacao) throw new Exception("As senhas não conferem.");
            if (codigo.Length != 6) throw new Exception("Informe os seis dígitos do código.");
            foreach (char c in codigo) if (c < '0' || c > '9') throw new Exception("O código deve conter apenas números.");
            string senhaHash = Senha.Criar(senha);
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                int usuario;
                using (MySqlCommand buscar = Banco.Comando(conexao, transacao,
                    "SELECT id FROM usuarios WHERE email=@email AND ativo=1 FOR UPDATE", Banco.P("@email", email)))
                {
                    object id = buscar.ExecuteScalar();
                    if (id == null) throw new Exception("Código inválido ou expirado. Solicite um novo código.");
                    usuario = Convert.ToInt32(id);
                }
                string salt = "", esperado = "";
                using (MySqlCommand buscar = Banco.Comando(conexao, transacao,
                    "SELECT salt,codigo_hash FROM recuperacao_senha WHERE usuario_id=@id AND expira_em>UTC_TIMESTAMP() AND tentativas<5 FOR UPDATE", Banco.P("@id", usuario)))
                using (MySqlDataReader leitor = buscar.ExecuteReader())
                {
                    if (!leitor.Read()) throw new Exception("Código inválido ou expirado. Solicite um novo código.");
                    salt = leitor.GetString(0); esperado = leitor.GetString(1);
                }
                if (!Comparar(esperado, HashCodigo(salt, codigo)))
                {
                    using (MySqlCommand falha = Banco.Comando(conexao, transacao,
                        "UPDATE recuperacao_senha SET tentativas=tentativas+1 WHERE usuario_id=@id", Banco.P("@id", usuario))) falha.ExecuteNonQuery();
                    transacao.Commit();
                    throw new Exception("Código inválido ou expirado. Solicite um novo código se necessário.");
                }
                using (MySqlCommand alterar = Banco.Comando(conexao, transacao,
                    "UPDATE usuarios SET senha_hash=@senha WHERE id=@id", Banco.P("@senha", senhaHash), Banco.P("@id", usuario))) alterar.ExecuteNonQuery();
                using (MySqlCommand consumir = Banco.Comando(conexao, transacao,
                    "DELETE FROM recuperacao_senha WHERE usuario_id=@id", Banco.P("@id", usuario))) consumir.ExecuteNonQuery();
                transacao.Commit();
            }
        }

        private static string GerarCodigo()
        {
            byte[] bytes = new byte[4]; uint valor;
            using (RandomNumberGenerator gerador = RandomNumberGenerator.Create())
                do { gerador.GetBytes(bytes); valor = BitConverter.ToUInt32(bytes, 0); } while (valor >= 4294000000U);
            return (valor % 1000000).ToString("D6", CultureInfo.InvariantCulture);
        }
        private static string HashCodigo(string salt, string codigo)
        {
            using (SHA256 algoritmo = SHA256.Create())
                return BitConverter.ToString(algoritmo.ComputeHash(Encoding.UTF8.GetBytes(salt + ":" + codigo))).Replace("-", "");
        }
        private static bool Comparar(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int diferenca = 0;
            for (int i = 0; i < a.Length; i++) diferenca |= a[i] ^ b[i];
            return diferenca == 0;
        }
    }
}
