using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.DTOs;
using System.Security.Claims;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductionController : ControllerBase
    {
        /// <summary>
        /// Get production machines with their current status
        /// </summary>
        [HttpGet("machines")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<ProductionMachineDto>>> GetMachines()
        {
            try
            {
                // Mock machine data
                var machines = new List<ProductionMachineDto>
                {
                    new ProductionMachineDto
                    {
                        Id = 1,
                        Name = "Máy trộn 1",
                        Status = "running",
                        Product = "NPK 16-16-8",
                        Progress = 68,
                        StartTime = DateTime.Today.AddHours(8.5),
                        EndTime = DateTime.Today.AddHours(15.5),
                        Batch = "LSX-2023-0045",
                        Operator = "Nguyễn Văn A",
                        Details = "Công suất: 5 tấn/mẻ, RPM: 30, Nhiệt độ: 65°C, Độ ẩm: 45%, Thời gian trộn: 15 phút",
                        LastUpdated = DateTime.UtcNow.AddMinutes(-5)
                    },
                    new ProductionMachineDto
                    {
                        Id = 2,
                        Name = "Máy trộn 2",
                        Status = "idle",
                        Product = null,
                        Progress = 0,
                        StartTime = null,
                        EndTime = null,
                        Batch = null,
                        Operator = null,
                        Details = "Công suất: 5 tấn/mẻ, RPM: 30, Nhiệt độ: -, Độ ẩm: -, Thời gian trộn: -",
                        LastUpdated = DateTime.UtcNow.AddMinutes(-2)
                    },
                    new ProductionMachineDto
                    {
                        Id = 3,
                        Name = "Máy đóng gói 1",
                        Status = "maintenance",
                        Product = null,
                        Progress = 0,
                        StartTime = null,
                        EndTime = null,
                        Batch = null,
                        Operator = "Trần Văn B",
                        Details = "Tốc độ đóng gói: 60 bao/giờ, Trọng lượng bao: 50 kg, Sản lượng hiện tại: 0, Mục tiêu: 300 bao, Loại bảo trì: Bảo trì định kỳ",
                        LastUpdated = DateTime.UtcNow.AddMinutes(-10)
                    },
                    new ProductionMachineDto
                    {
                        Id = 4,
                        Name = "Máy đóng gói 2",
                        Status = "error",
                        Product = "Ure",
                        Progress = 45,
                        StartTime = DateTime.Today.AddHours(9.25),
                        EndTime = null,
                        Batch = "LSX-2023-0044",
                        Operator = "Lê Thị C",
                        Details = "Tốc độ đóng gói: 60 bao/giờ, Trọng lượng bao: 50 kg, Sản lượng hiện tại: 135 bao, Mục tiêu: 300 bao, Mã lỗi: E-104, Thông báo: Nhiệt độ vượt ngưỡng an toàn",
                        LastUpdated = DateTime.UtcNow.AddMinutes(-1)
                    }
                };

                return Ok(machines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get production machine by ID
        /// </summary>
        [HttpGet("machines/{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<ProductionMachineDto>> GetMachineById(int id)
        {
            try
            {
                // Mock machine data
                var machine = new ProductionMachineDto
                {
                    Id = id,
                    Name = $"Máy {id}",
                    Status = "running",
                    Product = "NPK 16-16-8",
                    Progress = 68,
                    StartTime = DateTime.Today.AddHours(8.5),
                    EndTime = DateTime.Today.AddHours(15.5),
                    Batch = "LSX-2023-0045",
                    Operator = "Nguyễn Văn A",
                    Details = "Công suất: 5 tấn/mẻ, RPM: 30, Nhiệt độ: 65°C, Độ ẩm: 45%, Thời gian trộn: 15 phút",
                    LastUpdated = DateTime.UtcNow.AddMinutes(-5)
                };

                return Ok(machine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Control production machine (start, stop, pause, reset)
        /// </summary>
        [HttpPost("machines/{id}/control")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<ProductionMachineDto>> ControlMachine(int id, [FromBody] MachineControlDto controlDto)
        {
            try
            {
                if (controlDto.MachineId != id)
                {
                    return BadRequest(new { message = "Machine ID mismatch" });
                }

                var validActions = new[] { "start", "stop", "pause", "reset" };
                if (!validActions.Contains(controlDto.Action.ToLower()))
                {
                    return BadRequest(new { message = "Invalid action. Valid actions: start, stop, pause, reset" });
                }

                // Mock control action
                var newStatus = controlDto.Action.ToLower() switch
                {
                    "start" => "running",
                    "stop" => "idle",
                    "pause" => "idle",
                    "reset" => "idle",
                    _ => "idle"
                };

                var machine = new ProductionMachineDto
                {
                    Id = id,
                    Name = $"Máy {id}",
                    Status = newStatus,
                    Product = newStatus == "running" ? "NPK 16-16-8" : null,
                    Progress = newStatus == "running" ? 0 : 0,
                    StartTime = newStatus == "running" ? DateTime.Now : null,
                    EndTime = null,
                    Batch = newStatus == "running" ? $"LSX-2023-{new Random().Next(1000, 9999):D4}" : null,
                    Operator = "Current User",
                    Details = $"Công suất: 5 tấn/mẻ, RPM: {(newStatus == "running" ? "30 RPM" : "0 RPM")}, Nhiệt độ: {(newStatus == "running" ? "65°C" : "-")}, Độ ẩm: {(newStatus == "running" ? "45%" : "-")}, Thời gian trộn: {(newStatus == "running" ? "15 phút" : "-")}",
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(machine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get production orders
        /// </summary>
        [HttpGet("orders")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<ProductionOrderDto>>> GetProductionOrders(
            [FromQuery] string? status = null,
            [FromQuery] string? priority = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                // Mock production orders
                var orders = new List<ProductionOrderDto>
                {
                    new ProductionOrderDto
                    {
                        Id = 1,
                        OrderNumber = "LSX-2023-0045",
                        Product = null, // TODO: Create ProductDto
                        Quantity = 2500m,
                        StartDate = DateTime.Today,
                        Status = "running",
                        Priority = "high",
                        AssignedTo = 1,
                        CompletionRate = 68,
                        Machine = null, // TODO: Create MachineDto
                        Formula = "NPK-16-16-8",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new ProductionOrderDto
                    {
                        Id = 2,
                        OrderNumber = "LSX-2023-0046",
                        Product = null, // TODO: Create ProductDto
                        Quantity = 3000m,
                        StartDate = DateTime.Today,
                        Status = "running",
                        Priority = "medium",
                        AssignedTo = 2,
                        CompletionRate = 30,
                        Machine = null, // TODO: Create MachineDto
                        Formula = "NPK-20-20-15",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new ProductionOrderDto
                    {
                        Id = 3,
                        OrderNumber = "LSX-2023-0044",
                        Product = null, // TODO: Create ProductDto
                        Quantity = 1800m,
                        StartDate = DateTime.Today.AddDays(-1),
                        Status = "error",
                        Priority = "high",
                        AssignedTo = 3,
                        CompletionRate = 45,
                        Machine = null, // TODO: Create MachineDto
                        Formula = "URE-COATING-45",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    }
                };

                // Apply filters
                if (!string.IsNullOrEmpty(status))
                {
                    orders = orders.Where(o => o.Status == status).ToList();
                }

                if (!string.IsNullOrEmpty(priority))
                {
                    orders = orders.Where(o => o.Priority == priority).ToList();
                }

                // Apply pagination
                var pagedOrders = orders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(pagedOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new production order
        /// </summary>
        [HttpPost("orders")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<ProductionOrderDto>> CreateProductionOrder([FromBody] CreateProductionOrderDto createDto)
        {
            try
            {
                // Validate order number uniqueness (mock validation)
                // In real implementation, check database for duplicate

                // Mock creation
                var orderDto = new ProductionOrderDto
                {
                    Id = new Random().Next(1000, 9999),
                    OrderNumber = createDto.OrderNumber,
                    Product = null, // TODO: Create ProductDto from string
                    Quantity = createDto.Quantity,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    Status = "pending",
                    Priority = createDto.Priority,
                    AssignedTo = createDto.AssignedTo,
                    CompletionRate = 0,
                    Machine = null, // TODO: Create MachineDto from string
                    Formula = createDto.Formula,
                    Notes = createDto.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                return CreatedAtAction(nameof(GetProductionOrderById), new { id = orderDto.Id }, orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get production order by ID
        /// </summary>
        [HttpGet("orders/{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<ProductionOrderDto>> GetProductionOrderById(int id)
        {
            try
            {
                // Mock order data
                var order = new ProductionOrderDto
                {
                    Id = id,
                    OrderNumber = $"LSX-2023-{id:D4}",
                    Product = null, // TODO: Create ProductDto
                    Quantity = 2500m,
                    StartDate = DateTime.Today,
                    Status = "running",
                    Priority = "high",
                    AssignedTo = 1,
                    CompletionRate = 68,
                    Machine = null, // TODO: Create MachineDto
                    Formula = "NPK-16-16-8",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                };

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update production order
        /// </summary>
        [HttpPut("orders/{id}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<ProductionOrderDto>> UpdateProductionOrder(int id, [FromBody] UpdateProductionOrderDto updateDto)
        {
            try
            {
                // Mock update
                var updatedOrder = new ProductionOrderDto
                {
                    Id = id,
                    OrderNumber = updateDto.OrderNumber ?? $"LSX-2023-{id:D4}",
                    Product = null, // TODO: Convert string to ProductDto
                    Quantity = updateDto.Quantity ?? 2500m,
                    StartDate = updateDto.StartDate ?? DateTime.Today,
                    EndDate = updateDto.EndDate ?? DateTime.Today.AddDays(7),
                    Status = updateDto.Status ?? "pending",
                    Priority = updateDto.Priority ?? "medium",
                    AssignedTo = updateDto.AssignedTo,
                    CompletionRate = updateDto.CompletionRate ?? 0,
                    Machine = null, // TODO: Convert string to MachineDto
                    Formula = updateDto.Formula,
                    Notes = updateDto.Notes,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete production order
        /// </summary>
        [HttpDelete("orders/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteProductionOrder(int id)
        {
            try
            {
                // Mock deletion
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get production formulas
        /// </summary>
        [HttpGet("formulas")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<ProductionFormulaDto>>> GetFormulas()
        {
            try
            {
                // Mock formula data
                var formulas = new List<ProductionFormulaDto>
                {
                    new ProductionFormulaDto
                    {
                        Id = 1,
                        FormulaId = 1,
                        Name = "NPK 16-16-8",
                        Components = new List<FormulaComponentDto>
                        {
                            new FormulaComponentDto { Name = "Ure", Percentage = 34.5m, Amount = 862.5m },
                            new FormulaComponentDto { Name = "DAP", Percentage = 34.8m, Amount = 870m },
                            new FormulaComponentDto { Name = "KCl", Percentage = 13.2m, Amount = 330m },
                            new FormulaComponentDto { Name = "Phụ gia", Percentage = 17.5m, Amount = 437.5m }
                        },
                        ProductionRate = 15m,
                        Status = "active",
                        LastUsed = DateTime.UtcNow.AddHours(-2),
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new ProductionFormulaDto
                    {
                        Id = 2,
                        FormulaId = 2,
                        Name = "NPK 20-20-15",
                        Components = new List<FormulaComponentDto>
                        {
                            new FormulaComponentDto { Name = "Ure", Percentage = 43.5m, Amount = 1087.5m },
                            new FormulaComponentDto { Name = "DAP", Percentage = 43.5m, Amount = 1087.5m },
                            new FormulaComponentDto { Name = "KCl", Percentage = 9.0m, Amount = 225m },
                            new FormulaComponentDto { Name = "Phụ gia", Percentage = 4.0m, Amount = 100m }
                        },
                        ProductionRate = 15m,
                        Status = "active",
                        LastUsed = DateTime.UtcNow.AddHours(-5),
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    }
                };

                return Ok(formulas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new production formula
        /// </summary>
        [HttpPost("formulas")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<ProductionFormulaDto>> CreateFormula([FromBody] CreateProductionFormulaDto createDto)
        {
            try
            {
                // Validate total percentage equals 100
                var totalPercentage = createDto.Components.Sum(c => c.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.1m)
                {
                    return BadRequest(new { message = "Total component percentage must equal 100%" });
                }

                // Mock creation
                var formulaDto = new ProductionFormulaDto
                {
                    Id = new Random().Next(1000, 9999),
                    FormulaId = int.Parse(createDto.FormulaId ?? "1"),
                    Name = createDto.Name,
                    Components = createDto.Components.Select(c => new FormulaComponentDto
                    {
                        Name = c.Name,
                        Percentage = c.Percentage,
                        Amount = c.Amount
                    }).ToList(),
                    ProductionRate = createDto.ProductionRate,
                    Status = createDto.Status,
                    CreatedAt = DateTime.UtcNow
                };

                return CreatedAtAction(nameof(GetFormulaById), new { id = formulaDto.Id }, formulaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get production formula by ID
        /// </summary>
        [HttpGet("formulas/{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<ProductionFormulaDto>> GetFormulaById(int id)
        {
            try
            {
                // Mock formula data
                var formula = new ProductionFormulaDto
                {
                    Id = id,
                    FormulaId = id,
                    Name = $"Formula {id}",
                    Components = new List<FormulaComponentDto>
                    {
                        new FormulaComponentDto { Name = "Ure", Percentage = 34.5m, Amount = 862.5m },
                        new FormulaComponentDto { Name = "DAP", Percentage = 34.8m, Amount = 870m },
                        new FormulaComponentDto { Name = "KCl", Percentage = 13.2m, Amount = 330m },
                        new FormulaComponentDto { Name = "Phụ gia", Percentage = 17.5m, Amount = 437.5m }
                    },
                    ProductionRate = 15m,
                    Status = "active",
                    LastUsed = DateTime.UtcNow.AddHours(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                };

                return Ok(formula);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get material inventory for production
        /// </summary>
        [HttpGet("materials")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<MaterialInventoryDto>>> GetMaterialInventory()
        {
            try
            {
                // Mock material data
                var materials = new List<MaterialInventoryDto>
                {
                    new MaterialInventoryDto
                    {
                        Id = 1,
                        MaterialId = 1,
                        Name = "Urê 46% N",
                        Stock = 95m,
                        Purity = 99m,
                        NitrogenContent = 46.3m,
                        Location = "Kho A (A031)",
                        Status = "sufficient",
                        LastChecked = DateTime.Today.AddHours(8),
                        Supplier = "Công ty Phân bón Cà Mau"
                    },
                    new MaterialInventoryDto
                    {
                        Id = 2,
                        MaterialId = 2,
                        Name = "DAP 18-46",
                        Stock = 75m,
                        Purity = 98m,
                        P2O5Content = 46.1m,
                        Location = "Kho B (B052)",
                        Status = "sufficient",
                        LastChecked = DateTime.Today.AddHours(8),
                        Supplier = "Công ty DAP Đình Vũ"
                    },
                    new MaterialInventoryDto
                    {
                        Id = 3,
                        MaterialId = 3,
                        Name = "KCl 60% K2O",
                        Stock = 45m,
                        Purity = 97m,
                        K2OContent = 60.5m,
                        Location = "Kho A (A045)",
                        Status = "warning",
                        LastChecked = DateTime.Today.AddHours(8),
                        Supplier = "Công ty TNHH Kali Phú Mỹ"
                    },
                    new MaterialInventoryDto
                    {
                        Id = 4,
                        MaterialId = 4,
                        Name = "Phụ gia & Chất kết dính",
                        Stock = 12m,
                        Location = "Kho C (C018)",
                        Status = "low",
                        LastChecked = DateTime.Today.AddHours(8),
                        Supplier = "Công ty Hóa chất Đức Giang",
                        Components = new List<MaterialComponentDto>
                        {
                            new MaterialComponentDto { Name = "Bentonite", Amount = 12m },
                            new MaterialComponentDto { Name = "Coating", Amount = 5m },
                            new MaterialComponentDto { Name = "Chất màu", Amount = 0.8m }
                        }
                    }
                };

                return Ok(materials);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get production history
        /// </summary>
        [HttpGet("history")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<ProductionHistoryDto>>> GetProductionHistory(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                // Mock history data
                var history = new List<ProductionHistoryDto>
                {
                    new ProductionHistoryDto
                    {
                        Id = 1,
                        BatchId = "1",
                        Product = "NPK 20-20-15",
                        Quantity = 148.5m,
                        Timeframe = "08/05/2023 06:00 - 14:00",
                        Quality = "Đạt chuẩn",
                        Location = "Kho A (A031)",
                        Operator = "Nguyễn Văn A",
                        Supervisor = "Trần Minh Quân",
                        ProductionDate = DateTime.Parse("2023-05-08")
                    },
                    new ProductionHistoryDto
                    {
                        Id = 2,
                        BatchId = "2",
                        Product = "NPK 16-16-16",
                        Quantity = 175m,
                        Timeframe = "07/05/2023 22:00 - 06:00",
                        Quality = "Đạt chuẩn",
                        Location = "Kho B (B052)",
                        Operator = "Phạm Văn D",
                        Supervisor = "Trần Minh Quân",
                        ProductionDate = DateTime.Parse("2023-05-07")
                    },
                    new ProductionHistoryDto
                    {
                        Id = 3,
                        BatchId = "3",
                        Product = "Urê Coating",
                        Quantity = 118m,
                        Timeframe = "07/05/2023 14:00 - 22:00",
                        Quality = "Cần kiểm",
                        Location = "Kho C (C083)",
                        Operator = "Lê Thị C",
                        Supervisor = "Nguyễn Thị Hương",
                        ProductionDate = DateTime.Parse("2023-05-07")
                    }
                };

                // Apply date filters
                if (startDate.HasValue)
                {
                    history = history.Where(h => h.ProductionDate >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    history = history.Where(h => h.ProductionDate <= endDate.Value).ToList();
                }

                // Apply pagination
                var pagedHistory = history
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(pagedHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get production statistics
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<ProductionStatsDto>> GetProductionStats()
        {
            try
            {
                // Mock statistics
                var stats = new ProductionStatsDto
                {
                    RunningMachines = 3,
                    TotalMachines = 6,
                    ActiveOrders = 2,
                    MaterialAlerts = 3,
                    TodayProduction = 42.5m,
                    OverallEfficiency = 87.5m,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
