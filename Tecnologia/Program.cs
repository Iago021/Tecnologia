using System;
using System.Windows.Forms;
namespace Tecnologia
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Ao sair da conta, o formulário de login é aberto novamente.
            while (true)
            {
                Sessao.Limpar();
                using (LoginForm login = new LoginForm())
                    if (login.ShowDialog() != DialogResult.OK) return;
                using (PrincipalForm principal = new PrincipalForm())
                    Application.Run(principal);
                if (!Sessao.TrocarUsuario) return;
            }
        }
    }
}
