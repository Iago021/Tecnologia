using System;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class HistoricoForm : Form
    {
        public HistoricoForm() { InitializeComponent(); AlteracoesFormulario.Observar(this); }
        private int codigo;
        private bool ordem;
        public HistoricoForm(int id,bool historicoOrdem) : this() { codigo=id; ordem=historicoOrdem; }
        private async void HistoricoForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            lblTitulo.Text = (ordem ? "Histórico da ordem " : "Movimentações da peça ") + codigo;
            string sql = ordem ? "SELECT h.id,u.nome AS Usuário,h.status_anterior AS Anterior,h.status_novo AS Novo,h.data_alteracao AS Data FROM historico_status h JOIN usuarios u ON u.id=h.usuario_id WHERE h.ordem_id=@id ORDER BY h.id DESC" :
                    "SELECT m.id,m.tipo AS Tipo,m.quantidade AS Quantidade,m.ordem_id AS Ordem,u.nome AS Usuário,m.observacao AS Motivo,m.data_movimentacao AS Data FROM movimentacoes_estoque m JOIN usuarios u ON u.id=m.usuario_id WHERE m.peca_id=@id ORDER BY m.id DESC";
            await Tela.CarregarAsync(this, this, ordem ? "" : "Técnico",
                _ => Banco.Consultar(sql, Banco.P("@id", codigo)), dados =>
                {
                    grade.DataSource = dados;
                    Tela.AjustarGrade(grade);
                }, true);
        }
        private void btnExportar_Click(object sender,EventArgs e)
        { try { Sessao.Exigir(ordem ? "" : "Técnico"); Exportar.Menu(btnExportar, grade, lblTitulo.Text); } catch(Exception erro) { Tela.Erro(erro); } }
    }
}
