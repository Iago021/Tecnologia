using System;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class PerfilForm : Form
    {
        public PerfilForm() { InitializeComponent(); PrepararTela(); AlteracoesFormulario.Observar(this); }
        private async void PerfilForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            await Tela.CarregarAsync(this, this, "", conta =>
                Banco.Consultar("SELECT nome,email,telefone FROM usuarios WHERE id=@id", Banco.P("@id", conta["id"])).Rows[0],
                dados =>
                {
                    txtNome.Text = dados["nome"].ToString();
                    txtEmail.Text = dados["email"].ToString();
                    txtTelefone.Text = dados["telefone"].ToString();
                    AlteracoesFormulario.MarcarSalvo(this);
                }, true);
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Contas.AtualizarPerfil(txtNome.Text, txtEmail.Text, txtTelefone.Text, txtAtual.Text, txtNova.Text, txtConfirmacao.Text);
                AlteracoesFormulario.MarcarSalvo(this);
                MessageBox.Show("Dados atualizados."); Close();
            } catch(Exception erro) { Tela.Erro(erro); }
        }
    }
}
