using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using StoreApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace StoreApp.Controllers
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
        private StoreContext _context;
        private ILogger<HelloWorldController> _logger;
        private Repository.IProduct _productRepository;

        public HelloWorldController(
            StoreContext context,
            ILogger<HelloWorldController> logger,
            Repository.IProduct productRepository
            )
        {
            this._context = context;
            this._logger = logger;
            this._productRepository = productRepository;
        }

        [Route("HelloWorld/db")]
        public async Task<string> DbAccess()
        {
            var customers = _context.Customers.Select(c => c);
            foreach(var c in await customers.ToListAsync()) {
                this._logger.LogCritical($"customer : {c.FirstName}");
            }
            return HtmlEncoder.Default.Encode("check console");
        }

        [Route("HelloWorld/vo")]
        public async Task<IActionResult> ViewObjectTest()
        {
            var customers = _context.Customers.Select(c => c);
            var first = await customers.FirstAsync();
            var vm = new WelcomeViewModel(first.FirstName, 1);
            return View("Customer", vm);
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

        [Route("HelloWorld/Authed")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AuthorizedPage()
        {
            return View("AuthorizedPage");
        }
    }
}
