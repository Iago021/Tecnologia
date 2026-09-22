using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class RecuperarSenhaForm : Form
    {
        public RecuperarSenhaForm() : this("") { }
        public RecuperarSenhaForm(string endereco) { InitializeComponent(); PrepararTela(); email.Text = endereco; }
        private async void Enviar_Click(object sender, EventArgs e)
        {
            string endereco = email.Text.Trim();
            enviar.Enabled = false; enviar.Text = "ENVIANDO...";
            cartaoAcesso.Enabled = false;
            try
            {
                await Task.Run(() => Contas.SolicitarCodigo(endereco));
                if (IsDisposed) return;
                MessageBox.Show(this, "Se este e-mail pertence a uma conta ativa, você receberá um código. Confira também o spam. Aguarde 60 segundos entre os envios.", "Tecnologia");
                AbrirRedefinicao(endereco);
            }
            catch (Exception erro) { if (!IsDisposed) Tela.Erro(erro); }
            finally { if (!IsDisposed) { cartaoAcesso.Enabled = true; enviar.Enabled = true; enviar.Text = "ENVIAR CÓDIGO"; } }
        }
        private void AbrirRedefinicao(string endereco = null)
        {
            endereco = endereco ?? email.Text.Trim();
            try { Tela.Email(endereco, true); }
            catch (Exception erro) { Tela.Erro(erro); return; }
            using (RedefinirSenhaForm janela = new RedefinirSenhaForm(endereco))
            {
                bool sucesso = false;
                Hide();
                try { sucesso = janela.ShowDialog(this) == DialogResult.OK; }
                finally { if (!IsDisposed && !sucesso) Show(); }
                if (sucesso) { DialogResult = DialogResult.OK; Close(); }
            }
        }
    }
}
