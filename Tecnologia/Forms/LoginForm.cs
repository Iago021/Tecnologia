using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class LoginForm : Form
    {
        public LoginForm() { InitializeComponent(); PrepararTela(); }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            AcceptButton = btnEntrar;
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
    }
}
