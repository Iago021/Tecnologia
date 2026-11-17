using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class CriarContaForm : Form
    {
        private bool salvando;
        public string EmailCriado { get; private set; }

        public CriarContaForm()
        {
            InitializeComponent();
        }

        private async void Criar_Click(object sender, EventArgs e)
        {
            if (salvando) return;

            string nomeDigitado = nome.Text;
            string emailDigitado = email.Text.Trim();
            string emailConfirmado = confirmarEmail.Text;
            string senhaDigitada = senha.Text;
            string senhaConfirmada = confirmarSenha.Text;

            lblErro.Visible = false;
            salvando = true;
            cartaoAcesso.Enabled = false;
            criar.Text = "Criando conta...";
            bool primeiraConta = false;
            string mensagemErro = null;
            try
            {
                primeiraConta = await Task.Run(() =>
                    Contas.Criar(nomeDigitado, emailDigitado, emailConfirmado, senhaDigitada, senhaConfirmada));
            }
            catch (Exception erro)
            {
                mensagemErro = Tela.Mensagem(erro);
            }
            finally
            {
                salvando = false;
                if (!IsDisposed)
                {
                    cartaoAcesso.Enabled = true;
                    criar.Text = "Criar conta";
                }
            }

            if (IsDisposed) return;
            if (mensagemErro != null)
            {
                lblErro.Text = mensagemErro;
                lblErro.Visible = true;
                ScrollControlIntoView(lblErro);
                return;
            }

            EmailCriado = emailDigitado;
            string mensagem = primeiraConta
                ? "Conta criada. Você já pode entrar."
                : "Cadastro recebido. Um atendente pode ativar seu acesso em Cadastros > Equipe.";
            MessageBox.Show(this, mensagem, "Tecnologia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (salvando && e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        private void MostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            senha.UseSystemPasswordChar = !mostrarSenha.Checked;
            confirmarSenha.UseSystemPasswordChar = !mostrarSenha.Checked;
        }

        private void Voltar_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
