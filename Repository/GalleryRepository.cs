using EvaGallery.DAL;
using EvaGallery.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace EvaGallery.Repository
{
    public class GalleryRepository
    {
        private GalleryContext db = new GalleryContext();
        public List<ImagePostModel> getAlbums(string filter = null)
        {
            var records = new List<ImagePostModel>();
            var blogs = ImageManager.Read();
            records = (from blog in blogs
                       where filter == null || (blog.UserName.Contains(filter))
                       orderby blog.Album descending
                       select blog).Distinct().ToList();

            var customers = records.GroupBy(x => x.Album).Select(x => x.FirstOrDefault()).ToList();

            return customers;
        }


    }

}