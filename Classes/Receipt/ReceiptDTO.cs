using System.Collections.Generic;

namespace SmartHome.Classes.Receipt
{
    public class ReceiptDTO
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public string Duration { get; set; }
        public DownTime RestTime { get; set; } = new();
        public List<Picture> Pictures { get; set; }
        public List<IngredientDTO> Ingredients { get; set; } = new();
        public List<CategoryDTO> Categories { get; set; } = new();

    }
}
