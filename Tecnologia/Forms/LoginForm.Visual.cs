using System;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class LoginForm
    {
        private void ConfigurarVisual()
        {
            while (Controls.Count > 0) Controls[0].Dispose();
            FlowLayoutPanel cartao = AcessoLayout.Montar(this, "Entrar", "Acesse sua conta para continuar.");
            txtEmail = AcessoLayout.Campo(cartao, "E-mail");
            txtSenha = AcessoLayout.Campo(cartao, "Senha", true, 128);
            chkMostrar = new CheckBox { Text = "Mostrar senha", Width = 300, Height = 26,
                BackColor = System.Drawing.Color.Transparent, TabIndex = cartao.Controls.Count };
            chkMostrar.CheckedChanged += chkMostrar_CheckedChanged;
            cartao.Controls.Add(chkMostrar);
            AcessoLayout.Link(cartao, "Esqueceu a senha?", delegate
            {
                using (RecuperarSenhaForm janela = new RecuperarSenhaForm(txtEmail.Text)) AbrirAcesso(janela);
            });
            btnEntrar = AcessoLayout.Botao(cartao, "ENTRAR");
            btnEntrar.Click += btnEntrar_Click;
            AcessoLayout.Link(cartao, "Não tem uma conta? Criar conta", delegate
            {
                using (CriarContaForm janela = new CriarContaForm())
                    if (AbrirAcesso(janela) == DialogResult.OK) { txtEmail.Text = janela.EmailCriado; txtSenha.Clear(); txtSenha.Focus(); }
            });
            AcceptButton = btnEntrar;
        }
        private DialogResult AbrirAcesso(Form janela)
        {
            Hide();
            try { return janela.ShowDialog(this); }
            finally { if (!IsDisposed) Show(); }
        }
    }
}
