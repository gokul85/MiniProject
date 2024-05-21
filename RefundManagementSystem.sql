-- Create the database
CREATE DATABASE ReturnManagementSystem;
GO

-- Switch to the newly created database
USE ReturnManagementSystem;
GO

-- Create the tables
CREATE TABLE Users (
    Id INT PRIMARY KEY,
    Name VARCHAR(255),
    Email VARCHAR(255),
    Phone VARCHAR(20),
    Address VARCHAR(255),
    Role VARCHAR(50)
);

CREATE TABLE UserDetails (
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    Password VARBINARY(MAX),
    PasswordHashKey VARBINARY(MAX),
    Status VARCHAR(10) -- Active or Disabled
);

CREATE TABLE Products (
    ProductId INT PRIMARY KEY,
    Name VARCHAR(255),
    Description VARCHAR(MAX),
    Price DECIMAL(18, 2),
    Stock INT,
    ProductStatus VARCHAR(20) -- Fresh or Refurbished
);

CREATE TABLE ProductItems (
    SerialNumber VARCHAR(50) PRIMARY KEY,
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    Status VARCHAR(20) -- Available, Refurbished, Ordered
);

CREATE TABLE Policies (
    PolicyId INT PRIMARY KEY,
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    Policy VARCHAR(50), -- Warranty, Return, Replacement
    Duration INT -- Days for policy
);

CREATE TABLE Orders (
    OrderId INT PRIMARY KEY,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATETIME,
    TotalAmount DECIMAL(18, 2),
    OrderStatus VARCHAR(20) -- Pending, Delivered
);

CREATE TABLE OrderProducts (
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    Price DECIMAL(18, 2),
    SerialNumber VARCHAR(50) FOREIGN KEY REFERENCES ProductItems(SerialNumber),
    PRIMARY KEY (OrderId, ProductId, SerialNumber)
);

CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY,
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    PaymentDate DATETIME,
    TransactionId VARCHAR(100), -- Transaction ID from the payment gateway
    Amount DECIMAL(18, 2)
);

CREATE TABLE ReturnRequests (
    RequestId INT PRIMARY KEY,
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

CREATE TABLE RefundTransactions (
    RefundTransactionId INT PRIMARY KEY,
    RequestId INT FOREIGN KEY REFERENCES ReturnRequests(RequestId),
    TransactionDate DATETIME,
    TransactionAmount DECIMAL(18, 2),
    TransactionId VARCHAR(100) -- Transaction ID from the payment gateway
);


