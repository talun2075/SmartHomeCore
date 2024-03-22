namespace SmartHome.Classes.Receipt
{
    public class IngredientDTO
    {
        public int ID { get; set; }
        public string Ingredient { get; set; }
        public string Unit { get; set; }
        public string Amount { get; set; }
        public int IngredientUnitID { get; set; }
    }
}
