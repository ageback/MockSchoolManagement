using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    [Route("Error/{statusCode}")]
    public class ErrorController : Controller
    {
       public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "抱歉，用户访问的页面不存在";
                    break;
            }
            return View("NotFound");
        }
    }
}
