﻿using AsbtCore.Update.Server.Models;
using AsbtCore.UtilsV2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UniUpdateManage.Models;

namespace AsbtCore.Update.Controllers
{
    //   [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly ILogger<UpdateController> logger;
        private readonly IMemoryCache cache;

        public UpdateController(ILogger<UpdateController> logger, IMemoryCache cache)
        {
            this.logger = logger;
            this.cache = cache;
        }

        [HttpGet("check")]
        public ActionResult GetAsync(string app_name, string version)
        {
            try
            {
                var ver = new Version(version);

                List<UpdateInfoModel> ls;
                if (cache.TryGetValue($"MEM_CHECK_{app_name}", out List<UpdateInfoModel> value))
                {
                    ls = value;
                    return CheckVersion(ls, ver);
                }
                else
                {                    
                    var path = $"{AppDomain.CurrentDomain.BaseDirectory}UpdateFiles{Path.DirectorySeparatorChar}{app_name}{Path.DirectorySeparatorChar}";

                    if (!Directory.Exists(path))
                    {
                        return NotFound();
                    }
                    else
                    {
                        var s = CFile.GetFileContents(path + "index.json");
                        ls = s.FromJson<List<UpdateInfoModel>>();
                        
                        var cacheEntryOptions = new MemoryCacheEntryOptions();
                        cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(1));
                        cache.Set($"MEM_CHECK_{app_name}", ls, cacheEntryOptions);

                        return CheckVersion(ls, ver);
                    }
                } 
            }
            catch (System.Exception ee)
            {
                logger.LogError(ee, $"UpdateController.GetAsync File: {app_name}{Path.DirectorySeparatorChar}{version}");
                return StatusCode(500);
            }
        }

        private ActionResult CheckVersion(List<UpdateInfoModel> ls, Version ver)
        {
            UpdateInfoModel lastVersion = ls.Where(x => new Version(x.Version) > ver).OrderByDescending(o => new Version(o.Version)).FirstOrDefault();

            if (lastVersion != null)
            {                
                return Ok(lastVersion);
            }
            else
                return NotFound();
        }

        [HttpGet("download")]
        public async Task<ActionResult> DownloadAsync(string app_name, string version, string name)
        {
            try
            {
                var path = $"{AppDomain.CurrentDomain.BaseDirectory}UpdateFiles{Path.DirectorySeparatorChar}{app_name}{Path.DirectorySeparatorChar}{version}{Path.DirectorySeparatorChar}{name}";
                var bytes = await System.IO.File.ReadAllBytesAsync(path);
                return File(bytes, "application/octet-stream", name);
            }
            catch (System.Exception ee)
            {
                logger.LogError(ee, $"UpdateController.DownloadAsync File: {app_name}{Path.DirectorySeparatorChar}{version}{Path.DirectorySeparatorChar}{name}");
                return StatusCode(500);
            }
        }


        [HttpPost("list/{app_name}")]
        public ActionResult<List<UpdateInfoModel>> ListAsync(string app_name)
        {
            try
            {
                var path = $"{AppDomain.CurrentDomain.BaseDirectory}UpdateFiles{Path.DirectorySeparatorChar}{app_name}{Path.DirectorySeparatorChar}";

                if (System.IO.File.Exists(path + "index.json"))
                {
                    var str = CFile.GetFileContents(path + "index.json");
                    var list = str.FromJson<List<UpdateInfoModel>>();
                    return Ok(list);
                }
                else
                    return NotFound("AppName топилмади");
            }
            catch (System.Exception ee)
            {
                logger.LogError(ee, $"UpdateController.ListAsync AppName:{app_name}");
                return StatusCode(500);
            }
        }


        [HttpPost("app_name_list")]
        public ActionResult AppNameList()
        {
            try
            {
                var path = $"{AppDomain.CurrentDomain.BaseDirectory}UpdateFiles{Path.DirectorySeparatorChar}";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var di = new DirectoryInfo(path);
                var list = di.GetDirectories().Select(s => new { s.Name }).ToList();

                return Ok(list);
            }
            catch (System.Exception ee)
            {
                logger.LogError(ee, $"UpdateController.ListAsync Error:{ee.Message}");
                return StatusCode(500);
            }
        }


        [HttpPost("insert")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> InsertAsync([FromForm] UploadFileModel model)
        {
            try
            {
                if (model.FileData == null) return BadRequest("Файл топилмади");
                if (!Version.TryParse(model.Version, out _)) return BadRequest("Файл версияси тугри эмас");

                var path = $"{AppDomain.CurrentDomain.BaseDirectory}UpdateFiles{Path.DirectorySeparatorChar}{model.AppNameDirectory}{Path.DirectorySeparatorChar}";
                if (System.IO.File.Exists(path + model.Version + Path.DirectorySeparatorChar + model.FileName)) return BadRequest("Бундай версияда программа бор");

                List<UpdateInfoModel> list;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path + model.Version);


                if (System.IO.File.Exists(path + "index.json"))
                {
                    var str = CFile.GetFileContents(path + "index.json");
                    list = str.FromJson<List<UpdateInfoModel>>();
                }
                else
                    list = new List<UpdateInfoModel>();


                Directory.CreateDirectory(path + model.Version);

                using (var stream = new FileStream(path + model.Version + Path.DirectorySeparatorChar + model.FileName, FileMode.CreateNew))
                {
                    await model.FileData.CopyToAsync(stream);

                    list.Add(new UpdateInfoModel() { Name = model.FileName, Version = model.Version, Required = model.Required, CreateDate = DateTime.Now, DpInfo = model.DpInfo });
                    System.IO.File.WriteAllText(path + "index.json", list.ToJson());
                }

                return Ok();
            }
            catch (System.Exception ee)
            {
                logger.LogError(ee, $"UpdateController.InsertAsync File:{model.AppNameDirectory}{Path.DirectorySeparatorChar}{model.FileName} Version:{model.Version} DpInfo:{model.DpInfo}");
                return StatusCode(500);
            }
        }


    }
}
