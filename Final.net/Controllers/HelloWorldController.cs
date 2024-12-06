using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace Final.net.Controllers;

public class HelloWorldController : Microsoft.AspNetCore.Mvc.Controller
{
    // 
    // GET: /HelloWorld/

    // public string Index()
    // {
    //     return "This is my default action...";
    // }


    public IActionResult Index()
    {
        return View();
    }
    // GET: /HelloWorld/Welcome/ 
    // public string Welcome()
    // {
    //     return "This is the Welcome action method...";
    // }

    public string Welcome(string name, int numTimes = 1)
    {
        return HtmlEncoder.Default.Encode($"Hello {name}, NumTimes is: {numTimes}");
    }
}