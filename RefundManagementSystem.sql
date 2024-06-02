-- Drop the database if it exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ReturnManagementSystem')
BEGIN
    DROP DATABASE ReturnManagementSystem;
END
GO

-- Create the database
CREATE DATABASE ReturnManagementSystem;
GO

-- Switch to the newly created database
USE ReturnManagementSystem;
GO

-- Create the tables with IDENTITY property for primary keys
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255),
    Email VARCHAR(255),
    Phone VARCHAR(20),
    Address VARCHAR(255),
    Role VARCHAR(50)
);

CREATE TABLE UserDetails (
    UserId INT PRIMARY KEY FOREIGN KEY REFERENCES Users(Id),
    Username VARCHAR(MAX),
    Password VARBINARY(MAX),
    PasswordHashKey VARBINARY(MAX),
    Status VARCHAR(10) -- Active or Disabled
);

CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255),
    Description VARCHAR(MAX),
    Price DECIMAL(18, 2),
    Stock INT,
    ProductStatus VARCHAR(20) -- Fresh or Refurbished
);

CREATE TABLE ProductItems (
    ProductItemId INT IDENTITY(1,1) PRIMARY KEY,
    SerialNumber VARCHAR(50) UNIQUE,
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    Status VARCHAR(20) -- Available, Refurbished, Ordered
);

CREATE TABLE Policies (
    PolicyId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    PolicyType VARCHAR(50), -- Warranty, Return, Replacement
    Duration INT -- Days for policy
);

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATETIME,
    TotalAmount DECIMAL(18, 2),
    OrderStatus VARCHAR(20) -- Pending, Delivered
);

CREATE TABLE OrderProducts (
    OrderProductId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    Price DECIMAL(18, 2),
    SerialNumber VARCHAR(50) FOREIGN KEY REFERENCES ProductItems(SerialNumber)
);
CREATE TABLE ReturnRequests (
    RequestId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    SerialNumber VARCHAR(50) FOREIGN KEY REFERENCES ProductItems(SerialNumber),
    RequestDate DATETIME,
    ReturnPolicy VARCHAR(50), -- Return, Replacement, Warranty Claim
    Process VARCHAR(50), -- Refunded, Replaced, Repaired
    Feedback VARCHAR(MAX),
    Reason VARCHAR(MAX), -- Detailed reason for the return
    Status VARCHAR(20), -- Pending, Processing, Closed
    ClosedBy INT FOREIGN KEY REFERENCES Users(Id),
    ClosedDate DATETIME
);

CREATE TABLE Transactions (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NULL FOREIGN KEY REFERENCES Orders(OrderId),
    RequestId INT NULL FOREIGN KEY REFERENCES ReturnRequests(RequestId),
    TransactionDate DATETIME,
    TransactionAmount DECIMAL(18, 2),
    PaymentGatewayTransactionId VARCHAR(100), -- Transaction ID from the payment gateway
    TransactionType VARCHAR(50)
);