using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace Tecnologia
{
    internal static class Exportar
    {
        // Mesmo recurso do ZIP. O Excel só é iniciado quando o botão é clicado.
        public static void Excel(DataGridView grade)
        {
            if (grade.Rows.Count == 0) throw new Exception("Não há registros para exportar.");
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
                int quantidade = 0;
                foreach (DataGridViewColumn coluna in grade.Columns) if (coluna.Visible) quantidade++;
                object[,] dados = new object[grade.Rows.Count + 1, quantidade];
                int destino = 0;
                foreach (DataGridViewColumn coluna in grade.Columns)
                {
                    if (!coluna.Visible) continue;
                    dados[0, destino] = coluna.HeaderText;
                    for (int i = 0; i < grade.Rows.Count; i++)
                        dados[i + 1, destino] = Convert.ToString(grade.Rows[i].Cells[coluna.Index].FormattedValue);
                    destino++;
                }
                string ultima = "";
                for (int n = quantidade; n > 0; n = (n - 1) / 26) ultima = (char)('A' + (n - 1) % 26) + ultima;
                intervalo = folha.get_Range("A1", ultima + (grade.Rows.Count + 1));
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
