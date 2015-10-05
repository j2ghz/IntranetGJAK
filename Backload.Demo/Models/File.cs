using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Backload.Demo.Models
{
    public partial class File
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Original { get; set; }

        [Required]
        [StringLength(24)]
        public string Type { get; set; }

        public long Size { get; set; }

        public DateTime UploadTime { get; set; }

        [Required]
        public byte[] Data { get; set; }

        [Required]
        public string Preview { get; set; }
    }
}
