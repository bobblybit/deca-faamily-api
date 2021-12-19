using System.Threading.Tasks;

namespace DatabaseBackup.Services.Interface
{
    public interface IFilesService
    {
        void CreateZipFile(string dirPath, string destinationFilename);

        void DeleteDirectory(string path);


        void CreateFile(string filename, string data);

        void DeleteFile(string filename);

        Task<byte[]> GetBackupFileAsBytes();
    }
}
