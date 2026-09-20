using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class UsuariosForm : Form
    {
        public UsuariosForm() { InitializeComponent(); Tema.Aplicar(this); }
        private int codigo;
        private void UsuariosForm_Load(object sender, EventArgs e)
        {
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
            campodata_nascimento.Value = DateTime.Today; campodata_nascimento.Checked = false;
            campoperfil.SelectedIndex = 0;
            campoativo.Checked = true;
            camposenha.Clear();
            lblEdicao.Text = "Novo cadastro";
        }
        private void Carregar()
        {
            Sessao.Exigir("Atendente");
            // Os nomes das colunas vêm desta lista fixa, nunca do texto digitado.
            string[] campos = { "nome", "id", "email", "perfil", "cpf", "rg", "telefone", "cidade", "data_nascimento" };
            int indice = Math.Max(0,cmbCampo.SelectedIndex);
            grade.DataSource = Banco.Consultar("SELECT id, nome, email, telefone, cpf, rg, cidade, data_nascimento, perfil, ativo, data_cadastro FROM usuarios WHERE " + campos[indice] + " LIKE @busca ORDER BY id DESC", Banco.P("@busca", "%" + txtBusca.Text.Trim() + "%"));
            grade.Columns["id"].HeaderText = "Código";
            grade.Columns["nome"].HeaderText = "Nome";
            grade.Columns["email"].HeaderText = "E-mail de acesso";
            grade.Columns["telefone"].HeaderText = "Telefone";
            grade.Columns["cpf"].HeaderText = "CPF";
            grade.Columns["rg"].HeaderText = "RG";
            grade.Columns["cidade"].HeaderText = "Cidade";
            grade.Columns["data_nascimento"].HeaderText = "Nascimento";
            grade.Columns["perfil"].HeaderText = "Perfil";
            grade.Columns["ativo"].HeaderText = "Conta ativa";
            Tela.AjustarGrade(grade);
        }
        private void btnNovo_Click(object sender, EventArgs e) { Limpar(); }
        private void btnPesquisar_Click(object sender, EventArgs e)
        { try { Carregar(); } catch (Exception erro) { Tela.Erro(erro); } }
        private void btnExportar_Click(object sender, EventArgs e)
        { try { Sessao.Exigir("Atendente"); Exportar.Excel(grade); } catch (Exception erro) { Tela.Erro(erro); } }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                int selecionado = Tela.Selecionado(grade);
                DataTable dados = Banco.Consultar("SELECT * FROM usuarios WHERE id=@id",Banco.P("@id",selecionado));
                if (dados.Rows.Count == 0) throw new Exception("O registro não existe mais. Atualize a lista.");
                DataRow linha = dados.Rows[0];
                camponome.Text = linha["nome"].ToString();
                campoemail.Text = linha["email"].ToString();
                campotelefone.Text = linha["telefone"].ToString();
                campocpf.Text = linha["cpf"].ToString();
                camporg.Text = linha["rg"].ToString();
                campocidade.Text = linha["cidade"].ToString();
                campodata_nascimento.Value = linha["data_nascimento"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(linha["data_nascimento"]);
                campodata_nascimento.Checked = linha["data_nascimento"] != DBNull.Value;
                campoperfil.SelectedItem = linha["perfil"].ToString();
                campoativo.Checked = Convert.ToBoolean(linha["ativo"]);
                codigo = selecionado;
                lblEdicao.Text = "Editando código " + codigo;
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                Tela.Obrigatorio(camponome.Text, "Nome");
                Tela.Email(campoemail.Text.Trim(), true);
                Tela.Obrigatorio(campotelefone.Text, "Telefone");
                string cpf = campocpf.Text.Replace(".", "").Replace("-", "").Trim();
                if (cpf != "" && !System.Text.RegularExpressions.Regex.IsMatch(cpf, @"^\d{11}$")) throw new Exception("Informe o CPF com 11 números ou deixe em branco.");
                if (codigo == Sessao.Id) throw new Exception("Use Minha conta para alterar seus próprios dados.");
                if (codigo != 0 && (!campoativo.Checked || campoperfil.Text != "Técnico") &&
                    Convert.ToInt32(Banco.Valor("SELECT COUNT(*) FROM ordens_servico WHERE tecnico_id=@id AND status NOT IN ('Concluída','Entregue')",Banco.P("@id",codigo))) > 0)
                    throw new Exception("Este técnico possui ordens em andamento. Conclua os serviços antes de desativar ou mudar seu perfil.");
                string hash = "";
                if (codigo == 0 || camposenha.Text != "") hash = Senha.Criar(camposenha.Text);
                else hash = Convert.ToString(Banco.Valor("SELECT senha_hash FROM usuarios WHERE id=@id", Banco.P("@id",codigo)));
                string sql;
                if (codigo == 0) sql = "INSERT INTO usuarios(nome,email,telefone,cpf,rg,cidade,data_nascimento,perfil,ativo,senha_hash) VALUES(@nome,@email,@telefone,@cpf,@rg,@cidade,@data_nascimento,@perfil,@ativo,@senha_hash)";
                else sql = "UPDATE usuarios SET nome=@nome,email=@email,telefone=@telefone,cpf=@cpf,rg=@rg,cidade=@cidade,data_nascimento=@data_nascimento,perfil=@perfil,ativo=@ativo,senha_hash=@senha_hash WHERE id=@id";
                Banco.Executar(sql, Banco.P("@nome", camponome.Text.Trim()),
                    Banco.P("@email", campoemail.Text.Trim()),
                    Banco.P("@telefone", campotelefone.Text.Trim()),
                    Banco.P("@cpf", Tela.Opcional(campocpf.Text.Replace(".", "").Replace("-", "").Trim())),
                    Banco.P("@rg", camporg.Text.Trim()),
                    Banco.P("@cidade", campocidade.Text.Trim()),
                    Banco.P("@data_nascimento", Tela.Nascimento(campodata_nascimento)),
                    Banco.P("@perfil", campoperfil.Text),
                    Banco.P("@ativo", campoativo.Checked),
                    Banco.P("@senha_hash", hash),
                    Banco.P("@id",codigo));
                Limpar(); Carregar();
                MessageBox.Show("Cadastro salvo.");
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                int selecionado = Tela.Selecionado(grade);
                if (selecionado == Sessao.Id) throw new Exception("Você não pode excluir sua própria conta.");
                if (!Tela.Confirmar("Excluir o cadastro " + selecionado + "?")) return;
                Banco.Executar("DELETE FROM usuarios WHERE id=@id",Banco.P("@id",selecionado));
                Limpar(); Carregar();
            } catch (Exception erro) { Tela.Erro(erro); }
        }
    }
}
