using System;

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
            int largura = Tela.LarguraEditor(panel9, flowlayoutpanel10);
            lblEdicao.Width = largura;
            label11.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel12, largura);
            label17.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel18, largura);
            label22.Width = largura;
            Tela.AjustarSecao(flowlayoutpanel23, largura);
        }
    }
}
