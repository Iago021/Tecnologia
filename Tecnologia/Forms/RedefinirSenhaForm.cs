using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tecnologia
{
    public class RedefinirSenhaForm : Form
    {
        private readonly string email;
        private readonly TextBox codigo, senha, confirmacao;
        private readonly Button redefinir;
        public RedefinirSenhaForm(string endereco)
        {
            email = endereco;
            Text = "Tecnologia — Redefinir senha";
            FlowLayoutPanel cartao = AcessoLayout.Montar(this, "Redefinir senha", "Digite o código recebido por e-mail e escolha sua nova senha. O código expira em 10 minutos.");
            codigo = AcessoLayout.Campo(cartao, "Código de verificação", false, 6);
            senha = AcessoLayout.Campo(cartao, "Nova senha (8 a 128 caracteres)", true, 128);
            confirmacao = AcessoLayout.Campo(cartao, "Confirme a nova senha", true, 128);
            redefinir = AcessoLayout.Botao(cartao, "REDEFINIR SENHA");
            redefinir.Click += Redefinir_Click;
            AcessoLayout.Link(cartao, "Voltar / solicitar outro código", delegate { Close(); });
            AcceptButton = redefinir;
        }
        private async void Redefinir_Click(object sender, EventArgs e)
        {
            string c = codigo.Text, s = senha.Text, cs = confirmacao.Text;
            redefinir.Enabled = false; redefinir.Text = "SALVANDO...";
            try
            {
                await Task.Run(() => Contas.Redefinir(email, c, s, cs));
                if (IsDisposed) return;
                MessageBox.Show(this, "Senha redefinida. Entre com a nova senha.", "Tecnologia");
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception erro) { if (!IsDisposed) Tela.Erro(erro); }
            finally { if (!IsDisposed) { redefinir.Enabled = true; redefinir.Text = "REDEFINIR SENHA"; } }
        }
    }
}
