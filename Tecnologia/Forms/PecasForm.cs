using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class PecasForm : Form
    {
        public PecasForm() { InitializeComponent(); Tema.Aplicar(this); }
        private int codigo;
        private void PecasForm_Load(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Técnico");

                cmbCampo.SelectedIndex = 0;
                Limpar(); Carregar();
            } catch (Exception erro) { Tela.Erro(erro); Close(); }
        }
        private void Limpar()
        {
            codigo = 0;
            campocodigo.Clear();
            camponome.Clear();
            campotipo.SelectedIndex = 0;
            campomarca.Clear();
            campomodelo_compativel.Clear();
            campoestoque_minimo.Value = 0;
            campovalor_compra.Value = 0;
            campovalor_venda.Value = 0;
            campoativo.Checked = true;
            lblEdicao.Text = "Novo cadastro";
        }
        private void Carregar()
        {
            Sessao.Exigir("Técnico");
            // Os nomes das colunas vêm desta lista fixa, nunca do texto digitado.
            string[] campos = { "nome", "codigo", "modelo_compativel", "marca", "tipo" };
            int indice = Math.Max(0,cmbCampo.SelectedIndex);
            grade.DataSource = Banco.Consultar("SELECT id, codigo, nome, tipo, marca, modelo_compativel, estoque_minimo, valor_compra, valor_venda, ativo, quantidade FROM pecas WHERE " + campos[indice] + " LIKE @busca ORDER BY id DESC", Banco.P("@busca", "%" + txtBusca.Text.Trim() + "%"));
            grade.Columns["id"].HeaderText = "Código";
            grade.Columns["codigo"].HeaderText = "Código da peça";
            grade.Columns["nome"].HeaderText = "Nome da peça";
            grade.Columns["tipo"].HeaderText = "Tipo de aparelho";
            grade.Columns["marca"].HeaderText = "Marca compatível";
            grade.Columns["modelo_compativel"].HeaderText = "Modelo compatível";
            grade.Columns["estoque_minimo"].HeaderText = "Estoque mínimo";
            grade.Columns["valor_compra"].HeaderText = "Valor de compra";
            grade.Columns["valor_venda"].HeaderText = "Valor de venda";
            grade.Columns["ativo"].HeaderText = "Peça ativa";
            Tela.AjustarGrade(grade);
        }
        private void btnNovo_Click(object sender, EventArgs e) { Limpar(); }
        private void btnPesquisar_Click(object sender, EventArgs e)
        { try { Carregar(); } catch (Exception erro) { Tela.Erro(erro); } }
        private void btnExportar_Click(object sender, EventArgs e)
        { try { Sessao.Exigir("Técnico"); Exportar.Excel(grade); } catch (Exception erro) { Tela.Erro(erro); } }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Técnico");
                int selecionado = Tela.Selecionado(grade);
                DataTable dados = Banco.Consultar("SELECT * FROM pecas WHERE id=@id",Banco.P("@id",selecionado));
                if (dados.Rows.Count == 0) throw new Exception("O registro não existe mais. Atualize a lista.");
                DataRow linha = dados.Rows[0];
                campocodigo.Text = linha["codigo"].ToString();
                camponome.Text = linha["nome"].ToString();
                campotipo.SelectedItem = linha["tipo"].ToString();
                campomarca.Text = linha["marca"].ToString();
                campomodelo_compativel.Text = linha["modelo_compativel"].ToString();
                campoestoque_minimo.Value = Convert.ToDecimal(linha["estoque_minimo"]);
                campovalor_compra.Value = Convert.ToDecimal(linha["valor_compra"]);
                campovalor_venda.Value = Convert.ToDecimal(linha["valor_venda"]);
                campoativo.Checked = Convert.ToBoolean(linha["ativo"]);
                codigo = selecionado;
                lblEdicao.Text = "Editando código " + codigo;
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Técnico");
                Tela.Obrigatorio(campocodigo.Text, "Código da peça");
                Tela.Obrigatorio(camponome.Text, "Nome da peça");
                Tela.Obrigatorio(campomarca.Text, "Marca compatível (* = todas)");
                Tela.Obrigatorio(campomodelo_compativel.Text, "Modelo compatível (* = todos)");
                string sql;
                if (codigo == 0) sql = "INSERT INTO pecas(codigo,nome,tipo,marca,modelo_compativel,estoque_minimo,valor_compra,valor_venda,ativo) VALUES(@codigo,@nome,@tipo,@marca,@modelo_compativel,@estoque_minimo,@valor_compra,@valor_venda,@ativo)";
                else sql = "UPDATE pecas SET codigo=@codigo,nome=@nome,tipo=@tipo,marca=@marca,modelo_compativel=@modelo_compativel,estoque_minimo=@estoque_minimo,valor_compra=@valor_compra,valor_venda=@valor_venda,ativo=@ativo WHERE id=@id";
                Banco.Executar(sql, Banco.P("@codigo", campocodigo.Text.Trim()),
                    Banco.P("@nome", camponome.Text.Trim()),
                    Banco.P("@tipo", campotipo.Text),
                    Banco.P("@marca", campomarca.Text.Trim()),
                    Banco.P("@modelo_compativel", campomodelo_compativel.Text.Trim()),
                    Banco.P("@estoque_minimo", campoestoque_minimo.Value),
                    Banco.P("@valor_compra", campovalor_compra.Value),
                    Banco.P("@valor_venda", campovalor_venda.Value),
                    Banco.P("@ativo", campoativo.Checked),
                    Banco.P("@id",codigo));
                Limpar(); Carregar();
                MessageBox.Show("Cadastro salvo.");
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Técnico");
                int selecionado = Tela.Selecionado(grade);
                if (!Tela.Confirmar("Excluir o cadastro " + selecionado + "?")) return;
                Banco.Executar("DELETE FROM pecas WHERE id=@id",Banco.P("@id",selecionado));
                Limpar(); Carregar();
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnMovimentar_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Técnico");
                using (MovimentoForm janela = new MovimentoForm(Tela.Selecionado(grade))) janela.ShowDialog(this);
                Carregar();
            } catch (Exception erro) { Tela.Erro(erro); }
        }
        private void btnHistorico_Click(object sender, EventArgs e)
        {
            try
            {
                Sessao.Exigir("Técnico");
                using (HistoricoForm janela = new HistoricoForm(Tela.Selecionado(grade),false)) janela.ShowDialog(this);
            } catch (Exception erro) { Tela.Erro(erro); }
        }
    }
}
