using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Tecnologia
{
    // Apenas apresentação: não consulta o banco nem altera permissões ou eventos.
    internal static class Tema
    {
        public static readonly Color Verde = Color.FromArgb(0, 235, 112);
        public static readonly Color VerdeEscuro = Color.FromArgb(0, 118, 69);
        public static readonly Color Cartao = Color.FromArgb(238, 238, 238);
        public static readonly Color Campo = Color.FromArgb(218, 218, 218);
        public static readonly Color Texto = Color.FromArgb(70, 82, 76);

        public static void Aplicar(Form janela)
        {
            janela.SuspendLayout();
            janela.BackColor = Color.White;
            janela.ForeColor = Texto;
            // Uma imagem pequena é esticada pelo Windows Forms ao redimensionar.
            Bitmap fundo = new Bitmap(2, 720);
            using (Graphics desenho = Graphics.FromImage(fundo))
            {
                desenho.Clear(Color.White);
                using (LinearGradientBrush pincel = new LinearGradientBrush(
                    new Rectangle(0, 210, 2, 510), Color.White,
                    Color.FromArgb(189, 207, 195), LinearGradientMode.Vertical))
                {
                    pincel.WrapMode = WrapMode.Clamp;
                    desenho.FillRectangle(pincel, 0, 210, 2, 510);
                }
            }
            janela.BackgroundImage = fundo;
            janela.BackgroundImageLayout = ImageLayout.Stretch;
            janela.Disposed += delegate { fundo.Dispose(); };
            Estilizar(janela);
            // Mantém os campos acessíveis em monitores menores e com escala do Windows.
            janela.AutoScroll = true;
            int alturaConteudo = janela.ClientSize.Height;
            foreach (Control controle in janela.Controls)
                alturaConteudo = Math.Max(alturaConteudo, controle.Bottom + 10);
            janela.AutoScrollMinSize = new Size(janela.ClientSize.Width, alturaConteudo);
            janela.MinimumSize = new Size(480, 360);
            janela.ResumeLayout(true);
        }

        private static void CriarCartao(Form janela)
        {
            Control[] controles = new Control[janela.Controls.Count];
            janela.Controls.CopyTo(controles, 0);
            int limiteInferior = janela.ClientSize.Height - 10;
            foreach (Control controle in controles)
                limiteInferior = Math.Max(limiteInferior, controle.Bottom + 12);
            Panel painel = new Panel();
            painel.Name = "cartaoFormulario";
            painel.BackColor = Cartao;
            painel.SetBounds(10, 44, janela.ClientSize.Width - 20, limiteInferior - 44);
            painel.TabIndex = 0;
            janela.Controls.Add(painel);
            foreach (Control controle in controles)
            {
                if (controle.Top < 44) continue;
                Point posicao = controle.Location;
                painel.Controls.Add(controle);
                controle.Location = new Point(posicao.X - 10, posicao.Y - 44);
            }
            Arredondar(painel, 16);
        }

        private static void Estilizar(Control grupo)
        {
            foreach (Control controle in grupo.Controls)
            {
                Button botao = controle as Button;
                TextBox campo = controle as TextBox;
                Label rotulo = controle as Label;
                DataGridView grade = controle as DataGridView;
                if (botao != null) EstilizarBotao(botao);
                else if (campo != null)
                {
                    campo.BackColor = Campo;
                    campo.ForeColor = Texto;
                    campo.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (rotulo != null)
                {
                    rotulo.BackColor = Color.Transparent;
                    rotulo.ForeColor = Texto;
                    if (rotulo.Name == "lblTitulo")
                    {
                        rotulo.Font = new Font("Segoe UI", 13, FontStyle.Bold);
                        rotulo.ForeColor = VerdeEscuro;
                    }
                }
                else if (controle is CheckBox) controle.BackColor = Color.Transparent;
                else if (controle is ComboBox)
                {
                    ((ComboBox)controle).FlatStyle = FlatStyle.Flat;
                    controle.BackColor = Campo;
                    controle.ForeColor = Texto;
                }
                else if (controle is NumericUpDown)
                {
                    ((NumericUpDown)controle).BorderStyle = BorderStyle.FixedSingle;
                    controle.BackColor = Campo;
                    controle.ForeColor = Texto;
                }
                if (grade != null)
                {
                    grade.BackgroundColor = Cartao;
                    grade.BorderStyle = BorderStyle.None;
                    grade.GridColor = Color.FromArgb(218, 228, 220);
                    grade.EnableHeadersVisualStyles = false;
                    grade.ColumnHeadersDefaultCellStyle.BackColor = VerdeEscuro;
                    grade.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grade.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    grade.ColumnHeadersHeight = 34;
                    grade.DefaultCellStyle.BackColor = Color.White;
                    grade.DefaultCellStyle.ForeColor = Texto;
                    grade.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 243, 219);
                    grade.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 66, 43);
                    grade.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(243, 248, 245);
                    grade.RowTemplate.Height = 28;
                }
                // Não modifica os controles internos de tabelas e campos nativos.
                if (controle is Panel || controle is GroupBox || controle is TabControl || controle is TabPage)
                    Estilizar(controle);
            }
        }

        public static void EstilizarBotao(Button botao)
        {
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 0;
            botao.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 211, 100);
            botao.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 183, 88);
            botao.UseVisualStyleBackColor = false;
            botao.BackColor = Verde;
            // Mais legível que branco sobre o verde claro das capturas.
            botao.ForeColor = Color.FromArgb(0, 61, 36);
            botao.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            botao.Cursor = Cursors.Hand;
            Arredondar(botao, 16);
            botao.Resize += delegate { Arredondar(botao, 16); };
            botao.Paint += delegate(object sender, PaintEventArgs e)
            {
                if (botao.Focused && botao.Enabled)
                    ControlPaint.DrawFocusRectangle(e.Graphics,
                        new Rectangle(7, 6, Math.Max(1, botao.Width - 14), Math.Max(1, botao.Height - 12)),
                        botao.ForeColor, botao.BackColor);
            };
        }

        public static void Arredondar(Control controle, int raio)
        {
            if (controle.Width < 2 || controle.Height < 2) return;
            int diametro = Math.Min(raio * 2, Math.Min(controle.Width, controle.Height));
            using (GraphicsPath caminho = new GraphicsPath())
            {
                caminho.AddArc(0, 0, diametro, diametro, 180, 90);
                caminho.AddArc(controle.Width - diametro, 0, diametro, diametro, 270, 90);
                caminho.AddArc(controle.Width - diametro, controle.Height - diametro, diametro, diametro, 0, 90);
                caminho.AddArc(0, controle.Height - diametro, diametro, diametro, 90, 90);
                caminho.CloseFigure();
                Region anterior = controle.Region;
                controle.Region = new Region(caminho);
                if (anterior != null) anterior.Dispose();
            }
        }

        public static PictureBox Imagem(string arquivo)
        {
            PictureBox imagem = new PictureBox();
            imagem.BackColor = Color.Transparent;
            imagem.SizeMode = PictureBoxSizeMode.Zoom;
            imagem.TabStop = false;
            using (Stream recurso = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tecnologia.Imagens." + arquivo))
            {
                if (recurso == null) throw new InvalidOperationException("Imagem não incluída no projeto: " + arquivo);
                using (Image original = Image.FromStream(recurso)) imagem.Image = new Bitmap(original);
                // Chave de transparência para o pequeno recorte preto da moldura do protótipo.
                if (arquivo == "tecnologia-logo.png") ((Bitmap)imagem.Image).MakeTransparent(Color.Black);
            }
            imagem.Disposed += delegate { if (imagem.Image != null) imagem.Image.Dispose(); };
            return imagem;
        }

        public static Panel EnvolverCampo(TextBox campo, int largura)
        {
            Panel painel = new Panel();
            painel.Name = campo.Name + "Fundo";
            painel.BackColor = Campo;
            painel.Size = new Size(largura, 42);
            painel.TabIndex = campo.TabIndex;
            campo.BorderStyle = BorderStyle.None;
            campo.Location = new Point(12, 12);
            campo.Width = largura - 24;
            campo.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            campo.TabIndex = 0;
            painel.Controls.Add(campo);
            Arredondar(painel, 15);
            painel.Resize += delegate { Arredondar(painel, 15); };
            return painel;
        }
    }
}
