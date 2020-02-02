using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EvaGallery.Models
{
    public class BlogModels
    {
        public IEnumerable<UserPostModel> Blogs { get; set; }
        public IEnumerable<ImagePostModel> Album { get; set; }
        public int BlogPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(Blogs.Count() / (double)BlogPerPage));
        }
        public IEnumerable<UserPostModel> PaginatedBlogs()
        {
            int start = (CurrentPage - 1) * BlogPerPage;
            return Blogs.OrderBy(b => b.ID).Skip(start).Take(BlogPerPage);
        }
    }
}