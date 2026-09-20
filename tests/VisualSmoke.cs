using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

// Executado em Windows, sem conexão com o banco e sem enviar e-mails.
internal static class VisualSmoke
{
    [STAThread]
    private static int Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Assembly app = Assembly.LoadFrom(Path.GetFullPath(args[0]));
        string output = Path.GetFullPath(args[1]); Directory.CreateDirectory(output);
        string[] names = { "LoginForm", "CriarContaForm", "RecuperarSenhaForm", "RedefinirSenhaForm", "PrincipalForm",
            "ClientesForm", "UsuariosForm", "AparelhosForm", "PecasForm", "OrdensForm", "OrdemForm", "AbrirOrdemForm", "MovimentoForm", "HistoricoForm", "PerfilForm" };
        int assertions = 0;
        foreach (string name in names)
        {
            Type type = app.GetType("Tecnologia." + name, true);
            object[] ctorArgs = name == "RecuperarSenhaForm" || name == "RedefinirSenhaForm" ? new object[] { "pessoa@example.com" } : new object[0];
            using (Form form = (Form)Activator.CreateInstance(type, ctorArgs))
            {
                // Não chama Show/ShowDialog: os eventos Load do aplicativo acessam o MySQL.
                form.CreateControl(); form.PerformLayout();
                List<Control> all = Descendants(form).ToList();
                foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    Control control = field.GetValue(form) as Control;
                    if (control == null || control.IsDisposed) continue;
                    Assert(all.Contains(control), name + ": controle desconectado: " + field.Name); assertions++;
                }
                foreach (TabControl tabs in all.OfType<TabControl>())
                {
                    for (int i = 0; i < tabs.TabCount; i++)
                    {
                        tabs.SelectedIndex = i; tabs.PerformLayout(); form.PerformLayout();
                        Save(form, Path.Combine(output, name + "-tab-" + i + ".png"));
                    }
                    tabs.SelectedIndex = 0;
                }
                Save(form, Path.Combine(output, name + ".png"));
                form.ClientSize = new Size(900, 620); form.PerformLayout();
                Assert(form.Controls.Count > 0, name + ": tela vazia"); assertions++;
                foreach (var input in Descendants(form).OfType<TextBox>())
                {
                    Assert(input.Width >= 100 && input.Height >= 15, name + ": campo cortado: " + input.Name); assertions++;
                }
                Console.WriteLine("PASS " + name);
            }
        }
        Type contas = app.GetType("Tecnologia.Contas", true);
        var generate = contas.GetMethod("GerarCodigo", BindingFlags.NonPublic | BindingFlags.Static);
        for (int i = 0; i < 100; i++)
        {
            string code = (string)generate.Invoke(null, null);
            Assert(code.Length == 6 && code.All(char.IsDigit), "Formato do código"); assertions++;
        }
        var hash = contas.GetMethod("HashCodigo", BindingFlags.NonPublic | BindingFlags.Static);
        Assert(!Equals(hash.Invoke(null, new object[] { "salt1", "123456" }), hash.Invoke(null, new object[] { "salt2", "123456" })), "Salt deve alterar o hash"); assertions++;
        var compare = contas.GetMethod("Comparar", BindingFlags.NonPublic | BindingFlags.Static);
        Assert((bool)compare.Invoke(null, new object[] { "abc", "abc" }), "Comparação igual"); assertions++;
        Assert(!(bool)compare.Invoke(null, new object[] { "abc", "abd" }), "Comparação diferente"); assertions++;
        Console.WriteLine("PASS: " + assertions + " verificações; 15 formulários construídos. Sem teste de banco ou entrega SMTP.");
        return 0;
    }
    private static IEnumerable<Control> Descendants(Control parent)
    {
        foreach (Control c in parent.Controls) { yield return c; foreach (Control child in Descendants(c)) yield return child; }
    }
    private static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Save(Form form, string path)
    {
        using (Bitmap image = new Bitmap(form.Width, form.Height)) { form.DrawToBitmap(image, form.ClientRectangle); image.Save(path); }
    }
}
