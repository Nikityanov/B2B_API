namespace B2B_API.Models.Enums
{
    /// <summary>
    /// Статусы заказов в системе
    /// </summary>
    public enum OrderStatus
    {
        Pending = 1,    // Ожидает обработки
        Processing = 2, // В обработке
        Shipped = 3,    // Отправлен
        Delivered = 4,  // Доставлен
        Cancelled = 5   // Отменен
    }
}
