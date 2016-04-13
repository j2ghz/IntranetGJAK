namespace IntranetGJAK.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    using IntranetGJAK.Models.JSON.Blueimp_FileUpload;
    using IntranetGJAK;

    public class File
    {
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        [Display(Name = "Název souboru")]
        public string Name { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        [Display(Name = "Velikost")]
        public long Size { get; set; }
        [Required]
        [Display(Name = "Odesílatel")]
        public string Uploader { get; set; }
        [Required]
        [Display(Name = "Datum")]
        public DateTime DateUploaded { get; set; }

        public UploadSucceeded ToSerializeable()
        {
            return new UploadSucceeded
            {
                name = this.Name,
                size = this.Size,
                url = "/api/files/" + this.Id,
                thumbnailUrl = Thumbnails.GetThumbnail(this.Name),
                deleteUrl = "/api/files/" + this.Id,
                deleteType = "DELETE"
            };

        }

    }

    public interface IFileRepository
    {
        void Add(File item);

        IEnumerable<File> GetAll();

        File Find(string key);

        File Remove(string key);

        void Update(File item);
    }

    public class FileRepository : IFileRepository
    {
        private static ConcurrentDictionary<string, File> _Files = new ConcurrentDictionary<string, File>();


        public FileRepository()
        {
        }

        public IEnumerable<File> GetAll()
        {
            return _Files.Values;
        }

        public void Add(File item)
        {
            item.Id = Guid.NewGuid().ToString();
            _Files[item.Id] = item;
        }

        public File Find(string key)
        {
            File item;
            _Files.TryGetValue(key, out item);
            return item;
        }

        public File Remove(string key)
        {
            File item;
            _Files.TryGetValue(key, out item);
            _Files.TryRemove(key, out item);
            return item;
        }

        public void Update(File item)
        {
            _Files[item.Id] = item;
        }
    }
}