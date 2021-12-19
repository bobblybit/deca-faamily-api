using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseBackup.Services;
using DatabaseBackup.Services.Interface;
using DatabaseBackup.Settings;

using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;

using Microsoft.Extensions.Options;

using static Dropbox.Api.Files.SearchMatchType;

namespace DatabaseBackup.Services.Implementation
{
    public class DropBoxService : IDropBoxService
    {
        private readonly DropBoxConfig _dropBoxConfig;
        private readonly IFilesService _filesService;

        public DropBoxService(IOptions<DropBoxConfig> dropboxConfig, IFilesService filesService)
        {
            _dropBoxConfig = dropboxConfig.Value;
            _filesService = filesService;
        }

        public async Task<string> Upload()
        {
            string sharedLink = string.Empty;
            string path = "/Backup.zip";
            using (DropboxClient dbx = new DropboxClient(_dropBoxConfig.AccessToken))
{
                using (var memoryStream = new MemoryStream(await _filesService.GetBackupFileAsBytes()))
                {
                    var updated = await dbx.Files.UploadAsync(
                        "/Backup.zip",
                        WriteMode.Overwrite.Instance,
                        body: memoryStream);
                }
                

                SharedLinkMetadata sharedLinkMetadata;
                try
                {
                    sharedLinkMetadata = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(path);
                    sharedLink = sharedLinkMetadata.Url;
                }
                catch (ApiException<CreateSharedLinkWithSettingsError> err)
                {
                    if (err.ErrorResponse.IsSharedLinkAlreadyExists)
                    {
                        var sharedLinksMetadata = await dbx.Sharing.ListSharedLinksAsync(path, null, true);
                        sharedLinkMetadata = sharedLinksMetadata.Links.First();
                        sharedLink = sharedLinkMetadata.Url;
                    }
                    else
                    {
                        throw err;
                    }
                }
            }

            return sharedLink;
        }
    }
}
