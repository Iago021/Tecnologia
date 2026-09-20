using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tecnologia
{
    public partial class PrincipalForm
    {
        private Panel conteudoPainel;

        private void ConfigurarVisual()
        {
            SuspendLayout();
            ClientSize = new Size(1120, 820);
            Tema.Aplicar(this);
            AutoScrollMinSize = new Size(1100, 800);
            conteudoPainel = new Panel();
            conteudoPainel.Name = "conteudoPainel";
            conteudoPainel.Size = new Size(1100, 800);
            conteudoPainel.BackColor = Color.Transparent;
            Control[] existentes = new Control[Controls.Count];
            Controls.CopyTo(existentes, 0);
            Controls.Add(conteudoPainel);
            foreach (Control controle in existentes) conteudoPainel.Controls.Add(controle);

            lblTitulo.Visible = false;
            PictureBox logo = Tema.Imagem("tecnologia-logo.png");
            logo.SetBounds(28, 14, 496, 106);
            logo.AccessibleName = "Tecnologia";
            conteudoPainel.Controls.Add(logo);
            lblUsuario.SetBounds(552, 62, 518, 28);
            lblUsuario.TextAlign = ContentAlignment.MiddleRight;

            Panel linha = new Panel();
            linha.BackColor = Color.FromArgb(223, 227, 224);
            linha.SetBounds(0, 132, 1100, 1);
            conteudoPainel.Controls.Add(linha);

            btnAtualizar.Text = "Dashboard";
            btnOrdens.Text = "Manutenção";
            btnPecas.Text = "Peças";
            btnPerfil.Text = "Conta / Perfil";
            Button[] navegacao = { btnAtualizar, btnOrdens, btnPecas, btnPerfil };
            for (int i = 0; i < navegacao.Length; i++)
            {
                navegacao[i].SetBounds(250 + i * 152, 148, 128, 34);
                navegacao[i].TabIndex = i;
            }
            btnAtualizar.BackColor = Tema.VerdeEscuro;
            btnAtualizar.ForeColor = Color.White;

            Label boasVindas = new Label();
            boasVindas.Text = "Bem\nvindo!";
            boasVindas.Font = new Font("Segoe UI", 68, FontStyle.Bold);
            boasVindas.ForeColor = Color.FromArgb(147, 157, 151);
            boasVindas.BackColor = Color.Transparent;
            boasVindas.SetBounds(28, 211, 570, 250);
            conteudoPainel.Controls.Add(boasVindas);
            PictureBox foto = Tema.Imagem("boas-vindas.png");
            foto.SetBounds(658, 220, 386, 277);
            foto.AccessibleName = "Profissional trabalhando em um computador";
            Tema.Arredondar(foto, 18);
            conteudoPainel.Controls.Add(foto);

            // Os quatro indicadores continuam clicáveis e com os mesmos filtros.
            Button[] indicadores = { btnAbertas, btnManutencao, btnConcluidas, btnProntas };
            for (int i = 0; i < indicadores.Length; i++)
            {
                Button indicador = indicadores[i];
                indicador.SetBounds(28 + i * 268, 525, 248, 64);
                indicador.BackColor = Tema.Cartao;
                indicador.ForeColor = Tema.VerdeEscuro;
                indicador.TabIndex = i + 4;
            }
            lblResumo.SetBounds(28, 598, 1044, 48);
            lblResumo.Font = new Font("Segoe UI", 9);

            // A referência não mostra estes atalhos; ficam disponíveis para preservar o sistema.
            Button[] cadastros = { btnClientes, btnAparelhos, btnUsuarios, btnNova };
            for (int i = 0; i < cadastros.Length; i++)
            {
                cadastros[i].SetBounds(28 + i * 268, 662, 248, 38);
                cadastros[i].TabIndex = i + 8;
            }

            Panel rodape = new Panel();
            rodape.SetBounds(0, 738, 1100, 60);
            rodape.BackColor = Color.FromArgb(188, 209, 196);
            conteudoPainel.Controls.Add(rodape);
            Label descricao = new Label();
            descricao.Text = "Tecnologia  |  Assistência técnica";
            descricao.ForeColor = Tema.Texto;
            descricao.BackColor = Color.Transparent;
            descricao.SetBounds(28, 18, 730, 26);
            rodape.Controls.Add(descricao);
            btnSair.SetBounds(868, 12, 204, 36);
            btnSair.TabIndex = 12;
            rodape.Controls.Add(btnSair);
            rodape.TabIndex = 12;
            Resize += delegate { CentralizarPainel(); };
            CentralizarPainel();
            ResumeLayout(true);
        }

        private void CentralizarPainel()
        {
            conteudoPainel.Location = new Point(Math.Max(0, (ClientSize.Width - 1100) / 2) + AutoScrollPosition.X, AutoScrollPosition.Y);
        }
    }
}
