using System;
using System.Data;
using System.Drawing;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    internal static class Tela
    {
        public static async Task CarregarAsync<T>(Form janela, Control area, string perfil,
            Func<DataRow, T> consultar, Action<T> exibir, bool fecharAoFalhar = false)
        {
            if (!area.Enabled) return;

            int usuario = Sessao.Id;
            area.Enabled = false;
            area.UseWaitCursor = true;
            try
            {
                // Load também pode ocorrer durante a criação dos controles da janela.
                await Task.Yield();
                T dados = await Task.Run(() => consultar(Sessao.ConsultarAtiva(usuario, perfil)));
                if (!janela.IsDisposed && Sessao.Id == usuario) exibir(dados);
            }
            catch (Exception erro)
            {
                if (!janela.IsDisposed && area.Visible && Sessao.Id == usuario)
                {
                    MessageBox.Show(janela, Mensagem(erro), "Tecnologia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (fecharAoFalhar) janela.Close();
                }
            }
            finally
            {
                if (!area.IsDisposed)
                {
                    area.UseWaitCursor = false;
                    area.Enabled = true;
                }
            }
        }

        public static void Erro(Exception erro)
        {
            MessageBox.Show(Mensagem(erro), "Tecnologia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static string Mensagem(Exception erro)
        {
            string mensagem = erro.Message;
            MySqlException mysql = erro as MySqlException;
            if (mysql != null)
            {
                if (mysql.Number == 1062) mensagem = "Já existe um cadastro com este e-mail, CPF ou código.";
                else if (mysql.Number == 1451) mensagem = "Este registro já está em uso. Preserve o histórico e desative o cadastro quando essa opção estiver disponível.";
                else if (mysql.Number == 1452) mensagem = "O cadastro relacionado não existe mais. Atualize a lista.";
                else if (mysql.Number == 1205 || mysql.Number == 1213) mensagem = "Os dados estão em uso. Aguarde um instante e tente novamente.";
                else if (mysql.Number == 1049 || mysql.Number == 1146) mensagem = "Importe o arquivo Banco/tecnologia.sql antes de usar o sistema.";
                else if (mysql.Number == 0 || mysql.Number == 1045 || mysql.Number == 1042) mensagem = "Não foi possível acessar o banco. Inicie o MySQL e confira a conexão em App.config.";
            }
            return mensagem;
        }
        public static void Obrigatorio(string valor, string campo)
        { if (string.IsNullOrWhiteSpace(valor)) throw new Exception("Preencha o campo " + campo + "."); }
        public static void Email(string valor, bool obrigatorio)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                if (!obrigatorio) return;
                throw new InvalidOperationException("Preencha o campo E-mail.");
            }
            try { if (new MailAddress(valor).Address != valor) throw new FormatException(); }
            catch (FormatException) { throw new InvalidOperationException("Informe um e-mail válido."); }
        }
        public static object Opcional(string valor)
        { return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor.Trim(); }
        public static string Cpf(string valor)
        {
            string numero = valor.Replace(".", "").Replace("-", "").Trim();
            if (numero == "") return null;
            if (!System.Text.RegularExpressions.Regex.IsMatch(numero, @"^[0-9]{11}$"))
                throw new InvalidOperationException("Informe o CPF com 11 números ou deixe em branco.");
            return numero;
        }

        public static DateTime? Nascimento(DateTimePicker campo)
        {
            if (!campo.Checked) return null;
            if (campo.Value.Date > DateTime.Today) throw new Exception("A data de nascimento não pode estar no futuro.");
            return campo.Value.Date;
        }
        public static void Combo(ComboBox combo, DataTable dados)
        { combo.DisplayMember = "nome"; combo.ValueMember = "id"; combo.DataSource = dados; }
        public static int Codigo(ComboBox combo)
        {
            if (combo.SelectedValue == null || combo.SelectedValue is DataRowView) throw new Exception("Selecione um cadastro na lista.");
            return Convert.ToInt32(combo.SelectedValue);
        }
        public static void AjustarGrade(DataGridView grade)
        {
            if (grade.Columns.Contains("data_cadastro")) grade.Columns["data_cadastro"].HeaderText = "Cadastro em";
            if (grade.Columns.Contains("quantidade")) grade.Columns["quantidade"].HeaderText = "Quantidade";

            grade.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            foreach (DataGridViewColumn coluna in grade.Columns)
            {
                if (!coluna.Visible) continue;
                coluna.MinimumWidth = coluna.Name == "id" ? 65 : 120;
                coluna.Width = Math.Min(coluna.Width, 320);
            }
        }

        public static int LarguraEditor(Panel area, FlowLayoutPanel editor)
        {
            int largura = Math.Max(360, area.ClientSize.Width - 28);
            editor.MinimumSize = new Size(largura, 0);
            editor.MaximumSize = new Size(largura, 0);
            return largura - 26;
        }

        public static void AjustarSecao(FlowLayoutPanel secao, int largura)
        {
            secao.MinimumSize = new Size(largura, 0);
            secao.MaximumSize = new Size(largura, 0);
            secao.Width = largura;
            int larguraCampo = Math.Max(300, (largura - 24) / (largura >= 680 ? 2 : 1));
            foreach (Control campo in secao.Controls)
                campo.Width = larguraCampo;
        }

        public static int Selecionado(DataGridView grade)
        {
            DataGridViewRow linha = grade.CurrentRow;
            if (linha == null || linha.IsNewRow || !grade.Columns.Contains("id"))
                throw new InvalidOperationException("Selecione uma linha na tabela.");
            return Convert.ToInt32(linha.Cells["id"].Value);
        }
        public static bool Confirmar(string texto)
        { return MessageBox.Show(texto, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes; }
    }
}
