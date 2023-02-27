using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace App.Admin.User
{
    // phan biet chu hoa va thuong
    [Authorize(Roles = "admin,vip")] // 1 trong 2 deu duoc

    //[Authorize(Roles = "admin")] // phai co ca 2 admin va vip
    //[Authorize(Roles = "vip")] // phai co ca 2 admin va vip

    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager= userManager;
        }

        public class UserAndRole : AppUser
        {
            public string RoleNames { get; set; }
        }

        public List<UserAndRole> Users { get; set; }

        public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPages { get; set; }
        //  tongsotrang = tong_so_bai_viet / so_bai_viet_tren_mot_trang
        public int CountPage { get; set; }

        public int TotalUsers { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            
            var qr = _userManager.Users.OrderBy(user => user.UserName);
            TotalUsers = await qr.CountAsync();
            CountPage = (int)Math.Ceiling((double)TotalUsers / ITEMS_PER_PAGE);

            if (CurrentPages < 1) CurrentPages = 1;
            if (CurrentPages > CountPage) CurrentPages = CountPage;

            var qr1 = qr.Skip((CurrentPages - 1) * ITEMS_PER_PAGE)
                        .Take(ITEMS_PER_PAGE)
                        .Select(u => new UserAndRole()
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                        });

            Users = await qr1.ToListAsync();

            foreach(var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(", ", roles);
            }

            return Page();
        }
    }
}
