using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

using StoreApp.Data;

namespace StoreApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerHome : Controller
    {
        private StoreContext _context;
        private ILogger<HelloWorldController> _logger;

        public CustomerHome(
            StoreContext context,
            ILogger<HelloWorldController> logger
            )
        {
            this._context = context;
            this._logger = logger;
        }

        [Route("Customer/Home")]
        public async Task<string> Home()
        {
            return("this is the home page");
        }
    }
}
