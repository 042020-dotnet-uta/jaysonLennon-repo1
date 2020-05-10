using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using MvcPractice2.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MvcPractice2.Controllers
{
    public class WelcomeViewModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }
        private int _id;
        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }
        public WelcomeViewModel(string name, int id)
        {
            if (string.IsNullOrEmpty(name))
            {
                this.Name = "No name";
            }
            else
            {
                this.Name = name;
            }
            this.Id = id;
        }
    }

    public class HelloWorldController : Controller
    {
        private MvcPractice2Context _context;
        private ILogger<HelloWorldController> _logger;

        public HelloWorldController(MvcPractice2Context context, ILogger<HelloWorldController> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        [Route("HelloWorld/db")]
        public async Task<string> DbAccess()
        {
            var m = _context.Movie.Select(m => m);
            foreach(var mo in await m.ToListAsync()) {
                this._logger.LogCritical($"m00 : {mo.Title}");
            }
            this._logger.LogCritical($"m00vies : {m}");
            return HtmlEncoder.Default.Encode("check console");
        }

        [Route("HelloWorld")]
        [Route("HelloWorld/Index")]
        public string Index()
        {
            return HtmlEncoder.Default.Encode("suip");
        }

        // 
        // GET: /HelloWorld/Welcome/ 
        [Route("HelloWorld/Welcome")]
        [Route("HelloWorld/Welcome/{name}")]
        [Route("HelloWorld/Welcome/{name}/{id}")]
        public IActionResult Welcome(string name, int id = 1)
        {
            WelcomeViewModel vd = new WelcomeViewModel(name, id);
            return View("Index", vd);
        }
    }
}
