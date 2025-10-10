using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class UpdateProductBatchRequest
    {
        public string? BatchName { get; set; }
        public string? Description { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
        public int? QualityStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime? NgayVe { get; set; }
        public string? SoDotVe { get; set; }
        public string? SoXeContainerTungDot { get; set; }
        public string? NgayVeDetails { get; set; }
    }
}
