using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class MovimentoForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            Redondear(txtMotivoFundo);
            Redondear(btnSalvar);
            flowlayoutpanel7_Redimensionar(this, EventArgs.Empty);
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

        private void flowlayoutpanel7_Redimensionar(object sender, EventArgs e)
        {
            int width = Math.Max(360, panel6.ClientSize.Width - 28);
            flowlayoutpanel7.MinimumSize = new Size(width, 0);
            flowlayoutpanel7.MaximumSize = new Size(width, 0);
            lblPeca.Width = width - 26;
            label8.Width = width - 26;
            flowlayoutpanel9.Width = width - 26;
            flowlayoutpanel9.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel9.MaximumSize = new Size(width - 26, 0);
            panel10.Width = Math.Max(300, (flowlayoutpanel9.Width - 24) / (flowlayoutpanel9.Width >= 680 ? 2 : 1));
            panel11.Width = Math.Max(300, (flowlayoutpanel9.Width - 24) / (flowlayoutpanel9.Width >= 680 ? 2 : 1));
            panel12.Width = Math.Max(300, (flowlayoutpanel9.Width - 24) / (flowlayoutpanel9.Width >= 680 ? 2 : 1));
            lblDica.Width = width - 26;
        }
    }
}
