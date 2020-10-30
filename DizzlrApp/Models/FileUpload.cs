using System.Collections.Generic;

namespace DizzlrApp.Models
{
    public class FileUpload
    {
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
        public List<File> FilesOnFileSystem { get; set; }
    }
}
