using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class AparelhosForm : Form
    {
        public AparelhosForm() { InitializeComponent(); PrepararTela(); AlteracoesFormulario.Observar(this); }
        private int codigo;
        private void AparelhosForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            try
            {
                Sessao.Exigir("Atendente");
                Tela.Combo(campocliente_id, Banco.Consultar("SELECT id, CONCAT(id,' - ',nome) nome FROM clientes ORDER BY nome"));

                cmbCampo.SelectedIndex = 0;
                Limpar(); Carregar();
            } catch (Exception erro) { Tela.Erro(erro); Close(); }
        }
        private void Limpar()
        {
            codigo = 0;
            campocliente_id.SelectedIndex = -1;
            campotipo.SelectedIndex = 0;
            campomarca.Clear();
            campomodelo.Clear();
            camponumero_serie.Clear();
            campocor.Clear();
            campoacessorios.Clear();
            campoestado_fisico.Clear();
            campoobservacoes.Clear();
            lblEdicao.Text = "Novo cadastro";
            AlteracoesFormulario.MarcarSalvo(this);
        }
        private void Carregar()
        {
            Sessao.Exigir("Atendente");
            // Os nomes das colunas vêm desta lista fixa, nunca do texto digitado.
            string[] campos = { "modelo", "id", "cliente_id", "marca", "numero_serie", "tipo" };
            int indice = Math.Max(0,cmbCampo.SelectedIndex);
            grade.DataSource = Banco.Consultar("SELECT id, cliente_id, tipo, marca, modelo, numero_serie, cor, acessorios, estado_fisico, observacoes FROM aparelhos WHERE " + campos[indice] + " LIKE @busca ORDER BY id DESC", Banco.P("@busca", "%" + txtBusca.Text.Trim() + "%"));
            grade.Columns["id"].HeaderText = "Código";
            grade.Columns["cliente_id"].HeaderText = "Cliente";
            grade.Columns["tipo"].HeaderText = "Tipo";
            grade.Columns["marca"].HeaderText = "Marca";
            grade.Columns["modelo"].HeaderText = "Modelo";
            grade.Columns["numero_serie"].HeaderText = "Número de série / IMEI";
            grade.Columns["cor"].HeaderText = "Cor";
            grade.Columns["acessorios"].HeaderText = "Acessórios recebidos";
            grade.Columns["estado_fisico"].HeaderText = "Estado físico";
            grade.Columns["observacoes"].HeaderText = "Observações";
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
        { try { Sessao.Exigir("Atendente"); Exportar.Menu(btnExportar, grade, "Aparelhos"); } catch (Exception erro) { Tela.Erro(erro); } }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Atendente");
                if (!AlteracoesFormulario.PodeDescartar(this)) return;
                int selecionado = Tela.Selecionado(grade);
                DataTable dados = Banco.Consultar("SELECT * FROM aparelhos WHERE id=@id",Banco.P("@id",selecionado));
                if (dados.Rows.Count == 0) throw new Exception("O registro não existe mais. Atualize a lista.");
                DataRow linha = dados.Rows[0];
                campocliente_id.SelectedValue = Convert.ToInt32(linha["cliente_id"]);
                campotipo.SelectedItem = linha["tipo"].ToString();
                campomarca.Text = linha["marca"].ToString();
                campomodelo.Text = linha["modelo"].ToString();
                camponumero_serie.Text = linha["numero_serie"].ToString();
                campocor.Text = linha["cor"].ToString();
                campoacessorios.Text = linha["acessorios"].ToString();
                campoestado_fisico.Text = linha["estado_fisico"].ToString();
                campoobservacoes.Text = linha["observacoes"].ToString();
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
                Aparelhos.Salvar(codigo, Tela.Codigo(campocliente_id), campotipo.Text, campomarca.Text,
                    campomodelo.Text, camponumero_serie.Text, campocor.Text, campoacessorios.Text,
                    campoestado_fisico.Text, campoobservacoes.Text);
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
                Banco.Executar("DELETE FROM aparelhos WHERE id=@id",Banco.P("@id",selecionado));
                Limpar(); Carregar();
            } catch (Exception erro) { Tela.Erro(erro); }
        }
    }
}
