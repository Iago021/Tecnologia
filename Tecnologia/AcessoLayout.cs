using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    internal static class AcessoLayout
    {
        public static void Ocupar(Form janela, bool ocupado)
        {
            janela.Controls["cartaoAcesso"].Enabled = !ocupado;
        }
        public static FlowLayoutPanel Montar(Form janela, string titulo, string explicacao)
        {
            janela.SuspendLayout();
            janela.Font = new Font("Segoe UI", 10F);
            janela.ClientSize = new Size(1040, 780);
            janela.MinimumSize = new Size(460, 420);
            janela.StartPosition = FormStartPosition.CenterScreen;
            Tema.Aplicar(janela);
            janela.AutoScrollMinSize = Size.Empty;
            PictureBox logo = Tema.Imagem("tecnologia-logo.png");
            logo.Size = new Size(440, 94);
            logo.AccessibleName = "Tecnologia";
            janela.Controls.Add(logo);
            FlowLayoutPanel cartao = new FlowLayoutPanel();
            cartao.Name = "cartaoAcesso";
            cartao.Width = 384;
            cartao.AutoSize = true;
            cartao.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cartao.MinimumSize = new Size(384, 0);
            cartao.MaximumSize = new Size(384, 0);
            cartao.Padding = new Padding(28, 24, 28, 24);
            cartao.FlowDirection = FlowDirection.TopDown;
            cartao.WrapContents = false;
            cartao.BackColor = Tema.Cartao;
            janela.Controls.Add(cartao);
            Label cabecalho = Texto(titulo, 18, FontStyle.Bold);
            cabecalho.ForeColor = Tema.VerdeEscuro;
            cartao.Controls.Add(cabecalho);
            if (explicacao != "") cartao.Controls.Add(Texto(explicacao, 9, FontStyle.Regular));
            Action posicionar = delegate
            {
                int largura = Math.Max(440, janela.ClientSize.Width);
                logo.Location = new Point((largura - logo.Width) / 2 + janela.AutoScrollPosition.X, 24 + janela.AutoScrollPosition.Y);
                cartao.Location = new Point((largura - cartao.Width) / 2 + janela.AutoScrollPosition.X, 146 + janela.AutoScrollPosition.Y);
                janela.AutoScrollMinSize = new Size(440, cartao.Height + 172);
                Tema.Arredondar(cartao, 18);
            };
            cartao.SizeChanged += delegate { posicionar(); };
            janela.Resize += delegate { posicionar(); };
            posicionar();
            janela.ResumeLayout(true);
            return cartao;
        }

        public static Label Texto(string texto, float tamanho, FontStyle estilo)
        {
            return new Label { Text = texto, AutoSize = true, MaximumSize = new Size(322, 0),
                MinimumSize = new Size(322, 0), ForeColor = Tema.Texto, BackColor = Color.Transparent,
                Font = new Font("Segoe UI", tamanho, estilo), TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 14) };
        }

        public static TextBox Campo(FlowLayoutPanel cartao, string titulo, bool senha = false, int limite = 100)
        {
            Label rotulo = Texto(titulo, 9, FontStyle.Regular);
            rotulo.TextAlign = ContentAlignment.MiddleLeft;
            rotulo.Margin = new Padding(0, 0, 0, 5);
            cartao.Controls.Add(rotulo);
            TextBox campo = new TextBox { UseSystemPasswordChar = senha, MaxLength = limite, Font = new Font("Segoe UI", 10),
                BackColor = Tema.Campo, ForeColor = Tema.Texto, AccessibleName = titulo };
            Panel fundo = Tema.EnvolverCampo(campo, 322);
            fundo.Margin = new Padding(0, 0, 0, 14);
            fundo.TabIndex = cartao.Controls.Count;
            cartao.Controls.Add(fundo);
            return campo;
        }

        public static Button Botao(FlowLayoutPanel cartao, string texto)
        {
            Button botao = new Button { Text = texto, Size = new Size(220, 40), Margin = new Padding(51, 5, 0, 12), TabIndex = cartao.Controls.Count };
            Tema.EstilizarBotao(botao);
            cartao.Controls.Add(botao);
            return botao;
        }

        public static LinkLabel Link(FlowLayoutPanel cartao, string texto, EventHandler clicar)
        {
            LinkLabel link = new LinkLabel { Text = texto, Width = 322, Height = 26, TextAlign = ContentAlignment.MiddleCenter,
                LinkColor = Color.FromArgb(0, 112, 153), ActiveLinkColor = Tema.VerdeEscuro, VisitedLinkColor = Color.FromArgb(0, 112, 153),
                BackColor = Color.Transparent, Margin = new Padding(0, 3, 0, 3), TabIndex = cartao.Controls.Count };
            link.Click += clicar;
            cartao.Controls.Add(link);
            return link;
        }
    }
}
