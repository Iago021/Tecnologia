using System;
using System.Data;
using MySqlConnector;

namespace Tecnologia
{
    internal static class Sessao
    {
        public static int Id;
        public static string Nome = "";
        public static string Perfil = "";
        public static bool TrocarUsuario;

        public static bool Atendente { get { return Perfil == "Atendente"; } }

        public static void Iniciar(DataRow conta)
        {
            Id = Convert.ToInt32(conta["id"]);
            Nome = conta["nome"].ToString();
            Perfil = conta["perfil"].ToString();
        }

        public static void Limpar()
        {
            Id = 0;
            Nome = "";
            Perfil = "";
            TrocarUsuario = false;
        }

        public static void Exigir(string perfil = "")
        {
            DataRow conta = ConsultarAtiva(Id, perfil);
            Nome = conta["nome"].ToString();
            Perfil = conta["perfil"].ToString();
        }

        public static DataRow ConsultarAtiva(int id, string perfil)
        {
            DataTable dados = Banco.Consultar(
                "SELECT id,nome,perfil FROM usuarios WHERE id=@id AND ativo=1", Banco.P("@id", id));
            return Conferir(dados, perfil);
        }

        public static void Exigir(MySqlConnection conexao, MySqlTransaction transacao, string perfil)
        {
            DataTable dados = Banco.Consultar(conexao, transacao,
                "SELECT nome,perfil FROM usuarios WHERE id=@id AND ativo=1 FOR UPDATE", Banco.P("@id", Id));
            DataRow conta = Conferir(dados, perfil);
            Nome = conta["nome"].ToString();
            Perfil = conta["perfil"].ToString();
        }

        private static DataRow Conferir(DataTable dados, string perfil)
        {
            if (dados.Rows.Count == 0)
                throw new InvalidOperationException("Sua conta está inativa. Entre novamente.");

            DataRow conta = dados.Rows[0];
            if (perfil != "" && conta["perfil"].ToString() != perfil)
                throw new InvalidOperationException("Esta função é permitida apenas ao perfil " + perfil + ".");
            return conta;
        }
    }
}
