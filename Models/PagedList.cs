using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EvaGallery.Models
{
   
        public class Paged<T>
        {
            public List<T> Content { get; set; }
            public int ? CurrentPage { get; set; }
            public Int32 PageSize { get; set; }
            public int TotalRecords { get; set; }
        }

    


}