﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MockSchoolManagement.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger) => this.logger = logger;
        [Route("Error/{statusCode}")]
        [HttpGet]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            // 配合 app.UseStatusCodePagesWithReExecute("/Error/{0}") 获取url地址和参数
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    logger.LogWarning($"发生了一个404错误。路径 = {statusCodeResult.OriginalPath} 以及查询字符串 = {statusCodeResult.OriginalQueryString}");
                    //ViewBag.ErrorMessage = "抱歉，用户访问的页面不存在";
                    //ViewBag.Path = statusCodeResult.OriginalPath;
                    //ViewBag.QS = statusCodeResult.OriginalQueryString;
                    break;
            }
            return View("NotFound");
        }

        [Route("Error")]
        [HttpGet]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError($"路径{exceptionHandlerPathFeature.Path} 产生了一个错误{exceptionHandlerPathFeature.Error}");
            return View("Error");
        }
    }
} 