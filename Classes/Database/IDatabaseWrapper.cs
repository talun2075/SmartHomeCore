using SmartHome.Classes.Receipt;
using SmartHome.Classes.SmartHome.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHome.Classes.Database
{
    public interface IDatabaseWrapper
    {
        Task<List<IngredientDTOBase>> ReadIngrediensData();
        Task<List<CategoryDTO>> ReadCategoriesData();
        Task<List<ReceiptDTO>> ReadReceiptList();
        Task<List<UnitDTO>> ReadUnitsData();
        Task<bool> ReadButtons();
        Task<bool> UpdateButton(Button button);
        Task<Boolean> AddUnit(string unit);
        Task<Boolean> AddCategory(string categoryName);
        Task<Boolean> AddIngredient(string ingridient);
        Task<Boolean> UpdateCategory(CategoryDTO category);
        Task<Boolean> UpdateIngredient(IngredientDTOBase ingredient);
        Task<Boolean> UpdateUnit(int id, string unit);
    }
}