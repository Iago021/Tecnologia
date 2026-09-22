using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tecnologia
{
    // Controla alterações pendentes; não cria nem estiliza controles.
    internal static class AlteracoesFormulario
    {
        private sealed class Registro { public Registro() { } public string Estado; }
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Form, Registro> registros =
            new System.Runtime.CompilerServices.ConditionalWeakTable<Form, Registro>();
        public static void MarcarSalvo(Form janela) { registros.GetOrCreateValue(janela).Estado = Estado(janela); }
        public static bool TemAlteracoes(Form janela)
        {
            Registro registro;
            return registros.TryGetValue(janela, out registro) && registro.Estado != Estado(janela);
        }
        public static string Estado(Control raiz)
        {
            System.Text.StringBuilder estado = new System.Text.StringBuilder();
            foreach (Control c in raiz.Controls)
            {
                if (c.Name == "txtBusca" || c.Name == "cmbCampo" || (c.Name == "cmbStatus" && c.FindForm() is OrdensForm)) continue;
                if (c is TextBox && !((TextBox)c).ReadOnly) estado.Append(c.Name).Append(':').Append(c.Text.Length).Append(':').Append(c.Text).Append(';');
                else if (c is ComboBox) estado.Append(c.Name).Append(':').Append(((ComboBox)c).SelectedIndex).Append(';');
                else if (c is CheckBox) estado.Append(c.Name).Append(':').Append(((CheckBox)c).Checked).Append(';');
                else if (c is NumericUpDown) estado.Append(c.Name).Append(':').Append(((NumericUpDown)c).Value).Append(';');
                else if (c is DateTimePicker)
                {
                    DateTimePicker data = (DateTimePicker)c;
                    estado.Append(c.Name).Append(':').Append(data.Checked ? data.Value.Date.ToString("yyyy-MM-dd") : "vazio").Append(';');
                }
                if (c is Panel || c is TabControl || c is TabPage || c is GroupBox) estado.Append(Estado(c));
            }
            return estado.ToString();
        }

        public static void Observar(Form janela)
        {
            janela.Load += delegate { if (!janela.IsDisposed) MarcarSalvo(janela); };
            janela.FormClosing += delegate(object sender, FormClosingEventArgs e)
            {
                if (TemAlteracoes(janela) && !Tela.Confirmar("Sair sem salvar as alterações desta tela?")) e.Cancel = true;
            };
        }
    }
}
