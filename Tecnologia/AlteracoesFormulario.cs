using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace Tecnologia
{
    internal static class AlteracoesFormulario
    {
        private sealed class Registro
        {
            public string Estado;
        }

        private static readonly ConditionalWeakTable<Form, Registro> registros =
            new ConditionalWeakTable<Form, Registro>();

        public static void Observar(Form janela)
        {
            janela.Load += delegate
            {
                if (!janela.IsDisposed) MarcarSalvo(janela);
            };
            janela.FormClosing += delegate(object sender, FormClosingEventArgs e)
            {
                if (!e.Cancel && !PodeDescartar(janela)) e.Cancel = true;
            };
        }

        public static void MarcarSalvo(Form janela)
        {
            registros.GetValue(janela, _ => new Registro()).Estado = Estado(janela);
        }

        public static bool TemAlteracoes(Form janela)
        {
            Registro salvo;
            return registros.TryGetValue(janela, out salvo) && salvo.Estado != Estado(janela);
        }

        public static bool PodeDescartar(Form janela)
        {
            return janela.IsDisposed || !TemAlteracoes(janela) ||
                Tela.Confirmar("Descartar as alterações que ainda não foram salvas?");
        }

        private static string Estado(Form janela)
        {
            StringBuilder estado = new StringBuilder();
            LerCampos(janela, estado);
            return estado.ToString();
        }

        private static void LerCampos(Control raiz, StringBuilder estado)
        {
            foreach (Control controle in raiz.Controls)
            {
                if (controle.Name == "txtBusca" || controle.Name == "cmbCampo" ||
                    (controle.Name == "cmbStatus" && controle.FindForm() is OrdensForm) ||
                    controle.Name == "cmbPeca" || controle.Name == "numQuantidade" && controle.FindForm() is OrdemForm)
                    continue;

                string valor = ValorCampo(controle);
                if (valor != null)
                    estado.Append(controle.Name).Append(':').Append(valor.Length).Append(':').Append(valor).Append(';');
                if (controle is Panel || controle is TabControl || controle is GroupBox)
                    LerCampos(controle, estado);
            }
        }

        private static string ValorCampo(Control controle)
        {
            if (controle is TextBox texto && !texto.ReadOnly) return texto.Text;
            if (controle is ComboBox lista) return Convert.ToString(lista.SelectedValue ?? lista.SelectedItem);
            if (controle is CheckBox caixa) return caixa.Checked.ToString();
            if (controle is NumericUpDown numero) return numero.Value.ToString();
            if (controle is DateTimePicker data) return data.Checked ? data.Value.ToString("yyyy-MM-dd") : "";
            return null;
        }
    }
}
