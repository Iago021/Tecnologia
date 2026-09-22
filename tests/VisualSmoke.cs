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
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
        Assembly app = Assembly.LoadFrom(Path.GetFullPath(args[0]));
        string output = Path.GetFullPath(args[1]); Directory.CreateDirectory(output);
        string[] names = { "LoginForm", "CriarContaForm", "RecuperarSenhaForm", "RedefinirSenhaForm", "PrincipalForm",
            "ClientesForm", "UsuariosForm", "AparelhosForm", "PecasForm", "OrdensForm", "OrdemForm", "AbrirOrdemForm", "MovimentoForm", "HistoricoForm", "PerfilForm" };
        int assertions = 0;
        Assert(app.GetType("Tecnologia.Tema") == null && app.GetType("Tecnologia.LayoutTelas") == null && app.GetType("Tecnologia.AcessoLayout") == null, "As telas não podem depender do tema global"); assertions++;
        foreach (string name in names)
        {
            Type type = app.GetType("Tecnologia." + name, true);
            object[] ctorArgs = name == "RecuperarSenhaForm" || name == "RedefinirSenhaForm" ? new object[] { "pessoa@example.com" } : new object[0];
            using (Form form = (Form)Activator.CreateInstance(type, ctorArgs))
            {
                // Remove somente no teste o Load que consulta o banco e mostra os controles reais.
                var loadKey = typeof(Form).GetField("EVENT_LOAD", BindingFlags.Static | BindingFlags.NonPublic);
                var eventsProperty = typeof(System.ComponentModel.Component).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
                var events = (System.ComponentModel.EventHandlerList)eventsProperty.GetValue(form, null);
                object key = loadKey.GetValue(null);
                events.RemoveHandler(key, events[key]);
                form.Show(); Application.DoEvents(); form.PerformLayout();
                form.Location = Point.Empty;
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
                        tabs.SelectedIndex = i; tabs.PerformLayout(); form.PerformLayout(); Application.DoEvents();
                        Save(form, Path.Combine(output, name + "-tab-" + i + ".png"));
                    }
                    tabs.SelectedIndex = 0;
                }
                Save(form, Path.Combine(output, name + ".png"));
                form.ClientSize = new Size(900, 620); form.PerformLayout();
                Assert(form.Controls.Count > 0, name + ": tela vazia"); assertions++;
                foreach (var input in Descendants(form).OfType<TextBox>())
                {
                    // O editor interno de NumericUpDown acompanha a largura do seletor numérico.
                    if (input.Parent is UpDownBase) continue;
                    Assert(input.Width >= 100 && input.Height >= 15, name + ": campo cortado: " + input.Name + " " + input.Size); assertions++;
                }
                Console.WriteLine("PASS " + name);
                InspectLayout(form, output, name + "-compacto");
                if (name != "PrincipalForm" && !name.Contains("Senha") && name != "LoginForm" && name != "CriarContaForm")
                {
                    using (Form host = new Form())
                    {
                        host.ClientSize = new Size(940, 460); host.Location = Point.Empty;
                        form.Hide(); form.TopLevel = false; form.FormBorderStyle = FormBorderStyle.None;
                        form.MinimumSize = Size.Empty; form.Dock = DockStyle.Fill;
                        host.Controls.Add(form); host.Show(); form.Show(); Application.DoEvents();
                        InspectLayout(form, output, name + "-embutido");
                        host.Controls.Remove(form);
                    }
                }
            }
        }
        foreach (string name in names)
        {
            using (var surface = new System.ComponentModel.Design.DesignSurface(app.GetType("Tecnologia." + name, true)))
            {
                Assert(surface.IsLoaded && surface.LoadErrors.Count == 0, name + ": falha ao abrir no host de design"); assertions++;
                var host = (System.ComponentModel.Design.IDesignerHost)surface.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
                Assert(((Form)host.RootComponent).Controls.Count > 0, name + ": Designer sem controles"); assertions++;
                Console.WriteLine("PASS DESIGNER " + name);
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
        ReviewSheet(output, names);
        return 0;
    }
    private static IEnumerable<Control> Descendants(Control parent)
    {
        foreach (Control c in parent.Controls) { yield return c; foreach (Control child in Descendants(c)) yield return child; }
    }
    private static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Save(Form form, string path)
    {
        using (Bitmap image = new Bitmap(form.Width, form.Height)) { form.DrawToBitmap(image, new Rectangle(Point.Empty, form.Size)); image.Save(path); }
    }
    private static void InspectLayout(Form form, string output, string prefix)
    {
        var tabs = Descendants(form).OfType<TabControl>().FirstOrDefault();
        for (int tab = 0; tab < (tabs == null ? 1 : tabs.TabCount); tab++)
        {
            if (tabs != null) tabs.SelectedIndex = tab;
            form.PerformLayout(); Application.DoEvents();
            foreach (Control c in Descendants(form).Where(c => c.Visible && c.Parent != null))
            {
                var parent = c.Parent as ScrollableControl;
                if (parent != null && parent.AutoScroll) continue;
                if (c is TabPage || c.Parent is UpDownBase || c is HScrollBar || c is VScrollBar) continue;
                if (c.Right > c.Parent.ClientSize.Width + 2 || c.Bottom > c.Parent.ClientSize.Height + 2 || c.Left < -2 || c.Top < -2)
                    Console.WriteLine("LAYOUT " + prefix + " tab=" + tab + " " + c.Name + " " + c.Bounds + " parent=" + c.Parent.Name + " " + c.Parent.ClientSize);
            }
            Save(form, Path.Combine(output, prefix + "-" + tab + ".png"));
            try
            {
                form.Refresh(); Application.DoEvents();
                using (Bitmap actual = new Bitmap(form.ClientSize.Width, form.ClientSize.Height))
                using (Graphics g = Graphics.FromImage(actual))
                {
                    g.CopyFromScreen(form.PointToScreen(Point.Empty), Point.Empty, form.ClientSize);
                    actual.Save(Path.Combine(output, prefix + "-" + tab + "-screen.png"));
                }
            }
            catch (System.ComponentModel.Win32Exception) { Console.WriteLine("Captura do desktop indisponível"); }
        }
    }
    private static void ReviewSheet(string folder, string[] names)
    {
        // Prancha sem dados de usuários para revisão remota da apresentação.
        using (Bitmap sheet = new Bitmap(1800, 2400))
        using (Graphics g = Graphics.FromImage(sheet))
        using (Font font = new Font("Segoe UI", 14))
        {
            g.Clear(Color.White);
            for (int i = 0; i < names.Length; i++)
            {
                int x = (i % 3) * 600, y = (i / 3) * 480;
                g.DrawString(names[i], font, Brushes.Black, x + 10, y + 8);
                string editor = Path.Combine(folder, names[i] + "-tab-1.png");
                using (Image screenshot = Image.FromFile(File.Exists(editor) ? editor : Path.Combine(folder, names[i] + ".png")))
                    g.DrawImage(screenshot, new Rectangle(x + 5, y + 38, 590, 420));
            }
            string path = Path.Combine(folder, "revisao.jpg");
            sheet.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            string data = Convert.ToBase64String(File.ReadAllBytes(path));
            Console.WriteLine("VISUAL_REVIEW_BEGIN");
            for (int i = 0; i < data.Length; i += 12000) Console.WriteLine("VISUAL_REVIEW_DATA:" + data.Substring(i, Math.Min(12000, data.Length - i)));
            Console.WriteLine("VISUAL_REVIEW_END");
        }
    }
}
