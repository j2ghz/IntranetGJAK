using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetGJAK.Tools
{
    public static class Thumbnails
    {
        public static string GetThumbnail(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                default:
                    return "default.png";
            }
        }
    }
}