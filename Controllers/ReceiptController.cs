using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.Database;
using System.Threading.Tasks;
using System;
using SmartHome.Classes.Receipt;
using System.Collections.Generic;
using System.IO;


namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReceiptController : Controller
    {
        private readonly IDatabaseWrapper db;
        public ReceiptController(IDatabaseWrapper _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {

            ViewBag.Title = "Rezepte";
            ViewBag.png = "topf.png";
            ViewBag.svg = "topf.svg";
            ViewBag.png16 = "topf16.png";
            ViewBag.png32 = "topf32.png";
            ViewBag.NavClass = "navSeven";
            return View();
        }
        [HttpGet("GetCategories")]
        public async Task<List<CategoryDTO>> GetCategories()
        {
            return await db.ReadCategoriesData();
        }
        [HttpGet("GetUnits")]
        public async Task<List<UnitDTO>> GetUnits()
        {
            return await db.ReadUnitsData();
        }
        [HttpGet("GetIngrediens")]
        public async Task<List<IngredientDTOBase>> GetIngrediens()
        {
            return await db.ReadIngrediensData();
        }
        [HttpGet("GetList")]
        public async Task<List<ReceiptDTO>> GetList()
        {
            return await db.ReadReceiptList();
        }
        [HttpPost("AddCategory")]
        public async Task<Boolean> AddCategory([FromBody] string v)
        {
            return await db.AddCategory(v);
        }
        [HttpPost("AddReceipt")]
        public async Task<long> AddReceipt([FromBody] string v)
        {
            return await db.AddReceipt(v);
        }
        [HttpPost("AddUnit")]
        public async Task<Boolean> AddUnit([FromBody] string v)
        {
            return await db.AddUnit(v);
        }
        [HttpPost("AddIngredient")]
        public async Task<Boolean> AddIngredient([FromBody] string v)
        {
            return await db.AddIngredient(v);
        }
        [HttpPost("UpdateCategory/{id}")]
        public async Task<Boolean> UpdateCategory(int id, [FromBody] string v)
        {
            return await db.UpdateCategory(new CategoryDTO { Category = v, ID = id });
        }
        [HttpPost("UpdateUnit/{id}")]
        public async Task<Boolean> UpdateUnit(int id, [FromBody] string v)
        {
            return await db.UpdateUnit(id, v);
        }
        [HttpPost("UpdateIngredient/{id}")]
        public async Task<Boolean> UpdateIngredient(int id, [FromBody] string v)
        {
            return await db.UpdateIngredient(new IngredientDTOBase { ID = id, Ingredient = v });
        }
        [HttpPost("UpdateReceipt/{id}")]
        public async Task<long> UpdateReceipt(int id, [FromBody] ReceiptUpdateDTO v)
        {
            switch (v.Type)
            {
                case ReceiptUpdateType.Duration:
                case ReceiptUpdateType.Description:
                case ReceiptUpdateType.RestTime:
                case ReceiptUpdateType.ResTimeUnit:
                case ReceiptUpdateType.Title:
                    return await db.UpdateReceipt(id,v) == true ? 1 : 0;
                case ReceiptUpdateType.ImageSortOrder:
                    return await db.UpdateReceiptImageSortOrder(v) == true ? 1 : 0;
                case ReceiptUpdateType.ImageDelete:
                    if(await db.UpdateReceiptImageDelete(v))
                    {
                        //todo: delete image from path. v.value
                    }
                    break;
                case ReceiptUpdateType.IngridientUnitDelete:
                    return await db.UpdateReceiptIngridientUnitDelete(v) == true ? 1 : 0;
                case ReceiptUpdateType.IngridientUnitUpdate:
                    return await db.UpdateReceiptIngridientUnitUpdate(v) == true ? 1 : 0;
                case ReceiptUpdateType.IngridientUnitADD:
                    return await db.UpdateReceiptIngridientUnitAdd(id,v);
                case ReceiptUpdateType.CategoryDelete:
                    return await db.UpdateReceiptCategoryDelete(id, v) == true ? 1 : 0;
                case ReceiptUpdateType.CategoryAdd:
                    return await db.UpdateReceiptCategoryAdd(id,v)==true ? 1 : 0;
            }
            throw new NotImplementedException("Wrong ReceiptUpdateDTO Type send");
        }
        [HttpPost("UploadImage/{id}")]
        public async Task<long> UploadImage(int id, [FromForm] IFormFile imfile)
        {

            //foreach (var formFile in files)
            //{
            //    if (formFile.Length > 0)
            //    {
            //        var filePath = Path.GetTempFileName();

            //        using (var stream = System.IO.File.Create(filePath))
            //        {
            //            await formFile.CopyToAsync(stream);
            //        }
            //    }
            //}

            return 0;
        }

    }
}
