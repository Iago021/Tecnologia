using System;
using MySqlConnector;

namespace Tecnologia
{
    internal static class Estoque
    {
        public static void Movimentar(int peca, string tipo, int quantidade, string observacao)
        {
            if (tipo != "Entrada" && tipo != "Saída")
                throw new InvalidOperationException("Selecione Entrada ou Saída.");
            if (quantidade <= 0)
                throw new InvalidOperationException("Informe uma quantidade positiva.");
            Tela.Obrigatorio(observacao, "motivo da movimentação");

            using (MySqlConnection conexao = Banco.Abrir())
            using (MySqlTransaction transacao = conexao.BeginTransaction())
            {
                Sessao.Exigir(conexao, transacao, "Técnico");
                int ajuste = tipo == "Entrada" ? quantidade : -quantidade;
                if (!AjustarSaldo(conexao, transacao, peca, ajuste, true))
                    throw new InvalidOperationException("Peça inativa, inexistente ou estoque insuficiente.");

                RegistrarMovimento(conexao, transacao, peca, null, tipo, quantidade, observacao.Trim());
                transacao.Commit();
            }
        }

        public static bool AjustarSaldo(MySqlConnection conexao, MySqlTransaction transacao, int peca, int ajuste,
            bool exigirAtiva)
        {
            return Banco.Executar(conexao, transacao, @"UPDATE pecas SET quantidade=quantidade+@ajuste
                WHERE id=@id AND (ativo=1 OR NOT @exigirAtiva) AND quantidade+@ajuste>=0",
                Banco.P("@id", peca), Banco.P("@ajuste", ajuste), Banco.P("@exigirAtiva", exigirAtiva)) == 1;
        }

        public static void RegistrarMovimento(MySqlConnection conexao, MySqlTransaction transacao, int peca,
            int? ordem, string tipo, int quantidade, string observacao)
        {
            Banco.Executar(conexao, transacao, @"INSERT INTO movimentacoes_estoque
                (peca_id,usuario_id,ordem_id,tipo,quantidade,observacao)
                VALUES (@peca,@usuario,@ordem,@tipo,@quantidade,@observacao)",
                Banco.P("@peca", peca), Banco.P("@usuario", Sessao.Id), Banco.P("@ordem", ordem),
                Banco.P("@tipo", tipo), Banco.P("@quantidade", quantidade), Banco.P("@observacao", observacao));
        }
    }
}
