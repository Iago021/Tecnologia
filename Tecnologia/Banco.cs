using System;
using System.Configuration;
using System.Data;
using MySqlConnector;
namespace Tecnologia
{
    // Centraliza somente a conexão e as operações repetidas de banco.
    internal static class Banco
    {
        public static MySqlConnection Abrir()
        {
            MySqlConnection conexao = new MySqlConnection(ConfigurationManager.ConnectionStrings["Tecnologia"].ConnectionString);
            try { conexao.Open(); return conexao; }
            catch { conexao.Dispose(); throw; }
        }
        public static MySqlParameter P(string nome, object valor)
        { return new MySqlParameter(nome, valor ?? DBNull.Value); }
        public static MySqlCommand Comando(MySqlConnection conexao, MySqlTransaction transacao, string sql, params MySqlParameter[] parametros)
        {
            MySqlCommand comando = new MySqlCommand(sql, conexao, transacao);
            comando.Parameters.AddRange(parametros);
            return comando;
        }
        public static DataTable Consultar(string sql, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conexao = Abrir())
            using (MySqlCommand comando = Comando(conexao, null, sql, parametros))
            using (MySqlDataAdapter adaptador = new MySqlDataAdapter(comando))
            {
                DataTable tabela = new DataTable();
                adaptador.Fill(tabela);
                return tabela;
            }
        }
        public static int Executar(string sql, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conexao = Abrir())
            using (MySqlCommand comando = Comando(conexao, null, sql, parametros))
                return comando.ExecuteNonQuery();
        }
        public static object Valor(string sql, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conexao = Abrir())
            using (MySqlCommand comando = Comando(conexao, null, sql, parametros))
                return comando.ExecuteScalar();
        }
    }
}
