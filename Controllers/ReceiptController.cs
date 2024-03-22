using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.Database;
using System.Threading.Tasks;
using System;
using SmartHome.Classes.Receipt;

namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IDatabaseWrapper db;
        public ReceiptController(IDatabaseWrapper _db)
        {
            db = _db;
        }
        [HttpGet("Get/{id}")]
        public async Task<ReceiptDTO> Get(int id)
        {
            return await db.ReadReceiptData(id);
        }

        //todo: Lesen der Werte. Alles als ein Objekt rausgeben. 
        //todo: Schreiben von Kategorien, etc. 

    }
}
