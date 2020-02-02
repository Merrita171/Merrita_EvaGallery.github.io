using EvaGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EvaGallery.Controllers
{
    public class BlogPostController : Controller
    {
        public ActionResult BlogIndex(int page = 1)
        {
            // Read the list
            var blogs = PostManager.Read();
            if (blogs == null)
            {
                ViewBag.Empty = true;
                return View();
            }
            else
            {

                var blogsView = new BlogModels
                {
                    BlogPerPage = 5,
                    Blogs = (from blog in blogs
                             orderby blog.CreateTime descending
                             select blog).ToList(),

                    CurrentPage = page
                };

                return View(blogsView);
            }
        }
    }
}