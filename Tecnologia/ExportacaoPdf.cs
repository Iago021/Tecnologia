using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Tecnologia
{
    // Gera um PDF (A4 paisagem) com o conteúdo da tabela, sem precisar de nenhuma biblioteca extra.
    internal static class ExportacaoPdf
    {
        private const float LarguraPagina = 841.89f;
        private const float AlturaPagina = 595.28f;
        private const float Margem = 36f;
        private const float AlturaLinha = 18f;
        private const float AlturaCabecalho = 20f;
        private const float AlturaTitulo = 52f;
        private const float Rodape = 30f;
        private const float Preenchimento = 4f;

        internal static byte[] Gerar(List<string[]> tabela, string titulo)
        {
            string[] cabecalhos = tabela[0];
            List<string[]> linhas = tabela.GetRange(1, tabela.Count - 1);

            // Larguras das colunas (medidas com Arial, que tem as mesmas medidas da Helvetica)
            float larguraTabela = LarguraPagina - 2 * Margem;
            float[] larguras = new float[cabecalhos.Length];

            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Font normal = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point))
            using (Font negrito = new Font("Arial", 9f, FontStyle.Bold, GraphicsUnit.Point))
            {
                g.PageUnit = GraphicsUnit.Point;

                float total = 0;
                for (int c = 0; c < cabecalhos.Length; c++)
                {
                    float maior = Medir(g, negrito, cabecalhos[c]);
                    foreach (string[] linha in linhas)
                    {
                        float w = Medir(g, normal, linha[c]);
                        if (w > maior) maior = w;
                    }
                    larguras[c] = Math.Min(160f, Math.Max(45f, maior + 2 * Preenchimento));
                    total += larguras[c];
                }

                // Reserva espaço para todas as colunas antes de distribuir a sobra.
                float larguraMinima = Math.Min(45f, larguraTabela / larguras.Length);
                float sobra = larguraTabela - larguraMinima * larguras.Length;
                for (int c = 0; c < larguras.Length; c++)
                    larguras[c] = larguraMinima + sobra * larguras[c] / total;

                // Quebra em páginas
                List<int[]> paginas = new List<int[]>();
                int posicao = 0;
                while (posicao < linhas.Count)
                {
                    float disponivel = AlturaPagina - Margem - Rodape - AlturaCabecalho - AlturaTitulo;
                    int cabem = Math.Max(1, (int)(disponivel / AlturaLinha));
                    int quantas = Math.Min(cabem, linhas.Count - posicao);
                    paginas.Add(new int[] { posicao, quantas });
                    posicao += quantas;
                }

                // Monta o PDF
                Encoding latin1 = Encoding.GetEncoding("iso-8859-1");
                StringBuilder pdf = new StringBuilder();
                List<int> offsets = new List<int>();
                int totalObjetos = 4 + paginas.Count * 2;

                pdf.Append("%PDF-1.4\n");

                Action<int> iniciarObjeto = delegate (int numero)
                {
                    while (offsets.Count <= numero) offsets.Add(0);
                    offsets[numero] = pdf.Length;
                    pdf.Append(numero + " 0 obj\n");
                };

                iniciarObjeto(1);
                pdf.Append("<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

                iniciarObjeto(2);
                StringBuilder filhos = new StringBuilder();
                for (int p = 0; p < paginas.Count; p++)
                    filhos.Append((5 + p * 2) + " 0 R ");
                pdf.Append("<< /Type /Pages /Kids [ " + filhos + "] /Count " + paginas.Count + " >>\nendobj\n");

                iniciarObjeto(3);
                pdf.Append("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n");

                iniciarObjeto(4);
                pdf.Append("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n");

                string dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                for (int p = 0; p < paginas.Count; p++)
                {
                    StringBuilder cs = new StringBuilder();
                    float y = AlturaPagina - Margem;

                    Texto(cs, "F2", 16, Margem, y - 16, titulo);
                    Texto(cs, "F1", 9, Margem, y - 32, "Gerado em " + dataHora + "  -  " + linhas.Count + " registro(s)");
                    y -= AlturaTitulo;

                    // Cabeçalho da tabela
                    cs.Append("0.85 g " + N(Margem) + " " + N(y - AlturaCabecalho) + " " + N(larguraTabela) + " " + N(AlturaCabecalho) + " re f 0 g\n");
                    float x = Margem;
                    for (int c = 0; c < larguras.Length; c++)
                    {
                        string texto = Ajustar(g, negrito, cabecalhos[c], larguras[c] - 2 * Preenchimento);
                        Texto(cs, "F2", 9, x + Preenchimento, y - 14, texto);
                        x += larguras[c];
                    }
                    y -= AlturaCabecalho;

                    // Linhas
                    int inicio = paginas[p][0];
                    int quantidade = paginas[p][1];
                    for (int r = 0; r < quantidade; r++)
                    {
                        string[] linha = linhas[inicio + r];
                        x = Margem;
                        for (int c = 0; c < larguras.Length; c++)
                        {
                            string texto = Ajustar(g, normal, linha[c], larguras[c] - 2 * Preenchimento);
                            Texto(cs, "F1", 9, x + Preenchimento, y - 12, texto);
                            x += larguras[c];
                        }
                        y -= AlturaLinha;
                        cs.Append("0.8 G 0.5 w " + N(Margem) + " " + N(y) + " m " + N(Margem + larguraTabela) + " " + N(y) + " l S 0 G\n");
                    }

                    // Rodapé
                    Texto(cs, "F1", 8, Margem, 20, dataHora);
                    string pagina = "Página " + (p + 1) + " de " + paginas.Count;
                    float larguraPag = Medir(g, normal, pagina) * 8f / 9f;
                    Texto(cs, "F1", 8, LarguraPagina - Margem - larguraPag, 20, pagina);

                    // Converte o texto da página para bytes Latin-1 (1 caractere = 1 byte)
                    string conteudoPagina = cs.ToString();
                    int tamanho = latin1.GetByteCount(conteudoPagina);

                    iniciarObjeto(5 + p * 2);
                    pdf.Append("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 " + N(LarguraPagina) + " " + N(AlturaPagina) + "] " +
                               "/Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents " + (6 + p * 2) + " 0 R >>\nendobj\n");

                    iniciarObjeto(6 + p * 2);
                    pdf.Append("<< /Length " + tamanho + " >>\nstream\n" + conteudoPagina + "endstream\nendobj\n");
                }

                int inicioXref = pdf.Length;
                pdf.Append("xref\n0 " + (totalObjetos + 1) + "\n");
                pdf.Append("0000000000 65535 f \n");
                for (int i = 1; i <= totalObjetos; i++)
                    pdf.Append(offsets[i].ToString("0000000000", CultureInfo.InvariantCulture) + " 00000 n \n");
                pdf.Append("trailer\n<< /Size " + (totalObjetos + 1) + " /Root 1 0 R >>\nstartxref\n" + inicioXref + "\n%%EOF\n");

                return latin1.GetBytes(pdf.ToString());
            }
        }

        // O texto é convertido para Windows-1252 e depois lido como Latin-1,
        // assim cada byte vira um caractere e os deslocamentos do PDF ficam corretos.
        private static void Texto(StringBuilder cs, string fonte, float tamanho, float x, float y, string texto)
        {
            Encoding win1252 = Encoding.GetEncoding(1252);
            Encoding latin1 = Encoding.GetEncoding("iso-8859-1");
            string bytes = latin1.GetString(win1252.GetBytes(texto ?? ""));
            bytes = bytes.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
            cs.Append("BT /" + fonte + " " + N(tamanho) + " Tf " + N(x) + " " + N(y) + " Td (" + bytes + ") Tj ET\n");
        }

        private static string Ajustar(Graphics g, Font fonte, string texto, float maximo)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            texto = texto.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            if (maximo < Medir(g, fonte, "...")) return "";
            if (Medir(g, fonte, texto) <= maximo) return texto;

            int inicio = 0;
            int fim = texto.Length;
            while (inicio < fim)
            {
                int meio = (inicio + fim + 1) / 2;
                if (Medir(g, fonte, texto.Substring(0, meio) + "...") <= maximo)
                    inicio = meio;
                else
                    fim = meio - 1;
            }
            if (inicio > 0 && char.IsHighSurrogate(texto[inicio - 1])) inicio--;
            return texto.Substring(0, inicio) + "...";
        }

        private static float Medir(Graphics g, Font fonte, string texto)
        {
            if (string.IsNullOrEmpty(texto)) return 0;
            return g.MeasureString(texto, fonte, int.MaxValue, StringFormat.GenericTypographic).Width;
        }

        private static string N(float valor)
        {
            return valor.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}
