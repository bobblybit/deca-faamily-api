using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using DatabaseBackup.Services.Interface;

namespace DatabaseBackup.Services.Implementation
{
    public class FilesService: IFilesService
    {
        public void CreateZipFile(string dirPath, string destinationFilename) {
            if(Directory.Exists(dirPath))
                ZipFile.CreateFromDirectory(dirPath, destinationFilename);
        }

        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public void CreateFile(string filename, string data)
        {
            File.WriteAllText(filename, data);
        }

        public void DeleteFile(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }

        public async Task<byte[]> GetBackupFileAsBytes()
        {
            var filename = Path.Combine(Directory.GetCurrentDirectory(), "backup.zip");
            return await File.ReadAllBytesAsync(filename);
        }
    }
}
