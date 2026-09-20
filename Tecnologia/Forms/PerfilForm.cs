using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class PerfilForm : Form
    {
        public PerfilForm() { InitializeComponent(); }
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
                Sessao.Exigir(); Tela.Obrigatorio(txtNome.Text,"nome"); Tela.Email(txtEmail.Text.Trim(),true);
                string antiga=Convert.ToString(Banco.Valor("SELECT senha_hash FROM usuarios WHERE id=@id",Banco.P("@id",Sessao.Id)));
                if (!Senha.Conferir(txtAtual.Text,antiga)) throw new Exception("Senha atual incorreta.");
                if (txtNova.Text!=txtConfirmacao.Text) throw new Exception("A confirmação da nova senha está diferente.");
                string nova=txtNova.Text=="" ? antiga : Senha.Criar(txtNova.Text);
                Banco.Executar("UPDATE usuarios SET nome=@nome,email=@email,telefone=@telefone,senha_hash=@senha WHERE id=@id",
                    Banco.P("@nome",txtNome.Text.Trim()),Banco.P("@email",txtEmail.Text.Trim()),Banco.P("@telefone",txtTelefone.Text.Trim()),Banco.P("@senha",nova),Banco.P("@id",Sessao.Id));
                Sessao.Nome=txtNome.Text.Trim();
                MessageBox.Show("Dados atualizados."); Close();
            } catch(Exception erro) { Tela.Erro(erro); }
        }
    }
}
