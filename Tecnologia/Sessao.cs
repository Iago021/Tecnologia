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
            DataTable dados = Banco.Consultar(
                "SELECT nome,perfil FROM usuarios WHERE id=@id AND ativo=1", Banco.P("@id", Id));
            Conferir(dados, perfil);
        }

        public static void Exigir(MySqlConnection conexao, MySqlTransaction transacao, string perfil)
        {
            DataTable dados = Banco.Consultar(conexao, transacao,
                "SELECT nome,perfil FROM usuarios WHERE id=@id AND ativo=1 FOR UPDATE", Banco.P("@id", Id));
            Conferir(dados, perfil);
        }

        private static void Conferir(DataTable dados, string perfil)
        {
            if (dados.Rows.Count == 0)
                throw new InvalidOperationException("Sua conta está inativa. Entre novamente.");

            Nome = dados.Rows[0]["nome"].ToString();
            Perfil = dados.Rows[0]["perfil"].ToString();
            if (perfil != "" && Perfil != perfil)
                throw new InvalidOperationException("Esta função é permitida apenas ao perfil " + perfil + ".");
        }
    }
}
