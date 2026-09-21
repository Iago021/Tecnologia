using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;
namespace Tecnologia
{
    public partial class OrdemForm : Form
    {
        public OrdemForm() { InitializeComponent(); LayoutTelas.Aplicar(this); }
        private int codigo;
        private bool podeEditar;
        public OrdemForm(int id) : this() { codigo=id; }
        private void OrdemForm_Load(object sender,EventArgs e)
        { try { Carregar(); } catch(Exception erro) { Tela.Erro(erro); Close(); } }
        private void Carregar()
        {
            Sessao.Exigir();
            DataTable tabela=Banco.Consultar(@"SELECT o.*,c.nome cliente,c.telefone,c.email,a.tipo,a.marca,a.modelo,a.numero_serie,a.cor,a.acessorios,a.estado_fisico,a.observacoes obs_aparelho,
             COALESCE(u.nome,'Não atribuído') tecnico,at.nome atendente,d.descricao,d.servico_necessario FROM ordens_servico o
             JOIN aparelhos a ON a.id=o.aparelho_id JOIN clientes c ON c.id=a.cliente_id JOIN usuarios at ON at.id=o.atendente_id
             LEFT JOIN usuarios u ON u.id=o.tecnico_id LEFT JOIN diagnosticos d ON d.ordem_id=o.id WHERE o.id=@id",Banco.P("@id",codigo));
            if(tabela.Rows.Count==0) throw new Exception("Ordem não encontrada.");
            DataRow ordem=tabela.Rows[0];
            string status=ordem["status"].ToString();
            lblTitulo.Text="Ordem "+codigo+" | "+status+" | Técnico: "+ordem["tecnico"];
            txtRecebimento.Text="Cliente: "+ordem["cliente"]+" | Telefone: "+ordem["telefone"]+" | E-mail: "+ordem["email"]+Environment.NewLine+
                "Aparelho: "+ordem["tipo"]+" "+ordem["marca"]+" "+ordem["modelo"]+" | Série/IMEI: "+ordem["numero_serie"]+" | Cor: "+ordem["cor"]+Environment.NewLine+
                "Acessórios: "+ordem["acessorios"]+" | Estado físico: "+ordem["estado_fisico"]+" | Observações do aparelho: "+ordem["obs_aparelho"]+Environment.NewLine+
                "Atendente: "+ordem["atendente"]+" | Entrada: "+ordem["data_entrada"]+" | Previsão: "+ordem["previsao_entrega"]+Environment.NewLine+
                "Conclusão: "+ordem["data_conclusao"]+" | Entrega: "+ordem["data_entrega"]+Environment.NewLine+
                "Problema: "+ordem["problema_relatado"]+Environment.NewLine+"Observações: "+ordem["observacoes"];
            txtDiagnostico.Text=ordem["descricao"].ToString(); txtServico.Text=ordem["servico_necessario"].ToString();
            numMaoObra.Value=Convert.ToDecimal(ordem["valor_mao_obra"]); numDesconto.Value=Convert.ToDecimal(ordem["desconto"]);
            cmbStatus.SelectedItem=status=="Aberta" ? "Em manutenção" : status;
            podeEditar=!Sessao.Atendente && status!="Concluída" && status!="Entregue" && (ordem["tecnico_id"]==DBNull.Value || Convert.ToInt32(ordem["tecnico_id"])==Sessao.Id);
            txtDiagnostico.ReadOnly=txtServico.ReadOnly=!podeEditar;
            cmbStatus.Enabled=numMaoObra.Enabled=numDesconto.Enabled=btnSalvar.Enabled=btnAdicionar.Enabled=btnDevolver.Enabled=cmbPeca.Enabled=numQuantidade.Enabled=podeEditar;
            btnEntregar.Enabled=Sessao.Atendente && status=="Concluída";
            CarregarPecas();
            LayoutTelas.MarcarSalvo(this);
        }
        private void CarregarPecas()
        {
            grade.DataSource=Banco.Consultar(@"SELECT op.id,p.codigo AS Código,p.nome AS Peça,op.quantidade AS Quantidade,op.valor_unitario AS Unitário,
                op.quantidade*op.valor_unitario AS Subtotal FROM ordem_pecas op JOIN pecas p ON p.id=op.peca_id WHERE op.ordem_id=@id ORDER BY op.id",Banco.P("@id",codigo));
            grade.Columns["id"].Visible=false;
            grade.Columns["Unitário"].DefaultCellStyle.Format=grade.Columns["Subtotal"].DefaultCellStyle.Format="C2";
            Tela.AjustarGrade(grade);
            Tela.Combo(cmbPeca,Banco.Consultar(@"SELECT p.id,CONCAT(p.codigo,' - ',p.nome,' | Estoque: ',p.quantidade,' | R$ ',p.valor_venda) nome
                FROM pecas p JOIN ordens_servico o ON o.id=@id JOIN aparelhos a ON a.id=o.aparelho_id
                WHERE p.ativo=1 AND p.quantidade>0 AND p.tipo=a.tipo AND (p.marca='*' OR p.marca=a.marca)
                AND (p.modelo_compativel='*' OR p.modelo_compativel=a.modelo) ORDER BY p.nome",Banco.P("@id",codigo)));
            decimal total=Convert.ToDecimal(Banco.Valor(@"SELECT valor_mao_obra-desconto+COALESCE((SELECT SUM(quantidade*valor_unitario) FROM ordem_pecas WHERE ordem_id=@id),0)
                FROM ordens_servico WHERE id=@id",Banco.P("@id",codigo)));
            lblTotal.Text="Total salvo: "+total.ToString("C2");
        }
        private void btnSalvar_Click(object sender,EventArgs e)
        {
            try
            {
                if(cmbStatus.Text=="Concluída" && !Tela.Confirmar("Concluir o serviço? Diagnóstico e peças ficarão bloqueados para preservar o histórico.")) return;
                OperacoesOrdem.Diagnosticar(codigo,txtDiagnostico.Text,txtServico.Text,cmbStatus.Text,numMaoObra.Value,numDesconto.Value);
                Carregar(); MessageBox.Show("Ordem atualizada.");
            } catch(Exception erro) { Tela.Erro(erro); }
        }
        private void btnEntregar_Click(object sender,EventArgs e)
        {
            try { if(!Tela.Confirmar("Confirmar a entrega do aparelho ao cliente?")) return; OperacoesOrdem.Entregar(codigo); Carregar(); }
            catch(Exception erro) { Tela.Erro(erro); }
        }
        private void btnAdicionar_Click(object sender,EventArgs e)
        {
            try { OperacoesOrdem.UsarPeca(codigo,Tela.Codigo(cmbPeca),(int)numQuantidade.Value); CarregarPecas(); }
            catch(Exception erro) { Tela.Erro(erro); }
        }
        private void btnDevolver_Click(object sender,EventArgs e)
        {
            try { if(!Tela.Confirmar("Remover esta peça da ordem e devolver sua quantidade ao estoque?")) return; OperacoesOrdem.DevolverPeca(codigo,Tela.Selecionado(grade)); CarregarPecas(); }
            catch(Exception erro) { Tela.Erro(erro); }
        }
        private void btnHistorico_Click(object sender,EventArgs e)
        { try { using(HistoricoForm janela=new HistoricoForm(codigo,true)) janela.ShowDialog(TopLevelControl); } catch(Exception erro) { Tela.Erro(erro); } }
        private void btnAtualizar_Click(object sender,EventArgs e)
        { try { if(Tela.Confirmar("Recarregar os dados salvos? As alterações ainda não salvas serão descartadas.")) Carregar(); } catch(Exception erro) { Tela.Erro(erro); } }
        private void btnExportar_Click(object sender,EventArgs e)
        { try { Sessao.Exigir(); Exportar.Excel(grade); } catch(Exception erro) { Tela.Erro(erro); } }
    }
}
