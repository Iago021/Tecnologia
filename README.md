# 🔧 TechFix

### Sistema de gerenciamento para assistências técnicas

O **TechFix** é um sistema desenvolvido para organizar o atendimento, a manutenção, o controle de peças e o acompanhamento de equipamentos em uma assistência técnica.

O projeto conta com uma interface simples e objetiva, com áreas para **Dashboard, Manutenção, Peças e Conta/Perfil**, além de telas de autenticação, criação de conta e recuperação de senha.

---

## 📖 Sobre o Projeto

O TechFix foi desenvolvido com o objetivo de centralizar as principais informações de uma assistência técnica em um único sistema.

A aplicação permite organizar o fluxo de manutenção de equipamentos, acompanhar máquinas cadastradas, consultar peças e gerenciar os dados da conta do usuário.

O sistema é voltado principalmente para manutenção de:

- 📱 Celulares
- 💻 Notebooks
- 🖥️ Computadores (PCs)

---

## 🖥️ Interface do Sistema

O protótipo possui uma identidade visual voltada para tecnologia, utilizando principalmente:

- 🟢 Verde como cor de destaque;
- ⚪ Fundo claro;
- ⚫ Textos em tons escuros e cinza;
- 🔘 Botões arredondados;
- 💻 Interface simples e centralizada.

A navegação principal é composta por:

- **Dashboard**
- **Manutenção**
- **Peças**
- **Conta / Perfil**

---

## 🔐 Login e Autenticação

O sistema possui uma tela de login para acesso à plataforma.

O usuário pode:

- Entrar utilizando e-mail e senha;
- Criar uma nova conta;
- Solicitar recuperação de senha caso esqueça seus dados de acesso.

---

## 👤 Criação de Conta

Novos usuários podem realizar o cadastro diretamente pelo sistema.

A tela de criação de conta permite registrar as informações necessárias para acessar a plataforma.

Após o cadastro, o usuário poderá utilizar sua conta para acessar as funcionalidades do TechFix.

---

## 🔑 Recuperação e Redefinição de Senha

Caso o usuário esqueça sua senha, o sistema possui um fluxo de recuperação.

### Fluxo de recuperação

```text
Usuário informa o e-mail
          ↓
Sistema envia um código de verificação
          ↓
Usuário informa o código recebido
          ↓
Usuário cria uma nova senha
          ↓
Senha é redefinida
```

Na etapa final, o usuário deve informar:

- Código de verificação;
- Nova senha;
- Confirmação da nova senha.

---

## 📊 Dashboard

O Dashboard funciona como a página inicial do sistema após o login.

Ele apresenta uma tela de boas-vindas e permite acessar rapidamente as principais áreas da aplicação através do menu superior.

Principais acessos:

- Dashboard;
- Manutenção;
- Peças;
- Conta / Perfil.

---

## 🔧 Manutenção

A área de **Manutenção** é responsável pelo gerenciamento dos equipamentos cadastrados para reparo.

O usuário pode visualizar os equipamentos que estão registrados no sistema e consultar suas informações.

Cada equipamento pode apresentar informações como:

- Número de identificação;
- Tipo/modelo do equipamento;
- Marca;
- Problema relatado;
- Informações adicionais sobre o aparelho;
- Situação da manutenção.

Também é possível selecionar um equipamento para visualizar mais detalhes.

---

## ➕ Cadastro de Manutenção

O sistema permite cadastrar novos equipamentos para manutenção.

O cadastro pode conter informações relacionadas ao equipamento e ao problema apresentado pelo cliente.

Depois de cadastrado, o equipamento passa a aparecer na lista de **Manutenções Cadastradas**.

---

## 🧰 Peças

A área de **Peças** permite consultar componentes que podem ser utilizados durante os reparos.

O sistema pode apresentar informações como:

- Imagem da peça;
- Nome e modelo;
- Compatibilidade;
- Preço;
- Avaliação;
- Informações de compra;
- Disponibilidade ou frete.

Essa funcionalidade facilita a busca por peças necessárias durante uma manutenção.

---

## 👤 Conta / Perfil

A área de **Conta / Perfil** é destinada ao gerenciamento das informações do usuário.

Nessa área, o usuário poderá consultar e alterar informações relacionadas à sua conta.

Entre as funcionalidades previstas estão:

- Visualização dos dados da conta;
- Alteração de informações pessoais;
- Alteração de senha;
- Gerenciamento do perfil.

---

## 📱 Equipamentos Suportados

| Equipamento | Suportado |
| --- | :---: |
| 📱 Celulares | ✅ |
| 💻 Notebooks | ✅ |
| 🖥️ Computadores (PCs) | ✅ |

---

## 🔄 Fluxo Principal do Sistema

```text
Criação da conta
       ↓
Login
       ↓
Dashboard
       ↓
Cadastro do equipamento
       ↓
Manutenção
       ↓
Consulta dos equipamentos cadastrados
       ↓
Busca de peças
       ↓
Atualização da manutenção
       ↓
Finalização do serviço
```

---

## ✨ Principais Funcionalidades

- 🔐 Login de usuário;
- 👤 Criação de conta;
- 🔑 Recuperação de senha;
- 🔢 Verificação por código;
- 🔄 Redefinição de senha;
- 📊 Dashboard;
- 💻 Cadastro de equipamentos;
- 🔧 Gerenciamento de manutenções;
- 🔎 Consulta de equipamentos cadastrados;
- 🧰 Pesquisa de peças;
- 👤 Gerenciamento de conta e perfil.

---

## ✅ Requisitos Funcionais

| Código | Requisito |
| --- | --- |
| **RF01** | O sistema deve permitir que o usuário entre utilizando e-mail e senha. |
| **RF02** | O sistema deve permitir a criação de uma nova conta. |
| **RF03** | O sistema deve permitir a recuperação de senha através do e-mail cadastrado. |
| **RF04** | O sistema deve enviar um código de verificação para recuperação da conta. |
| **RF05** | O usuário deve poder cadastrar uma nova senha após a verificação. |
| **RF06** | O sistema deve possuir um Dashboard como página principal. |
| **RF07** | O usuário deve poder cadastrar equipamentos para manutenção. |
| **RF08** | O sistema deve listar as manutenções cadastradas. |
| **RF09** | O usuário deve poder consultar informações de um equipamento cadastrado. |
| **RF10** | O sistema deve permitir consultar peças para manutenção. |
| **RF11** | O sistema deve apresentar informações das peças encontradas. |
| **RF12** | O usuário deve poder acessar e gerenciar sua conta/perfil. |
| **RF13** | O usuário deve poder alterar sua senha. |
| **RF14** | O sistema deve permitir a atualização das informações relacionadas à manutenção. |

---

## ⚙️ Requisitos Não Funcionais

| Código | Requisito |
| --- | --- |
| **RNF01** | O sistema deve ser desenvolvido em C#. |
| **RNF02** | O sistema deve utilizar um banco de dados SQL. |
| **RNF03** | As senhas devem ser armazenadas de forma segura e não como texto simples. |
| **RNF04** | O sistema deve validar os dados antes de armazená-los no banco de dados. |
| **RNF05** | A interface deve ser simples, organizada e fácil de utilizar. |
| **RNF06** | O sistema deve possuir navegação clara entre Dashboard, Manutenção, Peças e Perfil. |
| **RNF07** | O sistema deve possuir mecanismos seguros para recuperação e redefinição de senha. |

---

## 🗃️ Diagrama do Banco de Dados

Nesta seção pode ser inserido o diagrama entidade-relacionamento utilizado no desenvolvimento do TechFix.

```text
[ Inserir imagem do diagrama do banco de dados aqui ]
```

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Utilização |
| --- | --- |
| **C#** | Desenvolvimento do sistema |
| **Banco de dados SQL** | Armazenamento das informações |
| **Figma** | Desenvolvimento e prototipação da interface |

---

## 🎨 Protótipo

A interface do TechFix foi planejada inicialmente no **Figma**, definindo a estrutura visual e a navegação entre as principais telas antes da implementação.

### Telas desenvolvidas no protótipo

1. Tela de Login;
2. Tela de Criação de Conta;
3. Recuperação de Senha;
4. Redefinição de Senha;
5. Dashboard / Tela inicial;
6. Manutenção;
7. Manutenções cadastradas;
8. Peças;
9. Conta / Perfil.

---

## 📸 Telas do Projeto

Para exibir as capturas de tela no GitHub, coloque as imagens dentro de uma pasta chamada `docs/images` no projeto e substitua os nomes abaixo pelos arquivos correspondentes.

```md
![Dashboard](docs/images/dashboard.jpg)

![Manutenções cadastradas](docs/images/manutencoes-cadastradas.jpg)

![Manutenção](docs/images/manutencao.jpg)

![Peças](docs/images/pecas.jpg)

![Login](docs/images/login.jpg)

![Recuperação de senha](docs/images/recuperacao-senha.jpg)

![Redefinição de senha](docs/images/redefinicao-senha.jpg)

![Criar conta](docs/images/criar-conta.jpg)
```

---

## 🚀 Objetivo

O objetivo do TechFix é facilitar o gerenciamento de uma assistência técnica, reunindo informações sobre equipamentos, manutenções, peças e usuários em uma única aplicação.

Com isso, busca-se tornar o processo de manutenção mais organizado e facilitar o acompanhamento dos equipamentos cadastrados.

---

## 📌 Status do Projeto

🚧 **Em desenvolvimento**

O projeto está sendo desenvolvido com base no protótipo criado no Figma.

---

Desenvolvido como projeto de gerenciamento para assistências técnicas.
