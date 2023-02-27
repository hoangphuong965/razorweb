using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Areas.Admin.Pages.Role;
using WebApplication1.Models;

namespace App.Admin.Role
{
    [Authorize(Roles = "admin,vip")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) 
            : base(roleManager, myBlogContext)
        {
        }

        public class InputModel
        {
            [Display(Name = "Tên role")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            Role = await _roleManager.FindByIdAsync(roleid);
            
            if(Role != null) 
            {
                Input = new InputModel() 
                { 
                    Name = Role.Name, 
                };

                return Page();
            }
            return NotFound("Không tìm thấy role");
        }

        public async Task<IActionResult> OnPost(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            Role = await _roleManager.FindByIdAsync(roleid);
            if (Role == null) return NotFound("Không tìm thấy role");
            if (!ModelState.IsValid) return Page();

            Role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(Role);

            if(result.Succeeded) 
            {
                StatusMessage = $"Bạn vừa đổi tên: {Input.Name}";
                return RedirectToPage("./Index");
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
