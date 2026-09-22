using System;
using System.Drawing;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class PrincipalForm
    {
        private Form paginaAtual;
        private void PrincipalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (paginaAtual != null && !paginaAtual.IsDisposed && AlteracoesFormulario.TemAlteracoes(paginaAtual) &&
                !Tela.Confirmar("Sair sem salvar as alterações desta tela?")) e.Cancel = true;
        }
        private bool FecharPagina()
        {
            if (paginaAtual == null) return true;
            if (!paginaAtual.IsDisposed && AlteracoesFormulario.TemAlteracoes(paginaAtual) &&
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
            AlteracoesFormulario.MarcarSalvo(janela);
            MarcarNavegacao(cadastro ? btnCadastros : janela is PecasForm ? btnPecas : janela is PerfilForm ? btnPerfil : btnOrdens);
        }

        private void MarcarNavegacao(Button selecionado)
        {
            foreach (Button botao in new[] { btnAtualizar, btnOrdens, btnPecas, btnCadastros, btnPerfil })
            {
                botao.BackColor = botao == selecionado ? Color.FromArgb(0, 118, 69) : Color.FromArgb(0, 235, 112);
                botao.ForeColor = botao == selecionado ? Color.White : Color.FromArgb(0, 61, 36);
            }
        }
    }
}
