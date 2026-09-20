namespace Tecnologia
{
    partial class MovimentoForm
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
            this.lblPeca = new System.Windows.Forms.Label();
            this.lblPeca.Name = "lblPeca";
            this.lblPeca.Location = new System.Drawing.Point(22, 20);
            this.lblPeca.Size = new System.Drawing.Size(520, 24);
            this.lblPeca.TabIndex = 0;
            this.lblPeca.Text = "Peça";
            this.lblPeca.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblPeca);
            this.lblTipo = new System.Windows.Forms.Label();
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Location = new System.Drawing.Point(22, 70);
            this.lblTipo.Size = new System.Drawing.Size(320, 24);
            this.lblTipo.TabIndex = 1;
            this.lblTipo.Text = "Movimentação";
            this.lblTipo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblTipo);
            this.cmbTipo = new System.Windows.Forms.ComboBox();
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Location = new System.Drawing.Point(22, 96);
            this.cmbTipo.Size = new System.Drawing.Size(520, 26);
            this.cmbTipo.TabIndex = 2;
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.Items.AddRange(new object[] { "Entrada", "Saída" });
            this.Controls.Add(this.cmbTipo);
            this.lblQuantidade = new System.Windows.Forms.Label();
            this.lblQuantidade.Name = "lblQuantidade";
            this.lblQuantidade.Location = new System.Drawing.Point(22, 140);
            this.lblQuantidade.Size = new System.Drawing.Size(320, 24);
            this.lblQuantidade.TabIndex = 3;
            this.lblQuantidade.Text = "Quantidade";
            this.lblQuantidade.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblQuantidade);
            this.numQuantidade = new System.Windows.Forms.NumericUpDown();
            this.numQuantidade.Name = "numQuantidade";
            this.numQuantidade.Location = new System.Drawing.Point(22, 166);
            this.numQuantidade.Size = new System.Drawing.Size(520, 26);
            this.numQuantidade.TabIndex = 4;
            this.numQuantidade.Maximum = 999999;
            this.numQuantidade.DecimalPlaces = 0;
            this.numQuantidade.Minimum=1;
            this.numQuantidade.Value=1;
            this.Controls.Add(this.numQuantidade);
            this.lblMotivo = new System.Windows.Forms.Label();
            this.lblMotivo.Name = "lblMotivo";
            this.lblMotivo.Location = new System.Drawing.Point(22, 210);
            this.lblMotivo.Size = new System.Drawing.Size(320, 24);
            this.lblMotivo.TabIndex = 5;
            this.lblMotivo.Text = "Motivo / fornecedor";
            this.lblMotivo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblMotivo);
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.Location = new System.Drawing.Point(22, 236);
            this.txtMotivo.Size = new System.Drawing.Size(520, 26);
            this.txtMotivo.TabIndex = 6;
            this.txtMotivo.MaxLength = 100;
            this.txtMotivo.MaxLength = 300;
            this.Controls.Add(this.txtMotivo);
            this.lblDica = new System.Windows.Forms.Label();
            this.lblDica.Name = "lblDica";
            this.lblDica.Location = new System.Drawing.Point(22, 290);
            this.lblDica.Size = new System.Drawing.Size(520, 24);
            this.lblDica.TabIndex = 7;
            this.lblDica.Text = "Para utilizar uma peça em um conserto, abra a ordem de serviço.";
            this.lblDica.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblDica);
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Location = new System.Drawing.Point(22, 342);
            this.btnSalvar.Size = new System.Drawing.Size(520, 34);
            this.btnSalvar.TabIndex = 8;
            this.btnSalvar.Text = "Registrar movimentação";
            this.btnSalvar.UseVisualStyleBackColor = true;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            this.Controls.Add(this.btnSalvar);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ClientSize = new System.Drawing.Size(568, 404);
            this.MinimumSize = new System.Drawing.Size(568, 404);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Movimentar estoque";
            this.Name = "MovimentoForm";
            this.Load += new System.EventHandler(this.MovimentoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label lblPeca;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cmbTipo;
        private System.Windows.Forms.Label lblQuantidade;
        private System.Windows.Forms.NumericUpDown numQuantidade;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Label lblDica;
        private System.Windows.Forms.Button btnSalvar;
    }
}
