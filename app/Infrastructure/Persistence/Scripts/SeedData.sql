USE DeviceManagement;
GO


MERGE INTO Users AS target
USING (VALUES
    ('Sara', 'Administrator', 'Cluj Napoca'),
    ('David', 'Standard User', 'Suceva'),
    ('Mihai', 'Technician', 'Bucharest')
) AS source (Name, Role, Location)
ON target.Name = source.Name
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Name, Role, Location)
    VALUES (source.Name, source.Role, source.Location);
GO


MERGE INTO Devices AS target
USING (VALUES
    ('iPhone 16 Pro', 'Apple', 'phone', 'iOS', '16.5', 'A16 Bionic', 6, 'Standard corporate smartphone'),
    ('Galaxy S23', 'Samsung', 'phone', 'Android', '13.0', 'Snapdragon 8 Gen 2', 8, 'Alternative corporate smartphone'),
    ('iPad Air', 'Apple', 'tablet', 'iPadOS', '16.0', 'M1', 8, 'Tablet for field operatives')
) AS source (Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description)
ON target.Name = source.Name 
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description)
    VALUES (source.Name, source.Manufacturer, source.Type, source.OS, source.OSVersion, source.Processor, source.RamGB, source.Description);
GO