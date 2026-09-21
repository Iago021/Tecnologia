using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class PerfilForm : Form
    {
        public PerfilForm() { InitializeComponent(); LayoutTelas.Aplicar(this); }
        private void PerfilForm_Load(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir();
                DataRow dados = Banco.Consultar("SELECT nome,email,telefone FROM usuarios WHERE id=@id",Banco.P("@id",Sessao.Id)).Rows[0];
                txtNome.Text=dados["nome"].ToString(); txtEmail.Text=dados["email"].ToString(); txtTelefone.Text=dados["telefone"].ToString();
            } catch(Exception erro) { Tela.Erro(erro); Close(); }
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Contas.AtualizarPerfil(txtNome.Text, txtEmail.Text, txtTelefone.Text, txtAtual.Text, txtNova.Text, txtConfirmacao.Text);
                LayoutTelas.MarcarSalvo(this);
                MessageBox.Show("Dados atualizados."); Close();
            } catch(Exception erro) { Tela.Erro(erro); }
        }
    }
}
