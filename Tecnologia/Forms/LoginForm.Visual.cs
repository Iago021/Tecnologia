using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class LoginForm
    {
        private Panel cartaoLogin;
        private PictureBox logoLogin;
        private Panel fundoNome;
        private bool primeiroAcesso;

        private void ConfigurarVisual()
        {
            SuspendLayout();
            ClientSize = new Size(1080, 720);
            Tema.Aplicar(this);
            AutoScrollMinSize = new Size(620, 700);
            logoLogin = Tema.Imagem("tecnologia-logo.png");
            logoLogin.Size = new Size(496, 106);
            logoLogin.AccessibleName = "Tecnologia";
            Controls.Add(logoLogin);
            lblTitulo.Visible = false;
            cartaoLogin = new Panel();
            cartaoLogin.Name = "cartaoLogin";
            cartaoLogin.BackColor = Tema.Cartao;
            cartaoLogin.Size = new Size(360, 310);
            cartaoLogin.TabIndex = 0;
            Controls.Add(cartaoLogin);

            lblEmail.SetBounds(34, 24, 292, 22);
            lblSenha.SetBounds(34, 108, 292, 22);
            cartaoLogin.Controls.Add(lblEmail);
            cartaoLogin.Controls.Add(lblSenha);
            Panel fundoEmail = Tema.EnvolverCampo(txtEmail, 292);
            fundoEmail.Location = new Point(34, 50);
            Panel fundoSenha = Tema.EnvolverCampo(txtSenha, 292);
            fundoSenha.Location = new Point(34, 134);
            cartaoLogin.Controls.Add(fundoEmail);
            cartaoLogin.Controls.Add(fundoSenha);
            txtEmail.AccessibleName = "E-mail";
            txtSenha.AccessibleName = "Senha";

            chkMostrar.SetBounds(34, 189, 292, 26);
            cartaoLogin.Controls.Add(chkMostrar);
            btnEntrar.SetBounds(114, 232, 132, 38);
            btnEntrar.Text = "ENTRAR";
            cartaoLogin.Controls.Add(btnEntrar);

            lblPrimeiro.SetBounds(34, 286, 292, 40);
            lblPrimeiro.Text = "Primeiro acesso? Informe seu nome para criar a conta de atendente.";
            lblPrimeiro.ForeColor = Tema.VerdeEscuro;
            cartaoLogin.Controls.Add(lblPrimeiro);
            fundoNome = Tema.EnvolverCampo(txtNome, 292);
            fundoNome.Location = new Point(34, 336);
            txtNome.AccessibleName = "Nome do primeiro atendente";
            cartaoLogin.Controls.Add(fundoNome);
            btnPrimeiro.SetBounds(60, 394, 240, 38);
            btnPrimeiro.Text = "CRIAR PRIMEIRO ATENDENTE";
            cartaoLogin.Controls.Add(btnPrimeiro);
            // Só aparece depois que a consulta confirma que o banco está vazio.
            MostrarPrimeiroAcesso(false);
            Resize += delegate { PosicionarLogin(); };
            PosicionarLogin();
            ResumeLayout(true);
        }

        private void MostrarPrimeiroAcesso(bool primeiro)
        {
            primeiroAcesso = primeiro;
            lblPrimeiro.Visible = txtNome.Visible = fundoNome.Visible = btnPrimeiro.Visible = primeiro;
            cartaoLogin.Height = primeiro ? 458 : 298;
            Tema.Arredondar(cartaoLogin, 18);
            PosicionarLogin();
        }

        private void PosicionarLogin()
        {
            int largura = Math.Max(620, ClientSize.Width);
            int topo = primeiroAcesso ? 24 : 70;
            logoLogin.Location = new Point((largura - logoLogin.Width) / 2 + AutoScrollPosition.X, topo + AutoScrollPosition.Y);
            cartaoLogin.Location = new Point((largura - cartaoLogin.Width) / 2 + AutoScrollPosition.X, topo + 148 + AutoScrollPosition.Y);
        }
    }
}
