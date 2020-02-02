using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace EvaGallery.Models
{
    public class Users
    {
        
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int ? Phone { get; set; }
        public int ? CatId { get; set; }
        public string Photo { get; set; }
        public int ? PhotoId { get; set; }

    }
    public class UserManager
    {
        // Define the members
        private static string PostsFile = HttpContext.Current.Server.MapPath("~/App_Data/Users.json");
        private static List<Users> posts = new List<Users>();

        // The CRUD functions
        public static void Create(string postJson)
        {
            var obj = JsonConvert.DeserializeObject<Users>(postJson);

            if (posts.Count > 0)
            {
                posts = (from post in posts
                         orderby post.UserId
                         select post).ToList();
                obj.UserId = posts.Last().UserId + 1;
            }
            else
            {
                obj.UserId = 1;
            }


            posts.Add(obj);
            save();
        }

        public static List<Users> Read()
        {
            // Check if the file exists.
            if (!File.Exists(PostsFile))
            {
                File.Create(PostsFile).Close();
                File.WriteAllText(PostsFile, "[]"); // Create the file if it doesn't exist.
            }
            posts = JsonConvert.DeserializeObject<List<Users>>(File.ReadAllText(PostsFile));
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
            posts.Remove(posts.Find(x => x.UserId == id));
            save();
        }

        // Output function
        private static void save()
        {
            File.WriteAllText(PostsFile, JsonConvert.SerializeObject(posts));
        }
    }
}