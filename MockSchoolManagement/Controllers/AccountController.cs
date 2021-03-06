﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AdminController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "电子邮箱未验证");
                    return View(model);
                }
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
                if (result.IsLockedOut) return View("AccountLocked");
                ModelState.AddModelError(string.Empty, "登录失败，请重试");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"第三方登录提供程序错误：{remoteError}");
                return View("Login", model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "加载第三方登录信息出错。");
                return View("Login", model);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;
            if (email != null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "电子邮箱未验证");
                    return View("Login", model);
                }
            }

            // 使用已经登录过的信息
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        // 创建没有密码的新用户
                        await _userManager.CreateAsync(user);
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        _logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.ErrorTitle = "注册成功";
                        ViewBag.ErrorMessage = "在您登入系统前，我们已经给您发了一封邮件，需要您先进行邮箱验证。单击确认链接即可完成。";
                        return View("Error");
                    }
                    //在AspNetUserLogins表中添加一行用户数据，然后将当前用户登录到系统中
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                ViewBag.ErrorTitle = $"我们无法从提供商：{info.LoginProvider}中解析到用户的邮件地址。";
                ViewBag.ErrorMessage = "请通过联系xxx@yyy.zzz寻求技术支持。";
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    // 用日志记录生成的链接
                    _logger.Log(LogLevel.Warning, confirmationLink);
                    // 如果用户已登录且为Admin角色，那么就是Admin正在创建新用户，所以重定向Admin用户到ListUsers视图。
                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Admin");
                    }
                    ViewBag.ErrorTitle = "注册成功";
                    ViewBag.ErrorMessage = "在您登入系统前，我们已经给您发了一封邮件，需要您先进行邮箱验证。单击确认链接即可完成。";
                    return View("Error");
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("index", "home");
                }
                foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"邮箱：{email} 已经被注册过了。");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"当前 {userId} 无效";
                return View("NotFound");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorMessage = "您的电子邮箱还未验证";
            return View("Error");
        }

        [HttpGet]
        public IActionResult ActivateUserEmail() => View();

        [HttpPost]
        public async Task<IActionResult> ActivateUserEmail(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        // 用日志记录生成的链接
                        _logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.Message = "如果您在我们系统有注册账号，我们已经发了邮件到您的邮箱中，请前往邮箱激活您的账号。";
                        return View("ActivateUserEmailConfirmation", ViewBag.Message);
                    }
                }
            }
            ViewBag.Message = "请确认邮箱是否存在异常，现在我们无法给您发送激活链接。";
            return View("ActivateUserEmailConfirmation", ViewBag.Message);
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);
                    _logger.Log(LogLevel.Warning, passwordResetLink);
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "无效的密码重置令牌");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    //重置用户密码
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        // 重置密码后立即解锁账号
                        if(await _userManager.IsLockedOutAsync(user))
                        {
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword() {
            var user = await _userManager.GetUserAsync(User);
            var userHasPassword = await _userManager.HasPasswordAsync(user);
            if (!userHasPassword) return RedirectToAction("AddPassword");
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach(var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            var userHasPassword = await _userManager.HasPasswordAsync(user);
            if (userHasPassword) return RedirectToAction("ChangePassword");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
                return View("AddPasswordConfirmation");
            }
            return View(model);
        }
    }
}