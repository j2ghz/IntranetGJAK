using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace $rootnamespace$.Models
{
    public partial class FilesModel : DbContext
    {
        public FilesModel()
            : base("name=FilesModel")
        {
        }

        public virtual DbSet<File> Files { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
