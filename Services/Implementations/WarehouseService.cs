using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Services.Interfaces;
using AutoMapper;

namespace FertilizerWarehouseAPI.Services.Implementations;

public class WarehouseService : IWarehouseService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<WarehouseService> _logger;

    public WarehouseService(ApplicationDbContext context, IMapper mapper, ILogger<WarehouseService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
    {
        var warehouses = await _context.Warehouses
            .Include(w => w.Company)
            .Where(w => w.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
    {
        var warehouse = await _context.Warehouses
            .Include(w => w.Company)
            .FirstOrDefaultAsync(w => w.Id == id && w.IsActive);

        return warehouse != null ? _mapper.Map<WarehouseDto>(warehouse) : null;
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto createDto)
    {
        var warehouse = _mapper.Map<Warehouse>(createDto);
        warehouse.CreatedAt = DateTime.UtcNow;
        warehouse.IsActive = true;

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Warehouse created: {WarehouseName}", warehouse.Name);
        return _mapper.Map<WarehouseDto>(warehouse);
    }

    public async Task<WarehouseDto?> UpdateWarehouseAsync(int id, UpdateWarehouseDto updateDto)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null || !warehouse.IsActive) return null;

        _mapper.Map(updateDto, warehouse);
        warehouse.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Warehouse updated: {WarehouseName}", warehouse.Name);
        return _mapper.Map<WarehouseDto>(warehouse);
    }

    public async Task<bool> DeleteWarehouseAsync(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null) return false;

        warehouse.IsActive = false;
        warehouse.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Warehouse deleted: {WarehouseName}", warehouse.Name);
        return true;
    }

    public async Task<IEnumerable<WarehouseCellDto>> GetWarehouseCellsAsync(int warehouseId)
    {
        var cells = await _context.WarehouseCells
            .Where(c => c.WarehouseId == warehouseId && c.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<WarehouseCellDto>>(cells);
    }

    public async Task<WarehouseCellDto?> GetWarehouseCellByIdAsync(int warehouseId, int cellId)
    {
        var cell = await _context.WarehouseCells
            .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId && c.IsActive);

        return cell != null ? _mapper.Map<WarehouseCellDto>(cell) : null;
    }

    public async Task<bool> UpdateWarehouseCellAsync(int warehouseId, int cellId, WarehouseCellDto cellDto)
    {
        var cell = await _context.WarehouseCells
            .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId && c.IsActive);

        if (cell == null) return false;

        _mapper.Map(cellDto, cell);
        cell.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MoveCellAsync(int warehouseId, int sourceCellId, int targetCellId)
    {
        // Implementation for moving cell
        return true;
    }

    public async Task<bool> TransferGoodsAsync(int warehouseId, WarehouseTransferDto transferDto)
    {
        // Implementation for goods transfer
        return true;
    }

    public async Task<bool> EmptyCellAsync(int warehouseId, int cellId)
    {
        var cell = await _context.WarehouseCells
            .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == cellId && c.IsActive);

        if (cell == null) return false;

        cell.CurrentAmount = 0;
        cell.ProductId = null;
        cell.ProductName = null;
        cell.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<WarehouseClusterDto>> GetWarehouseClustersAsync(int warehouseId)
    {
        var clusters = await _context.WarehouseClusters
            .Where(c => c.WarehouseId == warehouseId && c.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<WarehouseClusterDto>>(clusters);
    }

    public async Task<WarehouseClusterDto?> GetWarehouseClusterByIdAsync(int warehouseId, string clusterId)
    {
        var cluster = await _context.WarehouseClusters
            .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == clusterId && c.IsActive);

        return cluster != null ? _mapper.Map<WarehouseClusterDto>(cluster) : null;
    }

    public async Task<WarehouseClusterDto> CreateWarehouseClusterAsync(int warehouseId, CreateWarehouseClusterDto createDto)
    {
        var cluster = _mapper.Map<WarehouseCluster>(createDto);
        cluster.WarehouseId = warehouseId;
        cluster.CreatedAt = DateTime.UtcNow;
        cluster.IsActive = true;

        _context.WarehouseClusters.Add(cluster);
        await _context.SaveChangesAsync();

        return _mapper.Map<WarehouseClusterDto>(cluster);
    }

    public async Task<WarehouseClusterDto?> UpdateWarehouseClusterAsync(int warehouseId, string clusterId, UpdateWarehouseClusterDto updateDto)
    {
        var cluster = await _context.WarehouseClusters
            .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == clusterId && c.IsActive);

        if (cluster == null) return null;

        _mapper.Map(updateDto, cluster);
        cluster.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<WarehouseClusterDto>(cluster);
    }

    public async Task<bool> DeleteWarehouseClusterAsync(int warehouseId, string clusterId)
    {
        var cluster = await _context.WarehouseClusters
            .FirstOrDefaultAsync(c => c.WarehouseId == warehouseId && c.Id == clusterId && c.IsActive);

        if (cluster == null) return false;

        cluster.IsActive = false;
        cluster.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<WarehouseConfigDto?> GetWarehouseConfigAsync(int warehouseId)
    {
        // Implementation for warehouse configuration
        return new WarehouseConfigDto();
    }

    public async Task<WarehouseConfigDto> UpdateWarehouseConfigAsync(int warehouseId, WarehouseConfigDto configDto)
    {
        // Implementation for updating warehouse configuration
        return new WarehouseConfigDto();
    }

    public async Task<bool> UpdateGridSizeAsync(int warehouseId, int rows, int columns)
    {
        // Implementation for updating grid size
        return true;
    }

    public async Task<WarehouseStatsDto> GetWarehouseStatsAsync(int warehouseId)
    {
        // Implementation for warehouse statistics
        return new WarehouseStatsDto();
    }

    public async Task<IEnumerable<WarehouseActivityDto>> GetWarehouseActivitiesAsync(int warehouseId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        // Implementation for warehouse activities
        return new List<WarehouseActivityDto>();
    }

    public async Task<IEnumerable<WarehouseCellDto>> SearchWarehouseCellsAsync(int warehouseId, string searchQuery)
    {
        // Implementation for searching warehouse cells
        return new List<WarehouseCellDto>();
    }

    public async Task<IEnumerable<WarehouseCellDto>> FilterWarehouseCellsAsync(int warehouseId, string? status = null, string? product = null, string? capacity = null)
    {
        // Implementation for filtering warehouse cells
        return new List<WarehouseCellDto>();
    }

    public async Task<bool> ImportWarehouseDataAsync(int warehouseId, Stream fileStream)
    {
        // Implementation for importing warehouse data
        return true;
    }

    public async Task<Stream> ExportWarehouseDataAsync(int warehouseId, string format = "excel")
    {
        // Implementation for exporting warehouse data
        return new MemoryStream();
    }

    public async Task<bool> ScheduleMaintenanceAsync(int warehouseId, DateTime scheduledDate, string description)
    {
        // Implementation for scheduling maintenance
        return true;
    }

    public async Task<IEnumerable<MaintenanceTaskDto>> GetMaintenanceTasksAsync(int warehouseId)
    {
        // Implementation for getting maintenance tasks
        return new List<MaintenanceTaskDto>();
    }

    public async Task<bool> CompleteMaintenanceTaskAsync(int warehouseId, int taskId)
    {
        // Implementation for completing maintenance task
        return true;
    }
}
