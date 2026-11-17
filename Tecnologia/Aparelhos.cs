using System;
using MySqlConnector;

namespace Tecnologia
{
    internal static class Aparelhos
    {
        public static void Salvar(int id, int cliente, string tipo, string marca, string modelo, string serie,
            string cor, string acessorios, string estadoFisico, string observacoes)
        {
            Tela.Obrigatorio(marca, "Marca");
            Tela.Obrigatorio(modelo, "Modelo");

            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Atendente");
                if (id != 0)
                {
                    Travar(conexao, transacao, id);
                    object ordemExistente = Banco.Valor(conexao, transacao,
                        "SELECT id FROM ordens_servico WHERE aparelho_id=@id LIMIT 1", Banco.P("@id", id));
                    if (ordemExistente != null)
                        throw new InvalidOperationException("Este aparelho já possui uma ordem de serviço. Seus dados fazem parte do histórico.");
                }

                ConferirClienteAtivo(conexao, transacao, cliente);

                string sql = id == 0
                    ? @"INSERT INTO aparelhos (cliente_id,tipo,marca,modelo,numero_serie,cor,acessorios,estado_fisico,observacoes)
                        VALUES (@cliente,@tipo,@marca,@modelo,@serie,@cor,@acessorios,@estado,@observacoes)"
                    : @"UPDATE aparelhos SET cliente_id=@cliente,tipo=@tipo,marca=@marca,modelo=@modelo,numero_serie=@serie,
                        cor=@cor,acessorios=@acessorios,estado_fisico=@estado,observacoes=@observacoes WHERE id=@id";
                Banco.Executar(conexao, transacao, sql, Banco.P("@id", id), Banco.P("@cliente", cliente),
                    Banco.P("@tipo", tipo), Banco.P("@marca", marca.Trim()), Banco.P("@modelo", modelo.Trim()),
                    Banco.P("@serie", serie.Trim()), Banco.P("@cor", cor.Trim()), Banco.P("@acessorios", acessorios.Trim()),
                    Banco.P("@estado", estadoFisico.Trim()), Banco.P("@observacoes", observacoes.Trim()));
                transacao.Commit();
            }
        }

        public static int Travar(MySqlConnection conexao, MySqlTransaction transacao, int id)
        {
            object cliente = Banco.Valor(conexao, transacao,
                "SELECT cliente_id FROM aparelhos WHERE id=@id FOR UPDATE", Banco.P("@id", id));
            if (cliente == null)
                throw new InvalidOperationException("Aparelho não encontrado. Atualize a lista.");

            return Convert.ToInt32(cliente);
        }

        public static void ConferirClienteAtivo(MySqlConnection conexao, MySqlTransaction transacao, int cliente)
        {
            object ativo = Banco.Valor(conexao, transacao,
                "SELECT ativo FROM clientes WHERE id=@id FOR UPDATE", Banco.P("@id", cliente));
            if (ativo == null || !Convert.ToBoolean(ativo))
                throw new InvalidOperationException("Selecione um cliente ativo.");
        }
    }
}
