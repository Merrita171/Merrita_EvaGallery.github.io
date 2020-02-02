using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EvaGallery.Models;
using EvaGallery.DAL;
using System.Drawing;
using System.Net;
using Newtonsoft.Json;
using EvaGallery.Repository;
using PagedList;

namespace EvaGallery.Controllers
{
    
    public class HomeController : Controller
    {
        GalleryRepository galRepo = new GalleryRepository();
        // Index page
        public ActionResult Index(int? page,string filter = null)
        {
            // Read the list
            var blogs = ImageManager.Read();
            if (blogs == null)
            {
                ViewBag.Empty = true;
                return View();
            }
            else
            {
                // Just for sorting.
                // blogs = (from blog in blogs
                //   orderby blog.PhotoId descending
                // select blog).ToList();

                ////// ViewBag.Empty = false;
                //  return View(blogs);
                // }
                //  var records = new Paged<ImagePostModel>();
                var records = new List<ImagePostModel>();
                ViewBag.filter = filter;
                records= (from blog in blogs where filter == null || (blog.Album.Contains(filter))
                                   orderby blog.PhotoId descending
                                   select blog).ToList();
                // records.Content = db.Photos
                ////  .Where(x => filter == null || (x.Description.Contains(filter)))
                //.OrderByDescending(x => x.PhotoId)
                //.Skip((page - 1) * pageSize)
                //.Take(pageSize)
                //.ToList();

                // Count
              int count = (from blog in blogs
                                       orderby blog.PhotoId descending
                                        select blog).Count();

               // records.CurrentPage = page;
               // records.PageSize = 5;
                int pageSize = 3;
                int pageNumber = (page ?? 1);
                return View(records.ToPagedList(pageNumber, pageSize));
               // return View(records);
            }
        }
        // Album page
        public ActionResult Account(int? page, string filter = null)
        {
           using (var client = new WebClient())
           {
                var text = client.DownloadString("http://jsonplaceholder.typicode.com/posts/1");
                Details det = JsonConvert.DeserializeObject<Details>(text);
            }
            var records = new List<ImagePostModel>();
            records = galRepo.getAlbums(filter);
            ViewBag.filter = filter;
            if (records == null)
            {
                ViewBag.Empty = true;
                return View();
            }
            else
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(records.ToPagedList(pageNumber, pageSize));
            }
        }
        // Search result page
        public ActionResult searchResult(int? page, string filter = null)
        {
            var records = new List<ImagePostModel>();
            records = galRepo.getAlbums(filter);
            ViewBag.filter = filter;
            if (records == null)
            {
                ViewBag.Empty = true;
                return View();
            }
            else
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(records.ToPagedList(pageNumber, pageSize));
            }
        }
        // Gallery search result page
        public ActionResult GallerySearch(string filter)
        {
            ViewBag.User = filter;
            var User = new ImagePostModel();
            var blogs = ImageManager.Read();
            var userBlog = ImageManager.Read();
            List <ImagePostModel> usr = new List<ImagePostModel>();
            if (blogs == null)
            {
                ViewBag.Empty = true;
                return View();
            }
            else
            {
                User = (from blog in blogs where blog.Title == filter select blog).Take(1).SingleOrDefault();
                if(User==null )
                {
                    usr = (from blog in blogs where blog.UserName == filter select blog).ToList();
                    if (usr.Count > 0)
                    {
                        return RedirectToAction("searchResult", "Home", new { filter = filter });
                    }
                    else
                    {
                        ViewBag.Empty = true;
                        return View();
                    }
                }
                else
                {
                    return View(User);
                }

            }
            
        }
        public ActionResult Error()
        {
            ViewBag.Message = "User is not Found !";

            return View();
        }
        // User details page
        public ActionResult UserDetails(string filter)
        {
            ViewBag.User = filter;
            var User = new Users();
            var blogs = UserManager.Read();
            if (blogs == null)
            {
                ViewBag.Empty = true;
                return View();
            }
            else
            {
                User = (from blog in blogs where blog.Name == filter  select blog).SingleOrDefault();

            }
           
           return View(User);
        }
        // Craete album page
        [HttpGet]
        public ActionResult Create(string filter=null)
        {
            var photo = new ImagePostModel();
            if(filter == null)
            {  }
            else { photo.UserName = filter; }
           
            return View(photo);
        }
        [HttpPost]
        public ActionResult Create(ImagePostModel photo, IEnumerable<HttpPostedFileBase> files)
        {
            var ThumbPath = "";
            var ImagePath = "";

            var model = new ImagePostModel();
            var records = new Paged<ImagePostModel>();
            if (Request.HttpMethod == "POST")
            {

                // Save content


                if (!ModelState.IsValid)
                    return View(photo);
                if (files.Count() == 0 || files.FirstOrDefault() == null)
                {
                    ViewBag.error = "Please Choose a fi9le to upload !";
                    return View(photo);
                }
                // int typeid= photo.PhotoId;
                
                foreach (var file in files)
                {
                    if (file.ContentLength == 0) continue;
                    var UserName = photo.UserName;
                    var title = photo.Title;
                    var fileName = Guid.NewGuid().ToString();
                    var albumTitle = photo.Album;
                    var extension = System.IO.Path.GetExtension(file.FileName).ToLower();

                    using (var img = System.Drawing.Image.FromStream(file.InputStream))
                    {
                        ThumbPath = String.Format("/GalleryImages/thumbs/{0}{1}", fileName, extension);
                        ImagePath = String.Format("/GalleryImages/{0}{1}", fileName, extension);

                        // Save thumbnail size image, 100 x 100
                        SaveToFolder(img, fileName, extension, new Size(100, 100), ThumbPath);

                        // Save large size image, 800 x 800
                        SaveToFolder(img, fileName, extension, new Size(600, 600), ImagePath);
                    }
                    // model.PhotoId = typeid;
                    // galRepo.Upload(model);
                    var post = new ImagePostModel { UserName = UserName, CreatedOn = DateTime.Now.ToString(), Title = title, ImagePath = ImagePath, ThumbPath = ThumbPath,Album=albumTitle };
                    ImageManager.Create(JsonConvert.SerializeObject(post));
                }
               
            }

            return RedirectToAction("Account");
        }
        // Craete User page
        [HttpGet]
        public ActionResult CreateUser()
        {
            var User = new Users();
           
            return View(User);
        }
        [HttpPost]
        public ActionResult CreateUser(Users User, IEnumerable<HttpPostedFileBase> files)
        {

            if (Request.HttpMethod == "POST")
            {

                if (!ModelState.IsValid)
                    return View(User);
                if (files.Count() == 0 || files.FirstOrDefault() == null)
                {
                    ViewBag.error = "Please Choose a file to upload !";
                    return View(User);
                }

                foreach (var file in files)
                {
                    if (file.ContentLength == 0) continue;

                    var Name = User.Name;
                    var Address = User.Address;
                    var Email = User.Email;
                    var Phone = User.Phone;
                    var photo = "";
                    var fileName = Guid.NewGuid().ToString();
                    var extension = System.IO.Path.GetExtension(file.FileName).ToLower();

                    using (var img = System.Drawing.Image.FromStream(file.InputStream))
                    {
                        photo = String.Format("/GalleryImages/thumbs/{0}{1}", fileName, extension);
                        // model.Photo = String.Format("/GalleryImages/{0}{1}", fileName, extension);

                        // Save thumbnail size image, 100 x 100
                        SaveToFolder(img, fileName, extension, new Size(100, 100), photo);

                        // Save large size image, 800 x 800
                        SaveToFolder(img, fileName, extension, new Size(600, 600), photo);
                    }
                    // model.UserId = typeid;
                    //galRepo.UploadUser(model);
                    var post = new Users { Name = Name, Address = Address, Email = Email, Phone = Phone, Photo = photo };
                    UserManager.Create(JsonConvert.SerializeObject(post));
                }
            }
            return RedirectToAction("Account");
        }
        // Image Resizing / Modifications
        public Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            double tempval;
            if (imageSize.Height > newSize.Height || imageSize.Width > newSize.Width)
            {
                if (imageSize.Height > imageSize.Width)
                    tempval = newSize.Height / (imageSize.Height * 1.0);
                else
                    tempval = newSize.Width / (imageSize.Width * 1.0);

                finalSize = new Size((int)(tempval * imageSize.Width), (int)(tempval * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }
        private void SaveToFolder(Image img, string fileName, string extension, Size newSize, string pathToSave)
        {
            // Get new resolution
            Size imgSize = NewImageSize(img.Size, newSize);
            using (System.Drawing.Image newImg = new Bitmap(img, imgSize.Width, imgSize.Height))
            {
                newImg.Save(Server.MapPath(pathToSave), img.RawFormat);
            }
        }
        // Main search Page
        public ActionResult SearchAlbum()
        {
           return View();
        }
        // all users
        public ActionResult UserGrid(int? page)
        {
            var User = new List<Users>();
            var blogs = UserManager.Read();
            if (blogs == null)
            {
                ViewBag.Empty = true;
                return View();
            }
            else
            {
                User = (from blog in blogs
                        orderby blog.PhotoId descending
                        select blog).ToList();
            }
           

        int pageSize = 2;
        int pageNumber = (page ?? 1);
        return View(User.ToPagedList(pageNumber, pageSize));
    }
        // Main search Page
        public ActionResult Contact()
        {
            return View();
        }
    }

}