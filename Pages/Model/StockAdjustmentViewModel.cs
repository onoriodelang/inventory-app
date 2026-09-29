namespace inventory_app.Pages.Model
{
    public class StockAdjustmentViewModel
    {

        public string ItemSku { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public decimal CurrentQuantity { get; set; }
        public decimal NewQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
