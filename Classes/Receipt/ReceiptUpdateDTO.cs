namespace SmartHome.Classes.Receipt
{
    public class ReceiptUpdateDTO
    {
        public ReceiptUpdateType Type { get; set; }
        public string  Value { get; set; }
        public long UnitID { get; set; } = 0;
        public long UnitID2 { get; set; } = 0;
    }
}
