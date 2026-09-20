namespace Tecnologia
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Location = new System.Drawing.Point(28, 22);
            this.lblTitulo.Size = new System.Drawing.Size(430, 24);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Tecnologia | Assistência técnica";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblTitulo);
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Location = new System.Drawing.Point(28, 74);
            this.lblEmail.Size = new System.Drawing.Size(320, 24);
            this.lblEmail.TabIndex = 1;
            this.lblEmail.Text = "E-mail";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblEmail);
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Location = new System.Drawing.Point(28, 100);
            this.txtEmail.Size = new System.Drawing.Size(414, 26);
            this.txtEmail.TabIndex = 2;
            this.txtEmail.MaxLength = 100;
            this.txtEmail.MaxLength = 100;
            this.Controls.Add(this.txtEmail);
            this.lblSenha = new System.Windows.Forms.Label();
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Location = new System.Drawing.Point(28, 140);
            this.lblSenha.Size = new System.Drawing.Size(320, 24);
            this.lblSenha.TabIndex = 3;
            this.lblSenha.Text = "Senha";
            this.lblSenha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblSenha);
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.Location = new System.Drawing.Point(28, 166);
            this.txtSenha.Size = new System.Drawing.Size(414, 26);
            this.txtSenha.TabIndex = 4;
            this.txtSenha.MaxLength = 100;
            this.txtSenha.UseSystemPasswordChar = true;
            this.txtSenha.MaxLength = 128;
            this.Controls.Add(this.txtSenha);
            this.chkMostrar = new System.Windows.Forms.CheckBox();
            this.chkMostrar.Name = "chkMostrar";
            this.chkMostrar.Location = new System.Drawing.Point(28, 203);
            this.chkMostrar.Size = new System.Drawing.Size(200, 26);
            this.chkMostrar.TabIndex = 5;
            this.chkMostrar.Text = "Mostrar senha";
            this.chkMostrar.CheckedChanged += new System.EventHandler(this.chkMostrar_CheckedChanged);
            this.Controls.Add(this.chkMostrar);
            this.btnEntrar = new System.Windows.Forms.Button();
            this.btnEntrar.Name = "btnEntrar";
            this.btnEntrar.Location = new System.Drawing.Point(28, 245);
            this.btnEntrar.Size = new System.Drawing.Size(414, 34);
            this.btnEntrar.TabIndex = 6;
            this.btnEntrar.Text = "Entrar";
            this.btnEntrar.UseVisualStyleBackColor = true;
            this.btnEntrar.Click += new System.EventHandler(this.btnEntrar_Click);
            this.Controls.Add(this.btnEntrar);
            this.lblPrimeiro = new System.Windows.Forms.Label();
            this.lblPrimeiro.Name = "lblPrimeiro";
            this.lblPrimeiro.Location = new System.Drawing.Point(28, 310);
            this.lblPrimeiro.Size = new System.Drawing.Size(414, 24);
            this.lblPrimeiro.TabIndex = 7;
            this.lblPrimeiro.Text = "Primeiro acesso: informe seu nome, e-mail e uma senha.";
            this.lblPrimeiro.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblPrimeiro);
            this.txtNome = new System.Windows.Forms.TextBox();
            this.txtNome.Name = "txtNome";
            this.txtNome.Location = new System.Drawing.Point(28, 344);
            this.txtNome.Size = new System.Drawing.Size(414, 26);
            this.txtNome.TabIndex = 8;
            this.txtNome.MaxLength = 100;
            this.txtNome.MaxLength = 100;
            this.Controls.Add(this.txtNome);
            this.btnPrimeiro = new System.Windows.Forms.Button();
            this.btnPrimeiro.Name = "btnPrimeiro";
            this.btnPrimeiro.Location = new System.Drawing.Point(28, 386);
            this.btnPrimeiro.Size = new System.Drawing.Size(414, 34);
            this.btnPrimeiro.TabIndex = 9;
            this.btnPrimeiro.Text = "Criar primeiro atendente";
            this.btnPrimeiro.UseVisualStyleBackColor = true;
            this.btnPrimeiro.Click += new System.EventHandler(this.btnPrimeiro_Click);
            this.Controls.Add(this.btnPrimeiro);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ClientSize = new System.Drawing.Size(470, 450);
            this.MinimumSize = new System.Drawing.Size(470, 450);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tecnologia — Entrar";
            this.Name = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblSenha;
        private System.Windows.Forms.TextBox txtSenha;
        private System.Windows.Forms.CheckBox chkMostrar;
        private System.Windows.Forms.Button btnEntrar;
        private System.Windows.Forms.Label lblPrimeiro;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Button btnPrimeiro;
    }
}
