using System;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class MovimentoForm : Form
    {
        public MovimentoForm() { InitializeComponent(); PrepararTela(); AlteracoesFormulario.Observar(this); }
        private int peca;
        public MovimentoForm(int codigo) : this() { peca=codigo; }
        private async void MovimentoForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            cmbTipo.SelectedIndex = 0;
            await Tela.CarregarAsync(this, this, "Técnico", _ =>
            {
                object descricao = Banco.Valor("SELECT CONCAT(codigo,' - ',nome,' | Estoque: ',quantidade) FROM pecas WHERE id=@id", Banco.P("@id", peca));
                if (descricao == null) throw new InvalidOperationException("Peça não encontrada.");
                return descricao.ToString();
            }, descricao => lblPeca.Text = descricao, true);
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try { Estoque.Movimentar(peca,cmbTipo.Text,(int)numQuantidade.Value,txtMotivo.Text); AlteracoesFormulario.MarcarSalvo(this); MessageBox.Show("Movimentação registrada."); Close(); }
            catch(Exception erro) { Tela.Erro(erro); }
        }
    }
}
