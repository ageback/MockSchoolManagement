using Microsoft.AspNetCore.Diagnostics;
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
            // 配合 app.UseStatusCodePagesWithReExecute("/Error/{0}") 获取url地址和参数
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "抱歉，用户访问的页面不存在";
                    ViewBag.Path = statusCodeResult.OriginalPath;
                    ViewBag.QS = statusCodeResult.OriginalQueryString;
                    break;
            }
            return View("NotFound");
        }
    }
}
