using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class LoginForm : Form
    {
        public LoginForm() { InitializeComponent(); }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            AcceptButton = btnEntrar;
            try { AtualizarPrimeiroAcesso(); } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void AtualizarPrimeiroAcesso()
        {
            bool primeiro = Convert.ToInt32(Banco.Valor("SELECT COUNT(*) FROM usuarios")) == 0;
            lblPrimeiro.Visible = txtNome.Visible = btnPrimeiro.Visible = primeiro;
        }
        private void chkMostrar_CheckedChanged(object sender, EventArgs e) { txtSenha.UseSystemPasswordChar = !chkMostrar.Checked; }
        private void btnEntrar_Click(object sender, EventArgs e)
        {
            try
            {
                Tela.Email(txtEmail.Text.Trim(), true);
                DataTable dados = Banco.Consultar("SELECT id,nome,perfil,senha_hash FROM usuarios WHERE email=@email AND ativo=1", Banco.P("@email",txtEmail.Text.Trim()));
                if (dados.Rows.Count == 0 || !Senha.Conferir(txtSenha.Text, dados.Rows[0]["senha_hash"].ToString()))
                    throw new Exception("E-mail ou senha incorretos, ou conta inativa.");
                Sessao.Id = Convert.ToInt32(dados.Rows[0]["id"]);
                Sessao.Nome = dados.Rows[0]["nome"].ToString();
                Sessao.Perfil = dados.Rows[0]["perfil"].ToString();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnPrimeiro_Click(object sender, EventArgs e)
        {
            try
            {
                Tela.Obrigatorio(txtNome.Text,"nome");
                Tela.Email(txtEmail.Text.Trim(),true);
                string hash = Senha.Criar(txtSenha.Text);
                using (MySqlConnection conexao = Banco.Abrir())
                {
                    using (MySqlCommand trava = Banco.Comando(conexao,null,"SELECT GET_LOCK('tecnologia_primeiro_acesso',10)"))
                        if (Convert.ToInt32(trava.ExecuteScalar()) != 1) throw new Exception("Tente novamente em alguns segundos.");
                    try
                    {
                        using (MySqlCommand contar = Banco.Comando(conexao,null,"SELECT COUNT(*) FROM usuarios"))
                            if (Convert.ToInt32(contar.ExecuteScalar()) > 0) throw new Exception("O primeiro usuário já foi criado. Entre com sua conta.");
                        using (MySqlCommand inserir = Banco.Comando(conexao,null,"INSERT INTO usuarios(nome,email,senha_hash,perfil) VALUES(@nome,@email,@senha,'Atendente')",
                            Banco.P("@nome",txtNome.Text.Trim()),Banco.P("@email",txtEmail.Text.Trim()),Banco.P("@senha",hash))) inserir.ExecuteNonQuery();
                    }
                    finally { using (MySqlCommand liberar = Banco.Comando(conexao,null,"SELECT RELEASE_LOCK('tecnologia_primeiro_acesso')")) liberar.ExecuteScalar(); }
                }
                AtualizarPrimeiroAcesso();
                MessageBox.Show("Conta criada. Clique em Entrar.");
            }
            catch (Exception erro) { Tela.Erro(erro); }
        }
    }
}
