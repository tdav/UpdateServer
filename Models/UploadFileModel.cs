using Microsoft.AspNetCore.Http;

namespace UniUpdateManage.Models
{
    public class UploadFileModel
    {
        public string AppNameDirectory { get; set; }
        public string Version { get; set; }
        public int Required { get; set; }
        public string DpInfo { get; set; }
        public string FileName { get; set; }
        public IFormFile FileData { get; set; }
    }
}
