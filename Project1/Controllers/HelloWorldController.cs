using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using MvcPractice2.Data;

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
        public HelloWorldController(MvcPractice2Context context, ILogger<HelloWorldController> logger)
        {
            logger.LogCritical($"GETGOT LOGGER: {logger}");
            logger.LogCritical($"GETGOT context: {context}");
        }

        // 
        // GET: /HelloWorld/
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
