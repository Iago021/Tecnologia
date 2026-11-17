using System;

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
            int largura = Tela.LarguraEditor(panel9, flowlayoutpanel10);
            lblEdicao.Width = largura;
            label11.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel12, largura);
            label19.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel20, largura);
        }
    }
}
