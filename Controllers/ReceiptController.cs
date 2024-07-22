using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.Database;
using System.Threading.Tasks;
using System;
using SmartHome.Classes.Receipt;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Policy;
using Microsoft.Extensions.Configuration;
using SmartHome.Classes.SmartHome.Util;


namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReceiptController : Controller
    {
        private readonly IDatabaseWrapper db;
        private readonly IConfiguration _conf;
        public ReceiptController(IDatabaseWrapper _db, IConfiguration configuration)
        {
            db = _db;
            _conf = configuration;
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
                        try
                        {
                            var path = _conf["ReceiptImagesPath"];
                            var completepath = path + "\\" + v.Value;
                            if (System.IO.File.Exists(completepath))
                            {
                                System.IO.File.Delete(completepath);
                            }
                        }
                        catch(Exception ex)
                        {
                            SmartHomeConstants.log.ServerErrorsAdd("ReceiptController.ImageDelete:ImageID:" + v.UnitID, ex);
                            return 0;
                        }
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
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
        public async Task<Picture> UploadImage([FromForm] ImageUploadDTO imfile)
        {
            Picture pic = new();
            var path = _conf["ReceiptImagesPath"];
            var newimagename ="\\"+ imfile.ReceiptID + "_" + imfile.file.FileName;
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (imfile.file.Length > 0)
            {
                
                using (var stream = imfile.file.OpenReadStream())
                {
                    var image = Image.FromStream(stream);
                    int width = image.Width / 4;
                    int height = image.Height / 4;

                    Bitmap newSizeBitmap = new Bitmap(image, new Size(width, height));
                    using (var ms = new MemoryStream())
                    {
                        pic.ReceiptID = imfile.ReceiptID;
                        pic.SortOrder = "1";
                        pic.Image = imfile.ReceiptID + "_" + imfile.file.FileName;
                        try
                        {
                            newSizeBitmap.Save(path + newimagename);
                        }
                        catch(Exception ex)
                        {
                            SmartHomeConstants.log.ServerErrorsAdd("ReceiptController.UploadImage:RezeptID:" + imfile.ReceiptID, ex);
                        }
                        try
                        {
                            pic = await db.PictureAdd(pic);
                        }
                        catch(Exception ex)
                        {
                            SmartHomeConstants.log.ServerErrorsAdd("ReceiptController.AddImage:RezeptID:" + imfile.ReceiptID, ex);
                        }
                    }
                }


                
            }
            return pic;
        }
    }
}
