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
        Task<long> AddReceipt(string receiptName);
        Task<Boolean> UpdateReceipt(long receiptId, ReceiptUpdateDTO ru);
        Task<Boolean> UpdateReceiptCategoryAdd(long receiptId, ReceiptUpdateDTO ru);
        Task<Boolean> UpdateReceiptCategoryDelete(long receiptId, ReceiptUpdateDTO ru);
        Task<long> UpdateReceiptIngridientUnitAdd(long receiptId, ReceiptUpdateDTO ru);
        Task<Boolean> UpdateReceiptIngridientUnitDelete(ReceiptUpdateDTO ru);
        Task<Boolean> UpdateReceiptIngridientUnitUpdate(ReceiptUpdateDTO ru);
        Task<Boolean> UpdateReceiptImageSortOrder(ReceiptUpdateDTO ru);
        Task<Boolean> UpdateReceiptImageDelete(ReceiptUpdateDTO ru);
        Task<Picture> PictureAdd(Picture pic);
    }
}