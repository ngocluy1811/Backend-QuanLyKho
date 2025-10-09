namespace FertilizerWarehouseAPI.DTOs
{
    public class CalculateExportPricesRequest
    {
        public List<ProductQuantity> Products { get; set; } = new List<ProductQuantity>();
    }

    public class ProductQuantity
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
