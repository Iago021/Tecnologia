using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class PrincipalForm : Form
    {
        public PrincipalForm() { InitializeComponent(); PrepararTela(); }
        private async void PrincipalForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            await AtualizarAsync();
        }
        private Task AtualizarAsync()
        {
            AtualizarUsuario();
            return Tela.CarregarAsync(this, dashboard, "", conta => Tuple.Create(conta, Banco.Consultar(@"SELECT
                COUNT(CASE WHEN status='Aberta' THEN 1 END) abertas,
                COUNT(CASE WHEN status IN ('Em manutenção','Aguardando peça') THEN 1 END) manutencao,
                COUNT(CASE WHEN status IN ('Concluída','Entregue') THEN 1 END) concluidas,
                COUNT(CASE WHEN status='Concluída' THEN 1 END) prontas,
                (SELECT COUNT(*) FROM pecas WHERE ativo=1 AND quantidade<estoque_minimo) estoque_baixo
                FROM ordens_servico").Rows[0]), resultado =>
            {
                Sessao.Iniciar(resultado.Item1);
                AtualizarUsuario();
                DataRow dados = resultado.Item2;
                btnAbertas.Text = "Abertas: " + dados["abertas"];
                btnManutencao.Text = "Em manutenção: " + dados["manutencao"];
                btnConcluidas.Text = "Concluídas: " + dados["concluidas"];
                btnProntas.Text = "Aguardando entrega: " + dados["prontas"];
                lblResumo.Text = "Concluídas inclui os serviços já entregues. Em manutenção inclui os que aguardam peças.";
                if (!Sessao.Atendente) lblResumo.Text += "  Peças abaixo do mínimo: " + dados["estoque_baixo"];
            });
        }

        private void AtualizarUsuario()
        {
            lblUsuario.Text = Sessao.Nome + " | " + Sessao.Perfil;
            btnClientes.Enabled = btnAparelhos.Enabled = btnUsuarios.Enabled = btnNova.Enabled = Sessao.Atendente;
            btnPecas.Enabled = !Sessao.Atendente;
            btnCadastros.Visible = Sessao.Atendente;
        }

        private void Lista(string filtro) { MostrarPagina(new OrdensForm(filtro)); }
        private void btnClientes_Click(object sender, EventArgs e) { MostrarPagina(new ClientesForm()); }
        private void btnAparelhos_Click(object sender, EventArgs e) { MostrarPagina(new AparelhosForm()); }
        private void btnUsuarios_Click(object sender, EventArgs e) { MostrarPagina(new UsuariosForm()); }
        private void btnPecas_Click(object sender, EventArgs e) { MostrarPagina(new PecasForm()); }
        private void btnOrdens_Click(object sender, EventArgs e) { Lista("Todas"); }
        private void btnAbertas_Click(object sender, EventArgs e) { Lista("Aberta"); }
        private void btnManutencao_Click(object sender, EventArgs e) { Lista("Em andamento"); }
        private void btnConcluidas_Click(object sender, EventArgs e) { Lista("Concluídas e entregues"); }
        private void btnProntas_Click(object sender, EventArgs e) { Lista("Concluída"); }
        private void btnNova_Click(object sender, EventArgs e) { MostrarPagina(new AbrirOrdemForm()); }
        private void btnPerfil_Click(object sender, EventArgs e) { MostrarPagina(new PerfilForm()); }
        private void btnAtualizar_Click(object sender, EventArgs e) { MostrarDashboard(); }
        private void btnSair_Click(object sender, EventArgs e) { if (FecharPagina()) { Sessao.TrocarUsuario = true; Close(); } }
    }
}
