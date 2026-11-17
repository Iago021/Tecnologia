using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class PerfilForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            flowlayoutpanel6_Redimensionar(this, EventArgs.Empty);
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
            panel11.Width = Math.Max(300, (flowlayoutpanel8.Width - 24) / (flowlayoutpanel8.Width >= 680 ? 2 : 1));
            label12.Width = width - 26;
            flowlayoutpanel13.Width = width - 26;
            flowlayoutpanel13.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel13.MaximumSize = new Size(width - 26, 0);
            panel14.Width = Math.Max(300, (flowlayoutpanel13.Width - 24) / (flowlayoutpanel13.Width >= 680 ? 2 : 1));
            panel15.Width = Math.Max(300, (flowlayoutpanel13.Width - 24) / (flowlayoutpanel13.Width >= 680 ? 2 : 1));
            panel16.Width = Math.Max(300, (flowlayoutpanel13.Width - 24) / (flowlayoutpanel13.Width >= 680 ? 2 : 1));
        }
    }
}
