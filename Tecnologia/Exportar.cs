using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace Tecnologia
{
    internal static class Exportar
    {
        public static void Menu(Button botao, DataGridView grade, string titulo)
        {
            List<string[]> tabela = LerTabela(grade);
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Salvar CSV", null, delegate { Salvar(botao, tabela, titulo, "csv"); });
            menu.Items.Add("Salvar PDF", null, delegate { Salvar(botao, tabela, titulo, "pdf"); });
            menu.Items.Add("Abrir no Excel", null, delegate
            {
                try { Excel(tabela); }
                catch (Exception erro) { Tela.Erro(erro); }
            });
            menu.Closed += delegate
            {
                if (botao.IsHandleCreated && !botao.IsDisposed)
                    botao.BeginInvoke(new Action(menu.Dispose));
                else
                    menu.Dispose();
            };
            menu.Show(botao, new Point(0, botao.Height), ToolStripDropDownDirection.BelowRight);
        }

        internal static List<string[]> LerTabela(DataGridView grade)
        {
            List<DataGridViewColumn> colunas = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn coluna in grade.Columns)
                if (coluna.Visible) colunas.Add(coluna);
            colunas.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));

            string[] cabecalho = new string[colunas.Count];
            for (int i = 0; i < colunas.Count; i++)
                cabecalho[i] = colunas[i].HeaderText;

            List<string[]> tabela = new List<string[]> { cabecalho };
            foreach (DataGridViewRow linha in grade.Rows)
            {
                if (linha.IsNewRow || !linha.Visible) continue;
                string[] valores = new string[colunas.Count];
                for (int i = 0; i < colunas.Count; i++)
                    valores[i] = Convert.ToString(linha.Cells[colunas[i].Index].FormattedValue);
                tabela.Add(valores);
            }

            if (colunas.Count == 0 || tabela.Count == 1)
                throw new InvalidOperationException("Não há registros para exportar.");
            return tabela;
        }

        private static void Salvar(Control dono, List<string[]> tabela, string titulo, string formato)
        {
            try
            {
                using (SaveFileDialog janela = new SaveFileDialog())
                {
                    janela.Filter = formato == "csv" ? "Arquivo CSV (*.csv)|*.csv" : "Arquivo PDF (*.pdf)|*.pdf";
                    janela.DefaultExt = formato;
                    janela.FileName = titulo;
                    janela.RestoreDirectory = true;
                    if (janela.ShowDialog(dono) != DialogResult.OK) return;

                    if (formato == "csv")
                        File.WriteAllText(janela.FileName, GerarCsv(tabela), new UTF8Encoding(true));
                    else
                        File.WriteAllBytes(janela.FileName, ExportacaoPdf.Gerar(tabela, titulo));
                }
            }
            catch (Exception erro) { Tela.Erro(erro); }
        }

        internal static string GerarCsv(List<string[]> tabela)
        {
            StringBuilder csv = new StringBuilder();
            foreach (string[] linha in tabela)
            {
                for (int i = 0; i < linha.Length; i++)
                {
                    if (i > 0) csv.Append(';');
                    string celula = linha[i] ?? "";
                    string inicio = celula.TrimStart();
                    // Uma planilha pode interpretar texto iniciado por estes sinais como fórmula.
                    if (inicio.Length > 0 && "=+-@".IndexOf(inicio[0]) >= 0)
                        celula = "'" + celula;
                    csv.Append('"').Append(celula.Replace("\"", "\"\"")).Append('"');
                }
                csv.Append("\r\n");
            }
            return csv.ToString();
        }

        private static void Excel(List<string[]> tabela)
        {
            Excel.Application excel = null;
            Excel.Workbooks livros = null;
            Excel.Workbook livro = null;
            Excel.Worksheet folha = null;
            Excel.Range intervalo = null;
            Excel.Range colunas = null;
            bool abriu = false;
            try
            {
                excel = new Excel.Application();
                livros = excel.Workbooks;
                livro = livros.Add();
                folha = (Excel.Worksheet)livro.ActiveSheet;
                int quantidade = tabela[0].Length;
                object[,] dados = new object[tabela.Count, quantidade];
                for (int linha = 0; linha < tabela.Count; linha++)
                    for (int coluna = 0; coluna < quantidade; coluna++)
                        dados[linha, coluna] = tabela[linha][coluna];
                string ultima = "";
                for (int n = quantidade; n > 0; n = (n - 1) / 26) ultima = (char)('A' + (n - 1) % 26) + ultima;
                intervalo = folha.get_Range("A1", ultima + tabela.Count);
                intervalo.NumberFormat = "@"; // Texto também evita interpretar valores como fórmulas.
                intervalo.Value2 = dados;
                colunas = intervalo.EntireColumn;
                colunas.AutoFit();
                excel.Visible = true;
                abriu = true;
            }
            catch (COMException) { throw new Exception("Para exportar, instale o Microsoft Excel para desktop no Windows."); }
            finally
            {
                if (!abriu && excel != null) excel.Quit();
                if (colunas != null) Marshal.ReleaseComObject(colunas);
                if (intervalo != null) Marshal.ReleaseComObject(intervalo);
                if (folha != null) Marshal.ReleaseComObject(folha);
                if (livro != null) Marshal.ReleaseComObject(livro);
                if (livros != null) Marshal.ReleaseComObject(livros);
                if (excel != null) Marshal.ReleaseComObject(excel);
            }
        }
    }
}
