namespace BE_PRN232.Model;

public enum OrderStatus
{
    Pending = 0,     // Chờ xử lý
    Confirmed = 1,   // Đã xác nhận
    Shipped = 2,     // Đã giao hàng
    Delivered = 3,   // Đã nhận hàng
    Cancelled = 4    // Đã hủy
}