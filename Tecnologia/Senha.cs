using System;
using System.Security.Cryptography;
namespace Tecnologia
{
    internal static class Senha
    {
        // PBKDF2 com salt diferente para cada senha; não guarda a senha original.
        public static string Criar(string senha)
        {
            if (senha.Length < 8 || senha.Length > 128)
                throw new Exception("A senha deve ter de 8 a 128 caracteres.");
            byte[] salt = new byte[16];
            using (RandomNumberGenerator gerador = RandomNumberGenerator.Create()) gerador.GetBytes(salt);
            using (Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(senha, salt, 600000, HashAlgorithmName.SHA256))
                return "600000$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash.GetBytes(32));
        }
        public static bool Conferir(string senha, string armazenada)
        {
            try
            {
                string[] partes = armazenada.Split('$');
                if (partes.Length != 3) return false;
                int iteracoes = int.Parse(partes[0]);
                if (iteracoes < 100000 || iteracoes > 1000000) return false;
                byte[] esperado = Convert.FromBase64String(partes[2]);
                if (esperado.Length != 32) return false;
                using (Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(senha, Convert.FromBase64String(partes[1]), iteracoes, HashAlgorithmName.SHA256))
                {
                    byte[] recebido = hash.GetBytes(32);
                    int diferenca = 0;
                    for (int i = 0; i < recebido.Length; i++) diferenca |= recebido[i] ^ esperado[i];
                    return diferenca == 0;
                }
            }
            catch (FormatException) { return false; }
            catch (ArgumentException) { return false; }
            catch (OverflowException) { return false; }
        }
    }
}
