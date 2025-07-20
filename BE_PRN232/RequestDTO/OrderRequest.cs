namespace BE_PRN232.RequestDTO;

public class OrderRequest
{
    public string UserId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;

    public decimal DiscountAmount { get; set; } = decimal.Zero;
    
    public decimal ShippingFee { get; set; } = decimal.Zero;

    public int? VoucherId { get; set; }
    
    public string? Notes { get; set; } = String.Empty;
    public List<int> cartItemIds { get; set; } = new List<int>();
}