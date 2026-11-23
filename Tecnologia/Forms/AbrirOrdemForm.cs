using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class AbrirOrdemForm : Form
    {
        public AbrirOrdemForm() { InitializeComponent(); PrepararTela(); AlteracoesFormulario.Observar(this); }
        private readonly int codigo;
        private DataRow recebimentoOriginal;
        private bool carregando;

        internal AbrirOrdemForm(int id) : this()
        {
            codigo = id;
            Text = "Editar recebimento";
            lblTitulo.Text = "Editar recebimento";
            btnSalvar.Text = "Salvar alterações";
        }
        private void AbrirOrdemForm_Load(object sender,EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            try
            {
                Sessao.Exigir("Atendente");
                if (codigo != 0)
                {
                    CarregarRecebimento();
                    return;
                }
                carregando=true;
                Tela.Combo(cmbCliente,Banco.Consultar("SELECT id,CONCAT(id,' - ',nome) nome FROM clientes WHERE ativo=1 ORDER BY nome"));
                carregando=false; dtPrevisao.Checked=false; Aparelhos();
            } catch(Exception erro) { Tela.Erro(erro); Close(); }
        }
        private void CarregarRecebimento()
        {
            DataTable dados = Banco.Consultar(@"SELECT o.status, o.problema_relatado, o.previsao_entrega, o.observacoes,
                c.nome AS cliente, CONCAT(a.tipo, ' ', a.marca, ' ', a.modelo) AS aparelho
                FROM ordens_servico o JOIN aparelhos a ON a.id=o.aparelho_id
                JOIN clientes c ON c.id=a.cliente_id WHERE o.id=@id", Banco.P("@id", codigo));
            if (dados.Rows.Count == 0 || dados.Rows[0]["status"].ToString() != "Aberta")
                throw new InvalidOperationException("Apenas ordens abertas permitem editar o recebimento.");

            recebimentoOriginal = dados.Rows[0];
            carregando = true;
            cmbCliente.Items.Add(recebimentoOriginal["cliente"]);
            cmbCliente.SelectedIndex = 0;
            cmbCliente.Enabled = false;
            cmbAparelho.Items.Add(recebimentoOriginal["aparelho"]);
            cmbAparelho.SelectedIndex = 0;
            cmbAparelho.Enabled = false;
            txtProblema.Text = recebimentoOriginal["problema_relatado"].ToString();
            txtObservacoes.Text = recebimentoOriginal["observacoes"].ToString();
            DateTime? previsao = recebimentoOriginal.Field<DateTime?>("previsao_entrega");
            dtPrevisao.Value = previsao ?? DateTime.Today;
            dtPrevisao.Checked = previsao.HasValue;
            carregando = false;
        }

        private void Aparelhos()
        {
            if (carregando) return;
            if (cmbCliente.SelectedValue == null || cmbCliente.SelectedValue is DataRowView)
            {
                cmbAparelho.DataSource = null;
                return;
            }
            Tela.Combo(cmbAparelho,Banco.Consultar("SELECT id,CONCAT(id,' - ',tipo,' ',marca,' ',modelo) nome FROM aparelhos WHERE cliente_id=@cliente ORDER BY id DESC",Banco.P("@cliente",Tela.Codigo(cmbCliente))));
        }
        private void cmbCliente_SelectedIndexChanged(object sender,EventArgs e)
        { try { Aparelhos(); } catch(Exception erro) { Tela.Erro(erro); } }
        private void btnSalvar_Click(object sender,EventArgs e)
        {
            try
            {
                DateTime? previsao = dtPrevisao.Checked ? (DateTime?)dtPrevisao.Value.Date : null;
                if (codigo == 0)
                {
                    int id = OperacoesOrdem.Abrir(Tela.Codigo(cmbAparelho), txtProblema.Text, previsao, txtObservacoes.Text);
                    MessageBox.Show("Ordem " + id + " aberta.");
                }
                else
                    OperacoesOrdem.EditarRecebimento(codigo, txtProblema.Text, previsao, txtObservacoes.Text, recebimentoOriginal);

                AlteracoesFormulario.MarcarSalvo(this);
                DialogResult = DialogResult.OK;
                Close();
            } catch(Exception erro) { Tela.Erro(erro); }
        }
    }
}
