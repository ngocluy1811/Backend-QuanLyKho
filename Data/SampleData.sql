-- Sample Data for Warehouse Management System
-- This script creates sample data for testing the warehouse layout

-- Insert Companies
INSERT INTO Companies (CompanyName, Address, Phone, Email, IsActive, CreatedAt) VALUES
('Công ty Phân bón ABC', '123 Đường ABC, Quận 1, TP.HCM', '0123456789', 'contact@abc.com', 1, GETUTCDATE());

-- Get Company ID
DECLARE @CompanyId INT = SCOPE_IDENTITY();

-- Insert Product Categories
INSERT INTO ProductCategories (CompanyId, Code, CategoryName, Description, IsActive, CreatedAt) VALUES
(@CompanyId, 'NPK', 'NPK', 'Phân NPK', 1, GETUTCDATE()),
(@CompanyId, 'URE', 'Ure', 'Phân Ure', 1, GETUTCDATE()),
(@CompanyId, 'DAP', 'DAP', 'Phân DAP', 1, GETUTCDATE()),
(@CompanyId, 'KALI', 'Kali', 'Phân Kali', 1, GETUTCDATE()),
(@CompanyId, 'LAN', 'Lân', 'Phân Lân', 1, GETUTCDATE()),
(@CompanyId, 'QUICK', 'Quick', 'Phân Quick', 1, GETUTCDATE());

-- Get Category IDs
DECLARE @NPKCategoryId INT = (SELECT Id FROM ProductCategories WHERE Code = 'NPK' AND CompanyId = @CompanyId);
DECLARE @URECategoryId INT = (SELECT Id FROM ProductCategories WHERE Code = 'URE' AND CompanyId = @CompanyId);
DECLARE @DAPCategoryId INT = (SELECT Id FROM ProductCategories WHERE Code = 'DAP' AND CompanyId = @CompanyId);
DECLARE @KALICategoryId INT = (SELECT Id FROM ProductCategories WHERE Code = 'KALI' AND CompanyId = @CompanyId);
DECLARE @LANCategoryId INT = (SELECT Id FROM ProductCategories WHERE Code = 'LAN' AND CompanyId = @CompanyId);
DECLARE @QUICKCategoryId INT = (SELECT Id FROM ProductCategories WHERE Code = 'QUICK' AND CompanyId = @CompanyId);

-- Insert Products
INSERT INTO Products (ProductCode, ProductName, Description, Category, CategoryId, Unit, Price, UnitPrice, MinStockLevel, MaxStockLevel, CompanyId, Status, IsActive, CreatedAt) VALUES
('NPK001', 'NPK 16-16-8', 'Phân NPK 16-16-8', 'NPK', @NPKCategoryId, 'kg', 15000, 15000, 50, 1000, @CompanyId, 'Active', 1, GETUTCDATE()),
('URE001', 'Ure', 'Phân Ure', 'Ure', @URECategoryId, 'kg', 12000, 12000, 50, 1000, @CompanyId, 'Active', 1, GETUTCDATE()),
('DAP001', 'DAP', 'Phân DAP', 'DAP', @DAPCategoryId, 'kg', 18000, 18000, 50, 1000, @CompanyId, 'Active', 1, GETUTCDATE()),
('KALI001', 'Kali', 'Phân Kali', 'Kali', @KALICategoryId, 'kg', 14000, 14000, 50, 1000, @CompanyId, 'Active', 1, GETUTCDATE()),
('LAN001', 'Lân', 'Phân Lân', 'Lân', @LANCategoryId, 'kg', 13000, 13000, 50, 1000, @CompanyId, 'Active', 1, GETUTCDATE()),
('QUICK001', 'Quick', 'Phân Quick', 'Quick', @QUICKCategoryId, 'kg', 16000, 16000, 50, 1000, @CompanyId, 'Active', 1, GETUTCDATE());

-- Get Product IDs
DECLARE @NPKProductId INT = (SELECT Id FROM Products WHERE ProductCode = 'NPK001');
DECLARE @UREProductId INT = (SELECT Id FROM Products WHERE ProductCode = 'URE001');
DECLARE @DAPProductId INT = (SELECT Id FROM Products WHERE ProductCode = 'DAP001');
DECLARE @KALIProductId INT = (SELECT Id FROM Products WHERE ProductCode = 'KALI001');
DECLARE @LANProductId INT = (SELECT Id FROM Products WHERE ProductCode = 'LAN001');
DECLARE @QUICKProductId INT = (SELECT Id FROM Products WHERE ProductCode = 'QUICK001');

-- Insert Warehouse
INSERT INTO Warehouses (Name, Description, Address, Capacity, CurrentStock, Status, IsActive, CreatedAt) VALUES
('KHO A', 'Kho chính của công ty', '123 Đường ABC, Quận 1, TP.HCM', 100000, 0, 'Active', 1, GETUTCDATE());

-- Get Warehouse ID
DECLARE @WarehouseId INT = SCOPE_IDENTITY();

-- Insert Warehouse Zone
INSERT INTO WarehouseZones (WarehouseId, ZoneName, ZoneCode, ZoneType, MaxCapacity, CurrentCapacity, Status, IsActive, CreatedAt) VALUES
(@WarehouseId, 'Khu vực chính', 'ZONE-001', 'Storage', 100, 0, 'Active', 1, GETUTCDATE());

-- Get Zone ID
DECLARE @ZoneId INT = SCOPE_IDENTITY();

-- Insert Warehouse Clusters
INSERT INTO WarehouseClusters (Id, WarehouseId, ClusterName, ClusterType, Color, Status, IsActive, CreatedAt) VALUES
('cluster-1', @WarehouseId, 'Khu vực A', 'Storage', '#3b82f6', 'Active', 1, GETUTCDATE()),
('cluster-2', @WarehouseId, 'Khu vực B', 'Storage', '#10b981', 'Active', 1, GETUTCDATE()),
('cluster-3', @WarehouseId, 'Khu vực C', 'Storage', '#f59e0b', 'Active', 1, GETUTCDATE()),
('cluster-4', @WarehouseId, 'Khu vực D', 'Storage', '#ef4444', 'Active', 1, GETUTCDATE());

-- Insert Warehouse Cells (100 cells: A01-A10, B01-B10, C01-C10, D01-D10)
-- Row A (A01-A10)
INSERT INTO WarehouseCells (WarehouseId, ZoneId, CellCode, CellType, Row, Column, MaxCapacity, CurrentAmount, ProductId, ProductName, BatchNumber, LastMoved, Status, ClusterName, IsActive, CreatedAt) VALUES
(@WarehouseId, @ZoneId, 'A01', 'Shelf', 0, 0, 1000, 710, @NPKProductId, 'NPK 16-16-8', 'BATCH20250122A01', GETUTCDATE(), 'Occupied', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A02', 'Shelf', 0, 1, 1000, 450, @UREProductId, 'Ure', 'BATCH20250122A02', GETUTCDATE(), 'Occupied', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A03', 'Shelf', 0, 2, 1000, 380, @DAPProductId, 'DAP', 'BATCH20250122A03', GETUTCDATE(), 'Occupied', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A04', 'Shelf', 0, 3, 1000, 620, @KALIProductId, 'Kali', 'BATCH20250122A04', GETUTCDATE(), 'Occupied', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A05', 'Shelf', 0, 4, 1000, 0, NULL, NULL, NULL, NULL, 'Empty', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A06', 'Shelf', 0, 5, 1000, 520, @LANProductId, 'Lân', 'BATCH20250122A06', GETUTCDATE(), 'Occupied', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A07', 'Shelf', 0, 6, 1000, 250, @QUICKProductId, 'Quick', 'BATCH20250122A07', GETUTCDATE(), 'Occupied', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A08', 'Shelf', 0, 7, 1000, 950, @NPKProductId, 'NPK 16-16-8', 'BATCH20250122A08', GETUTCDATE(), 'Full', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A09', 'Shelf', 0, 8, 1000, 180, @UREProductId, 'Ure', 'BATCH20250122A09', GETUTCDATE(), 'Occupied', 'Khu vực A', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'A10', 'Shelf', 0, 9, 1000, 920, @DAPProductId, 'DAP', 'BATCH20250122A10', GETUTCDATE(), 'Full', 'Khu vực A', 1, GETUTCDATE());

-- Row B (B01-B10)
INSERT INTO WarehouseCells (WarehouseId, ZoneId, CellCode, CellType, Row, Column, MaxCapacity, CurrentAmount, ProductId, ProductName, BatchNumber, LastMoved, Status, ClusterName, IsActive, CreatedAt) VALUES
(@WarehouseId, @ZoneId, 'B01', 'Shelf', 1, 0, 1000, 680, @KALIProductId, 'Kali', 'BATCH20250122B01', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B02', 'Shelf', 1, 1, 1000, 420, @LANProductId, 'Lân', 'BATCH20250122B02', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B03', 'Shelf', 1, 2, 1000, 150, @QUICKProductId, 'Quick', 'BATCH20250122B03', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B04', 'Shelf', 1, 3, 1000, 580, @NPKProductId, 'NPK 16-16-8', 'BATCH20250122B04', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B05', 'Shelf', 1, 4, 1000, 320, @UREProductId, 'Ure', 'BATCH20250122B05', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B06', 'Shelf', 1, 5, 1000, 280, @DAPProductId, 'DAP', 'BATCH20250122B06', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B07', 'Shelf', 1, 6, 1000, 190, @KALIProductId, 'Kali', 'BATCH20250122B07', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B08', 'Shelf', 1, 7, 1000, 750, @LANProductId, 'Lân', 'BATCH20250122B08', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B09', 'Shelf', 1, 8, 1000, 640, @QUICKProductId, 'Quick', 'BATCH20250122B09', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'B10', 'Shelf', 1, 9, 1000, 480, @NPKProductId, 'NPK 16-16-8', 'BATCH20250122B10', GETUTCDATE(), 'Occupied', 'Khu vực B', 1, GETUTCDATE());

-- Row C (C01-C10)
INSERT INTO WarehouseCells (WarehouseId, ZoneId, CellCode, CellType, Row, Column, MaxCapacity, CurrentAmount, ProductId, ProductName, BatchNumber, LastMoved, Status, ClusterName, IsActive, CreatedAt) VALUES
(@WarehouseId, @ZoneId, 'C01', 'Shelf', 2, 0, 1000, 980, @UREProductId, 'Ure', 'BATCH20250122C01', GETUTCDATE(), 'Full', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C02', 'Shelf', 2, 1, 1000, 220, @DAPProductId, 'DAP', 'BATCH20250122C02', GETUTCDATE(), 'Occupied', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C03', 'Shelf', 2, 2, 1000, 350, @KALIProductId, 'Kali', 'BATCH20250122C03', GETUTCDATE(), 'Occupied', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C04', 'Shelf', 2, 3, 1000, 120, @LANProductId, 'Lân', 'BATCH20250122C04', GETUTCDATE(), 'Occupied', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C05', 'Shelf', 2, 4, 1000, 280, @QUICKProductId, 'Quick', 'BATCH20250122C05', GETUTCDATE(), 'Occupied', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C06', 'Shelf', 2, 5, 1000, 720, @NPKProductId, 'NPK 16-16-8', 'BATCH20250122C06', GETUTCDATE(), 'Occupied', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C07', 'Shelf', 2, 6, 1000, 650, @UREProductId, 'Ure', 'BATCH20250122C07', GETUTCDATE(), 'Occupied', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C08', 'Shelf', 2, 7, 1000, 580, @DAPProductId, 'DAP', 'BATCH20250122C08', GETUTCDATE(), 'Occupied', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C09', 'Shelf', 2, 8, 1000, 940, @KALIProductId, 'Kali', 'BATCH20250122C09', GETUTCDATE(), 'Full', 'Khu vực C', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'C10', 'Shelf', 2, 9, 1000, 0, NULL, NULL, NULL, NULL, 'Empty', 'Khu vực C', 1, GETUTCDATE());

-- Row D (D01-D10)
INSERT INTO WarehouseCells (WarehouseId, ZoneId, CellCode, CellType, Row, Column, MaxCapacity, CurrentAmount, ProductId, ProductName, BatchNumber, LastMoved, Status, ClusterName, IsActive, CreatedAt) VALUES
(@WarehouseId, @ZoneId, 'D01', 'Shelf', 3, 0, 1000, 820, @LANProductId, 'Lân', 'BATCH20250122D01', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D02', 'Shelf', 3, 1, 1000, 560, @QUICKProductId, 'Quick', 'BATCH20250122D02', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D03', 'Shelf', 3, 2, 1000, 180, @NPKProductId, 'NPK 16-16-8', 'BATCH20250122D03', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D04', 'Shelf', 3, 3, 1000, 740, @UREProductId, 'Ure', 'BATCH20250122D04', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D05', 'Shelf', 3, 4, 1000, 690, @DAPProductId, 'DAP', 'BATCH20250122D05', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D06', 'Shelf', 3, 5, 1000, 520, @KALIProductId, 'Kali', 'BATCH20250122D06', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D07', 'Shelf', 3, 6, 1000, 960, @LANProductId, 'Lân', 'BATCH20250122D07', GETUTCDATE(), 'Full', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D08', 'Shelf', 3, 7, 1000, 480, @QUICKProductId, 'Quick', 'BATCH20250122D08', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D09', 'Shelf', 3, 8, 1000, 620, @NPKProductId, 'NPK 16-16-8', 'BATCH20250122D09', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE()),
(@WarehouseId, @ZoneId, 'D10', 'Shelf', 3, 9, 1000, 320, @UREProductId, 'Ure', 'BATCH20250122D10', GETUTCDATE(), 'Occupied', 'Khu vực D', 1, GETUTCDATE());

-- Update Warehouse current stock
UPDATE Warehouses 
SET CurrentStock = (
    SELECT SUM(CurrentAmount) 
    FROM WarehouseCells 
    WHERE WarehouseId = @WarehouseId AND IsActive = 1
)
WHERE Id = @WarehouseId;

-- Update Zone current capacity
UPDATE WarehouseZones 
SET CurrentCapacity = (
    SELECT COUNT(*) 
    FROM WarehouseCells 
    WHERE ZoneId = @ZoneId AND IsActive = 1 AND CurrentAmount > 0
)
WHERE Id = @ZoneId;

PRINT 'Sample warehouse data created successfully!';
PRINT 'Warehouse ID: ' + CAST(@WarehouseId AS VARCHAR(10));
PRINT 'Total cells created: 40';
PRINT 'Products: NPK, Ure, DAP, Kali, Lân, Quick';
