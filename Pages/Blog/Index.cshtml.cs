using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Pages_Blog
{
    public class IndexModel : PageModel
    {
        private readonly WebApplication1.Models.MyBlogContext _context;

        public IndexModel(WebApplication1.Models.MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; }

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPages { get; set; }

        //  tongsotrang = tong_so_bai_viet / so_bai_viet_tren_mot_trang
        public int CountPage { get; set; }

        public async Task OnGetAsync(string SearchString)
        {
            int totalArticle = await _context.Articles.CountAsync();
            CountPage = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);

            if (CurrentPages < 1) CurrentPages = 1;
            if (CurrentPages > CountPage) CurrentPages = CountPage;

            var qr = (from a in _context.Articles
                     orderby a.Created descending
                     select a)
                     .Skip((CurrentPages - 1) * ITEMS_PER_PAGE)
                     .Take(ITEMS_PER_PAGE);

            if(!string.IsNullOrEmpty(SearchString))
            {
                Article = qr.Where(a => a.Title.Contains(SearchString)).ToList();
            }
            else
            {
                Article = await qr.ToListAsync();
            }
        }
    }
}
