using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.Database;
using System.Threading.Tasks;
using System;
using SmartHome.Classes.Receipt;
using System.Collections.Generic;

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
            return await db.UpdateCategory(new CategoryDTO { Category=v,ID = id});
        }
        [HttpPost("UpdateUnit/{id}")]
        public async Task<Boolean> UpdateUnit(int id, [FromBody] string v)
        {
            return await db.UpdateUnit(id,v);
        }
        [HttpPost("UpdateIngredient/{id}")]
        public async Task<Boolean> UpdateIngredient(int id, [FromBody] string v)
        {
            return await db.UpdateIngredient(new IngredientDTOBase { ID=id, Ingredient = v});
        }
    }
}
