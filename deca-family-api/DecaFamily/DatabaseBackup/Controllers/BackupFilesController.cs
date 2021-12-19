using System;
using System.IO;

using Microsoft.AspNetCore.Mvc;

namespace DatabaseBackup.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BackupFilesController : ControllerBase
    {
        public FileContentResult GetZipFile()
        {
            var filename = Path.Combine(Directory.GetCurrentDirectory(), "backup.zip");
            if (System.IO.File.Exists(filename))
                return File(System.IO.File.ReadAllBytes(filename), "application/octet-stream", $"backup_{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}.zip");
                
            return null;
        }
    }
}
