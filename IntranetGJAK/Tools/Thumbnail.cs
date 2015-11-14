using System.IO;

namespace IntranetGJAK.Tools
{
    public static class Thumbnails
    {
        public static string GetThumbnail(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                default:
                    return "/images/filetypeicons/default.svg";
            }
        }
    }
}