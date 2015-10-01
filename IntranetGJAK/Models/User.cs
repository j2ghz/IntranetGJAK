using System.Collections.Generic;

namespace IntranetGJAK.Models
{
    public class User
    {
        public int UserId { get; set; }
        public virtual ICollection<File> Files { get; set; }
    }
}