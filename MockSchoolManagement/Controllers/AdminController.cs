using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

        [HttpGet]
        public IActionResult CreateRole() => View();

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole { Name = model.RoleName };
                IdentityResult result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                    return RedirectToAction("index", "home");

                foreach (IdentityError err in result.Errors)
                    ModelState.AddModelError("", err.Description);
            }

            return View(model);
        }
    }
}
