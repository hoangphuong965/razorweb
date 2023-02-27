using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Areas.Admin.Pages.Role;
using WebApplication1.Models;

namespace App.Admin.Role
{
    [Authorize(Roles = "admin,vip")]
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) 
            : base(roleManager, myBlogContext)
        {
        }

        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            Role = await _roleManager.FindByIdAsync(roleid);
            if (Role == null) return NotFound("Không tìm thấy role");
            return Page();
        }

        public async Task<IActionResult> OnPost(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            Role = await _roleManager.FindByIdAsync(roleid);
            if (Role == null) return NotFound("Không tìm thấy role");

            var result = await _roleManager.DeleteAsync(Role);

            if(result.Succeeded)
            {
                StatusMessage = $"Bạn vừa xóa role: {Role.Name}";
                return RedirectToPage("./index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
