using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class ClientesForm : Form
    {
        public ClientesForm() { InitializeComponent(); PrepararTela(); AlteracoesFormulario.Observar(this); }
        private int codigo;
        private void ClientesForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            try
            {
                Sessao.Exigir("Atendente");

                cmbCampo.SelectedIndex = 0;
                Limpar(); Carregar();
            } catch (Exception erro) { Tela.Erro(erro); Close(); }
        }
        private void Limpar()
        {
            codigo = 0;
            camponome.Clear();
            campoemail.Clear();
            campotelefone.Clear();
            campocpf.Clear();
            camporg.Clear();
            campocidade.Clear();
            campoendereco.Clear();
            campodata_nascimento.Value = DateTime.Today; campodata_nascimento.Checked = false;
            campoativo.Checked = true;
            lblEdicao.Text = "Novo cadastro";
            AlteracoesFormulario.MarcarSalvo(this);
        }
        private void Carregar()
        {
            Sessao.Exigir("Atendente");
            // Os nomes das colunas vêm desta lista fixa, nunca do texto digitado.
            string[] campos = { "nome", "id", "email", "telefone", "cpf", "rg", "cidade", "data_nascimento" };
            int indice = Math.Max(0,cmbCampo.SelectedIndex);
            string campo = campos[indice];
            if (campo == "data_nascimento") campo = "DATE_FORMAT(data_nascimento, '%d/%m/%Y')";
            string busca = txtBusca.Text.Trim();
            grade.DataSource = Banco.Consultar("SELECT id, nome, email, telefone, cpf, rg, cidade, endereco, data_nascimento, ativo, data_cadastro FROM clientes WHERE (@vazia OR " + campo + " LIKE @busca) ORDER BY id DESC", Banco.P("@vazia", busca == ""), Banco.P("@busca", "%" + busca + "%"));
            grade.Columns["id"].HeaderText = "Código";
            grade.Columns["nome"].HeaderText = "Nome";
            grade.Columns["email"].HeaderText = "E-mail";
            grade.Columns["telefone"].HeaderText = "Telefone";
            grade.Columns["cpf"].HeaderText = "CPF";
            grade.Columns["rg"].HeaderText = "RG";
            grade.Columns["cidade"].HeaderText = "Cidade";
            grade.Columns["endereco"].HeaderText = "Endereço";
            grade.Columns["data_nascimento"].HeaderText = "Nascimento";
            grade.Columns["ativo"].HeaderText = "Cadastro ativo";
            Tela.AjustarGrade(grade);
        }
        private void btnNovo_Click(object sender, EventArgs e)
        {
            if (!AlteracoesFormulario.PodeDescartar(this)) return;
            Limpar();
            tabcontrol3.SelectedIndex = 1;
        }

        private void VoltarConsulta_Click(object sender, EventArgs e)
        {
            if (!AlteracoesFormulario.PodeDescartar(this)) return;
            Limpar();
            tabcontrol3.SelectedIndex = 0;
        }
        private void btnPesquisar_Click(object sender, EventArgs e)
        { try { Carregar(); } catch (Exception erro) { Tela.Erro(erro); } }
        private void btnExportar_Click(object sender, EventArgs e)
        { try { Sessao.Exigir("Atendente"); Exportar.Menu(btnExportar, grade, "Clientes"); } catch (Exception erro) { Tela.Erro(erro); } }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                if (!AlteracoesFormulario.PodeDescartar(this)) return;
                int selecionado = Tela.Selecionado(grade);
                DataTable dados = Banco.Consultar("SELECT * FROM clientes WHERE id=@id",Banco.P("@id",selecionado));
                if (dados.Rows.Count == 0) throw new Exception("O registro não existe mais. Atualize a lista.");
                DataRow linha = dados.Rows[0];
                camponome.Text = linha["nome"].ToString();
                campoemail.Text = linha["email"].ToString();
                campotelefone.Text = linha["telefone"].ToString();
                campocpf.Text = linha["cpf"].ToString();
                camporg.Text = linha["rg"].ToString();
                campocidade.Text = linha["cidade"].ToString();
                campoendereco.Text = linha["endereco"].ToString();
                campodata_nascimento.Value = linha["data_nascimento"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(linha["data_nascimento"]);
                campodata_nascimento.Checked = linha["data_nascimento"] != DBNull.Value;
                campoativo.Checked = Convert.ToBoolean(linha["ativo"]);
                codigo = selecionado;
                lblEdicao.Text = "Editando código " + codigo;
                AlteracoesFormulario.MarcarSalvo(this);
                tabcontrol3.SelectedIndex = 1;
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                Tela.Obrigatorio(camponome.Text, "Nome");
                Tela.Email(campoemail.Text.Trim(), false);
                Tela.Obrigatorio(campotelefone.Text, "Telefone");
                string cpf = Tela.Cpf(campocpf.Text);
                string sql;
                if (codigo == 0) sql = "INSERT INTO clientes(nome,email,telefone,cpf,rg,cidade,endereco,data_nascimento,ativo) VALUES(@nome,@email,@telefone,@cpf,@rg,@cidade,@endereco,@data_nascimento,@ativo)";
                else sql = "UPDATE clientes SET nome=@nome,email=@email,telefone=@telefone,cpf=@cpf,rg=@rg,cidade=@cidade,endereco=@endereco,data_nascimento=@data_nascimento,ativo=@ativo WHERE id=@id";
                Banco.Executar(sql, Banco.P("@nome", camponome.Text.Trim()),
                    Banco.P("@email", campoemail.Text.Trim()),
                    Banco.P("@telefone", campotelefone.Text.Trim()),
                    Banco.P("@cpf", cpf),
                    Banco.P("@rg", camporg.Text.Trim()),
                    Banco.P("@cidade", campocidade.Text.Trim()),
                    Banco.P("@endereco", campoendereco.Text.Trim()),
                    Banco.P("@data_nascimento", Tela.Nascimento(campodata_nascimento)),
                    Banco.P("@ativo", campoativo.Checked),
                    Banco.P("@id",codigo));
                Limpar();
                tabcontrol3.SelectedIndex = 0;
                Carregar();
                MessageBox.Show("Cadastro salvo.");
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                int selecionado = Tela.Selecionado(grade);
                if (!AlteracoesFormulario.PodeDescartar(this)) return;
                if (!Tela.Confirmar("Excluir o cadastro " + selecionado + "?")) return;
                Banco.Executar("DELETE FROM clientes WHERE id=@id",Banco.P("@id",selecionado));
                Limpar(); Carregar();
            } catch (Exception erro) { Tela.Erro(erro); }
        }
    }
}
