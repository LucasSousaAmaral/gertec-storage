-- 1) Cria o banco de dados
CREATE DATABASE IF NOT EXISTS GertecStorage
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

-- 2) Garante que estamos usando o DB recem-criado
USE GertecStorage;

-- 3) Tabela Products
DROP TABLE IF EXISTS Products;
CREATE TABLE Products (
    Id CHAR(36) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    PartNumber VARCHAR(100) NOT NULL,
    AverageCost DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (Id)
) ENGINE=InnoDB;

-- 4) Tabela InventoryMovements
DROP TABLE IF EXISTS InventoryMovements;
CREATE TABLE InventoryMovements (
    Id CHAR(36) NOT NULL,
    ProductId CHAR(36) NOT NULL,
    MovementDate DATETIME NOT NULL,
    Quantity INT NOT NULL,
    MovementType INT NOT NULL,           -- 0 = Entrada, 1 = Saída (por exemplo)
    UnitCost DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_InventoryMovements_Products
        FOREIGN KEY (ProductId)
        REFERENCES Products (Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
) ENGINE=InnoDB;

-- 5) Tabela Logs
DROP TABLE IF EXISTS Logs;
CREATE TABLE Logs (
    Id CHAR(36) NOT NULL,
    ErrorMessage TEXT NOT NULL,
    StackTrace TEXT NULL,
    CreatedDate DATETIME NOT NULL,
    PRIMARY KEY (Id)
) ENGINE=InnoDB;

-- 1) Cria o banco de dados
CREATE DATABASE IF NOT EXISTS GertecStorageTest
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

-- 2) Garante que estamos usando o DB recem-criado
USE GertecStorageTest;

-- 3) Tabela Products
DROP TABLE IF EXISTS Products;
CREATE TABLE Products (
    Id CHAR(36) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    PartNumber VARCHAR(100) NOT NULL,
    AverageCost DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (Id)
) ENGINE=InnoDB;

-- 4) Tabela InventoryMovements
DROP TABLE IF EXISTS InventoryMovements;
CREATE TABLE InventoryMovements (
    Id CHAR(36) NOT NULL,
    ProductId CHAR(36) NOT NULL,
    MovementDate DATETIME NOT NULL,
    Quantity INT NOT NULL,
    MovementType INT NOT NULL,           -- 0 = Entrada, 1 = Saída (por exemplo)
    UnitCost DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_InventoryMovements_Products
        FOREIGN KEY (ProductId)
        REFERENCES Products (Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
) ENGINE=InnoDB;

-- 5) Tabela Logs
DROP TABLE IF EXISTS Logs;
CREATE TABLE Logs (
    Id CHAR(36) NOT NULL,
    ErrorMessage TEXT NOT NULL,
    StackTrace TEXT NULL,
    CreatedDate DATETIME NOT NULL,
    PRIMARY KEY (Id)
) ENGINE=InnoDB;

