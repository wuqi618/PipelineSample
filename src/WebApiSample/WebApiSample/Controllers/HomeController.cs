using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApiSample.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public string Index()
        {
            var hostname = Environment.GetEnvironmentVariable("HOSTNAME");
            return $"WebApiSample-{hostname}";
        }
    }
}
