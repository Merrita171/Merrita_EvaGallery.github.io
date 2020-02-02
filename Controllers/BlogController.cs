using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using EvaGallery.Models;

namespace EvaGallery.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index( string filter)
        {
            ViewBag.User = filter;
            // Read the list
            var blogs = PostManager.Read();
            if (blogs == null)
            {
                ViewBag.Empty = true;
                return View();
            }
            else
            {
                // Just for sorting.
                blogs = (from blog in blogs
                         where filter == null || (blog.User == filter)
                         orderby blog.CreateTime descending
                         select blog).ToList();

                ViewBag.Empty = false;
                return View(blogs);
            }
        }

        [Route("blog/read/{id}")] // Set the ID parameter
        public ActionResult Read(int id)
        {
            // Read one single blog
            var blogs = PostManager.Read();
            UserPostModel post = null;

            if (blogs != null && blogs.Count > 0)
            {
                post = blogs.Find(x => x.ID == id);
            }

            if (post == null)
            {
                ViewBag.PostFound = false;
                return View();
            }
            else
            {
                ViewBag.PostFound = true;
                return View(post);
            }
        }

        public ActionResult Create(string filter)
        {

            ViewBag.User = filter;
            if (Request.HttpMethod == "POST")
            {
                // Post request method
                var title = Request.Form["title"].ToString();
                var tags = Request.Form["tags"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var content = Request.Form["content"].ToString();
                var User = filter;
                // Save content
                var post = new UserPostModel { Title = title, CreateTime = DateTime.Now, Content = content, Tags = tags.ToList() ,User=User};
                PostManager.Create(JsonConvert.SerializeObject(post));

                // Redirect
                return RedirectToAction("Index", "Blog", new { filter = filter });
                // return RedirectToAction("Index");
            }
            return View();
        }

        [Route("blog/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (Request.HttpMethod == "POST")
            {
                // Post request method
                var title = Request.Form["title"].ToString();
                var tags = Request.Form["tags"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var content = Request.Form["content"].ToString();
                var User = Request.Form["User"].ToString();
                // Save content
                var post = new UserPostModel { Title = title, CreateTime = DateTime.Now, Content = content, Tags = tags.ToList(), User = User };
                PostManager.Update(id, JsonConvert.SerializeObject(post));

                return RedirectToAction("Index", "Blog", new { filter = User });
            }
            else
            {
                // Find the required post.
                var post = PostManager.Read().Find(x => x.ID == id);

                if (post != null)
                {
                    // Set the values
                    ViewBag.Found = true;
                    ViewBag.PostTitle = post.Title;
                    ViewBag.Tags = post.Tags;
                    ViewBag.Content = post.Content;
                    ViewBag.User = post.User;
                }
                else
                {
                    ViewBag.Found = false;
                }
            }

            // Finally return the view.
            return View();
        }
    }
}