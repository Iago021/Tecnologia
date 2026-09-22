using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class AbrirOrdemForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            Redondear(txtObservacoesFundo);
            Redondear(btnSalvar);
            flowlayoutpanel6_Redimensionar(this, EventArgs.Empty);
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
            label7.Width = width - 26;
            flowlayoutpanel8.Width = width - 26;
            flowlayoutpanel8.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel8.MaximumSize = new Size(width - 26, 0);
            panel9.Width = Math.Max(300, (flowlayoutpanel8.Width - 24) / (flowlayoutpanel8.Width >= 680 ? 2 : 1));
            panel10.Width = Math.Max(300, (flowlayoutpanel8.Width - 24) / (flowlayoutpanel8.Width >= 680 ? 2 : 1));
            label11.Width = width - 26;
            flowlayoutpanel12.Width = width - 26;
            flowlayoutpanel12.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel12.MaximumSize = new Size(width - 26, 0);
            panel13.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            panel14.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            panel15.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            flowlayoutpanel16.Width = width - 26;
            flowlayoutpanel16.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel16.MaximumSize = new Size(width - 26, 0);
        }
    }
}
