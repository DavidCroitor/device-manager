IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        Name NVARCHAR(100) NOT NULL,
        Role NVARCHAR(50) NOT NULL,
        Location NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL
    );
END
GO


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Devices')
BEGIN
    CREATE TABLE Devices (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Manufacturer NVARCHAR(100) NOT NULL,
        Type NVARCHAR(20) NOT NULL CHECK (Type IN ('phone', 'tablet')),
        OS NVARCHAR(50) NOT NULL,
        OSVersion NVARCHAR(50) NOT NULL,
        Processor NVARCHAR(100) NOT NULL,
        RamGB INT NOT NULL,
        Description NVARCHAR(MAX) NULL,

        UserId INT NULL FOREIGN KEY REFERENCES Users(Id) ON DELETE SET NULL
    );
END
GO