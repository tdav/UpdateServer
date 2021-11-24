using System;

namespace AsbtCore.Update.Server.Models
{
    public class UpdateInfoModel
    {        
        public string Name { get; set; }
        public string Version { get; set; }
        public int Required { get; set; }
        public string DpInfo { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
