using System;

namespace Tecnologia
{
    public partial class AbrirOrdemForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            flowlayoutpanel6_Redimensionar(this, EventArgs.Empty);
        }

        private void flowlayoutpanel6_Redimensionar(object sender, EventArgs e)
        {
            int largura = Tela.LarguraEditor(panel5, flowlayoutpanel6);
            label7.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel8, largura);
            label11.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel12, largura);
        }
    }
}
