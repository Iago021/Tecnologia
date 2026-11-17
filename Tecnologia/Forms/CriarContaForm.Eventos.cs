using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class CriarContaForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            CentralizarAcesso(this, EventArgs.Empty);
        }

        private void CentralizarAcesso(object sender, EventArgs e)
        {
            int width = Math.Max(440, ClientSize.Width);
            picturebox1.Location = new Point((width - picturebox1.Width) / 2 + AutoScrollPosition.X, 24 + AutoScrollPosition.Y);
            cartaoAcesso.Location = new Point((width - cartaoAcesso.Width) / 2 + AutoScrollPosition.X, 146 + AutoScrollPosition.Y);
            AutoScrollMinSize = new Size(440, cartaoAcesso.Height + 172);
        }

        private void linklabel13_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
