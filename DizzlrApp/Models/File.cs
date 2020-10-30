using DizzlrApp.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DizzlrApp.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [NotMapped]
        public bool IsChecked { get; set; } 
        [NotMapped]
        public string GoogleFileId { get; set; }
    }
}
