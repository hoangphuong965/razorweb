using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace App.Admin.User
{
    [Authorize(Roles = "admin,vip")]
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _useManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddRoleModel(UserManager<AppUser> useManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _useManager = useManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [TempData]
        public string StatusMessage { get; set; }

        public AppUser User { get; set; }

        [BindProperty]
        [DisplayName("Role gán cho user")]
        public string[] RoleNames { get; set; }

        public SelectList AllRoles { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }

            User = await _useManager.FindByIdAsync(id);

            if (User == null) 
            {
                return NotFound($"Không thấy user, id = {id}");   
            }

            RoleNames = (await _useManager.GetRolesAsync(User)).ToArray<string>();

            List<string> roleName = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(roleName);

            return Page();
        }

        public async Task<IActionResult> OnPost(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }

            User = await _useManager.FindByIdAsync(id);

            if (User == null)
            {
                return NotFound($"Không thấy user, id = {id}");
            }

            // RoleNames
            var oldRoleNames = (await _useManager.GetRolesAsync(User)).ToArray();
            
            var deleteRoles = oldRoleNames.Where(r => !RoleNames.Contains(r));
          
            var addRoles = RoleNames.Where(r => !oldRoleNames.Contains(r));
          
            List<string> roleName = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(roleName);

            // delete
            var resultDelete = await _useManager.RemoveFromRolesAsync(User, deleteRoles);
            if(!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            // add
            var resultAdd = await _useManager.AddToRolesAsync(User, addRoles);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"Vừa cập nhật role cho user: {User.UserName}";

            return RedirectToPage("./index");
        }
    }
}
