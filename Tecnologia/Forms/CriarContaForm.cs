using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tecnologia
{
    public class CriarContaForm : Form
    {
        private readonly TextBox nome, email, confirmarEmail, senha, confirmarSenha;
        private readonly Button criar;
        public string EmailCriado { get { return email.Text.Trim(); } }
        public CriarContaForm()
        {
            Text = "Tecnologia — Criar conta";
            FlowLayoutPanel cartao = AcessoLayout.Montar(this, "Criar conta", "Preencha seus dados para solicitar acesso ao sistema.");
            nome = AcessoLayout.Campo(cartao, "Nome completo");
            email = AcessoLayout.Campo(cartao, "E-mail");
            confirmarEmail = AcessoLayout.Campo(cartao, "Confirme o e-mail");
            senha = AcessoLayout.Campo(cartao, "Senha (8 a 128 caracteres)", true, 128);
            confirmarSenha = AcessoLayout.Campo(cartao, "Confirme a senha", true, 128);
            criar = AcessoLayout.Botao(cartao, "CRIAR CONTA");
            criar.Click += Criar_Click;
            AcessoLayout.Link(cartao, "Já tenho conta · Entrar", delegate { Close(); });
            AcceptButton = criar;
        }
        private async void Criar_Click(object sender, EventArgs e)
        {
            string n = nome.Text, m = email.Text, cm = confirmarEmail.Text, s = senha.Text, cs = confirmarSenha.Text;
            criar.Enabled = false; criar.Text = "CRIANDO...";
            AcessoLayout.Ocupar(this, true);
            try
            {
                bool primeira = await Task.Run(() => Contas.Criar(n, m, cm, s, cs));
                if (IsDisposed) return;
                MessageBox.Show(this, primeira ? "Conta criada! Você já pode entrar." : "Conta criada! Um atendente precisa ativar seu acesso em Cadastros > Equipe.", "Tecnologia");
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception erro) { if (!IsDisposed) Tela.Erro(erro); }
            finally { if (!IsDisposed) { AcessoLayout.Ocupar(this, false); criar.Enabled = true; criar.Text = "CRIAR CONTA"; } }
        }
    }
}
