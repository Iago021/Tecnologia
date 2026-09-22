using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class HistoricoForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            Redondear(btnExportar);
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
    }
}
