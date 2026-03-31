IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DeviceManagement')
BEGIN
    CREATE DATABASE DeviceManagement;
END
GO
