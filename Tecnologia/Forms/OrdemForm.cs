using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Tecnologia
{
    public partial class OrdemForm : Form
    {
        public OrdemForm() { InitializeComponent(); PrepararTela(); AlteracoesFormulario.Observar(this); }
        private int codigo;
        private int tecnicoAtual;
        private string nomeTecnicoAtual;
        public OrdemForm(int id) : this() { codigo=id; }
        private async void OrdemForm_Load(object sender, EventArgs e)
        {
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            await CarregarAsync(true);
        }

        private Task CarregarAsync(bool inicial = false)
        {
            return Tela.CarregarAsync(this, this, "", conta =>
            {
                DataRow ordem = ConsultarOrdem();
                bool atendente = conta["perfil"].ToString() == "Atendente";
                DataTable tecnicos = atendente && ordem["status"].ToString() == "Aberta"
                    ? Banco.Consultar("SELECT id, CONCAT(nome, ' (#', id, ')') AS nome FROM usuarios WHERE ativo=1 AND perfil='Técnico' ORDER BY nome")
                    : null;
                return Tuple.Create(ordem, atendente, tecnicos, ConsultarPecas());
            }, dados =>
            {
                ExibirOrdem(dados.Item1, dados.Item2, dados.Item3);
                ExibirPecas(dados.Item4);
                AlteracoesFormulario.MarcarSalvo(this);
            }, inicial);
        }

        private DataRow ConsultarOrdem()
        {
            DataTable tabela=Banco.Consultar(@"SELECT o.*,c.nome cliente,c.telefone,c.email,a.tipo,a.marca,a.modelo,a.numero_serie,a.cor,a.acessorios,a.estado_fisico,a.observacoes obs_aparelho,
             COALESCE(u.nome,'Não atribuído') tecnico,at.nome atendente,d.descricao,d.servico_necessario FROM ordens_servico o
             JOIN aparelhos a ON a.id=o.aparelho_id JOIN clientes c ON c.id=a.cliente_id JOIN usuarios at ON at.id=o.atendente_id
             LEFT JOIN usuarios u ON u.id=o.tecnico_id LEFT JOIN diagnosticos d ON d.ordem_id=o.id WHERE o.id=@id",Banco.P("@id",codigo));
            if(tabela.Rows.Count==0) throw new Exception("Ordem não encontrada.");
            return tabela.Rows[0];
        }

        private void ExibirOrdem(DataRow ordem, bool atendente, DataTable tecnicos)
        {
            string status = ordem["status"].ToString();
            lblTitulo.Text = "Ordem " + codigo + " | " + status;
            txtRecebimento.Text="Cliente: "+ordem["cliente"]+" | Telefone: "+ordem["telefone"]+" | E-mail: "+ordem["email"]+Environment.NewLine+
                "Aparelho: "+ordem["tipo"]+" "+ordem["marca"]+" "+ordem["modelo"]+" | Série/IMEI: "+ordem["numero_serie"]+" | Cor: "+ordem["cor"]+Environment.NewLine+
                "Acessórios: "+ordem["acessorios"]+" | Estado físico: "+ordem["estado_fisico"]+" | Observações do aparelho: "+ordem["obs_aparelho"]+Environment.NewLine+
                "Atendente: "+ordem["atendente"]+" | Técnico: "+ordem["tecnico"]+Environment.NewLine+
                "Entrada: "+ordem["data_entrada"]+" | Previsão: "+ordem["previsao_entrega"]+Environment.NewLine+
                "Conclusão: "+ordem["data_conclusao"]+" | Entrega: "+ordem["data_entrega"]+Environment.NewLine+
                "Problema: "+ordem["problema_relatado"]+Environment.NewLine+"Observações: "+ordem["observacoes"];
            txtDiagnostico.Text=ordem["descricao"].ToString(); txtServico.Text=ordem["servico_necessario"].ToString();
            numMaoObra.Value=Convert.ToDecimal(ordem["valor_mao_obra"]); numDesconto.Value=Convert.ToDecimal(ordem["desconto"]);
            cmbStatus.SelectedItem=status=="Aberta" ? "Em manutenção" : status;
            tecnicoAtual = ordem.Field<int?>("tecnico_id") ?? 0;
            nomeTecnicoAtual = ordem["tecnico"].ToString();
            bool podeEditar = !atendente && (status == "Aberta" || status == "Em manutenção" || status == "Aguardando peça")
                && (tecnicoAtual == 0 || tecnicoAtual == Sessao.Id);
            txtDiagnostico.ReadOnly=txtServico.ReadOnly=!podeEditar;
            cmbStatus.Enabled = numMaoObra.Enabled = numDesconto.Enabled = btnSalvar.Enabled = podeEditar;
            cmbPeca.Enabled = numQuantidade.Enabled = btnAdicionar.Enabled = btnDevolver.Enabled = podeEditar && status != "Aberta";
            btnEntregar.Enabled = atendente && status == "Concluída";
            painelTecnico.Visible = tecnicos != null;
            if (tecnicos != null)
            {
                DataRow semTecnico = tecnicos.NewRow();
                semTecnico["id"] = 0;
                semTecnico["nome"] = "Sem responsável";
                tecnicos.Rows.InsertAt(semTecnico, 0);
                Tela.Combo(cmbTecnico, tecnicos);
                cmbTecnico.SelectedValue = tecnicoAtual;
                if (cmbTecnico.SelectedIndex < 0) cmbTecnico.SelectedIndex = 0;
            }
        }

        private Tuple<DataTable, DataTable, decimal> ConsultarPecas()
        {
            DataTable usadas = Banco.Consultar(@"SELECT op.id,p.codigo AS Código,p.nome AS Peça,op.quantidade AS Quantidade,op.valor_unitario AS Unitário,
                op.quantidade*op.valor_unitario AS Subtotal FROM ordem_pecas op JOIN pecas p ON p.id=op.peca_id WHERE op.ordem_id=@id ORDER BY op.id",Banco.P("@id",codigo));
            DataTable disponiveis = Banco.Consultar(@"SELECT p.id,CONCAT(p.codigo,' - ',p.nome,' | Estoque: ',p.quantidade,' | R$ ',p.valor_venda) nome
                FROM pecas p JOIN ordens_servico o ON o.id=@id JOIN aparelhos a ON a.id=o.aparelho_id
                WHERE p.ativo=1 AND p.quantidade>0 AND p.tipo=a.tipo AND (p.marca='*' OR p.marca=a.marca)
                AND (p.modelo_compativel='*' OR p.modelo_compativel=a.modelo) ORDER BY p.nome",Banco.P("@id",codigo));
            decimal total=Convert.ToDecimal(Banco.Valor(@"SELECT valor_mao_obra-desconto+COALESCE((SELECT SUM(quantidade*valor_unitario) FROM ordem_pecas WHERE ordem_id=@id),0)
                FROM ordens_servico WHERE id=@id",Banco.P("@id",codigo)));
            return Tuple.Create(usadas, disponiveis, total);
        }

        private void ExibirPecas(Tuple<DataTable, DataTable, decimal> dados)
        {
            grade.DataSource = dados.Item1;
            grade.Columns["id"].Visible = false;
            grade.Columns["Unitário"].DefaultCellStyle.Format = grade.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
            Tela.AjustarGrade(grade);
            Tela.Combo(cmbPeca, dados.Item2);
            lblTotal.Text = "Total salvo: " + dados.Item3.ToString("C2");
        }

        private Task CarregarPecasAsync()
        {
            return Tela.CarregarAsync(this, this, "Técnico", _ => ConsultarPecas(), ExibirPecas);
        }

        private async void btnAtribuir_Click(object sender, EventArgs e)
        {
            try
            {
                int tecnico = Tela.Codigo(cmbTecnico);
                if (tecnico == tecnicoAtual)
                {
                    MessageBox.Show("A ordem já está com essa atribuição.");
                    return;
                }

                if (tecnicoAtual != 0)
                {
                    string confirmacao = tecnico == 0
                        ? "Remover " + nomeTecnicoAtual + " da ordem?"
                        : "Trocar " + nomeTecnicoAtual + " por " + cmbTecnico.Text + "?";
                    if (!Tela.Confirmar(confirmacao)) return;
                }
                OperacoesOrdem.AtribuirTecnico(codigo, tecnico, tecnicoAtual);
                await CarregarAsync();
                if (!IsDisposed) MessageBox.Show(this, "Responsável atualizado.");
            }
            catch (Exception erro) { Tela.Erro(erro); }
        }

        private async void btnEditarRecebimento_Click(object sender, EventArgs e)
        {
            try
            {
                if (!AlteracoesFormulario.PodeDescartar(this)) return;
                using (AbrirOrdemForm janela = new AbrirOrdemForm(codigo))
                    if (janela.ShowDialog(this) == DialogResult.OK) await CarregarAsync();
            }
            catch (Exception erro) { Tela.Erro(erro); }
        }

        private async void btnCancelarOrdem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Tela.Confirmar("Cancelar a ordem " + codigo + "? O cancelamento ficará registrado no histórico.")) return;
                OperacoesOrdem.Cancelar(codigo);
                await CarregarAsync();
            }
            catch (Exception erro) { Tela.Erro(erro); }
        }

        private async void btnSalvar_Click(object sender,EventArgs e)
        {
            try
            {
                if(cmbStatus.Text=="Concluída" && !Tela.Confirmar("Concluir o serviço? Diagnóstico e peças ficarão bloqueados para preservar o histórico.")) return;
                OperacoesOrdem.Diagnosticar(codigo,txtDiagnostico.Text,txtServico.Text,cmbStatus.Text,numMaoObra.Value,numDesconto.Value);
                await CarregarAsync();
                if (!IsDisposed) MessageBox.Show(this, "Ordem atualizada.");
            } catch(Exception erro) { Tela.Erro(erro); }
        }
        private async void btnEntregar_Click(object sender,EventArgs e)
        {
            try { if(!Tela.Confirmar("Confirmar a entrega do aparelho ao cliente?")) return; OperacoesOrdem.Entregar(codigo); await CarregarAsync(); }
            catch(Exception erro) { Tela.Erro(erro); }
        }
        private async void btnAdicionar_Click(object sender,EventArgs e)
        {
            try { OperacoesOrdem.UsarPeca(codigo,Tela.Codigo(cmbPeca),(int)numQuantidade.Value); await CarregarPecasAsync(); }
            catch(Exception erro) { Tela.Erro(erro); }
        }
        private async void btnDevolver_Click(object sender,EventArgs e)
        {
            try { if(!Tela.Confirmar("Remover esta peça da ordem e devolver sua quantidade ao estoque?")) return; OperacoesOrdem.DevolverPeca(codigo,Tela.Selecionado(grade)); await CarregarPecasAsync(); }
            catch(Exception erro) { Tela.Erro(erro); }
        }
        private void btnHistorico_Click(object sender,EventArgs e)
        { try { using(HistoricoForm janela=new HistoricoForm(codigo,true)) janela.ShowDialog(TopLevelControl); } catch(Exception erro) { Tela.Erro(erro); } }
        private async void btnAtualizar_Click(object sender,EventArgs e)
        { try { if (AlteracoesFormulario.PodeDescartar(this)) await CarregarAsync(); } catch(Exception erro) { Tela.Erro(erro); } }
        private void btnExportar_Click(object sender,EventArgs e)
        { try { Sessao.Exigir(); Exportar.Menu(btnExportar, grade, "Peças da ordem " + codigo); } catch(Exception erro) { Tela.Erro(erro); } }
    }
}
