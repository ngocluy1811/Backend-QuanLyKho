using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedDataController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SeedDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seed roles data for testing
    /// </summary>
    [HttpPost("roles-data")]
    public async Task<IActionResult> SeedRolesData()
    {
        try
        {
            // Check if roles already exist
            var existingRoles = await _context.Roles.ToListAsync();
            if (existingRoles.Any())
            {
                return Ok(new { message = "Roles already exist", count = existingRoles.Count });
            }

            // Create roles
            var roles = new List<Role>
            {
                new Role 
                { 
                    RoleName = "Quản trị viên", 
                    Description = "Quản trị viên hệ thống - có toàn quyền",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Role 
                { 
                    RoleName = "Nhân viên kho", 
                    Description = "Nhân viên quản lý kho hàng",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Role 
                { 
                    RoleName = "Tổ trưởng", 
                    Description = "Tổ trưởng quản lý nhóm",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Role 
                { 
                    RoleName = "Nhân viên kinh doanh", 
                    Description = "Nhân viên kinh doanh và bán hàng",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Roles.AddRange(roles);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Roles seeded successfully", 
                count = roles.Count,
                roles = roles.Select(r => new { r.Id, r.RoleName, r.Description })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error seeding roles", error = ex.Message });
        }
    }

    /// <summary>
    /// Seed warehouse data for testing
    /// </summary>
    [HttpPost("warehouse-data")]
    public async Task<IActionResult> SeedWarehouseData()
    {
        try
        {
            // Clear existing data first - in correct order (child tables first)
            _context.WarehouseCells.RemoveRange(_context.WarehouseCells);
            await _context.SaveChangesAsync();
            
            _context.WarehouseClusters.RemoveRange(_context.WarehouseClusters);
            await _context.SaveChangesAsync();
            
            _context.WarehouseZones.RemoveRange(_context.WarehouseZones);
            await _context.SaveChangesAsync();
            
            _context.Warehouses.RemoveRange(_context.Warehouses);
            await _context.SaveChangesAsync();
            
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();
            
            _context.ProductCategories.RemoveRange(_context.ProductCategories);
            await _context.SaveChangesAsync();
            
            _context.Companies.RemoveRange(_context.Companies);
            await _context.SaveChangesAsync();

            // Create company first
            var company = new Company
            {
                CompanyName = "Công ty Phân bón ABC",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                Phone = "0123456789",
                Email = "contact@abc.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Create product categories
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CompanyId = company.Id, Code = "NPK", CategoryName = "NPK", Description = "Phân NPK", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "URE", CategoryName = "Ure", Description = "Phân Ure", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "DAP", CategoryName = "DAP", Description = "Phân DAP", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "KALI", CategoryName = "Kali", Description = "Phân Kali", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "LAN", CategoryName = "Lân", Description = "Phân Lân", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "QUICK", CategoryName = "Quick", Description = "Phân Quick", IsActive = true, CreatedAt = DateTime.UtcNow }
            };

            _context.ProductCategories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Create products
            var products = new List<Product>
            {
                new Product { ProductCode = "NPK001", ProductName = "NPK 16-16-8", Description = "Phân NPK 16-16-8", CategoryId = categories[0].Id, Unit = "kg", Price = 15000, UnitPrice = 15000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id },
                new Product { ProductCode = "URE001", ProductName = "Ure", Description = "Phân Ure", CategoryId = categories[1].Id, Unit = "kg", Price = 12000, UnitPrice = 12000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id },
                new Product { ProductCode = "DAP001", ProductName = "DAP", Description = "Phân DAP", CategoryId = categories[2].Id, Unit = "kg", Price = 18000, UnitPrice = 18000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id },
                new Product { ProductCode = "KALI001", ProductName = "Kali", Description = "Phân Kali", CategoryId = categories[3].Id, Unit = "kg", Price = 14000, UnitPrice = 14000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id },
                new Product { ProductCode = "LAN001", ProductName = "Lân", Description = "Phân Lân", CategoryId = categories[4].Id, Unit = "kg", Price = 13000, UnitPrice = 13000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id },
                new Product { ProductCode = "QUICK001", ProductName = "Quick", Description = "Phân Quick", CategoryId = categories[5].Id, Unit = "kg", Price = 16000, UnitPrice = 16000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Create warehouse
            var warehouse = new Warehouse
            {
                Name = "KHO A",
                Description = "Kho chính",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                Width = 10,
                Height = 10,
                CompanyId = company.Id,
                Status = "Active",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            // Create warehouse zone
            var zone = new WarehouseZone
            {
                WarehouseId = warehouse.Id,
                ZoneName = "Khu vực chính",
                ZoneCode = "ZONE-001",
                ZoneType = "Storage",
                MaxCapacity = 100,
                CurrentCapacity = 0,
                Status = "Active",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.WarehouseZones.Add(zone);
            await _context.SaveChangesAsync();

            // Create clusters
            var clusters = new List<WarehouseCluster>
            {
                new WarehouseCluster
                {
                    Id = Guid.NewGuid().ToString(),
                    WarehouseId = warehouse.Id,
                    ClusterName = "Khu vực A",
                    ClusterType = "Storage",
                    Color = "#3b82f6",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new WarehouseCluster
                {
                    Id = Guid.NewGuid().ToString(),
                    WarehouseId = warehouse.Id,
                    ClusterName = "Khu vực B",
                    ClusterType = "Storage",
                    Color = "#10b981",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.WarehouseClusters.AddRange(clusters);
            await _context.SaveChangesAsync();

            // Create warehouse cells
            var cells = new List<WarehouseCell>();
            var productIndex = 0;

            for (int row = 1; row <= 10; row++)
            {
                for (int col = 1; col <= 10; col++)
                {
                    var cellCode = $"{(char)('A' + row - 1)}{col:D2}";
                    var product = products[productIndex % products.Count];
                    var currentAmount = new Random().Next(0, 1001);
                    var status = currentAmount == 0 ? "Empty" : 
                                currentAmount < 100 ? "Low" :
                                currentAmount < 700 ? "Medium" :
                                currentAmount < 900 ? "High" : "Full";

                    var clusterName = row <= 5 ? "Khu vực A" : "Khu vực B";

                    cells.Add(new WarehouseCell
                    {
                        WarehouseId = warehouse.Id,
                        ZoneId = zone.Id,
                        CellCode = cellCode,
                        CellType = "Shelf",
                        Row = row,
                        Column = col,
                        MaxCapacity = 1000,
                        CurrentAmount = currentAmount,
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        BatchNumber = $"BATCH{DateTime.Now:yyyyMMdd}{cellCode}",
                        LastMoved = currentAmount > 0 ? DateTime.UtcNow.AddDays(-new Random().Next(1, 30)) : null,
                        Status = status,
                        ClusterName = clusterName,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });

                    productIndex++;
                }
            }

            _context.WarehouseCells.AddRange(cells);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Warehouse data seeded successfully", warehouseId = warehouse.Id, cellCount = cells.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error seeding warehouse data", error = ex.Message });
        }
    }

    /// <summary>
    /// Clear warehouse data
    /// </summary>
    [HttpDelete("warehouse-data")]
    public async Task<IActionResult> ClearWarehouseData()
    {
        try
        {
            // Clear all related data in correct order
            _context.WarehouseCells.RemoveRange(_context.WarehouseCells);
            await _context.SaveChangesAsync();
            
            _context.WarehouseClusters.RemoveRange(_context.WarehouseClusters);
            await _context.SaveChangesAsync();
            
            _context.WarehouseZones.RemoveRange(_context.WarehouseZones);
            await _context.SaveChangesAsync();
            
            _context.Warehouses.RemoveRange(_context.Warehouses);
            await _context.SaveChangesAsync();
            
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();
            
            _context.ProductCategories.RemoveRange(_context.ProductCategories);
            await _context.SaveChangesAsync();
            
            _context.Companies.RemoveRange(_context.Companies);
            await _context.SaveChangesAsync();

            return Ok(new { message = "All warehouse data cleared successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error clearing warehouse data", error = ex.Message });
        }
    }

    /// <summary>
    /// Create sample warehouse layout data (40 cells exactly like mock)
    /// </summary>
    [HttpPost("warehouse-layout")]
    public async Task<IActionResult> CreateWarehouseLayout()
    {
        try
        {
            // Get existing warehouse
            var warehouse = await _context.Warehouses.FirstOrDefaultAsync();
            if (warehouse == null)
            {
                return BadRequest(new { message = "No warehouse found. Please create warehouse first." });
            }

            // Get existing products
            var products = await _context.Products.Take(6).ToListAsync();
            if (products.Count < 6)
            {
                return BadRequest(new { message = "Not enough products. Please create products first." });
            }

            // Get existing zone
            var zone = await _context.WarehouseZones.FirstOrDefaultAsync(z => z.WarehouseId == warehouse.Id);
            if (zone == null)
            {
                return BadRequest(new { message = "No warehouse zone found. Please create zone first." });
            }

            // Clear existing cells
            var existingCells = await _context.WarehouseCells.Where(c => c.WarehouseId == warehouse.Id).ToListAsync();
            _context.WarehouseCells.RemoveRange(existingCells);
            await _context.SaveChangesAsync();

            // Create warehouse cells exactly like the mock data (40 cells: 4 rows x 10 columns)
            var cells = new List<WarehouseCell>();
            var random = new Random();
            var clusterNames = new[] { "Khu vực A", "Khu vực B", "Khu vực C", "Khu vực D" };

            // Predefined data to match the mock exactly
            var cellData = new[]
            {
                // Row A (A01-A10)
                new { Code = "A01", Row = 0, Col = 0, Amount = 710, Product = 0, Cluster = 0 },
                new { Code = "A02", Row = 0, Col = 1, Amount = 450, Product = 1, Cluster = 0 },
                new { Code = "A03", Row = 0, Col = 2, Amount = 380, Product = 2, Cluster = 0 },
                new { Code = "A04", Row = 0, Col = 3, Amount = 620, Product = 3, Cluster = 0 },
                new { Code = "A05", Row = 0, Col = 4, Amount = 0, Product = -1, Cluster = 0 },
                new { Code = "A06", Row = 0, Col = 5, Amount = 520, Product = 4, Cluster = 0 },
                new { Code = "A07", Row = 0, Col = 6, Amount = 250, Product = 5, Cluster = 0 },
                new { Code = "A08", Row = 0, Col = 7, Amount = 950, Product = 0, Cluster = 0 },
                new { Code = "A09", Row = 0, Col = 8, Amount = 180, Product = 1, Cluster = 0 },
                new { Code = "A10", Row = 0, Col = 9, Amount = 920, Product = 2, Cluster = 0 },
                
                // Row B (B01-B10)
                new { Code = "B01", Row = 1, Col = 0, Amount = 680, Product = 3, Cluster = 1 },
                new { Code = "B02", Row = 1, Col = 1, Amount = 420, Product = 4, Cluster = 1 },
                new { Code = "B03", Row = 1, Col = 2, Amount = 150, Product = 5, Cluster = 1 },
                new { Code = "B04", Row = 1, Col = 3, Amount = 580, Product = 0, Cluster = 1 },
                new { Code = "B05", Row = 1, Col = 4, Amount = 320, Product = 1, Cluster = 1 },
                new { Code = "B06", Row = 1, Col = 5, Amount = 280, Product = 2, Cluster = 1 },
                new { Code = "B07", Row = 1, Col = 6, Amount = 190, Product = 3, Cluster = 1 },
                new { Code = "B08", Row = 1, Col = 7, Amount = 750, Product = 4, Cluster = 1 },
                new { Code = "B09", Row = 1, Col = 8, Amount = 640, Product = 5, Cluster = 1 },
                new { Code = "B10", Row = 1, Col = 9, Amount = 480, Product = 0, Cluster = 1 },
                
                // Row C (C01-C10)
                new { Code = "C01", Row = 2, Col = 0, Amount = 980, Product = 1, Cluster = 2 },
                new { Code = "C02", Row = 2, Col = 1, Amount = 220, Product = 2, Cluster = 2 },
                new { Code = "C03", Row = 2, Col = 2, Amount = 350, Product = 3, Cluster = 2 },
                new { Code = "C04", Row = 2, Col = 3, Amount = 120, Product = 4, Cluster = 2 },
                new { Code = "C05", Row = 2, Col = 4, Amount = 280, Product = 5, Cluster = 2 },
                new { Code = "C06", Row = 2, Col = 5, Amount = 720, Product = 0, Cluster = 2 },
                new { Code = "C07", Row = 2, Col = 6, Amount = 650, Product = 1, Cluster = 2 },
                new { Code = "C08", Row = 2, Col = 7, Amount = 580, Product = 2, Cluster = 2 },
                new { Code = "C09", Row = 2, Col = 8, Amount = 940, Product = 3, Cluster = 2 },
                new { Code = "C10", Row = 2, Col = 9, Amount = 0, Product = -1, Cluster = 2 },
                
                // Row D (D01-D10)
                new { Code = "D01", Row = 3, Col = 0, Amount = 820, Product = 4, Cluster = 3 },
                new { Code = "D02", Row = 3, Col = 1, Amount = 560, Product = 5, Cluster = 3 },
                new { Code = "D03", Row = 3, Col = 2, Amount = 180, Product = 0, Cluster = 3 },
                new { Code = "D04", Row = 3, Col = 3, Amount = 740, Product = 1, Cluster = 3 },
                new { Code = "D05", Row = 3, Col = 4, Amount = 690, Product = 2, Cluster = 3 },
                new { Code = "D06", Row = 3, Col = 5, Amount = 520, Product = 3, Cluster = 3 },
                new { Code = "D07", Row = 3, Col = 6, Amount = 960, Product = 4, Cluster = 3 },
                new { Code = "D08", Row = 3, Col = 7, Amount = 480, Product = 5, Cluster = 3 },
                new { Code = "D09", Row = 3, Col = 8, Amount = 620, Product = 0, Cluster = 3 },
                new { Code = "D10", Row = 3, Col = 9, Amount = 320, Product = 1, Cluster = 3 }
            };

            foreach (var data in cellData)
            {
                var currentAmount = data.Amount;
                var productId = data.Product >= 0 ? products[data.Product].Id : (int?)null;
                var product = data.Product >= 0 ? products[data.Product] : null;
                var status = currentAmount == 0 ? "Empty" : (currentAmount >= 900 ? "Full" : "Occupied");
                var clusterName = clusterNames[data.Cluster];

                cells.Add(new WarehouseCell
                {
                    WarehouseId = warehouse.Id,
                    ZoneId = zone.Id,
                    CellCode = data.Code,
                    CellType = "Shelf",
                    Row = data.Row,
                    Column = data.Col,
                    MaxCapacity = 1000,
                    CurrentAmount = currentAmount,
                    ProductId = productId,
                    ProductName = product?.ProductName,
                    BatchNumber = currentAmount > 0 ? $"BATCH{DateTime.Now:yyyyMMdd}{data.Code}" : null,
                    LastMoved = currentAmount > 0 ? DateTime.UtcNow.AddDays(-random.Next(1, 30)) : null,
                    Status = status,
                    ClusterName = clusterName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.WarehouseCells.AddRange(cells);
            await _context.SaveChangesAsync();

            // Update warehouse current stock (if property exists)
            // warehouse.CurrentStock = cells.Sum(c => c.CurrentAmount);
            // await _context.SaveChangesAsync();

            // Update zone current capacity
            zone.CurrentCapacity = cells.Count(c => c.CurrentAmount > 0);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Warehouse layout created successfully", 
                warehouseId = warehouse.Id, 
                totalCells = cells.Count,
                occupiedCells = cells.Count(c => c.CurrentAmount > 0),
                emptyCells = cells.Count(c => c.CurrentAmount == 0),
                fullCells = cells.Count(c => c.CurrentAmount >= 900)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating warehouse layout", error = ex.Message });
        }
    }

    /// <summary>
    /// Execute SQL script to create warehouse data
    /// </summary>
    [HttpPost("execute-sql")]
    public async Task<IActionResult> ExecuteSqlScript()
    {
        try
        {
            var sqlScript = await System.IO.File.ReadAllTextAsync("Data/SimpleWarehouseData.sql");
            
            // Split by GO statements and execute each part
            var statements = sqlScript.Split(new[] { "GO", "go" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var statement in statements)
            {
                if (!string.IsNullOrWhiteSpace(statement.Trim()))
                {
                    await _context.Database.ExecuteSqlRawAsync(statement.Trim());
                }
            }

            return Ok(new { message = "SQL script executed successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error executing SQL script", error = ex.Message });
        }
    }

    /// <summary>
    /// Create simple warehouse data for testing
    /// </summary>
    [HttpPost("create-simple-warehouse")]
    public async Task<IActionResult> CreateSimpleWarehouse()
    {
        try
        {
            // Check if data already exists
            var existingWarehouse = await _context.Warehouses.FirstOrDefaultAsync();
            if (existingWarehouse != null)
            {
                return Ok(new { message = "Warehouse data already exists", warehouseId = existingWarehouse.Id });
            }

            // Create company
            var company = new Company
            {
                CompanyName = "Công ty Phân bón ABC",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                Phone = "0123456789",
                Email = "contact@abc.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Create product categories
            var categories = new List<ProductCategory>
            {
                new ProductCategory { CompanyId = company.Id, Code = "NPK", CategoryName = "NPK", Description = "Phân NPK", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "URE", CategoryName = "Ure", Description = "Phân Ure", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "DAP", CategoryName = "DAP", Description = "Phân DAP", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "KALI", CategoryName = "Kali", Description = "Phân Kali", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "LAN", CategoryName = "Lân", Description = "Phân Lân", IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { CompanyId = company.Id, Code = "QUICK", CategoryName = "Quick", Description = "Phân Quick", IsActive = true, CreatedAt = DateTime.UtcNow }
            };
            _context.ProductCategories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Create products
            var products = new List<Product>
            {
                new Product { ProductCode = "NPK001", ProductName = "NPK 16-16-8", Description = "Phân NPK 16-16-8", CategoryId = categories[0].Id, Unit = "kg", Price = 15000, UnitPrice = 15000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id, Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "URE001", ProductName = "Ure", Description = "Phân Ure", CategoryId = categories[1].Id, Unit = "kg", Price = 12000, UnitPrice = 12000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id, Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "DAP001", ProductName = "DAP", Description = "Phân DAP", CategoryId = categories[2].Id, Unit = "kg", Price = 18000, UnitPrice = 18000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id, Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "KALI001", ProductName = "Kali", Description = "Phân Kali", CategoryId = categories[3].Id, Unit = "kg", Price = 14000, UnitPrice = 14000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id, Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "LAN001", ProductName = "Lân", Description = "Phân Lân", CategoryId = categories[4].Id, Unit = "kg", Price = 13000, UnitPrice = 13000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id, Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { ProductCode = "QUICK001", ProductName = "Quick", Description = "Phân Quick", CategoryId = categories[5].Id, Unit = "kg", Price = 16000, UnitPrice = 16000, MinStockLevel = 50, MaxStockLevel = 1000, CompanyId = company.Id, Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow }
            };
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Create warehouse
            var warehouse = new Warehouse
            {
                Name = "KHO A",
                Description = "Kho chính của công ty",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                Width = 10,
                Height = 4,
                Status = "Active",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            // Create warehouse zone
            var zone = new WarehouseZone
            {
                WarehouseId = warehouse.Id,
                ZoneName = "Khu vực chính",
                ZoneCode = "ZONE-001",
                ZoneType = "Storage",
                MaxCapacity = 100,
                CurrentCapacity = 0,
                Status = "Active",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.WarehouseZones.Add(zone);
            await _context.SaveChangesAsync();

            // Create warehouse clusters
            var clusters = new List<WarehouseCluster>
            {
                new WarehouseCluster { Id = "cluster-1", WarehouseId = warehouse.Id, ClusterName = "Khu vực A", ClusterType = "Storage", Color = "#3b82f6", Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow },
                new WarehouseCluster { Id = "cluster-2", WarehouseId = warehouse.Id, ClusterName = "Khu vực B", ClusterType = "Storage", Color = "#10b981", Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow },
                new WarehouseCluster { Id = "cluster-3", WarehouseId = warehouse.Id, ClusterName = "Khu vực C", ClusterType = "Storage", Color = "#f59e0b", Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow },
                new WarehouseCluster { Id = "cluster-4", WarehouseId = warehouse.Id, ClusterName = "Khu vực D", ClusterType = "Storage", Color = "#ef4444", Status = "Active", IsActive = true, CreatedAt = DateTime.UtcNow }
            };
            _context.WarehouseClusters.AddRange(clusters);
            await _context.SaveChangesAsync();

            // Create warehouse cells (40 cells: 4 rows x 10 columns) - exactly like mock data
            var cells = new List<WarehouseCell>();
            var random = new Random();
            var clusterNames = new[] { "Khu vực A", "Khu vực B", "Khu vực C", "Khu vực D" };

            // Predefined data to match the mock exactly
            var cellData = new[]
            {
                // Row A (A01-A10)
                new { Code = "A01", Row = 0, Col = 0, Amount = 710, Product = 0, Cluster = 0 },
                new { Code = "A02", Row = 0, Col = 1, Amount = 450, Product = 1, Cluster = 0 },
                new { Code = "A03", Row = 0, Col = 2, Amount = 380, Product = 2, Cluster = 0 },
                new { Code = "A04", Row = 0, Col = 3, Amount = 620, Product = 3, Cluster = 0 },
                new { Code = "A05", Row = 0, Col = 4, Amount = 0, Product = -1, Cluster = 0 },
                new { Code = "A06", Row = 0, Col = 5, Amount = 520, Product = 4, Cluster = 0 },
                new { Code = "A07", Row = 0, Col = 6, Amount = 250, Product = 5, Cluster = 0 },
                new { Code = "A08", Row = 0, Col = 7, Amount = 950, Product = 0, Cluster = 0 },
                new { Code = "A09", Row = 0, Col = 8, Amount = 180, Product = 1, Cluster = 0 },
                new { Code = "A10", Row = 0, Col = 9, Amount = 920, Product = 2, Cluster = 0 },
                
                // Row B (B01-B10)
                new { Code = "B01", Row = 1, Col = 0, Amount = 680, Product = 3, Cluster = 1 },
                new { Code = "B02", Row = 1, Col = 1, Amount = 420, Product = 4, Cluster = 1 },
                new { Code = "B03", Row = 1, Col = 2, Amount = 150, Product = 5, Cluster = 1 },
                new { Code = "B04", Row = 1, Col = 3, Amount = 580, Product = 0, Cluster = 1 },
                new { Code = "B05", Row = 1, Col = 4, Amount = 320, Product = 1, Cluster = 1 },
                new { Code = "B06", Row = 1, Col = 5, Amount = 280, Product = 2, Cluster = 1 },
                new { Code = "B07", Row = 1, Col = 6, Amount = 190, Product = 3, Cluster = 1 },
                new { Code = "B08", Row = 1, Col = 7, Amount = 750, Product = 4, Cluster = 1 },
                new { Code = "B09", Row = 1, Col = 8, Amount = 640, Product = 5, Cluster = 1 },
                new { Code = "B10", Row = 1, Col = 9, Amount = 480, Product = 0, Cluster = 1 },
                
                // Row C (C01-C10)
                new { Code = "C01", Row = 2, Col = 0, Amount = 980, Product = 1, Cluster = 2 },
                new { Code = "C02", Row = 2, Col = 1, Amount = 220, Product = 2, Cluster = 2 },
                new { Code = "C03", Row = 2, Col = 2, Amount = 350, Product = 3, Cluster = 2 },
                new { Code = "C04", Row = 2, Col = 3, Amount = 120, Product = 4, Cluster = 2 },
                new { Code = "C05", Row = 2, Col = 4, Amount = 280, Product = 5, Cluster = 2 },
                new { Code = "C06", Row = 2, Col = 5, Amount = 720, Product = 0, Cluster = 2 },
                new { Code = "C07", Row = 2, Col = 6, Amount = 650, Product = 1, Cluster = 2 },
                new { Code = "C08", Row = 2, Col = 7, Amount = 580, Product = 2, Cluster = 2 },
                new { Code = "C09", Row = 2, Col = 8, Amount = 940, Product = 3, Cluster = 2 },
                new { Code = "C10", Row = 2, Col = 9, Amount = 0, Product = -1, Cluster = 2 },
                
                // Row D (D01-D10)
                new { Code = "D01", Row = 3, Col = 0, Amount = 820, Product = 4, Cluster = 3 },
                new { Code = "D02", Row = 3, Col = 1, Amount = 560, Product = 5, Cluster = 3 },
                new { Code = "D03", Row = 3, Col = 2, Amount = 180, Product = 0, Cluster = 3 },
                new { Code = "D04", Row = 3, Col = 3, Amount = 740, Product = 1, Cluster = 3 },
                new { Code = "D05", Row = 3, Col = 4, Amount = 690, Product = 2, Cluster = 3 },
                new { Code = "D06", Row = 3, Col = 5, Amount = 520, Product = 3, Cluster = 3 },
                new { Code = "D07", Row = 3, Col = 6, Amount = 960, Product = 4, Cluster = 3 },
                new { Code = "D08", Row = 3, Col = 7, Amount = 480, Product = 5, Cluster = 3 },
                new { Code = "D09", Row = 3, Col = 8, Amount = 620, Product = 0, Cluster = 3 },
                new { Code = "D10", Row = 3, Col = 9, Amount = 320, Product = 1, Cluster = 3 }
            };

            foreach (var data in cellData)
            {
                var currentAmount = data.Amount;
                var productId = data.Product >= 0 ? products[data.Product].Id : (int?)null;
                var product = data.Product >= 0 ? products[data.Product] : null;
                var status = currentAmount == 0 ? "Empty" : (currentAmount >= 900 ? "Full" : "Occupied");
                var clusterName = clusterNames[data.Cluster];

                cells.Add(new WarehouseCell
                {
                    WarehouseId = warehouse.Id,
                    ZoneId = zone.Id,
                    CellCode = data.Code,
                    CellType = "Shelf",
                    Row = data.Row,
                    Column = data.Col,
                    MaxCapacity = 1000,
                    CurrentAmount = currentAmount,
                    ProductId = productId,
                    ProductName = product?.ProductName,
                    BatchNumber = currentAmount > 0 ? $"BATCH{DateTime.Now:yyyyMMdd}{data.Code}" : null,
                    LastMoved = currentAmount > 0 ? DateTime.UtcNow.AddDays(-random.Next(1, 30)) : null,
                    Status = status,
                    ClusterName = clusterName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.WarehouseCells.AddRange(cells);
            await _context.SaveChangesAsync();

            // Update zone current capacity
            zone.CurrentCapacity = cells.Count(c => c.CurrentAmount > 0);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Simple warehouse data created successfully", 
                warehouseId = warehouse.Id, 
                totalCells = cells.Count,
                occupiedCells = cells.Count(c => c.CurrentAmount > 0),
                emptyCells = cells.Count(c => c.CurrentAmount == 0),
                fullCells = cells.Count(c => c.CurrentAmount >= 900)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating simple warehouse data", error = ex.Message });
        }
    }

    /// <summary>
    /// Test database connection
    /// </summary>
    [HttpGet("test-db")]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            var userCount = await _context.Users.CountAsync();
            var warehouseCount = await _context.Warehouses.CountAsync();
            return Ok(new { 
                message = "Database connection successful", 
                userCount = userCount,
                warehouseCount = warehouseCount
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Database connection failed", error = ex.Message });
        }
    }

    /// <summary>
    /// Seed permissions data
    /// </summary>
    [HttpPost("permissions-data")]
    public async Task<IActionResult> SeedPermissionsData()
    {
        try
        {
            // Check if permissions already exist
            if (await _context.Permissions.AnyAsync())
            {
                return Ok(new { message = "Permissions already exist" });
            }

            var permissions = new List<Permission>
            {
                // Tổng quan
                new Permission { Name = "Xem tổng quan", Description = "Xem dashboard tổng quan", Category = "Tổng quan" },
                new Permission { Name = "Tùy chỉnh tổng quan", Description = "Tùy chỉnh giao diện dashboard", Category = "Tổng quan" },
                new Permission { Name = "Xem phân tích", Description = "Xem các biểu đồ phân tích", Category = "Tổng quan" },
                
                // Warehouse Management
                new Permission { Name = "Xem thông tin kho", Description = "Xem thông tin kho hàng", Category = "Quản lý kho" },
                new Permission { Name = "Chỉnh sửa kho", Description = "Chỉnh sửa thông tin kho", Category = "Quản lý kho" },
                new Permission { Name = "Xem chi tiết ô", Description = "Xem chi tiết từng ô trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Quản lý vị trí", Description = "Quản lý vị trí các ô trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Tùy chỉnh kho", Description = "Tùy chỉnh kích thước và layout kho", Category = "Quản lý kho" },
                new Permission { Name = "Xem sơ đồ 3D", Description = "Xem sơ đồ kho 3D", Category = "Quản lý kho" },
                new Permission { Name = "Quản lý cluster", Description = "Quản lý các cluster trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Quản lý zone", Description = "Quản lý các zone trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Quản lý cell", Description = "Quản lý các cell trong kho", Category = "Quản lý kho" },
                new Permission { Name = "Theo dõi hàng hóa", Description = "Theo dõi vị trí hàng hóa", Category = "Quản lý kho" },
                new Permission { Name = "Kiểm kê kho", Description = "Thực hiện kiểm kê kho", Category = "Quản lý kho" },
                new Permission { Name = "Báo cáo kho", Description = "Xem báo cáo tình trạng kho", Category = "Quản lý kho" },
                
                // Product Management
                new Permission { Name = "Xem sản phẩm", Description = "Xem danh sách sản phẩm", Category = "Sản phẩm" },
                new Permission { Name = "Tạo sản phẩm", Description = "Thêm sản phẩm mới", Category = "Sản phẩm" },
                new Permission { Name = "Chỉnh sửa sản phẩm", Description = "Cập nhật thông tin sản phẩm", Category = "Sản phẩm" },
                new Permission { Name = "Xóa sản phẩm", Description = "Xóa sản phẩm", Category = "Sản phẩm" },
                new Permission { Name = "Quản lý danh mục", Description = "Quản lý danh mục sản phẩm", Category = "Sản phẩm" },
                new Permission { Name = "Quản lý nhà cung cấp", Description = "Quản lý thông tin nhà cung cấp", Category = "Sản phẩm" },
                new Permission { Name = "Quản lý khách hàng", Description = "Quản lý thông tin khách hàng", Category = "Sản phẩm" },
                
                // User Management
                new Permission { Name = "Xem người dùng", Description = "Xem danh sách người dùng", Category = "Người dùng" },
                new Permission { Name = "Tạo người dùng", Description = "Thêm người dùng mới", Category = "Người dùng" },
                new Permission { Name = "Chỉnh sửa người dùng", Description = "Cập nhật thông tin người dùng", Category = "Người dùng" },
                new Permission { Name = "Xóa người dùng", Description = "Xóa người dùng", Category = "Người dùng" },
                new Permission { Name = "Quản lý vai trò", Description = "Phân quyền vai trò người dùng", Category = "Người dùng" },
                new Permission { Name = "Quản lý trạng thái", Description = "Quản lý trạng thái người dùng", Category = "Người dùng" },
                
                // Department Management
                new Permission { Name = "Xem phòng ban", Description = "Xem danh sách phòng ban", Category = "Phòng ban" },
                new Permission { Name = "Tạo phòng ban", Description = "Thêm phòng ban mới", Category = "Phòng ban" },
                new Permission { Name = "Chỉnh sửa phòng ban", Description = "Cập nhật thông tin phòng ban", Category = "Phòng ban" },
                new Permission { Name = "Xóa phòng ban", Description = "Xóa phòng ban", Category = "Phòng ban" },
                new Permission { Name = "Quản lý trưởng phòng", Description = "Phân công trưởng phòng", Category = "Phòng ban" },
                
                // Company Management
                new Permission { Name = "Xem công ty", Description = "Xem danh sách công ty", Category = "Công ty" },
                new Permission { Name = "Tạo công ty", Description = "Thêm công ty mới", Category = "Công ty" },
                new Permission { Name = "Chỉnh sửa công ty", Description = "Cập nhật thông tin công ty", Category = "Công ty" },
                new Permission { Name = "Xóa công ty", Description = "Xóa công ty", Category = "Công ty" },
                new Permission { Name = "Quản lý trạng thái công ty", Description = "Quản lý trạng thái hoạt động công ty", Category = "Công ty" },
                
                // Reports
                new Permission { Name = "Xem báo cáo", Description = "Xem các báo cáo thống kê", Category = "Báo cáo" },
                new Permission { Name = "Xuất báo cáo", Description = "Xuất báo cáo ra file", Category = "Báo cáo" },
                new Permission { Name = "Báo cáo kho", Description = "Xem báo cáo tình trạng kho", Category = "Báo cáo" },
                new Permission { Name = "Báo cáo sản phẩm", Description = "Xem báo cáo sản phẩm", Category = "Báo cáo" },
                new Permission { Name = "Báo cáo người dùng", Description = "Xem báo cáo người dùng", Category = "Báo cáo" },
                new Permission { Name = "Báo cáo tài chính", Description = "Xem báo cáo tài chính", Category = "Báo cáo" },
                
                // Admin
                new Permission { Name = "Truy cập Admin Panel", Description = "Truy cập trang quản trị", Category = "Quản trị" },
                new Permission { Name = "Cài đặt hệ thống", Description = "Cấu hình hệ thống", Category = "Quản trị" },
                new Permission { Name = "Quản lý backup", Description = "Quản lý sao lưu dữ liệu", Category = "Quản trị" },
                new Permission { Name = "Quản lý log", Description = "Xem và quản lý log hệ thống", Category = "Quản trị" },
                new Permission { Name = "Quản lý cấu hình", Description = "Cấu hình các tham số hệ thống", Category = "Quản trị" }
            };

            _context.Permissions.AddRange(permissions);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Permissions seeded successfully", count = permissions.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while seeding permissions", error = ex.Message });
        }
    }
}
