using System;
using System.Data;
using MySqlConnector;
namespace Tecnologia
{
    // As transações mantêm a ordem, o histórico e o estoque consistentes.
    internal static class OperacoesOrdem
    {
        public static int Abrir(int aparelho, string problema, DateTime? previsao, string observacoes)
        {
            Sessao.Exigir("Atendente");
            Tela.Obrigatorio(problema, "problema relatado");
            if (previsao.HasValue && previsao.Value.Date < DateTime.Today) throw new Exception("A previsão não pode ser anterior a hoje.");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "SELECT c.ativo FROM aparelhos a JOIN clientes c ON c.id=a.cliente_id WHERE a.id=@id FOR UPDATE", Banco.P("@id", aparelho)))
                {
                    object ativo = comando.ExecuteScalar();
                    if (ativo == null || !Convert.ToBoolean(ativo)) throw new Exception("Selecione um aparelho de um cliente ativo.");
                }
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "SELECT COUNT(*) FROM ordens_servico WHERE aparelho_id=@id AND status<>'Entregue'", Banco.P("@id", aparelho)))
                    if (Convert.ToInt32(comando.ExecuteScalar()) > 0) throw new Exception("Este aparelho já possui uma ordem em andamento ou aguardando entrega.");
                int id;
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "INSERT INTO ordens_servico(aparelho_id,atendente_id,problema_relatado,previsao_entrega,observacoes) VALUES(@aparelho,@usuario,@problema,@previsao,@obs)",
                    Banco.P("@aparelho", aparelho), Banco.P("@usuario", Sessao.Id), Banco.P("@problema", problema.Trim()),
                    Banco.P("@previsao", previsao), Banco.P("@obs", observacoes.Trim())))
                { comando.ExecuteNonQuery(); id = (int)comando.LastInsertedId; }
                Historico(conexao, transacao, id, "", "Aberta");
                transacao.Commit();
                return id;
            }
        }
        public static DataRow Travar(MySqlConnection conexao, MySqlTransaction transacao, int ordem)
        {
            using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                "SELECT * FROM ordens_servico WHERE id=@id FOR UPDATE", Banco.P("@id", ordem)))
            using (MySqlDataAdapter adaptador = new MySqlDataAdapter(comando))
            {
                DataTable tabela = new DataTable(); adaptador.Fill(tabela);
                if (tabela.Rows.Count == 0) throw new Exception("Ordem não encontrada.");
                return tabela.Rows[0];
            }
        }
        private static void ConferirTecnico(DataRow ordem, bool permitirSemTecnico)
        {
            string status = ordem["status"].ToString();
            if (status == "Concluída" || status == "Entregue") throw new Exception("Esta ordem já foi concluída e não pode mais ser alterada.");
            if (ordem["tecnico_id"] == DBNull.Value)
            { if (!permitirSemTecnico) throw new Exception("Salve o diagnóstico primeiro para assumir esta ordem."); }
            else if (Convert.ToInt32(ordem["tecnico_id"]) != Sessao.Id)
                throw new Exception("Esta ordem está sob responsabilidade de outro técnico.");
        }
        public static void Diagnosticar(int id, string descricao, string servico, string status, decimal maoObra, decimal desconto)
        {
            Sessao.Exigir("Técnico");
            Tela.Obrigatorio(descricao, "diagnóstico"); Tela.Obrigatorio(servico, "serviço necessário");
            if (status != "Em manutenção" && status != "Aguardando peça" && status != "Concluída") throw new Exception("Selecione um andamento válido.");
            if (maoObra < 0 || desconto < 0) throw new Exception("Os valores não podem ser negativos.");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                DataRow ordem = Travar(conexao, transacao, id);
                ConferirTecnico(ordem, true);
                decimal pecas;
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "SELECT COALESCE(SUM(quantidade*valor_unitario),0) FROM ordem_pecas WHERE ordem_id=@id", Banco.P("@id", id)))
                    pecas = Convert.ToDecimal(comando.ExecuteScalar());
                if (desconto > maoObra + pecas) throw new Exception("O desconto não pode ultrapassar o total do serviço.");
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "INSERT INTO diagnosticos(ordem_id,tecnico_id,descricao,servico_necessario) VALUES(@id,@tecnico,@descricao,@servico) ON DUPLICATE KEY UPDATE tecnico_id=@tecnico,descricao=@descricao,servico_necessario=@servico,data_diagnostico=NOW()",
                    Banco.P("@id", id), Banco.P("@tecnico", Sessao.Id), Banco.P("@descricao", descricao.Trim()), Banco.P("@servico", servico.Trim()))) comando.ExecuteNonQuery();
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "UPDATE ordens_servico SET tecnico_id=@tecnico,status=@status,valor_mao_obra=@valor,desconto=@desconto,data_conclusao=@conclusao WHERE id=@id",
                    Banco.P("@tecnico", Sessao.Id), Banco.P("@status", status), Banco.P("@valor", maoObra), Banco.P("@desconto", desconto),
                    Banco.P("@conclusao", status == "Concluída" ? (object)DateTime.Now : DBNull.Value), Banco.P("@id", id))) comando.ExecuteNonQuery();
                string anterior = ordem["status"].ToString();
                if (anterior != status) Historico(conexao, transacao, id, anterior, status);
                transacao.Commit();
            }
        }
        public static void Entregar(int id)
        {
            Sessao.Exigir("Atendente");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                DataRow ordem = Travar(conexao, transacao, id);
                if (ordem["status"].ToString() != "Concluída") throw new Exception("Apenas serviços concluídos podem ser entregues.");
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "UPDATE ordens_servico SET status='Entregue',data_entrega=NOW() WHERE id=@id", Banco.P("@id", id))) comando.ExecuteNonQuery();
                Historico(conexao, transacao, id, "Concluída", "Entregue");
                transacao.Commit();
            }
        }
        public static void UsarPeca(int id, int peca, int quantidade)
        {
            Sessao.Exigir("Técnico");
            if (quantidade <= 0) throw new Exception("Informe uma quantidade positiva.");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                DataRow ordem = Travar(conexao, transacao, id);
                ConferirTecnico(ordem, false);
                decimal preco;
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "SELECT p.valor_venda FROM pecas p JOIN aparelhos a ON a.id=@aparelho WHERE p.id=@peca AND p.ativo=1 AND p.tipo=a.tipo AND (p.marca='*' OR p.marca=a.marca) AND (p.modelo_compativel='*' OR p.modelo_compativel=a.modelo) FOR UPDATE",
                    Banco.P("@aparelho", ordem["aparelho_id"]), Banco.P("@peca", peca)))
                {
                    object valor = comando.ExecuteScalar();
                    if (valor == null) throw new Exception("A peça não está ativa ou não é compatível com o aparelho.");
                    preco = Convert.ToDecimal(valor);
                }
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "UPDATE pecas SET quantidade=quantidade-@qtd WHERE id=@peca AND quantidade>=@qtd", Banco.P("@peca", peca), Banco.P("@qtd", quantidade)))
                    if (comando.ExecuteNonQuery() != 1) throw new Exception("Estoque insuficiente. Atualize a lista de peças.");
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "INSERT INTO ordem_pecas(ordem_id,peca_id,quantidade,valor_unitario) VALUES(@ordem,@peca,@qtd,@valor)",
                    Banco.P("@ordem", id), Banco.P("@peca", peca), Banco.P("@qtd", quantidade), Banco.P("@valor", preco))) comando.ExecuteNonQuery();
                Movimento(conexao, transacao, peca, id, "Saída", quantidade, "Peça utilizada no conserto");
                transacao.Commit();
            }
        }
        public static void DevolverPeca(int ordemId, int itemId)
        {
            Sessao.Exigir("Técnico");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                DataRow ordem = Travar(conexao, transacao, ordemId);
                ConferirTecnico(ordem, false);
                int peca, quantidade; decimal subtotal;
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "SELECT peca_id,quantidade,valor_unitario FROM ordem_pecas WHERE id=@item AND ordem_id=@ordem FOR UPDATE", Banco.P("@item", itemId), Banco.P("@ordem", ordemId)))
                using (MySqlDataReader leitor = comando.ExecuteReader())
                {
                    if (!leitor.Read()) throw new Exception("Item não encontrado. Atualize a ordem.");
                    peca = leitor.GetInt32(0); quantidade = leitor.GetInt32(1); subtotal = quantidade * leitor.GetDecimal(2);
                }
                decimal totalPecas;
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "SELECT COALESCE(SUM(quantidade*valor_unitario),0) FROM ordem_pecas WHERE ordem_id=@id",Banco.P("@id",ordemId))) totalPecas=Convert.ToDecimal(comando.ExecuteScalar());
                if (Convert.ToDecimal(ordem["desconto"]) > Convert.ToDecimal(ordem["valor_mao_obra"]) + totalPecas - subtotal)
                    throw new Exception("Reduza e salve o desconto antes de devolver esta peça.");
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,"UPDATE pecas SET quantidade=quantidade+@qtd WHERE id=@peca", Banco.P("@qtd", quantidade), Banco.P("@peca", peca))) comando.ExecuteNonQuery();
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,"DELETE FROM ordem_pecas WHERE id=@id",Banco.P("@id",itemId))) comando.ExecuteNonQuery();
                Movimento(conexao, transacao, peca, ordemId, "Devolução", quantidade, "Peça removida da ordem e devolvida ao estoque");
                transacao.Commit();
            }
        }
        public static void Movimentar(int peca, string tipo, int quantidade, string observacao)
        {
            Sessao.Exigir("Técnico");
            if (tipo != "Entrada" && tipo != "Saída") throw new Exception("Movimentação inválida.");
            if (quantidade <= 0) throw new Exception("Informe uma quantidade positiva.");
            Tela.Obrigatorio(observacao, "motivo da movimentação");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                string sql = tipo == "Entrada" ? "UPDATE pecas SET quantidade=quantidade+@qtd WHERE id=@id AND ativo=1" : "UPDATE pecas SET quantidade=quantidade-@qtd WHERE id=@id AND ativo=1 AND quantidade>=@qtd";
                using (MySqlCommand comando = Banco.Comando(conexao, transacao, sql, Banco.P("@qtd", quantidade), Banco.P("@id", peca)))
                    if (comando.ExecuteNonQuery() != 1) throw new Exception("Peça inativa, inexistente ou estoque insuficiente.");
                Movimento(conexao, transacao, peca, null, tipo, quantidade, observacao.Trim());
                transacao.Commit();
            }
        }
        private static void Historico(MySqlConnection conexao, MySqlTransaction transacao, int id, string anterior, string novo)
        {
            using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                "INSERT INTO historico_status(ordem_id,usuario_id,status_anterior,status_novo) VALUES(@id,@usuario,@anterior,@novo)",
                Banco.P("@id", id), Banco.P("@usuario", Sessao.Id), Banco.P("@anterior", anterior), Banco.P("@novo", novo))) comando.ExecuteNonQuery();
        }
        private static void Movimento(MySqlConnection conexao, MySqlTransaction transacao, int peca, int? ordem, string tipo, int qtd, string obs)
        {
            using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                "INSERT INTO movimentacoes_estoque(peca_id,usuario_id,ordem_id,tipo,quantidade,observacao) VALUES(@peca,@usuario,@ordem,@tipo,@qtd,@obs)",
                Banco.P("@peca", peca), Banco.P("@usuario", Sessao.Id), Banco.P("@ordem", ordem), Banco.P("@tipo", tipo), Banco.P("@qtd", qtd), Banco.P("@obs", obs))) comando.ExecuteNonQuery();
        }
    }
}
