using System;
using System.Data;
using System.Net.Mail;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    internal static class Tela
    {
        public static void Erro(Exception erro)
        {
            string mensagem = erro.Message;
            MySqlException mysql = erro as MySqlException;
            if (mysql != null)
            {
                if (mysql.Number == 1062) mensagem = "Já existe um cadastro com este e-mail, CPF ou código.";
                else if (mysql.Number == 1451) mensagem = "Este registro já está em uso. Preserve o histórico e desative o cadastro quando essa opção estiver disponível.";
                else if (mysql.Number == 1452) mensagem = "O cadastro relacionado não existe mais. Atualize a lista.";
                else if (mysql.Number == 1049 || mysql.Number == 1146) mensagem = "Importe o arquivo Banco/tecnologia.sql antes de usar o sistema.";
                else if (mysql.Number == 0 || mysql.Number == 1045 || mysql.Number == 1042) mensagem = "Não foi possível acessar o banco. Inicie o MySQL e confira a conexão em App.config.";
            }
            MessageBox.Show(mensagem, "Tecnologia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Obrigatorio(string valor, string campo)
        { if (string.IsNullOrWhiteSpace(valor)) throw new Exception("Preencha o campo " + campo + "."); }
        public static void Email(string valor, bool obrigatorio)
        {
            if (!obrigatorio && string.IsNullOrWhiteSpace(valor)) return;
            try { if (new MailAddress(valor).Address != valor) throw new FormatException(); }
            catch { throw new Exception("Informe um e-mail válido."); }
        }
        public static object Opcional(string valor)
        { return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor.Trim(); }
        public static object Nascimento(DateTimePicker campo)
        {
            if (!campo.Checked) return DBNull.Value;
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
            grade.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            foreach (DataGridViewColumn coluna in grade.Columns)
                coluna.MinimumWidth = coluna.Name == "id" ? 65 : 120;
            if (grade.Columns.Contains("data_cadastro")) grade.Columns["data_cadastro"].HeaderText = "Cadastro em";
            if (grade.Columns.Contains("quantidade")) grade.Columns["quantidade"].HeaderText = "Quantidade";
        }
        public static int Selecionado(DataGridView grade)
        {
            if (grade.CurrentRow == null) throw new Exception("Selecione uma linha na tabela.");
            return Convert.ToInt32(grade.CurrentRow.Cells["id"].Value);
        }
        public static bool Confirmar(string texto)
        { return MessageBox.Show(texto, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes; }
    }
}
