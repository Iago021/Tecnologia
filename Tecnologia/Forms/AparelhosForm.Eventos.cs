using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class AparelhosForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            flowlayoutpanel10_Redimensionar(this, EventArgs.Empty);
        }

        private void flowlayoutpanel10_Redimensionar(object sender, EventArgs e)
        {
            int width = Math.Max(360, panel9.ClientSize.Width - 28);
            flowlayoutpanel10.MinimumSize = new Size(width, 0);
            flowlayoutpanel10.MaximumSize = new Size(width, 0);
            lblEdicao.Width = width - 26;
            label11.Width = width - 26;
            flowlayoutpanel12.Width = width - 26;
            flowlayoutpanel12.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel12.MaximumSize = new Size(width - 26, 0);
            panel13.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            panel14.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            panel15.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            panel16.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            panel17.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            panel18.Width = Math.Max(300, (flowlayoutpanel12.Width - 24) / (flowlayoutpanel12.Width >= 680 ? 2 : 1));
            label19.Width = width - 26;
            flowlayoutpanel20.Width = width - 26;
            flowlayoutpanel20.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel20.MaximumSize = new Size(width - 26, 0);
            panel21.Width = Math.Max(300, (flowlayoutpanel20.Width - 24) / (flowlayoutpanel20.Width >= 680 ? 2 : 1));
            panel22.Width = Math.Max(300, (flowlayoutpanel20.Width - 24) / (flowlayoutpanel20.Width >= 680 ? 2 : 1));
            panel23.Width = Math.Max(300, (flowlayoutpanel20.Width - 24) / (flowlayoutpanel20.Width >= 680 ? 2 : 1));
        }

        private void NovoCadastro_Click(object sender, EventArgs e) { tabcontrol3.SelectedIndex = 1; }

        private void EditarCadastro_Click(object sender, EventArgs e) { if (lblEdicao.Text.StartsWith("Editando")) tabcontrol3.SelectedIndex = 1; }

        private void VoltarConsulta_Click(object sender, EventArgs e) { tabcontrol3.SelectedIndex = 0; }
    }
}
