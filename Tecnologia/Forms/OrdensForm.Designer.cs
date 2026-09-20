namespace Tecnologia
{
    partial class OrdensForm
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
            this.lblTitulo.Location = new System.Drawing.Point(22, 14);
            this.lblTitulo.Size = new System.Drawing.Size(1040, 24);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Ordens de serviço";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblTitulo);
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Location = new System.Drawing.Point(22, 60);
            this.cmbStatus.Size = new System.Drawing.Size(240, 26);
            this.cmbStatus.TabIndex = 1;
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] { "Todas", "Aberta", "Em manutenção", "Aguardando peça", "Concluída", "Entregue", "Em andamento", "Concluídas e entregues" });
            this.Controls.Add(this.cmbStatus);
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Location = new System.Drawing.Point(278, 60);
            this.txtBusca.Size = new System.Drawing.Size(360, 26);
            this.txtBusca.TabIndex = 2;
            this.txtBusca.MaxLength = 100;
            this.txtBusca.MaxLength = 100;
            this.Controls.Add(this.txtBusca);
            this.btnPesquisar = new System.Windows.Forms.Button();
            this.btnPesquisar.Name = "btnPesquisar";
            this.btnPesquisar.Location = new System.Drawing.Point(654, 55);
            this.btnPesquisar.Size = new System.Drawing.Size(142, 34);
            this.btnPesquisar.TabIndex = 3;
            this.btnPesquisar.Text = "Pesquisar";
            this.btnPesquisar.UseVisualStyleBackColor = true;
            this.btnPesquisar.Click += new System.EventHandler(this.btnPesquisar_Click);
            this.Controls.Add(this.btnPesquisar);
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Location = new System.Drawing.Point(810, 55);
            this.btnExportar.Size = new System.Drawing.Size(180, 34);
            this.btnExportar.TabIndex = 4;
            this.btnExportar.Text = "Exportar Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            this.Controls.Add(this.btnExportar);
            this.lblDica = new System.Windows.Forms.Label();
            this.lblDica.Name = "lblDica";
            this.lblDica.Location = new System.Drawing.Point(22, 99);
            this.lblDica.Size = new System.Drawing.Size(1040, 24);
            this.lblDica.TabIndex = 5;
            this.lblDica.Text = "Pesquise por número, cliente, aparelho ou técnico.";
            this.lblDica.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblDica);
            this.btnNova = new System.Windows.Forms.Button();
            this.btnNova.Name = "btnNova";
            this.btnNova.Location = new System.Drawing.Point(22, 141);
            this.btnNova.Size = new System.Drawing.Size(190, 34);
            this.btnNova.TabIndex = 6;
            this.btnNova.Text = "Abrir nova ordem";
            this.btnNova.UseVisualStyleBackColor = true;
            this.btnNova.Click += new System.EventHandler(this.btnNova_Click);
            this.Controls.Add(this.btnNova);
            this.btnDetalhes = new System.Windows.Forms.Button();
            this.btnDetalhes.Name = "btnDetalhes";
            this.btnDetalhes.Location = new System.Drawing.Point(230, 141);
            this.btnDetalhes.Size = new System.Drawing.Size(220, 34);
            this.btnDetalhes.TabIndex = 7;
            this.btnDetalhes.Text = "Ver ordem selecionada";
            this.btnDetalhes.UseVisualStyleBackColor = true;
            this.btnDetalhes.Click += new System.EventHandler(this.btnDetalhes_Click);
            this.Controls.Add(this.btnDetalhes);
            this.grade = new System.Windows.Forms.DataGridView();
            this.grade.Name = "grade";
            this.grade.Location = new System.Drawing.Point(22, 198);
            this.grade.Size = new System.Drawing.Size(1076, 446);
            this.grade.TabIndex = 8;
            this.grade.ReadOnly = true;
            this.grade.AllowUserToAddRows = false;
            this.grade.AllowUserToDeleteRows = false;
            this.grade.MultiSelect = false;
            this.grade.RowHeadersVisible = false;
            this.grade.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grade.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grade.BackgroundColor = System.Drawing.Color.White;
            this.grade.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.grade);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ClientSize = new System.Drawing.Size(1120, 680);
            this.MinimumSize = new System.Drawing.Size(1120, 680);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ordens de serviço";
            this.Name = "OrdensForm";
            this.Load += new System.EventHandler(this.OrdensForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.Button btnPesquisar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Label lblDica;
        private System.Windows.Forms.Button btnNova;
        private System.Windows.Forms.Button btnDetalhes;
        private System.Windows.Forms.DataGridView grade;
    }
}
