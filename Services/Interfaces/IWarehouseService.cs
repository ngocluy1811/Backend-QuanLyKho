using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Services.Interfaces
{
    public interface IWarehouseService
    {
        // Warehouse CRUD operations
        Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
        Task<WarehouseDto?> GetWarehouseByIdAsync(int id);
        Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto createDto);
        Task<WarehouseDto?> UpdateWarehouseAsync(int id, UpdateWarehouseDto updateDto);
        Task<bool> DeleteWarehouseAsync(int id);

        // Warehouse cells management
        Task<IEnumerable<WarehouseCellDto>> GetWarehouseCellsAsync(int warehouseId);
        Task<WarehouseCellDto?> GetWarehouseCellByIdAsync(int warehouseId, int cellId);
        Task<bool> UpdateWarehouseCellAsync(int warehouseId, int cellId, WarehouseCellDto cellDto);
        Task<bool> MoveCellAsync(int warehouseId, int sourceCellId, int targetCellId);
        Task<bool> TransferGoodsAsync(int warehouseId, WarehouseTransferDto transferDto);
        Task<bool> EmptyCellAsync(int warehouseId, int cellId);

        // Warehouse clusters management
        Task<IEnumerable<WarehouseClusterDto>> GetWarehouseClustersAsync(int warehouseId);
        Task<WarehouseClusterDto?> GetWarehouseClusterByIdAsync(int warehouseId, string clusterId);
        Task<WarehouseClusterDto> CreateWarehouseClusterAsync(int warehouseId, CreateWarehouseClusterDto createDto);
        Task<WarehouseClusterDto?> UpdateWarehouseClusterAsync(int warehouseId, string clusterId, UpdateWarehouseClusterDto updateDto);
        Task<bool> DeleteWarehouseClusterAsync(int warehouseId, string clusterId);

        // Warehouse configuration
        Task<WarehouseConfigDto?> GetWarehouseConfigAsync(int warehouseId);
        Task<WarehouseConfigDto> UpdateWarehouseConfigAsync(int warehouseId, WarehouseConfigDto configDto);
        Task<bool> UpdateGridSizeAsync(int warehouseId, int rows, int columns);

        // Warehouse statistics and reporting
        Task<WarehouseStatsDto> GetWarehouseStatsAsync(int warehouseId);
        Task<IEnumerable<WarehouseActivityDto>> GetWarehouseActivitiesAsync(int warehouseId, DateTime? fromDate = null, DateTime? toDate = null);

        // Warehouse search and filtering
        Task<IEnumerable<WarehouseCellDto>> SearchWarehouseCellsAsync(int warehouseId, string searchQuery);
        Task<IEnumerable<WarehouseCellDto>> FilterWarehouseCellsAsync(int warehouseId, string? status = null, string? product = null, string? capacity = null);

        // Warehouse import/export operations
        Task<bool> ImportWarehouseDataAsync(int warehouseId, Stream fileStream);
        Task<Stream> ExportWarehouseDataAsync(int warehouseId, string format = "excel");

        // Warehouse maintenance
        Task<bool> ScheduleMaintenanceAsync(int warehouseId, DateTime scheduledDate, string description);
        Task<IEnumerable<MaintenanceTaskDto>> GetMaintenanceTasksAsync(int warehouseId);
        Task<bool> CompleteMaintenanceTaskAsync(int warehouseId, int taskId);
    }
}
