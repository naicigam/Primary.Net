namespace Primary.Data
{
    /// <summary>
    /// Size and price data.
    /// </summary>
    public class PriceSize : IPrice, ISize
    {
        public decimal Size { get; set; }
        public decimal Price { get; set; }
    }
}
