using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    partial class CriarContaForm
    {
        private TableLayoutPanel janela;
        private Panel areaCampos;
        private TableLayoutPanel layout;
        private FlowLayoutPanel acoes;
        private PictureBox logo;
        private FlowLayoutPanel cartaoAcesso;
        private Label titulo;
        private Label instrucao;
        private Label lblNome;
        private TextBox nome;
        private Label lblEmail;
        private TextBox email;
        private Label lblConfirmarEmail;
        private TextBox confirmarEmail;
        private Label lblSenha;
        private TextBox senha;
        private Label lblConfirmarSenha;
        private TextBox confirmarSenha;
        private CheckBox mostrarSenha;
        private Label lblErro;
        private Button criar;
        private LinkLabel voltar;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (BackgroundImage != null) BackgroundImage.Dispose();
                if (logo != null && logo.Image != null) logo.Image.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager recursos = new ComponentResourceManager(typeof(CriarContaForm));
            this.janela = new TableLayoutPanel();
            this.areaCampos = new Panel();
            this.layout = new TableLayoutPanel();
            this.acoes = new FlowLayoutPanel();
            this.logo = new PictureBox();
            this.cartaoAcesso = new FlowLayoutPanel();
            this.titulo = new Label();
            this.instrucao = new Label();
            this.lblNome = new Label();
            this.nome = new TextBox();
            this.lblEmail = new Label();
            this.email = new TextBox();
            this.lblConfirmarEmail = new Label();
            this.confirmarEmail = new TextBox();
            this.lblSenha = new Label();
            this.senha = new TextBox();
            this.lblConfirmarSenha = new Label();
            this.confirmarSenha = new TextBox();
            this.mostrarSenha = new CheckBox();
            this.lblErro = new Label();
            this.criar = new Button();
            this.voltar = new LinkLabel();
            this.SuspendLayout();
            this.janela.SuspendLayout();
            this.areaCampos.SuspendLayout();
            this.layout.SuspendLayout();
            this.acoes.SuspendLayout();
            this.cartaoAcesso.SuspendLayout();

            this.janela.BackColor = Color.Transparent;
            this.janela.ColumnCount = 1;
            this.janela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.janela.Dock = DockStyle.Fill;
            this.janela.Name = "janela";
            this.janela.RowCount = 2;
            this.janela.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.janela.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            this.areaCampos.AutoScroll = true;
            this.areaCampos.BackColor = Color.Transparent;
            this.areaCampos.Dock = DockStyle.Fill;
            this.areaCampos.Margin = new Padding(0);
            this.areaCampos.Name = "areaCampos";

            this.acoes.Anchor = AnchorStyles.None;
            this.acoes.AutoSize = true;
            this.acoes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.acoes.BackColor = Color.FromArgb(245, 248, 247);
            this.acoes.BorderStyle = BorderStyle.FixedSingle;
            this.acoes.FlowDirection = FlowDirection.TopDown;
            this.acoes.Margin = new Padding(0, 0, 0, 12);
            this.acoes.Name = "acoes";
            this.acoes.Padding = new Padding(22, 10, 22, 12);
            this.acoes.TabIndex = 1;
            this.acoes.WrapContents = false;

            this.layout.AutoSize = true;
            this.layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.layout.BackColor = Color.Transparent;
            this.layout.Dock = DockStyle.Top;
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.layout.RowCount = 2;
            this.layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.layout.Padding = new Padding(12, 14, 12, 10);
            this.layout.Name = "layout";

            this.logo.Anchor = AnchorStyles.None;
            this.logo.BackColor = Color.Transparent;
            this.logo.Image = (Image)recursos.GetObject("picturebox1.Image");
            this.logo.Margin = new Padding(0, 0, 0, 10);
            this.logo.Size = new Size(310, 66);
            this.logo.SizeMode = PictureBoxSizeMode.Zoom;
            this.logo.AccessibleName = "Tecnologia";
            this.logo.Name = "logo";
            this.logo.TabStop = false;

            this.cartaoAcesso.Anchor = AnchorStyles.None;
            this.cartaoAcesso.AutoSize = true;
            this.cartaoAcesso.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.cartaoAcesso.BackColor = Color.FromArgb(245, 248, 247);
            this.cartaoAcesso.BorderStyle = BorderStyle.FixedSingle;
            this.cartaoAcesso.FlowDirection = FlowDirection.TopDown;
            this.cartaoAcesso.WrapContents = false;
            this.cartaoAcesso.Padding = new Padding(22, 16, 22, 15);
            this.cartaoAcesso.TabIndex = 0;
            this.cartaoAcesso.Name = "cartaoAcesso";

            this.titulo.AutoSize = true;
            this.titulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.titulo.ForeColor = Color.FromArgb(25, 97, 78);
            this.titulo.Margin = new Padding(0, 0, 0, 4);
            this.titulo.Name = "titulo";
            this.titulo.Text = "Criar conta";

            this.instrucao.AutoSize = true;
            this.instrucao.Margin = new Padding(0, 0, 0, 12);
            this.instrucao.MaximumSize = new Size(336, 0);
            this.instrucao.Name = "instrucao";
            this.instrucao.Text = "A senha deve ter de 8 a 128 caracteres.";

            this.lblNome.AutoSize = true;
            this.lblNome.Margin = new Padding(0, 0, 0, 3);
            this.lblNome.Name = "lblNome";
            this.lblNome.Text = "Nome completo";
            this.nome.AccessibleName = "Nome completo";
            this.nome.BackColor = Color.White;
            this.nome.Margin = new Padding(0, 0, 0, 10);
            this.nome.MaxLength = 100;
            this.nome.Name = "nome";
            this.nome.Size = new Size(336, 26);
            this.nome.TabIndex = 0;

            this.lblEmail.AutoSize = true;
            this.lblEmail.Margin = new Padding(0, 0, 0, 3);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Text = "E-mail";
            this.email.AccessibleName = "E-mail";
            this.email.BackColor = Color.White;
            this.email.Margin = new Padding(0, 0, 0, 10);
            this.email.MaxLength = 100;
            this.email.Name = "email";
            this.email.Size = new Size(336, 26);
            this.email.TabIndex = 1;

            this.lblConfirmarEmail.AutoSize = true;
            this.lblConfirmarEmail.Margin = new Padding(0, 0, 0, 3);
            this.lblConfirmarEmail.Name = "lblConfirmarEmail";
            this.lblConfirmarEmail.Text = "Confirme o e-mail";
            this.confirmarEmail.AccessibleName = "Confirme o e-mail";
            this.confirmarEmail.BackColor = Color.White;
            this.confirmarEmail.Margin = new Padding(0, 0, 0, 10);
            this.confirmarEmail.MaxLength = 100;
            this.confirmarEmail.Name = "confirmarEmail";
            this.confirmarEmail.Size = new Size(336, 26);
            this.confirmarEmail.TabIndex = 2;

            this.lblSenha.AutoSize = true;
            this.lblSenha.Margin = new Padding(0, 0, 0, 3);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Text = "Senha (8 a 128 caracteres)";
            this.senha.AccessibleName = "Senha (8 a 128 caracteres)";
            this.senha.BackColor = Color.White;
            this.senha.Margin = new Padding(0, 0, 0, 10);
            this.senha.MaxLength = 128;
            this.senha.Name = "senha";
            this.senha.Size = new Size(336, 26);
            this.senha.TabIndex = 3;
            this.senha.UseSystemPasswordChar = true;

            this.lblConfirmarSenha.AutoSize = true;
            this.lblConfirmarSenha.Margin = new Padding(0, 0, 0, 3);
            this.lblConfirmarSenha.Name = "lblConfirmarSenha";
            this.lblConfirmarSenha.Text = "Confirme a senha";
            this.confirmarSenha.AccessibleName = "Confirme a senha";
            this.confirmarSenha.BackColor = Color.White;
            this.confirmarSenha.Margin = new Padding(0, 0, 0, 10);
            this.confirmarSenha.MaxLength = 128;
            this.confirmarSenha.Name = "confirmarSenha";
            this.confirmarSenha.Size = new Size(336, 26);
            this.confirmarSenha.TabIndex = 4;
            this.confirmarSenha.UseSystemPasswordChar = true;

            this.mostrarSenha.AutoSize = true;
            this.mostrarSenha.Margin = new Padding(0, 0, 0, 8);
            this.mostrarSenha.Name = "mostrarSenha";
            this.mostrarSenha.TabIndex = 5;
            this.mostrarSenha.Text = "Mostrar senhas";
            this.mostrarSenha.CheckedChanged += this.MostrarSenha_CheckedChanged;

            this.lblErro.AccessibleRole = AccessibleRole.Alert;
            this.lblErro.AutoSize = true;
            this.lblErro.ForeColor = Color.FromArgb(151, 41, 41);
            this.lblErro.Margin = new Padding(0, 0, 0, 10);
            this.lblErro.MaximumSize = new Size(336, 0);
            this.lblErro.Name = "lblErro";
            this.lblErro.Visible = false;

            this.criar.BackColor = Color.FromArgb(25, 97, 78);
            this.criar.FlatStyle = FlatStyle.Flat;
            this.criar.FlatAppearance.BorderSize = 0;
            this.criar.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, 82, 67);
            this.criar.ForeColor = Color.White;
            this.criar.Margin = new Padding(0, 0, 0, 8);
            this.criar.Name = "criar";
            this.criar.Size = new Size(336, 38);
            this.criar.TabIndex = 6;
            this.criar.Text = "Criar conta";
            this.criar.UseVisualStyleBackColor = false;
            this.criar.Click += this.Criar_Click;

            this.voltar.AutoSize = true;
            this.voltar.LinkColor = Color.FromArgb(25, 97, 78);
            this.voltar.Margin = new Padding(0);
            this.voltar.Name = "voltar";
            this.voltar.TabIndex = 7;
            this.voltar.Text = "Voltar ao login";
            this.voltar.LinkClicked += this.Voltar_Click;

            this.cartaoAcesso.Controls.Add(this.titulo);
            this.cartaoAcesso.Controls.Add(this.instrucao);
            this.cartaoAcesso.Controls.Add(this.lblNome);
            this.cartaoAcesso.Controls.Add(this.nome);
            this.cartaoAcesso.Controls.Add(this.lblEmail);
            this.cartaoAcesso.Controls.Add(this.email);
            this.cartaoAcesso.Controls.Add(this.lblConfirmarEmail);
            this.cartaoAcesso.Controls.Add(this.confirmarEmail);
            this.cartaoAcesso.Controls.Add(this.lblSenha);
            this.cartaoAcesso.Controls.Add(this.senha);
            this.cartaoAcesso.Controls.Add(this.lblConfirmarSenha);
            this.cartaoAcesso.Controls.Add(this.confirmarSenha);
            this.cartaoAcesso.Controls.Add(this.mostrarSenha);
            this.acoes.Controls.Add(this.lblErro);
            this.acoes.Controls.Add(this.criar);
            this.acoes.Controls.Add(this.voltar);
            this.layout.Controls.Add(this.logo, 0, 0);
            this.layout.Controls.Add(this.cartaoAcesso, 0, 1);
            this.areaCampos.Controls.Add(this.layout);
            this.janela.Controls.Add(this.areaCampos, 0, 0);
            this.janela.Controls.Add(this.acoes, 0, 1);
            this.Controls.Add(this.janela);

            this.AcceptButton = this.criar;
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(245, 248, 247);
            this.BackgroundImage = (Image)recursos.GetObject("$this.BackgroundImage");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.ClientSize = new Size(900, 638);
            this.Font = new Font("Segoe UI", 10F);
            this.ForeColor = Color.FromArgb(21, 59, 50);
            this.MinimumSize = new Size(460, 360);
            this.Name = "CriarContaForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Tecnologia — Criar conta";
            this.cartaoAcesso.ResumeLayout(false);
            this.cartaoAcesso.PerformLayout();
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.areaCampos.ResumeLayout(true);
            this.acoes.ResumeLayout(true);
            this.janela.ResumeLayout(true);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
