﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace SmartHome.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ResetController : Controller
    {
        readonly IHostApplicationLifetime applicationLifetime;

        public ResetController(IHostApplicationLifetime appLifetime)
        {
            applicationLifetime = appLifetime;
        }
        [HttpGet("")]
        public bool Reset()
        {
            applicationLifetime.StopApplication();
            return true;
        }
    }
}
