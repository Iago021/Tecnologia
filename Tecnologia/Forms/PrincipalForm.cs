using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class PrincipalForm : Form
    {
        public PrincipalForm() { InitializeComponent(); }
        private void PrincipalForm_Load(object sender, EventArgs e)
        {
            MenuStrip menu = new MenuStrip();
            menu.Items.Add("Clientes",null,btnClientes_Click);
            menu.Items.Add("Aparelhos",null,btnAparelhos_Click);
            menu.Items.Add("Equipe",null,btnUsuarios_Click);
            menu.Items.Add("Ordens",null,btnOrdens_Click);
            menu.Items.Add("Estoque",null,btnPecas_Click);
            menu.Items.Add("Minha conta",null,btnPerfil_Click);
            menu.Items.Add("Sair",null,btnSair_Click);
            menu.Items[0].Visible = menu.Items[1].Visible = menu.Items[2].Visible = Sessao.Atendente;
            menu.Items[4].Visible = !Sessao.Atendente;
            MainMenuStrip = menu; Controls.Add(menu);
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
        { using(janela) janela.ShowDialog(this); Atualizar(); }
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
        private void btnAtualizar_Click(object sender, EventArgs e) { Atualizar(); }
        private void btnSair_Click(object sender, EventArgs e) { Sessao.TrocarUsuario = true; Close(); }
    }
}
