using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetGJAK.Models.JSON.Blueimp_FileUpload
{
    using System.IO;

    using File = IntranetGJAK.Models.File;

    /// <summary>
    /// Core class for returning upload data
    /// </summary>
    public class FilesData
    {
        public List<IBlueimpResult> files { get; set; }

        public FilesData()
        {
            this.files = new List<IBlueimpResult>(1);
        }
    }

    /// <summary>
    /// Core class for returning delete data
    /// </summary>
    public class DeletedData
    {
        public Dictionary<string, bool> files { get; set; }

        public DeletedData()
        {
            files = new Dictionary<string, bool>(1);
        }
    }

    public interface IBlueimpResult
    {
        string name { get; set; }

        long size { get; set; }
    }

    /// <summary>
    /// Class with info about an uploaded file
    /// </summary>
    public class UploadSucceeded : IBlueimpResult
    {
        public string name { get; set; }

        public long size { get; set; }

        public string url { get; set; }

        public string thumbnailUrl { get; set; }

        public string deleteUrl { get; set; }

        public string deleteType { get; set; }
    }

    /// <summary>
    /// Class with info about an upload failure
    /// </summary>
    public class UploadFailed : IBlueimpResult
    {
        public string name { get; set; }

        public long size { get; set; }

        public string error { get; set; }
    }

}

////{"files": [
////  {
////    "picture1.jpg": true
////  },
////  {
////    "picture2.jpg": true
////  }
////]}

////{"files": [
////  {
////    "name": "picture1.jpg",
////    "size": 902604,
////    "error": "Filetype not allowed"
////  },
////  {
////    "name": "picture2.jpg",
////    "size": 841946,
////    "error": "Filetype not allowed"
////  }
////]}

////{"files": [
////  {
////    "name": "picture1.jpg",
////    "size": 902604,
////    "url": "http:\/\/example.org\/files\/picture1.jpg",
////    "thumbnailUrl": "http:\/\/example.org\/files\/thumbnail\/picture1.jpg",
////    "deleteUrl": "http:\/\/example.org\/files\/picture1.jpg",
////    "deleteType": "DELETE"
////  },
////  {
////    "name": "picture2.jpg",
////    "size": 841946,
////    "url": "http:\/\/example.org\/files\/picture2.jpg",
////    "thumbnailUrl": "http:\/\/example.org\/files\/thumbnail\/picture2.jpg",
////    "deleteUrl": "http:\/\/example.org\/files\/picture2.jpg",
////    "deleteType": "DELETE"
////  }
////]}