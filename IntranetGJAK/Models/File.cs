using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntranetGJAK.Models
{
    public class File
    {
        public int FileId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] Content { get; set; }
        public int PersonId { get; set; }
        public virtual User Person { get; set; }
    }
}