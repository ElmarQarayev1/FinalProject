using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Medical.UI.Models;
using Medical.UI.Filter;

namespace Medical.UI.Controllers;

[ServiceFilter(typeof(AuthFilter))]
public class HomeController : Controller
{
    

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }

   
}

