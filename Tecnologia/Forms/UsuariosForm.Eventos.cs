using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class UsuariosForm
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
            label17.Width = width - 26;
            flowlayoutpanel18.Width = width - 26;
            flowlayoutpanel18.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel18.MaximumSize = new Size(width - 26, 0);
            panel19.Width = Math.Max(300, (flowlayoutpanel18.Width - 24) / (flowlayoutpanel18.Width >= 680 ? 2 : 1));
            panel20.Width = Math.Max(300, (flowlayoutpanel18.Width - 24) / (flowlayoutpanel18.Width >= 680 ? 2 : 1));
            panel21.Width = Math.Max(300, (flowlayoutpanel18.Width - 24) / (flowlayoutpanel18.Width >= 680 ? 2 : 1));
            label22.Width = width - 26;
            flowlayoutpanel23.Width = width - 26;
            flowlayoutpanel23.MinimumSize = new Size(width - 26, 0);
            flowlayoutpanel23.MaximumSize = new Size(width - 26, 0);
            panel24.Width = Math.Max(300, (flowlayoutpanel23.Width - 24) / (flowlayoutpanel23.Width >= 680 ? 2 : 1));
            panel25.Width = Math.Max(300, (flowlayoutpanel23.Width - 24) / (flowlayoutpanel23.Width >= 680 ? 2 : 1));
            panel26.Width = Math.Max(300, (flowlayoutpanel23.Width - 24) / (flowlayoutpanel23.Width >= 680 ? 2 : 1));
        }

        private void NovoCadastro_Click(object sender, EventArgs e) { tabcontrol3.SelectedIndex = 1; }

        private void EditarCadastro_Click(object sender, EventArgs e) { if (lblEdicao.Text.StartsWith("Editando")) tabcontrol3.SelectedIndex = 1; }

        private void VoltarConsulta_Click(object sender, EventArgs e) { tabcontrol3.SelectedIndex = 0; }
    }
}
