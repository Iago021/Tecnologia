using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            PrepararTela();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            AcceptButton = btnEntrar;
        }

        private void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {
            txtSenha.UseSystemPasswordChar = !chkMostrar.Checked;
        }

        private async void btnEntrar_Click(object sender, EventArgs e)
        {
            if (!btnEntrar.Enabled) return;

            string email = txtEmail.Text;
            string senha = txtSenha.Text;
            cartaoAcesso.Enabled = false;
            try
            {
                DataRow conta = await Task.Run(() => Contas.Autenticar(email, senha));
                if (IsDisposed || !Visible) return;

                Sessao.Iniciar(conta);
                DialogResult = DialogResult.OK;
            }
            catch (Exception erro)
            {
                if (!IsDisposed && Visible) Tela.Erro(erro);
            }
            finally
            {
                if (!IsDisposed)
                {
                    cartaoAcesso.Enabled = true;
                    if (Visible && DialogResult != DialogResult.OK) txtSenha.Focus();
                }
            }
        }
    }
}
