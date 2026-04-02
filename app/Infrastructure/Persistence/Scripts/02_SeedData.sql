MERGE INTO Users AS target
USING (VALUES
    ('sara@example.com', 'Sara', 'Administrator', 'Cluj Napoca', 'hash'),
    ('david@example.com', 'David', 'Standard User', 'Suceava', 'hash'),
    ('mihai@example.com', 'Mihai', 'Technician', 'Bucharest', 'hash'),
    ('elena@example.com', 'Elena', 'QA Engineer', 'Timisoara', 'hash'),
    ('andrei@example.com', 'Andrei', 'Backend Developer', 'Iasi', 'hash'),
    ('cristina@example.com', 'Cristina', 'Project Manager', 'Cluj Napoca', 'hash'),
    ('radu@example.com', 'Radu', 'Sales Executive', 'Brasov', 'hash'),
    ('ioana@example.com', 'Ioana', 'UI/UX Designer', 'Remote', 'hash')
) AS source (Email, Name, Role, Location, PasswordHash)
ON target.Email = source.Email
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Email, Name, Role, Location, PasswordHash)
    VALUES (source.Email, source.Name, source.Role, source.Location, source.PasswordHash);
GO

MERGE INTO Devices AS target
USING (VALUES
    ('iPhone 16 Pro', 'Apple', 'phone', 'iOS', '16.5', 'A16 Bionic', 6, 'Standard corporate smartphone', 2),
    ('Galaxy S23', 'Samsung', 'phone', 'Android', '13.0', 'Snapdragon 8 Gen 2', 8, 'Alternative corporate smartphone', 3),
    ('iPad Air', 'Apple', 'tablet', 'iPadOS', '16.0', 'M1', 8, 'Tablet for field operatives', 1),
    ('Surface Pro 9', 'Microsoft', 'tablet', 'Windows', '11', 'Intel i5', 16, 'High-performance windows tablet', 5),
    ('Pixel 8', 'Google', 'phone', 'Android', '14.0', 'Tensor G3', 8, 'Developer testing device', 4),
    ('Galaxy Tab S9', 'Samsung', 'tablet', 'Android', '13.0', 'Snapdragon 8 Gen 2', 12, 'Designer tablet', 8),
    ('iPhone 15', 'Apple', 'phone', 'iOS', '17.0', 'A16 Bionic', 6, 'Marketing team phone', 6),
    ('Xiaomi 13', 'Xiaomi', 'phone', 'Android', '13.0', 'Snapdragon 8 Gen 2', 8, 'Budget friendly corporate phone', 7),
    ('iPad Pro', 'Apple', 'tablet', 'iPadOS', '17.0', 'M2', 16, 'Video editing tablet', 3),
    ('ThinkPad X1 Tablet', 'Lenovo', 'tablet', 'Windows', '10', 'Intel i7', 16, 'Legacy technician tablet', 1),
    ('OnePlus 11', 'OnePlus', 'phone', 'Android', '13.0', 'Snapdragon 8 Gen 2', 16, 'High speed testing phone', 2),
    ('Galaxy A54', 'Samsung', 'phone', 'Android', '13.0', 'Exynos 1380', 6, 'Mid-range testing device', 4)
) AS source (Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description, UserId)
ON target.Name = source.Name 
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description, UserId)
    VALUES (source.Name, source.Manufacturer, source.Type, source.OS, source.OSVersion, source.Processor, source.RamGB, source.Description, source.UserId);
GO