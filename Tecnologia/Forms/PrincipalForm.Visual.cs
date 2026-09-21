using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class PrincipalForm
    {
        private Panel area;
        private TableLayoutPanel dashboard;
        private FlowLayoutPanel menuCadastros;
        private Button btnCadastros;
        private Form paginaAtual;
        private TableLayoutPanel conteudo;

        private void ConfigurarVisual()
        {
            SuspendLayout();
            Tema.Aplicar(this);
            Controls.Clear();
            AutoScroll = false; AutoScrollMinSize = Size.Empty;
            ClientSize = new Size(1180, 820); MinimumSize = new Size(940, 650);
            TableLayoutPanel estrutura = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, BackColor = Color.Transparent };
            estrutura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            Controls.Add(estrutura);
            TableLayoutPanel topo = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(28, 12, 28, 10), BackColor = Color.White };
            topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            PictureBox logo = Tema.Imagem("tecnologia-logo.png"); logo.Dock = DockStyle.Fill;
            topo.Controls.Add(logo, 0, 0);
            lblUsuario.Dock = DockStyle.Fill; lblUsuario.TextAlign = ContentAlignment.MiddleRight;
            topo.Controls.Add(lblUsuario, 1, 0); estrutura.Controls.Add(topo, 0, 0);
            FlowLayoutPanel navegacao = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(28, 5, 0, 0), BackColor = Color.White };
            btnCadastros = new Button { Text = "Cadastros" }; Tema.EstilizarBotao(btnCadastros);
            btnCadastros.Click += delegate { MostrarCadastros(); };
            btnAtualizar.Text = "Dashboard"; btnOrdens.Text = "Manutenção"; btnPecas.Text = "Peças";
            btnPerfil.Text = "Conta / Perfil"; btnSair.Text = "Sair";
            foreach (Button b in new[] { btnAtualizar, btnOrdens, btnPecas, btnCadastros, btnPerfil, btnSair })
            {
                b.Size = new Size(132, 36); b.Margin = new Padding(0, 0, 12, 0);
                b.TabIndex = navegacao.Controls.Count; navegacao.Controls.Add(b);
            }
            estrutura.Controls.Add(navegacao, 0, 1);
            conteudo = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            conteudo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            conteudo.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
            conteudo.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            estrutura.Controls.Add(conteudo, 0, 2);
            menuCadastros = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(28, 6, 0, 0) };
            foreach (Button b in new[] { btnClientes, btnAparelhos, btnUsuarios })
            {
                b.Size = new Size(190, 36); b.Margin = new Padding(0, 0, 12, 0);
                menuCadastros.Controls.Add(b);
            }
            conteudo.Controls.Add(menuCadastros, 0, 0);
            area = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            conteudo.Controls.Add(area, 0, 1);
            MontarDashboard();
            Label rodape = new Label { Text = "Tecnologia  ·  Assistência técnica", Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter, ForeColor = Tema.Texto, BackColor = Color.FromArgb(189, 207, 195) };
            estrutura.Controls.Add(rodape, 0, 3);
            MarcarNavegacao(btnAtualizar);
            ResumeLayout(true);
            FormClosing += delegate(object sender, FormClosingEventArgs e)
            {
                if (paginaAtual != null && !paginaAtual.IsDisposed && LayoutTelas.TemAlteracoes(paginaAtual) &&
                    !Tela.Confirmar("Sair sem salvar as alterações desta tela?")) e.Cancel = true;
            };
        }

        private void MontarDashboard()
        {
            dashboard = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(32, 20, 32, 20), BackColor = Color.Transparent };
            dashboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            dashboard.RowStyles.Add(new RowStyle(SizeType.Percent, 68));
            dashboard.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            dashboard.RowStyles.Add(new RowStyle(SizeType.Percent, 32));
            area.Controls.Add(dashboard);
            TableLayoutPanel boasVindas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            boasVindas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            boasVindas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            Panel chamada = new Panel { Dock = DockStyle.Fill };
            lblTitulo.Text = "Bem\nvindo!"; lblTitulo.Font = new Font("Segoe UI", 42, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(133, 150, 140); lblTitulo.SetBounds(0, 0, 500, 156);
            lblTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            chamada.Controls.Add(lblTitulo);
            btnNova.Text = "Nova ordem de serviço"; btnNova.SetBounds(6, 170, 230, 38);
            chamada.Controls.Add(btnNova);
            boasVindas.Controls.Add(chamada, 0, 0);
            PictureBox foto = Tema.Imagem("boas-vindas.png"); foto.Dock = DockStyle.Fill; foto.Margin = new Padding(20, 0, 20, 18);
            boasVindas.Controls.Add(foto, 1, 0);
            dashboard.Controls.Add(boasVindas, 0, 0);
            TableLayoutPanel indicadores = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4 };
            foreach (Button botao in new[] { btnAbertas, btnManutencao, btnConcluidas, btnProntas })
            {
                indicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                botao.Dock = DockStyle.Fill; botao.Margin = new Padding(5, 10, 5, 14);
                botao.BackColor = Tema.Cartao; botao.ForeColor = Tema.VerdeEscuro;
                indicadores.Controls.Add(botao);
            }
            dashboard.Controls.Add(indicadores, 0, 1);
            lblResumo.Dock = DockStyle.Fill; lblResumo.TextAlign = ContentAlignment.TopLeft;
            lblResumo.Padding = new Padding(7, 12, 7, 0);
            dashboard.Controls.Add(lblResumo, 0, 2);
        }

        private bool FecharPagina()
        {
            if (paginaAtual == null) return true;
            if (!paginaAtual.IsDisposed && LayoutTelas.TemAlteracoes(paginaAtual) &&
                !Tela.Confirmar("Você alterou campos nesta tela. Deseja sair? Alterações que não foram salvas serão perdidas.")) return false;
            Form anterior = paginaAtual; paginaAtual = null;
            anterior.Dispose();
            return true;
        }

        private void MostrarDashboard()
        {
            if (!FecharPagina()) return;
            conteudo.RowStyles[0].Height = 0;
            dashboard.Visible = true; dashboard.BringToFront();
            MarcarNavegacao(btnAtualizar); Atualizar();
        }

        private void MostrarCadastros()
        {
            if (!Sessao.Atendente) return;
            Abrir(new ClientesForm());
        }

        private void MostrarPagina(Form janela)
        {
            if (!FecharPagina()) { janela.Dispose(); return; }
            dashboard.Visible = false;
            bool cadastro = janela is ClientesForm || janela is AparelhosForm || janela is UsuariosForm;
            conteudo.RowStyles[0].Height = cadastro ? 50 : 0;
            paginaAtual = janela;
            janela.TopLevel = false; janela.FormBorderStyle = FormBorderStyle.None; janela.Dock = DockStyle.Fill;
            janela.MinimumSize = Size.Empty;
            foreach (Control marca in janela.Controls.Find("marcaSecundaria", true)) marca.Visible = false;
            area.Controls.Add(janela);
            janela.FormClosed += delegate
            {
                if (paginaAtual != janela) return;
                paginaAtual = null;
                dashboard.Visible = true; conteudo.RowStyles[0].Height = 0;
                MarcarNavegacao(btnAtualizar); Atualizar();
            };
            janela.Show();
            if (janela.IsDisposed || paginaAtual != janela) return;
            janela.BringToFront();
            LayoutTelas.MarcarSalvo(janela);
            MarcarNavegacao(cadastro ? btnCadastros : janela is PecasForm ? btnPecas : janela is PerfilForm ? btnPerfil : btnOrdens);
        }

        private void MarcarNavegacao(Button selecionado)
        {
            foreach (Button botao in new[] { btnAtualizar, btnOrdens, btnPecas, btnCadastros, btnPerfil })
            {
                botao.BackColor = botao == selecionado ? Tema.VerdeEscuro : Tema.Verde;
                botao.ForeColor = botao == selecionado ? Color.White : Color.FromArgb(0, 61, 36);
            }
        }
    }
}
