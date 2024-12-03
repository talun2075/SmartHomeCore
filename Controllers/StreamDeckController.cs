using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.SmartHome.Interfaces;
using SmartHome.Classes.SmartHome.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StreamDeckController : ControllerBase
    {
        ISmartHomeWrapper shw;
        public StreamDeckController(IWebHostEnvironment env, ISmartHomeWrapper _shw)
        {
            SmartHomeConstants.Env = env;
            shw = _shw;
        }
        [HttpGet("Date")]
        public string Date()
        {
            return DateTime.Now.ToString("dd.MM");
        }
        [HttpGet("Hour/{right}")]
        public string Hour(Boolean right)
        {
            var min = DateTime.Now.ToString("HH");
            
            int imgkey = 0;
            if (!right)
            {
                if (min.StartsWith("0"))
                {
                    imgkey = 0;
                }
                if (min.StartsWith("1"))
                {
                    imgkey = 1;
                }
                if (min.StartsWith("2"))
                {
                    imgkey = 2;
                }
            }
            else
            {
                if (min.EndsWith("0"))
                {
                    imgkey = 0;
                }
                if (min.EndsWith("1"))
                {
                    imgkey = 1;
                }
                if (min.EndsWith("2"))
                {
                    imgkey = 2;
                }
                if (min.EndsWith("3"))
                {
                    imgkey = 3;
                }
                if (min.EndsWith("4"))
                {
                    imgkey = 4;
                }
                if (min.EndsWith("5"))
                {
                    imgkey = 5;
                }
                if (min.EndsWith("6"))
                {
                    imgkey = 6;
                }
                if (min.EndsWith("7"))
                {
                    imgkey = 7;
                }
                if (min.EndsWith("8"))
                {
                    imgkey = 8;
                }
                if (min.EndsWith("9"))
                {
                    imgkey = 9;
                }
            }
            String retval = "http://" + this.Request.Host.Host + "/images/"+imgkey+".png";
            return retval;
            
        }
        [HttpGet("Minute/{right}")]
        public string Minute(Boolean right)
        {
            var min = DateTime.Now.ToString("mm");

            int imgkey = 0;
            if (!right)
            {
                if (min.StartsWith("0"))
                {
                    imgkey = 0;
                }
                if (min.StartsWith("1"))
                {
                    imgkey = 1;
                }
                if (min.StartsWith("2"))
                {
                    imgkey = 2;
                }
                if (min.StartsWith("3"))
                {
                    imgkey = 3;
                }
                if (min.StartsWith("4"))
                {
                    imgkey = 4;
                }
                if (min.StartsWith("5"))
                {
                    imgkey = 5;
                }
            }
            else
            {
                if (min.EndsWith("0"))
                {
                    imgkey = 0;
                }
                if (min.EndsWith("1"))
                {
                    imgkey = 1;
                }
                if (min.EndsWith("2"))
                {
                    imgkey = 2;
                }
                if (min.EndsWith("3"))
                {
                    imgkey = 3;
                }
                if (min.EndsWith("4"))
                {
                    imgkey = 4;
                }
                if (min.EndsWith("5"))
                {
                    imgkey = 5;
                }
                if (min.EndsWith("6"))
                {
                    imgkey = 6;
                }
                if (min.EndsWith("7"))
                {
                    imgkey = 7;
                }
                if (min.EndsWith("8"))
                {
                    imgkey = 8;
                }
                if (min.EndsWith("9"))
                {
                    imgkey = 9;
                }
            }
            String retval = "http://" + this.Request.Host.Host + "/images/" + imgkey + ".png";
            return retval;

        }
    }


}
