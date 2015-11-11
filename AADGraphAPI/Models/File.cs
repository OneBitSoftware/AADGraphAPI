using Microsoft.Office365.SharePoint.FileServices;
using System;

namespace AADGraphAPI.Models
{
    public class File
    {
        public string Name;
        public string DisplayName;
        public string ID;

        public string FileText { get; set; }

        public string UpdatedText { get; set; }

        public string CreatedBy { get; set; }
        public string FileType { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
        public File(IItem fileItem)
        {

            ID = fileItem.Id;
            CreatedBy = fileItem.CreatedBy.User.DisplayName;
            FileType = fileItem.Type;
            Name = fileItem.Name;
            Size = fileItem.Size;
            LastModified = fileItem.DateTimeLastModified.LocalDateTime;
            DisplayName = (fileItem is Folder) ? "Folder" : "File";

        }
    }
}