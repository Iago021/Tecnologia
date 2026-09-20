using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tecnologia
{
    public class RecuperarSenhaForm : Form
    {
        private readonly TextBox email;
        private readonly Button enviar;
        public RecuperarSenhaForm(string endereco = "")
        {
            Text = "Tecnologia — Esqueci a senha";
            FlowLayoutPanel cartao = AcessoLayout.Montar(this, "Esqueceu a senha?", "Informe o e-mail da sua conta. Enviaremos um código de verificação para redefinir a senha.");
            email = AcessoLayout.Campo(cartao, "E-mail"); email.Text = endereco;
            enviar = AcessoLayout.Botao(cartao, "ENVIAR CÓDIGO");
            enviar.Click += Enviar_Click;
            AcessoLayout.Link(cartao, "Já tenho um código", delegate { AbrirRedefinicao(); });
            AcessoLayout.Link(cartao, "Voltar para entrar", delegate { Close(); });
            AcceptButton = enviar;
        }
        private async void Enviar_Click(object sender, EventArgs e)
        {
            string endereco = email.Text.Trim();
            enviar.Enabled = false; enviar.Text = "ENVIANDO...";
            try
            {
                await Task.Run(() => Contas.SolicitarCodigo(endereco));
                if (IsDisposed) return;
                MessageBox.Show(this, "Se este e-mail pertence a uma conta ativa, você receberá um código. Confira também o spam. Aguarde 60 segundos entre os envios.", "Tecnologia");
                AbrirRedefinicao();
            }
            catch (Exception erro) { if (!IsDisposed) Tela.Erro(erro); }
            finally { if (!IsDisposed) { enviar.Enabled = true; enviar.Text = "ENVIAR CÓDIGO"; } }
        }
        private void AbrirRedefinicao()
        {
            try { Tela.Email(email.Text.Trim(), true); }
            catch (Exception erro) { Tela.Erro(erro); return; }
            using (RedefinirSenhaForm janela = new RedefinirSenhaForm(email.Text.Trim()))
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
