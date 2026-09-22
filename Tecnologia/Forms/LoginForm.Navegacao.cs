using System;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class LoginForm
    {
        private DialogResult AbrirAcesso(Form janela)
        {
            Hide();
            try { return janela.ShowDialog(this); }
            finally { if (!IsDisposed) Show(); }
        }
    }
}
