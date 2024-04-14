using SmartHome.Classes.Receipt;
using SmartHome.Classes.SmartHome.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHome.Classes.Database
{
    public interface IDatabaseWrapper
    {
        Task<List<IngredientDTOBase>> ReadIngrediensData();
        Task<List<CategoryDTO>> ReadCategoriesData();
        Task<List<ReceiptDTO>> ReadReceiptList();
        Task<bool> ReadButtons();
        Task<bool> UpdateButton(Button button);
    }
}