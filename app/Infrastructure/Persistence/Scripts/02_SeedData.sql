MERGE INTO Users AS target
USING (VALUES
    ('sara@example.com', 'Sara', 'Administrator', 'Cluj Napoca', 'hash'),
    ('david@example.com', 'David', 'Standard User', 'Suceava', 'hash'),
    ('mihai@example.com', 'Mihai', 'Technician', 'Bucharest', 'hash'),
    ('elena@example.com', 'Elena', 'QA Engineer', 'Timisoara', 'hash'),
    ('andrei@example.com', 'Andrei', 'Backend Developer', 'Iasi', 'hash'),
    ('cristina@example.com', 'Cristina', 'Project Manager', 'Cluj Napoca', 'hash'),
    ('radu@example.com', 'Radu', 'Sales Executive', 'Brasov', 'hash'),
    ('ioana@example.com', 'Ioana', 'UI/UX Designer', 'Remote', 'hash'),
    ('alex@example.com', 'Alex', 'DevOps Engineer', 'Constanta', 'hash'),
    ('monica@example.com', 'Monica', 'Data Analyst', 'Cluj Napoca', 'hash'),
    ('florin@example.com', 'Florin', 'Frontend Developer', 'Iasi', 'hash'),
    ('diana@example.com', 'Diana', 'HR Specialist', 'Bucharest', 'hash')
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
    ('Galaxy Tab S9', 'Samsung', 'tablet', 'Android', '13.0', 'Snapdragon 8 Gen 2', 12, 'Designer tablet', NULL),
    ('iPhone 15', 'Apple', 'phone', 'iOS', '17.0', 'A16 Bionic', 6, 'Marketing team phone', 6),
    ('Xiaomi 13', 'Xiaomi', 'phone', 'Android', '13.0', 'Snapdragon 8 Gen 2', 8, 'Budget friendly corporate phone', NULL),
    ('iPad Pro', 'Apple', 'tablet', 'iPadOS', '17.0', 'M2', 16, 'Video editing tablet', 3),
    ('ThinkPad X1 Tablet', 'Lenovo', 'tablet', 'Windows', '10', 'Intel i7', 16, 'Legacy technician tablet', 1),
    ('OnePlus 11', 'OnePlus', 'phone', 'Android', '13.0', 'Snapdragon 8 Gen 2', 16, 'High speed testing phone', NULL),
    ('Galaxy A54', 'Samsung', 'phone', 'Android', '13.0', 'Exynos 1380', 6, 'Mid-range testing device', NULL),
    ('iPhone 14 Pro Max', 'Apple', 'phone', 'iOS', '16.2', 'A16 Bionic', 6, 'Executive phone', 7),
    ('Pixel 7 Pro', 'Google', 'phone', 'Android', '13.0', 'Tensor G2', 12, 'Developer phone', 4),
    ('OnePlus 10 Pro', 'OnePlus', 'phone', 'Android', '12.0', 'Snapdragon 8 Gen 1', 12, 'Testing device', 9),
    ('Xiaomi 12S Ultra', 'Xiaomi', 'phone', 'Android', '12.0', 'Snapdragon 8+ Gen 1', 12, 'Camera testing', 10),
    ('iPad Mini', 'Apple', 'tablet', 'iPadOS', '16.0', 'A15 Bionic', 4, 'Field agent tablet', 8),
    ('Galaxy Tab S8', 'Samsung', 'tablet', 'Android', '12.0', 'Snapdragon 8 Gen 1', 8, 'Sales team tablet', 7),
    ('Lenovo Tab P12', 'Lenovo', 'tablet', 'Android', '13.0', 'MediaTek Dimensity', 8, 'Content consumption', 11),
    ('Huawei MatePad Pro', 'Huawei', 'tablet', 'HarmonyOS', '3.0', 'Snapdragon 888', 8, 'Design tablet', 6),
    ('iPhone SE 3rd Gen', 'Apple', 'phone', 'iOS', '15.4', 'A15 Bionic', 4, 'Spare phone', NULL),
    ('Galaxy A14', 'Samsung', 'phone', 'Android', '13.0', 'MediaTek Helio', 4, 'Budget spare', NULL),
    ('iPad 10th Gen', 'Apple', 'tablet', 'iPadOS', '16.0', 'A14 Bionic', 4, 'Loaner tablet', NULL),
    ('Nokia X30', 'Nokia', 'phone', 'Android', '12.0', 'Snapdragon 695', 6, 'Test device', NULL),
    ('Motorola Edge 30', 'Motorola', 'phone', 'Android', '12.0', 'Snapdragon 778G+', 8, 'Test device', NULL),
    ('Sony Xperia 1 IV', 'Sony', 'phone', 'Android', '12.0', 'Snapdragon 8 Gen 1', 12, 'Media testing', NULL),
    ('iPad Pro 11"', 'Apple', 'tablet', 'iPadOS', '16.0', 'M2', 8, 'Backup device', NULL),
    ('Galaxy Tab A8', 'Samsung', 'tablet', 'Android', '12.0', 'Unisoc Tiger', 3, 'Kiosk device', NULL),
    ('Lenovo Yoga Tab 13', 'Lenovo', 'tablet', 'Android', '11.0', 'Snapdragon 870', 8, 'Display device', NULL),
    ('Nothing Phone 2', 'Nothing', 'phone', 'Android', '13.0', 'Snapdragon 8+ Gen 1', 12, 'UI testing', 12),
    ('Asus ROG Phone 6', 'Asus', 'phone', 'Android', '12.0', 'Snapdragon 8+ Gen 1', 16, 'Gaming test', 5),
    ('iPhone 13', 'Apple', 'phone', 'iOS', '15.0', 'A15 Bionic', 4, 'Legacy testing', 2),
    ('Redmi Note 12', 'Xiaomi', 'phone', 'Android', '12.0', 'Snapdragon 4 Gen 1', 4, 'Budget testing', 10),
    ('iPad 9th Gen', 'Apple', 'tablet', 'iPadOS', '15.0', 'A13 Bionic', 3, 'Training device', NULL),
    ('Realme GT 2 Pro', 'Realme', 'phone', 'Android', '12.0', 'Snapdragon 8 Gen 1', 12, 'Testing pool', NULL),
    ('Vivo X90 Pro', 'Vivo', 'phone', 'Android', '13.0', 'Dimensity 9200', 12, 'Camera testing', NULL),
    ('Oppo Find X5', 'Oppo', 'phone', 'Android', '12.0', 'Snapdragon 8 Gen 1', 8, 'Test device', NULL)
) AS source (Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description, UserId)
ON target.Name = source.Name 
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Name, Manufacturer, Type, OS, OSVersion, Processor, RamGB, Description, UserId)
    VALUES (source.Name, source.Manufacturer, source.Type, source.OS, source.OSVersion, source.Processor, source.RamGB, source.Description, source.UserId);
GO