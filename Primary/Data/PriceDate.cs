namespace Primary.Data
{
    /// <summary>
    /// Price and date data.
    /// </summary>
    public class PriceDate : Date, IPrice
    {
        public decimal Price { get; set; }
    }
}
