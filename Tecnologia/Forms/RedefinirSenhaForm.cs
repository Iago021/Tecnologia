using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class RedefinirSenhaForm : Form
    {
        private readonly string email;
        public RedefinirSenhaForm() : this("") { }
        public RedefinirSenhaForm(string endereco) { email = endereco; InitializeComponent(); PrepararTela(); }
        private async void Redefinir_Click(object sender, EventArgs e)
        {
            string c = codigo.Text, s = senha.Text, cs = confirmacao.Text;
            redefinir.Enabled = false; redefinir.Text = "SALVANDO...";
            cartaoAcesso.Enabled = false;
            try
            {
                await Task.Run(() => Contas.Redefinir(email, c, s, cs));
                if (IsDisposed) return;
                MessageBox.Show(this, "Senha redefinida. Entre com a nova senha.", "Tecnologia");
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception erro) { if (!IsDisposed) Tela.Erro(erro); }
            finally { if (!IsDisposed) { cartaoAcesso.Enabled = true; redefinir.Enabled = true; redefinir.Text = "REDEFINIR SENHA"; } }
        }
    }
}
