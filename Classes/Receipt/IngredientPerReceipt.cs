namespace SmartHome.Classes.Receipt
{
    public class IngredientPerReceipt : IngredientDTOBase
    {
        public string Unit { get; set; }
        public string Amount { get; set; }
        public int IngredientUnitID { get; set; }
    }
}
