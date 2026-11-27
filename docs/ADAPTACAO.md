# O que foi adaptado de escola2.0.zip

O arquivo contém um projeto de escola em C# Windows Forms. A tabela mostra como os recursos entraram no sistema de assistência técnica Tecnologia.

| No projeto Escola | No Tecnologia |
|---|---|
| Menu principal (`Form1`) | Painel principal com navegação entre cadastros, manutenção e estoque |
| Cadastro, alteração, pesquisa e exclusão de alunos | Cadastro de clientes, com edição pela linha selecionada e confirmação antes de excluir |
| Cadastro, alteração, pesquisa e exclusão de professores e funcionários | Cadastro único de equipe, com perfis Técnico e Atendente |
| Dados pessoais (nome, e-mail, telefone, RG, CPF, cidade e nascimento) | Campos equivalentes no cadastro de clientes e equipe, conforme o tipo de cadastro |
| RA e demais códigos dos cadastros | Código interno gerado pelo banco, usado nas tabelas e nas pesquisas |
| Cadastro, pesquisa e alteração de turmas | Abertura, pesquisa e edição do recebimento de ordens de serviço |
| Designar ou trocar professor da turma | Designar, trocar ou retirar o técnico responsável por uma ordem aberta, com confirmação ao substituir ou retirar um técnico |
| Associação dos alunos à turma | Uma ordem liga o aparelho ao cliente dono do equipamento. Cada ordem acompanha um aparelho |
| Exclusão de uma turma | Cancelamento da ordem aberta com registro no histórico |
| Exportação CSV e PDF das tabelas | Menu **Exportar...** nos cadastros, nas ordens e nos históricos |
| Acesso ao MySQL e validações de cadastro | Conexão centralizada no `App.config`, consultas com parâmetros e validação nos formulários |

## O que ficou de fora

- As telas e tabelas escolares de alunos, professores, funcionários, turmas e matrículas. O banco usa a estrutura da assistência técnica.
- O RA como documento escolar, o ano e o período da turma e a seleção de vários alunos em uma turma. A ordem usa o aparelho, o cliente, a previsão de entrega e o andamento do serviço.
- Os dados de exemplo, as configurações do banco escolar, a solução `escolaa` e a aparência dos formulários do ZIP.
- A exclusão definitiva de ordens. O cancelamento permite consultar o histórico e abrir outro atendimento para o aparelho.

Login, recuperação de senha, aparelhos, diagnóstico, peças, estoque e entrega fazem parte do Tecnologia. A exportação para o Excel já existia no Tecnologia. O script para o banco é `Banco/tecnologia.sql`.

Para editar visualmente uma tela, abra `Tecnologia.sln` no Visual Studio, expanda `Tecnologia > Forms` e use **Exibir Designer** no arquivo do formulário (**Shift+F7**). Propriedades, botões, abas e os recursos de cada tela ficam no respectivo `.Designer.cs` e `.resx`. Os arquivos `.cs` e `.Eventos.cs` tratam a lógica e os ajustes de layout feitos durante a execução.
