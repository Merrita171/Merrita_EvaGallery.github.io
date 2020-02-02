using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EvaGallery.Models
{
    public class UserPostModel
    {

        // General properties
        public int ID { get; set; }
        public string Title { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }

        // Time based properties
        public DateTime CreateTime { get; set; }

        // Other properties and settings may include UserID, RoleID etc.
    }

    // The class to manage the data-sources
    public class PostManager
    {
        // Define the members
        private static string PostsFile = HttpContext.Current.Server.MapPath("~/App_Data/Posts.json");
        private static List<UserPostModel> posts = new List<UserPostModel>();

        // The CRUD functions
        public static void Create(string postJson)
        {
            var obj = JsonConvert.DeserializeObject<UserPostModel>(postJson);

            if (posts.Count > 0)
            {
                posts = (from post in posts
                         orderby post.CreateTime
                         select post).ToList();
                obj.ID = posts.Last().ID + 1;
            }
            else
            {
                obj.ID = 1;
            }


            posts.Add(obj);
            save();
        }

        public static List<UserPostModel> Read()
        {
            // Check if the file exists.
            if (!File.Exists(PostsFile))
            {
                File.Create(PostsFile).Close();
                File.WriteAllText(PostsFile, "[]"); // Create the file if it doesn't exist.
            }
            posts = JsonConvert.DeserializeObject<List<UserPostModel>>(File.ReadAllText(PostsFile));
            return posts;
        }

        public static void Update(int id, string postJson)
        {
            Delete(id);
            Create(postJson);
            save();
        }

        public static void Delete(int id)
        {
            posts.Remove(posts.Find(x => x.ID == id));
            save();
        }

        // Output function
        private static void save()
        {
            File.WriteAllText(PostsFile, JsonConvert.SerializeObject(posts));
        }
    }
}