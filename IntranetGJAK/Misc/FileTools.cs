﻿using System.IO;
using System.Threading.Tasks;

namespace IntranetGJAK
{
    public class FileTools
    {
    }

    public static class FileExtensions
    {
        public static Task DeleteAsync(this FileInfo fi)
        {
            return Task.Factory.StartNew(fi.Delete);
        }
    }
}