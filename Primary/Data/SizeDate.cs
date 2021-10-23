namespace Primary.Data
{
    /// <summary>
    /// Size and date data.
    /// </summary>
    public class SizeDate : Date, ISize
    {
        public decimal Size { get; set; }
    }
}
