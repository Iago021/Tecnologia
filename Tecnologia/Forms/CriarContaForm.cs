using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class CriarContaForm : Form
    {
        public string EmailCriado { get { return email.Text.Trim(); } }
        public CriarContaForm() { InitializeComponent(); PrepararTela(); }
        private async void Criar_Click(object sender, EventArgs e)
        {
            string n = nome.Text, m = email.Text, cm = confirmarEmail.Text, s = senha.Text, cs = confirmarSenha.Text;
            criar.Enabled = false; criar.Text = "CRIANDO...";
            cartaoAcesso.Enabled = false;
            try
            {
                bool primeira = await Task.Run(() => Contas.Criar(n, m, cm, s, cs));
                if (IsDisposed) return;
                MessageBox.Show(this, primeira ? "Conta criada! Você já pode entrar." : "Conta criada! Um atendente precisa ativar seu acesso em Cadastros > Equipe.", "Tecnologia");
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception erro) { if (!IsDisposed) Tela.Erro(erro); }
            finally { if (!IsDisposed) { cartaoAcesso.Enabled = true; criar.Enabled = true; criar.Text = "CRIAR CONTA"; } }
        }
    }
}
