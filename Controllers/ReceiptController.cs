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


    }
}
