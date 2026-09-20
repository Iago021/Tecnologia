using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class AbrirOrdemForm : Form
    {
        public AbrirOrdemForm() { InitializeComponent(); Tema.Aplicar(this); }
        private bool carregando;
        private void AbrirOrdemForm_Load(object sender,EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente"); carregando=true;
                Tela.Combo(cmbCliente,Banco.Consultar("SELECT id,CONCAT(id,' - ',nome) nome FROM clientes WHERE ativo=1 ORDER BY nome"));
                carregando=false; dtPrevisao.Checked=false; Aparelhos();
            } catch(Exception erro) { Tela.Erro(erro); Close(); }
        }
        private void Aparelhos()
        {
            if (carregando || cmbCliente.SelectedValue==null) return;
            Tela.Combo(cmbAparelho,Banco.Consultar("SELECT id,CONCAT(id,' - ',tipo,' ',marca,' ',modelo) nome FROM aparelhos WHERE cliente_id=@cliente ORDER BY id DESC",Banco.P("@cliente",Tela.Codigo(cmbCliente))));
        }
        private void cmbCliente_SelectedIndexChanged(object sender,EventArgs e)
        { try { Aparelhos(); } catch(Exception erro) { Tela.Erro(erro); } }
        private void btnSalvar_Click(object sender,EventArgs e)
        {
            try
            {
                int id=OperacoesOrdem.Abrir(Tela.Codigo(cmbAparelho),txtProblema.Text,dtPrevisao.Checked ? (DateTime?)dtPrevisao.Value.Date : null,txtObservacoes.Text);
                MessageBox.Show("Ordem "+id+" aberta."); Close();
            } catch(Exception erro) { Tela.Erro(erro); }
        }
    }
}
