using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class PrincipalForm
    {
        // Eventos locais desta tela; cores e controles ficam no respectivo Designer.
        private void PrepararTela()
        {
            MarcarNavegacao(btnAtualizar);
        }

        private void Cadastros_Click(object sender, EventArgs e) { MostrarCadastros(); }
    }
}
