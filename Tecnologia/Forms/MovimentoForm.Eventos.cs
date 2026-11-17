using System;

namespace Tecnologia
{
    public partial class MovimentoForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            flowlayoutpanel7_Redimensionar(this, EventArgs.Empty);
        }

        private void flowlayoutpanel7_Redimensionar(object sender, EventArgs e)
        {
            int largura = Tela.LarguraEditor(panel6, flowlayoutpanel7);
            lblPeca.Width = largura;
            label8.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel9, largura);
            lblDica.Width = largura;
        }
    }
}
