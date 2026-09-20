# Verificação da implementação

## O que foi executado

- Compilação de todos os arquivos C# com referências do .NET Framework 4.7.2.
- Compilação da solução completa em Release com MSBuild, incluindo formulários, recursos `.resx` e bibliotecas.
- Importação do SQL em um banco MariaDB 10.11.14 vazio e reimportação do mesmo script sem apagar registros.
- 33 verificações de integração usando o código do sistema e um banco de teste isolado.
- Cadastro, pesquisa, alteração e exclusão de cliente pelos eventos dos botões do formulário.
- Abertura das 12 classes de formulário, incluindo a ordem em dois estados (aberta e entregue) e o histórico em dois modos (ordens e estoque), totalizando 14 verificações de abertura.

As verificações foram executadas em Linux, com o compilador C#, referências oficiais do .NET Framework e Mono para executar os testes. A aplicação é destinada ao Windows. Nos testes dos eventos, as caixas de confirmação foram respondidas automaticamente; os comandos SQL e as operações de negócio usados são os mesmos do projeto.

## Integração com o banco

| Grupo | Casos verificados |
|---|---|
| Senhas | Senha correta, incorreta, salt diferente e hash inválido |
| Texto e SQL | Cadastro com apóstrofo e acentos |
| Perfis | Técnico impedido de abrir ordem, outro técnico impedido de alterar um reparo e conta desativada sem acesso |
| Abertura | Criação da ordem e histórico inicial; bloqueio de atendimento duplicado |
| Diagnóstico | Definição do técnico responsável antes de adicionar peças |
| Compatibilidade | Rejeição de peça incompatível |
| Estoque | Entrada, uso, devolução, rejeição de quantidade zero e de saída maior que o saldo |
| Consistência | Falha na operação preserva estoque, itens e movimentações |
| Valores | Preço histórico, desconto máximo e total final |
| Conclusão e entrega | Bloqueio de peças após concluir, entrega apenas de serviço concluído, data registrada e rejeição de entrega repetida |
| Histórico | Registro das mudanças de status e novo atendimento após entrega |
| Relacionamentos | Exclusão de cliente vinculado recusada pelo banco |

## Conferência no Windows

Ainda deve ser conferida na instalação do usuário a execução pelo próprio Visual Studio, o Designer no Windows e a automação do Microsoft Excel. Não houve acesso a um Windows com Excel durante esta verificação.

Roteiro curto para essa conferência:

1. Importar o SQL, restaurar o NuGet e executar a solução.
2. Criar o primeiro atendente e cadastrar um técnico.
3. Cadastrar cliente, aparelho e uma ordem de serviço.
4. Entrar como técnico, cadastrar uma peça compatível e registrar uma entrada.
5. Salvar o diagnóstico, adicionar a peça e conferir a redução do estoque.
6. Devolver uma peça e conferir a reposição; depois adicionar novamente, se for usada.
7. Informar mão de obra e desconto e concluir o serviço.
8. Entrar como atendente e registrar a entrega.
9. Exportar uma lista para o Excel e conferir cabeçalho, acentos e a última linha.
10. Fechar e abrir o sistema e conferir que os dados permanecem salvos.

O banco de teste e seus dados não fazem parte do SQL entregue.
