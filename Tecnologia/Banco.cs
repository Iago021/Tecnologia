using System;
using System.Configuration;
using System.Data;
using MySqlConnector;

namespace Tecnologia
{
    internal static class Banco
    {
        public static MySqlConnection Abrir()
        {
            ConnectionStringSettings configuracao = ConfigurationManager.ConnectionStrings["Tecnologia"];
            if (configuracao == null || string.IsNullOrWhiteSpace(configuracao.ConnectionString))
                throw new ConfigurationErrorsException("Configure a conexão Tecnologia no arquivo App.config.");

            MySqlConnection conexao = new MySqlConnection(configuracao.ConnectionString);
            try
            {
                conexao.Open();
                return conexao;
            }
            catch (MySqlException erro) when (erro.Number == 0 || erro.Number == 1042)
            {
                conexao.Dispose();
                var destino = new MySqlConnectionStringBuilder(configuracao.ConnectionString);
                throw new InvalidOperationException("Não foi possível conectar ao MySQL em " + destino.Server + ":" + destino.Port +
                    ". Inicie o MySQL no XAMPP e confira Server e Port no App.config.", erro);
            }
            catch
            {
                conexao.Dispose();
                throw;
            }
        }

        public static MySqlParameter P(string nome, object valor)
        {
            return new MySqlParameter(nome, valor ?? DBNull.Value);
        }

        public static MySqlCommand Comando(MySqlConnection conexao, MySqlTransaction transacao, string sql,
            params MySqlParameter[] parametros)
        {
            MySqlCommand comando = new MySqlCommand(sql, conexao, transacao);
            comando.Parameters.AddRange(parametros);
            return comando;
        }

        public static DataTable Consultar(string sql, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conexao = Abrir())
                return Consultar(conexao, null, sql, parametros);
        }

        public static DataTable Consultar(MySqlConnection conexao, MySqlTransaction transacao, string sql,
            params MySqlParameter[] parametros)
        {
            using (MySqlCommand comando = Comando(conexao, transacao, sql, parametros))
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
                return Executar(conexao, null, sql, parametros);
        }

        public static int Executar(MySqlConnection conexao, MySqlTransaction transacao, string sql,
            params MySqlParameter[] parametros)
        {
            using (MySqlCommand comando = Comando(conexao, transacao, sql, parametros))
                return comando.ExecuteNonQuery();
        }

        public static object Valor(string sql, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conexao = Abrir())
                return Valor(conexao, null, sql, parametros);
        }

        public static object Valor(MySqlConnection conexao, MySqlTransaction transacao, string sql,
            params MySqlParameter[] parametros)
        {
            using (MySqlCommand comando = Comando(conexao, transacao, sql, parametros))
                return comando.ExecuteScalar();
        }
    }
}
