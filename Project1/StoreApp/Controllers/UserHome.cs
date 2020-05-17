using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using StoreApp.Data;

namespace StoreApp.Controllers
{
    [Authorize(Roles = Auth.Role.Customer)]
    public class UserHome : Controller
    {
        private StoreContext _context;
        private ILogger<UserHome> _logger;

        public UserHome(
            StoreContext context,
            ILogger<UserHome> logger
            )
        {
            this._context = context;
            this._logger = logger;
        }

        [Route("User/Home")]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(CartHeader.CartHeaderFilter))]
        public async Task<IActionResult> Home()
        {
            return View("Index");
        }
    }
}
