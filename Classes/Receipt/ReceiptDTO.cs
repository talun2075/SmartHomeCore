﻿using System.Collections.Generic;

namespace SmartHome.Classes.Receipt
{
    public class ReceiptDTO
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; } = string.Empty;
        public string Duration { get; set; }
        public DownTime RestTime { get; set; } = new();
        public List<Picture> Pictures { get; set; } = new();
        public List<IngredientPerReceipt> Ingredients { get; set; } = new();
        public List<CategoryDTO> Categories { get; set; } = new();

    }
}
