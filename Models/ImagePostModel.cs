using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EvaGallery.Models
{
    public class ImagePostModel
    {
        public int PhotoId { get; set; }
        public String Title { get; set; }
        public String ImagePath { get; set; }
        public String ThumbPath { get; set; }
        public string CreatedOn { get; set; }
        public String UserName { get; set; }
        public String Album { get; set; }
    }
    public class ImageManager
    {
        // Define the members
        private static string PostsFile = HttpContext.Current.Server.MapPath("~/App_Data/ImagePosts.json");
        private static List<ImagePostModel> posts = new List<ImagePostModel>();
        private static ImagePostModel postImg = new ImagePostModel();

        // The CRUD functions
        public static void Create(string postJson)
        {
            var obj = JsonConvert.DeserializeObject<ImagePostModel>(postJson);

            if (posts.Count > 0)
            {
                posts = (from post in posts
                         orderby post.PhotoId
                         select post).ToList();
                obj.PhotoId = posts.Last().PhotoId + 1;
            }
            else
            {
                obj.PhotoId = 1;
            }


            posts.Add(obj);
            save();
        }

        public static List<ImagePostModel> Read()
        {
            // Check if the file exists.
            if (!File.Exists(PostsFile))
            {
                File.Create(PostsFile).Close();
                File.WriteAllText(PostsFile, "[]"); // Create the file if it doesn't exist.
            }
            posts = JsonConvert.DeserializeObject<List<ImagePostModel>>(File.ReadAllText(PostsFile));
            return posts;
        }
        public static ImagePostModel ReadImage()
        {
            // Check if the file exists.
            if (!File.Exists(PostsFile))
            {
                File.Create(PostsFile).Close();
                File.WriteAllText(PostsFile, "[]"); // Create the file if it doesn't exist.
            }
            postImg = JsonConvert.DeserializeObject<ImagePostModel>(File.ReadAllText(PostsFile));
            return postImg;
        }

        public static void Update(int id, string postJson)
        {
            Delete(id);
            Create(postJson);
            save();
        }

        public static void Delete(int id)
        {
            posts.Remove(posts.Find(x => x.PhotoId == id));
            save();
        }

        // Output function
        private static void save()
        {
            File.WriteAllText(PostsFile, JsonConvert.SerializeObject(posts));
        }
    }
}