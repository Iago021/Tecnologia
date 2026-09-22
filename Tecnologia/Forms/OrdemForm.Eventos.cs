using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class OrdemForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            Redondear(btnSalvar);
            Redondear(btnAtualizar);
            Redondear(btnAdicionar);
            Redondear(btnDevolver);
            Redondear(btnExportar);
            Redondear(btnEntregar);
            Redondear(btnHistorico);
            flowlayoutpanel6_Redimensionar(this, EventArgs.Empty);
            flowlayoutpanel9_Redimensionar(this, EventArgs.Empty);
            flowlayoutpanel26_Redimensionar(this, EventArgs.Empty);
        }

        private void ControlRedimensionado(object sender, EventArgs e) { Redondear((Control)sender); }
        private void Redondear(Control control)
        {
            if (control.Width < 2 || control.Height < 2) return;
            int d = Math.Min(30, Math.Min(control.Width, control.Height));
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(0, 0, d, d, 180, 90);
                path.AddArc(control.Width-d, 0, d, d, 270, 90);
                path.AddArc(control.Width-d, control.Height-d, d, d, 0, 90);
                path.AddArc(0, control.Height-d, d, d, 90, 90); path.CloseFigure();
                Region previous = control.Region; control.Region = new Region(path);
                if (previous != null) previous.Dispose();
            }
        }

        private void flowlayoutpanel6_Redimensionar(object sender, EventArgs e)
        {
            int width = Math.Max(360, panel5.ClientSize.Width - 28);
            flowlayoutpanel6.MinimumSize = new Size(width, 0);
            flowlayoutpanel6.MaximumSize = new Size(width, 0);
            txtRecebimento.Width = width - 26;
            lblDica.Width = width - 26;
        }

        private void flowlayoutpanel9_Redimensionar(object sender, EventArgs e)
        {
            int width = Math.Max(360, panel8.ClientSize.Width - 28);
            flowlayoutpanel9.MinimumSize = new Size(width, 0);
            flowlayoutpanel9.MaximumSize = new Size(width, 0);
            label10.Width = width - 26;
            flowlayoutpanel11.Width = width - 26;
            flowlayoutpanel11.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel11.MaximumSize = new Size(width - 26, 0);
            panel12.Width = Math.Max(300, (flowlayoutpanel11.Width - 24) / (flowlayoutpanel11.Width >= 680 ? 2 : 1));
            panel13.Width = Math.Max(300, (flowlayoutpanel11.Width - 24) / (flowlayoutpanel11.Width >= 680 ? 2 : 1));
            label14.Width = width - 26;
            flowlayoutpanel15.Width = width - 26;
            flowlayoutpanel15.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel15.MaximumSize = new Size(width - 26, 0);
            panel16.Width = Math.Max(300, (flowlayoutpanel15.Width - 24) / (flowlayoutpanel15.Width >= 680 ? 2 : 1));
            panel17.Width = Math.Max(300, (flowlayoutpanel15.Width - 24) / (flowlayoutpanel15.Width >= 680 ? 2 : 1));
            panel18.Width = Math.Max(300, (flowlayoutpanel15.Width - 24) / (flowlayoutpanel15.Width >= 680 ? 2 : 1));
            flowlayoutpanel19.Width = width - 26;
            flowlayoutpanel19.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel19.MaximumSize = new Size(width - 26, 0);
        }

        private void flowlayoutpanel26_Redimensionar(object sender, EventArgs e)
        {
            int width = Math.Max(360, panel25.ClientSize.Width - 28);
            flowlayoutpanel26.MinimumSize = new Size(width, 0);
            flowlayoutpanel26.MaximumSize = new Size(width, 0);
            lblTotal.Width = width - 26;
            label27.Width = width - 26;
            flowlayoutpanel28.Width = width - 26;
            flowlayoutpanel28.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel28.MaximumSize = new Size(width - 26, 0);
        }
    }
}
