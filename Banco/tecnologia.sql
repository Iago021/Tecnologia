-- Importar uma vez no phpMyAdmin ou no MySQL Workbench.
-- Pode ser importado novamente: não apaga tabelas nem registros.
SET NAMES utf8mb4;
CREATE DATABASE IF NOT EXISTS tecnologia CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE tecnologia;
CREATE TABLE IF NOT EXISTS usuarios (
 id INT AUTO_INCREMENT PRIMARY KEY,
 nome VARCHAR(100) NOT NULL,
 email VARCHAR(100) NOT NULL UNIQUE,
 telefone VARCHAR(20) NOT NULL DEFAULT '',
 cpf VARCHAR(14) NULL UNIQUE,
 rg VARCHAR(20) NOT NULL DEFAULT '',
 cidade VARCHAR(60) NOT NULL DEFAULT '',
 data_nascimento DATE NULL,
 senha_hash VARCHAR(200) NOT NULL,
 perfil ENUM('Atendente','Técnico') NOT NULL,
 ativo BOOLEAN NOT NULL DEFAULT TRUE,
 data_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS clientes (
 id INT AUTO_INCREMENT PRIMARY KEY,
 nome VARCHAR(100) NOT NULL,
 cpf VARCHAR(14) NULL UNIQUE,
 rg VARCHAR(20) NOT NULL DEFAULT '',
 telefone VARCHAR(20) NOT NULL,
 email VARCHAR(100) NOT NULL DEFAULT '',
 endereco VARCHAR(200) NOT NULL DEFAULT '',
 cidade VARCHAR(60) NOT NULL DEFAULT '',
 data_nascimento DATE NULL,
 ativo BOOLEAN NOT NULL DEFAULT TRUE,
 data_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS aparelhos (
 id INT AUTO_INCREMENT PRIMARY KEY,
 cliente_id INT NOT NULL,
 tipo ENUM('Celular','Notebook','PC') NOT NULL,
 marca VARCHAR(80) NOT NULL,
 modelo VARCHAR(100) NOT NULL,
 numero_serie VARCHAR(100) NOT NULL DEFAULT '',
 cor VARCHAR(50) NOT NULL DEFAULT '',
 acessorios VARCHAR(200) NOT NULL DEFAULT '',
 estado_fisico VARCHAR(300) NOT NULL DEFAULT '',
 observacoes VARCHAR(500) NOT NULL DEFAULT '',
 FOREIGN KEY (cliente_id) REFERENCES clientes(id)
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS pecas (
 id INT AUTO_INCREMENT PRIMARY KEY,
 codigo VARCHAR(40) NOT NULL UNIQUE,
 nome VARCHAR(100) NOT NULL,
 tipo ENUM('Celular','Notebook','PC') NOT NULL,
 marca VARCHAR(80) NOT NULL DEFAULT '*',
 modelo_compativel VARCHAR(100) NOT NULL DEFAULT '*',
 quantidade INT NOT NULL DEFAULT 0,
 estoque_minimo INT NOT NULL DEFAULT 0,
 valor_compra DECIMAL(10,2) NOT NULL DEFAULT 0,
 valor_venda DECIMAL(10,2) NOT NULL DEFAULT 0,
 ativo BOOLEAN NOT NULL DEFAULT TRUE,
 CHECK (quantidade >= 0), CHECK (estoque_minimo >= 0),
 CHECK (valor_compra >= 0), CHECK (valor_venda >= 0)
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS ordens_servico (
 id INT AUTO_INCREMENT PRIMARY KEY,
 aparelho_id INT NOT NULL,
 atendente_id INT NOT NULL,
 tecnico_id INT NULL,
 problema_relatado VARCHAR(2000) NOT NULL,
 status ENUM('Aberta','Em manutenção','Aguardando peça','Concluída','Entregue') NOT NULL DEFAULT 'Aberta',
 data_entrada DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
 previsao_entrega DATE NULL,
 data_conclusao DATETIME NULL,
 data_entrega DATETIME NULL,
 valor_mao_obra DECIMAL(10,2) NOT NULL DEFAULT 0,
 desconto DECIMAL(10,2) NOT NULL DEFAULT 0,
 observacoes VARCHAR(1000) NOT NULL DEFAULT '',
 FOREIGN KEY (aparelho_id) REFERENCES aparelhos(id),
 FOREIGN KEY (atendente_id) REFERENCES usuarios(id),
 FOREIGN KEY (tecnico_id) REFERENCES usuarios(id),
 INDEX (status), CHECK (valor_mao_obra >= 0), CHECK (desconto >= 0)
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS diagnosticos (
 id INT AUTO_INCREMENT PRIMARY KEY,
 ordem_id INT NOT NULL UNIQUE,
 tecnico_id INT NOT NULL,
 descricao VARCHAR(3000) NOT NULL,
 servico_necessario VARCHAR(2000) NOT NULL,
 data_diagnostico DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
 FOREIGN KEY (ordem_id) REFERENCES ordens_servico(id),
 FOREIGN KEY (tecnico_id) REFERENCES usuarios(id)
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS ordem_pecas (
 id INT AUTO_INCREMENT PRIMARY KEY,
 ordem_id INT NOT NULL,
 peca_id INT NOT NULL,
 quantidade INT NOT NULL,
 valor_unitario DECIMAL(10,2) NOT NULL,
 FOREIGN KEY (ordem_id) REFERENCES ordens_servico(id),
 FOREIGN KEY (peca_id) REFERENCES pecas(id),
 CHECK (quantidade > 0), CHECK (valor_unitario >= 0)
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS movimentacoes_estoque (
 id INT AUTO_INCREMENT PRIMARY KEY,
 peca_id INT NOT NULL,
 usuario_id INT NOT NULL,
 ordem_id INT NULL,
 tipo ENUM('Entrada','Saída','Devolução') NOT NULL,
 quantidade INT NOT NULL,
 observacao VARCHAR(300) NOT NULL,
 data_movimentacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
 FOREIGN KEY (peca_id) REFERENCES pecas(id),
 FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
 FOREIGN KEY (ordem_id) REFERENCES ordens_servico(id), CHECK (quantidade > 0)
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS historico_status (
 id INT AUTO_INCREMENT PRIMARY KEY,
 ordem_id INT NOT NULL,
 usuario_id INT NOT NULL,
 status_anterior VARCHAR(30) NOT NULL,
 status_novo VARCHAR(30) NOT NULL,
 data_alteracao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
 FOREIGN KEY (ordem_id) REFERENCES ordens_servico(id),
 FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
) ENGINE=InnoDB;
CREATE TABLE IF NOT EXISTS recuperacao_senha (
 usuario_id INT PRIMARY KEY,
 codigo_hash CHAR(64) NOT NULL,
 salt CHAR(32) NOT NULL,
 expira_em DATETIME NOT NULL,
 tentativas INT NOT NULL DEFAULT 0,
 enviado_em DATETIME NOT NULL,
 FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE
) ENGINE=InnoDB;
-- Sem contas ou clientes de exemplo. Crie o primeiro atendente em Criar conta.
