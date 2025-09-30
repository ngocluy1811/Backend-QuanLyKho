using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.DTOs;
using FertilizerWarehouseAPI.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/warehouse
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetWarehouses()
        {
            var warehouses = await _context.Warehouses
                .Where(w => w.IsActive)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    Width = w.Width,
                    Height = w.Height,
                    IsActive = w.IsActive,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();

            return Ok(warehouses);
        }

        // GET: api/warehouse/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseDto>> GetWarehouse(int id)
        {
            var warehouse = await _context.Warehouses
                .Where(w => w.Id == id && w.IsActive)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    Width = w.Width,
                    Height = w.Height,
                    IsActive = w.IsActive,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(warehouse);
        }

        // POST: api/warehouse
        [HttpPost]
        public async Task<ActionResult<WarehouseDto>> CreateWarehouse(CreateWarehouseDto createDto)
        {
            var warehouse = new Warehouse
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Width = createDto.Width,
                Height = createDto.Height,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            // Create cells for the warehouse
            await CreateWarehouseCells(warehouse.Id, warehouse.Width, warehouse.Height);

            var warehouseDto = new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Description = warehouse.Description,
                Width = warehouse.Width,
                Height = warehouse.Height,
                IsActive = warehouse.IsActive,
                CreatedAt = warehouse.CreatedAt,
                UpdatedAt = warehouse.UpdatedAt
            };

            return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, warehouseDto);
        }

        // GET: api/warehouse/{id}/positions
        [HttpGet("{id}/positions")]
        public async Task<ActionResult<IEnumerable<WarehousePositionDto>>> GetWarehousePositions(int id)
        {
            var positions = await _context.WarehouseCells
                .Where(p => p.WarehouseId == id)
                .Select(p => new WarehousePositionDto
                {
                    Id = p.Id,
                    WarehouseId = p.WarehouseId,
                    Row = p.Row,
                    Column = p.Column,
                    ProductName = p.ProductName,
                    BatchNumber = p.BatchNumber,
                    MaxCapacity = p.MaxCapacity,
                    CurrentAmount = p.CurrentAmount,
                    Status = p.Status,
                    ClusterName = p.ClusterName,
                    LastMoved = p.LastMoved
                })
                .ToListAsync();

            return Ok(positions);
        }

        // GET: api/warehouse/{id}/stats
        [HttpGet("{id}/stats")]
        public async Task<ActionResult<object>> GetWarehouseStats(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            var positions = await _context.WarehouseCells
                .Where(p => p.WarehouseId == id)
                .ToListAsync();

            var totalPositions = positions.Count;
            var positionsWithGoods = positions.Count(p => p.CurrentAmount > 0);
            var fullPositions = positions.Count(p => p.Status == "Full");
            var averageCapacity = totalPositions > 0 ? (int)positions.Average(p => (double)p.CurrentAmount / p.MaxCapacity * 100) : 0;
            var warnings = positions.Count(p => p.Status == "Full" || p.CurrentAmount > p.MaxCapacity * 0.9);

            return Ok(new
            {
                TotalPositions = totalPositions,
                PositionsWithGoods = positionsWithGoods,
                FullPositions = fullPositions,
                AverageCapacity = averageCapacity,
                Warnings = warnings,
                EmptyPositions = totalPositions - positionsWithGoods
            });
        }

        private async System.Threading.Tasks.Task CreateWarehouseCells(int warehouseId, int width, int height)
        {
            var cells = new List<WarehouseCell>();

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    cells.Add(new WarehouseCell
                    {
                        WarehouseId = warehouseId,
                        Row = row,
                        Column = col,
                        CellCode = $"{row}-{col}",
                        MaxCapacity = 100,
                        CurrentAmount = 0,
                        Status = "Empty",
                        LastMoved = DateTime.UtcNow
                    });
                }
            }

            _context.WarehouseCells.AddRange(cells);
            await _context.SaveChangesAsync();
        }
    }
}
