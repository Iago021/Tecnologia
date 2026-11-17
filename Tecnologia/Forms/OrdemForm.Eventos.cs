using System;
using System.Drawing;

namespace Tecnologia
{
    public partial class OrdemForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            flowlayoutpanel6_Redimensionar(this, EventArgs.Empty);
            flowlayoutpanel9_Redimensionar(this, EventArgs.Empty);
            flowlayoutpanel26_Redimensionar(this, EventArgs.Empty);
        }

        private void flowlayoutpanel6_Redimensionar(object sender, EventArgs e)
        {
            int largura = Tela.LarguraEditor(panel5, flowlayoutpanel6);
            painelTecnico.MaximumSize = new Size(largura, 0);
            txtRecebimento.Width = largura;
            lblDica.Width = largura;
        }

        private void flowlayoutpanel9_Redimensionar(object sender, EventArgs e)
        {
            int largura = Tela.LarguraEditor(panel8, flowlayoutpanel9);
            label10.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel11, largura);
            label14.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel15, largura);
        }

        private void flowlayoutpanel26_Redimensionar(object sender, EventArgs e)
        {
            int largura = Tela.LarguraEditor(panel25, flowlayoutpanel26);
            lblTotal.Width = largura;
            label27.Width = largura;
        }
    }
}
