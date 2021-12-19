using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseBackup.Services.Interface
{
    public interface IDropBoxService
    {
        public Task<string> Upload();
    }
}
