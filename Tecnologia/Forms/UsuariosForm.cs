using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class UsuariosForm : Form
    {
        public UsuariosForm() { InitializeComponent(); PrepararTela(); AlteracoesFormulario.Observar(this); }
        private int codigo;
        private async void UsuariosForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;

            cmbCampo.SelectedIndex = 0;
            Limpar();
            await CarregarAsync(true);
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
            AlteracoesFormulario.MarcarSalvo(this);
        }
        private Task CarregarAsync(bool inicial = false)
        {
            // Os nomes das colunas vêm desta lista fixa, nunca do texto digitado.
            string[] campos = { "nome", "id", "email", "perfil", "cpf", "rg", "telefone", "cidade", "data_nascimento" };
            int indice = Math.Max(0,cmbCampo.SelectedIndex);
            string campo = campos[indice];
            if (campo == "data_nascimento") campo = "DATE_FORMAT(data_nascimento, '%d/%m/%Y')";
            string busca = txtBusca.Text.Trim();
            return Tela.CarregarAsync(this, this, "Atendente", _ =>
                Banco.Consultar("SELECT id, nome, email, telefone, cpf, rg, cidade, data_nascimento, perfil, ativo, data_cadastro FROM usuarios WHERE (@vazia OR " + campo + " LIKE @busca) ORDER BY id DESC", Banco.P("@vazia", busca == ""), Banco.P("@busca", "%" + busca + "%")), ExibirLista, inicial);
        }

        private void ExibirLista(DataTable dados)
        {
            grade.DataSource = dados;
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
        private async void btnPesquisar_Click(object sender, EventArgs e)
        {
            await CarregarAsync();
        }
        private void btnExportar_Click(object sender, EventArgs e)
        { try { Sessao.Exigir("Atendente"); Exportar.Menu(btnExportar, grade, "Equipe"); } catch (Exception erro) { Tela.Erro(erro); } }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                if (!AlteracoesFormulario.PodeDescartar(this)) return;
                int selecionado = Tela.Selecionado(grade);
                DataTable dados = Banco.Consultar("SELECT * FROM usuarios WHERE id=@id",Banco.P("@id",selecionado));
                if (dados.Rows.Count == 0) throw new Exception("O registro não existe mais. Atualize a lista.");
                DataRow linha = dados.Rows[0];
                camposenha.Clear();
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
                AlteracoesFormulario.MarcarSalvo(this);
                tabcontrol3.SelectedIndex = 1;
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Contas.SalvarUsuario(codigo, camponome.Text, campoemail.Text, campotelefone.Text,
                    Tela.Cpf(campocpf.Text), camporg.Text, campocidade.Text, Tela.Nascimento(campodata_nascimento),
                    campoperfil.Text, campoativo.Checked, camposenha.Text);
                Limpar();
                tabcontrol3.SelectedIndex = 0;
                await CarregarAsync();
                if (!IsDisposed) MessageBox.Show(this, "Cadastro salvo.");
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private async void btnTrocarPerfil_Click(object sender, EventArgs e)
        {
            try
            {
                int selecionado = Tela.Selecionado(grade);
                string perfil = Convert.ToString(grade.CurrentRow.Cells["perfil"].Value);
                string nome = Convert.ToString(grade.CurrentRow.Cells["nome"].Value);
                string novoPerfil = perfil == "Atendente" ? "Técnico" : "Atendente";
                bool contaAtual = selecionado == Sessao.Id;
                string confirmacao = "Alterar o perfil de " + nome + " para " + novoPerfil + "?";
                if (contaAtual) confirmacao += Environment.NewLine + "Você voltará à tela de login.";
                if (!AlteracoesFormulario.PodeDescartar(this) || !Tela.Confirmar(confirmacao)) return;

                Contas.TrocarPerfil(selecionado, perfil);
                Limpar();
                if (contaAtual && TopLevelControl is PrincipalForm principal)
                {
                    Sessao.TrocarUsuario = true;
                    principal.Close();
                    return;
                }

                if (contaAtual)
                {
                    Close();
                    return;
                }

                await CarregarAsync();
            }
            catch (Exception erro) { Tela.Erro(erro); }
        }

        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                int selecionado = Tela.Selecionado(grade);
                if (selecionado == Sessao.Id) throw new Exception("Você não pode excluir sua própria conta.");
                if (!AlteracoesFormulario.PodeDescartar(this)) return;
                if (!Tela.Confirmar("Excluir o cadastro " + selecionado + "?")) return;
                Contas.ExcluirUsuario(selecionado);
                Limpar();
                await CarregarAsync();
            } catch (Exception erro) { Tela.Erro(erro); }
        }
    }
}
