using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class PrincipalForm : Form
    {
        public PrincipalForm() { InitializeComponent(); PrepararTela(); }
        private void PrincipalForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            // Os botões de navegação mantêm os mesmos eventos e permissões.
            Atualizar();
        }
        private void Atualizar()
        {
            try
            {
                Sessao.Exigir();
                lblUsuario.Text = Sessao.Nome + " | " + Sessao.Perfil;
                btnClientes.Enabled = btnAparelhos.Enabled = btnUsuarios.Enabled = btnNova.Enabled = Sessao.Atendente;
                btnPecas.Enabled = !Sessao.Atendente;
                btnCadastros.Visible = Sessao.Atendente;
                DataRow dados = Banco.Consultar(@"SELECT
                  COUNT(CASE WHEN status='Aberta' THEN 1 END) abertas,
                  COUNT(CASE WHEN status IN ('Em manutenção','Aguardando peça') THEN 1 END) manutencao,
                  COUNT(CASE WHEN status IN ('Concluída','Entregue') THEN 1 END) concluidas,
                  COUNT(CASE WHEN status='Concluída' THEN 1 END) prontas FROM ordens_servico").Rows[0];
                btnAbertas.Text = "Abertas: " + dados["abertas"];
                btnManutencao.Text = "Em manutenção: " + dados["manutencao"];
                btnConcluidas.Text = "Concluídas: " + dados["concluidas"];
                btnProntas.Text = "Aguardando entrega: " + dados["prontas"];
                lblResumo.Text = "Concluídas inclui os serviços já entregues. Em manutenção inclui os que aguardam peças.";
                if (!Sessao.Atendente) lblResumo.Text += "  Peças abaixo do mínimo: " + Banco.Valor("SELECT COUNT(*) FROM pecas WHERE ativo=1 AND quantidade<estoque_minimo");
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void Abrir(Form janela)
        { MostrarPagina(janela); }
        private void Lista(string filtro) { Abrir(new OrdensForm(filtro)); }
        private void btnClientes_Click(object sender, EventArgs e) { Abrir(new ClientesForm()); }
        private void btnAparelhos_Click(object sender, EventArgs e) { Abrir(new AparelhosForm()); }
        private void btnUsuarios_Click(object sender, EventArgs e) { Abrir(new UsuariosForm()); }
        private void btnPecas_Click(object sender, EventArgs e) { Abrir(new PecasForm()); }
        private void btnOrdens_Click(object sender, EventArgs e) { Lista("Todas"); }
        private void btnAbertas_Click(object sender, EventArgs e) { Lista("Aberta"); }
        private void btnManutencao_Click(object sender, EventArgs e) { Lista("Em andamento"); }
        private void btnConcluidas_Click(object sender, EventArgs e) { Lista("Concluídas e entregues"); }
        private void btnProntas_Click(object sender, EventArgs e) { Lista("Concluída"); }
        private void btnNova_Click(object sender, EventArgs e) { Abrir(new AbrirOrdemForm()); }
        private void btnPerfil_Click(object sender, EventArgs e) { Abrir(new PerfilForm()); }
        private void btnAtualizar_Click(object sender, EventArgs e) { MostrarDashboard(); }
        private void btnSair_Click(object sender, EventArgs e) { if (FecharPagina()) { Sessao.TrocarUsuario = true; Close(); } }
    }
}
