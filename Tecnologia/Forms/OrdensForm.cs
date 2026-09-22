using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class OrdensForm : Form
    {
        public OrdensForm() { InitializeComponent(); PrepararTela(); AlteracoesFormulario.Observar(this); }
        private string filtro="Todas";
        public OrdensForm(string status) : this() { filtro=status; }
        private void OrdensForm_Load(object sender,EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            try { Sessao.Exigir(); cmbStatus.SelectedItem=filtro; btnNova.Enabled=Sessao.Atendente; Carregar(); }
            catch(Exception erro) { Tela.Erro(erro); Close(); }
        }
        private void Carregar()
        {
            Sessao.Exigir();
            grade.DataSource=Banco.Consultar(@"SELECT o.id,c.nome AS Cliente,CONCAT(a.tipo,' ',a.marca,' ',a.modelo) AS Aparelho,
             o.status AS Status,COALESCE(u.nome,'Não atribuído') AS Técnico,o.data_entrada AS Entrada,o.previsao_entrega AS Previsão,
             o.data_conclusao AS Conclusão,o.data_entrega AS Entrega,
             o.valor_mao_obra+COALESCE((SELECT SUM(p.quantidade*p.valor_unitario) FROM ordem_pecas p WHERE p.ordem_id=o.id),0)-o.desconto AS Total
             FROM ordens_servico o JOIN aparelhos a ON a.id=o.aparelho_id JOIN clientes c ON c.id=a.cliente_id LEFT JOIN usuarios u ON u.id=o.tecnico_id
             WHERE (@status='Todas' OR o.status=@status OR (@status='Em andamento' AND o.status IN ('Em manutenção','Aguardando peça'))
             OR (@status='Concluídas e entregues' AND o.status IN ('Concluída','Entregue')))
             AND (CAST(o.id AS CHAR) LIKE @busca OR c.nome LIKE @busca OR a.modelo LIKE @busca OR a.marca LIKE @busca OR u.nome LIKE @busca)
             ORDER BY o.id DESC",Banco.P("@status",cmbStatus.Text),Banco.P("@busca","%"+txtBusca.Text.Trim()+"%"));
            grade.Columns["id"].HeaderText="Ordem";
            grade.Columns["Total"].DefaultCellStyle.Format="C2";
            Tela.AjustarGrade(grade);
        }
        private void btnPesquisar_Click(object sender,EventArgs e) { try { Carregar(); } catch(Exception erro) { Tela.Erro(erro); } }
        private void btnExportar_Click(object sender,EventArgs e) { try { Sessao.Exigir(); Exportar.Excel(grade); } catch(Exception erro) { Tela.Erro(erro); } }
        private void btnNova_Click(object sender,EventArgs e)
        { try { using(AbrirOrdemForm janela=new AbrirOrdemForm()) janela.ShowDialog(TopLevelControl); Carregar(); } catch(Exception erro) { Tela.Erro(erro); } }
        private void btnDetalhes_Click(object sender,EventArgs e)
        { try { using(OrdemForm janela=new OrdemForm(Tela.Selecionado(grade))) janela.ShowDialog(TopLevelControl); Carregar(); } catch(Exception erro) { Tela.Erro(erro); } }
    }
}
