namespace inventory_app.Pages.Model
{
    public class InventoryViewModel
    {
        public List<StockDto> StockItems { get; set; } = new();
        public List<StockSummaryDto> StockSummary { get; set; } = new();
        public string? SearchTerm { get; set; }
        public bool ShowSummary { get; set; }
    }
}
