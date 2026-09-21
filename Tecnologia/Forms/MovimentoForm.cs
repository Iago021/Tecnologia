using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class MovimentoForm : Form
    {
        public MovimentoForm() { InitializeComponent(); LayoutTelas.Aplicar(this); }
        private int peca;
        public MovimentoForm(int codigo) : this() { peca=codigo; }
        private void MovimentoForm_Load(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Técnico"); cmbTipo.SelectedIndex=0;
                lblPeca.Text=Convert.ToString(Banco.Valor("SELECT CONCAT(codigo,' - ',nome,' | Estoque: ',quantidade) FROM pecas WHERE id=@id",Banco.P("@id",peca)));
            } catch(Exception erro) { Tela.Erro(erro); Close(); }
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try { OperacoesOrdem.Movimentar(peca,cmbTipo.Text,(int)numQuantidade.Value,txtMotivo.Text); LayoutTelas.MarcarSalvo(this); MessageBox.Show("Movimentação registrada."); Close(); }
            catch(Exception erro) { Tela.Erro(erro); }
        }
    }
}
