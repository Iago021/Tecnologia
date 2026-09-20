namespace Tecnologia
{
    partial class HistoricoForm
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
            this.lblTitulo.Text = "Histórico";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.lblTitulo);
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Location = new System.Drawing.Point(22, 52);
            this.btnExportar.Size = new System.Drawing.Size(180, 34);
            this.btnExportar.TabIndex = 1;
            this.btnExportar.Text = "Exportar Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            this.Controls.Add(this.btnExportar);
            this.grade = new System.Windows.Forms.DataGridView();
            this.grade.Name = "grade";
            this.grade.Location = new System.Drawing.Point(22, 105);
            this.grade.Size = new System.Drawing.Size(1076, 540);
            this.grade.TabIndex = 2;
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
            this.Text = "Histórico";
            this.Name = "HistoricoForm";
            this.Load += new System.EventHandler(this.HistoricoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.DataGridView grade;
    }
}
