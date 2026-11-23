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
            Tela.Obrigatorio(problema, "problema relatado");
            if (previsao.HasValue && previsao.Value.Date < DateTime.Today) throw new Exception("A previsão não pode ser anterior a hoje.");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
                int cliente = Aparelhos.Travar(conexao, transacao, aparelho);
                Aparelhos.ConferirClienteAtivo(conexao, transacao, cliente);
                object pendente = Banco.Valor(conexao, transacao,
                    "SELECT id FROM ordens_servico WHERE aparelho_id=@id AND status NOT IN ('Entregue','Cancelada') LIMIT 1", Banco.P("@id", aparelho));
                if (pendente != null)
                    throw new InvalidOperationException("Este aparelho já possui uma ordem em andamento ou aguardando entrega.");

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
        public static void EditarRecebimento(int id, string problema, DateTime? previsao, string observacoes, DataRow original)
        {
            Tela.Obrigatorio(problema, "problema relatado");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
                DataRow ordem = Travar(conexao, transacao, id);
                if (ordem["status"].ToString() != "Aberta")
                    throw new InvalidOperationException("Apenas ordens abertas permitem editar o recebimento.");

                foreach (string campo in new[] { "problema_relatado", "previsao_entrega", "observacoes" })
                    if (!Equals(ordem[campo], original[campo]))
                        throw new InvalidOperationException("O recebimento foi alterado. Feche e abra a edição novamente.");

                DateTime? dataAnterior = ordem.Field<DateTime?>("previsao_entrega");
                if (previsao.HasValue && previsao.Value.Date < DateTime.Today && previsao.Value.Date != dataAnterior)
                    throw new InvalidOperationException("A previsão não pode ser anterior a hoje.");

                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "UPDATE ordens_servico SET problema_relatado=@problema,previsao_entrega=@previsao,observacoes=@obs WHERE id=@id",
                    Banco.P("@problema", problema.Trim()), Banco.P("@previsao", previsao),
                    Banco.P("@obs", observacoes.Trim()), Banco.P("@id", id)))
                    comando.ExecuteNonQuery();
                transacao.Commit();
            }
        }

        public static void Cancelar(int id)
        {
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
                DataRow ordem = Travar(conexao, transacao, id);
                if (ordem["status"].ToString() != "Aberta")
                    throw new InvalidOperationException("Apenas ordens abertas podem ser canceladas.");

                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "UPDATE ordens_servico SET status='Cancelada' WHERE id=@id", Banco.P("@id", id)))
                    comando.ExecuteNonQuery();
                Historico(conexao, transacao, id, "Aberta", "Cancelada");
                transacao.Commit();
            }
        }

        public static void AtribuirTecnico(int id, int tecnico, int? responsavelEsperado = null)
        {
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
                if (tecnico != 0) ConferirTecnicoAtivo(conexao, transacao, tecnico);
                DataRow ordem = Travar(conexao, transacao, id);
                if (ordem["status"].ToString() != "Aberta")
                    throw new Exception("O responsável só pode ser alterado antes do diagnóstico.");

                int responsavel = ordem.Field<int?>("tecnico_id") ?? 0;
                if (responsavelEsperado.HasValue && responsavelEsperado.Value != responsavel)
                    throw new InvalidOperationException("O responsável mudou. Atualize a ordem antes de tentar novamente.");

                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "UPDATE ordens_servico SET tecnico_id=@tecnico WHERE id=@id",
                    Banco.P("@tecnico", tecnico == 0 ? (int?)null : tecnico), Banco.P("@id", id)))
                    comando.ExecuteNonQuery();
                transacao.Commit();
            }
        }

        private static void ConferirTecnicoAtivo(MySqlConnection conexao, MySqlTransaction transacao, int tecnico)
        {
            using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                "SELECT id FROM usuarios WHERE id=@id AND perfil='Técnico' AND ativo=1 FOR UPDATE", Banco.P("@id", tecnico)))
            {
                if (comando.ExecuteScalar() == null)
                    throw new Exception("Selecione um técnico ativo.");
            }
        }

        private static DataRow Travar(MySqlConnection conexao, MySqlTransaction transacao, int ordem)
        {
            DataTable tabela = Banco.Consultar(conexao, transacao,
                "SELECT status,tecnico_id,aparelho_id,desconto,valor_mao_obra,problema_relatado,previsao_entrega,observacoes FROM ordens_servico WHERE id=@id FOR UPDATE",
                Banco.P("@id", ordem));
            if (tabela.Rows.Count == 0)
                throw new InvalidOperationException("Ordem não encontrada.");

            return tabela.Rows[0];
        }

        private static void ConferirTecnico(DataRow ordem, bool podeIniciar)
        {
            string status = ordem["status"].ToString();
            if (status == "Cancelada") throw new InvalidOperationException("Esta ordem foi cancelada.");
            if (status == "Concluída" || status == "Entregue") throw new Exception("Esta ordem já foi concluída e não pode mais ser alterada.");
            if (ordem["tecnico_id"] != DBNull.Value && Convert.ToInt32(ordem["tecnico_id"]) != Sessao.Id)
                throw new Exception("Esta ordem está sob responsabilidade de outro técnico.");
            if (!podeIniciar && (status == "Aberta" || ordem["tecnico_id"] == DBNull.Value))
                throw new Exception("Salve o diagnóstico primeiro para iniciar o serviço.");
        }
        public static void Diagnosticar(int id, string descricao, string servico, string status, decimal maoObra, decimal desconto)
        {
            Tela.Obrigatorio(descricao, "diagnóstico"); Tela.Obrigatorio(servico, "serviço necessário");
            if (status != "Em manutenção" && status != "Aguardando peça" && status != "Concluída") throw new Exception("Selecione um andamento válido.");
            if (maoObra < 0 || desconto < 0) throw new Exception("Os valores não podem ser negativos.");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Técnico");
                DataRow ordem = Travar(conexao, transacao, id);
                ConferirTecnico(ordem, true);
                decimal pecas = TotalPecas(conexao, transacao, id);
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
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
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
            if (quantidade <= 0) throw new Exception("Informe uma quantidade positiva.");
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Técnico");
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
                if (!Estoque.AjustarSaldo(conexao, transacao, peca, -quantidade, true))
                    throw new InvalidOperationException("Estoque insuficiente. Atualize a lista de peças.");
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                    "INSERT INTO ordem_pecas(ordem_id,peca_id,quantidade,valor_unitario) VALUES(@ordem,@peca,@qtd,@valor)",
                    Banco.P("@ordem", id), Banco.P("@peca", peca), Banco.P("@qtd", quantidade), Banco.P("@valor", preco))) comando.ExecuteNonQuery();
                Estoque.RegistrarMovimento(conexao, transacao, peca, id, "Saída", quantidade, "Peça utilizada no conserto");
                transacao.Commit();
            }
        }
        public static void DevolverPeca(int ordemId, int itemId)
        {
            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Técnico");
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
                decimal totalPecas = TotalPecas(conexao, transacao, ordemId);
                if (Convert.ToDecimal(ordem["desconto"]) > Convert.ToDecimal(ordem["valor_mao_obra"]) + totalPecas - subtotal)
                    throw new Exception("Reduza e salve o desconto antes de devolver esta peça.");
                if (!Estoque.AjustarSaldo(conexao, transacao, peca, quantidade, false))
                    throw new InvalidOperationException("Peça não encontrada. Atualize a ordem.");
                using (MySqlCommand comando = Banco.Comando(conexao, transacao,"DELETE FROM ordem_pecas WHERE id=@id",Banco.P("@id",itemId))) comando.ExecuteNonQuery();
                Estoque.RegistrarMovimento(conexao, transacao, peca, ordemId, "Devolução", quantidade, "Peça removida da ordem e devolvida ao estoque");
                transacao.Commit();
            }
        }
        private static decimal TotalPecas(MySqlConnection conexao, MySqlTransaction transacao, int ordem)
        {
            return Convert.ToDecimal(Banco.Valor(conexao, transacao,
                "SELECT COALESCE(SUM(quantidade*valor_unitario),0) FROM ordem_pecas WHERE ordem_id=@id",
                Banco.P("@id", ordem)));
        }

        private static void Historico(MySqlConnection conexao, MySqlTransaction transacao, int id, string anterior, string novo)
        {
            using (MySqlCommand comando = Banco.Comando(conexao, transacao,
                "INSERT INTO historico_status(ordem_id,usuario_id,status_anterior,status_novo) VALUES(@id,@usuario,@anterior,@novo)",
                Banco.P("@id", id), Banco.P("@usuario", Sessao.Id), Banco.P("@anterior", anterior), Banco.P("@novo", novo))) comando.ExecuteNonQuery();
        }
    }
}
