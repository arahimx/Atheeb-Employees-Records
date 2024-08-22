using Microsoft.AspNetCore.Mvc;

namespace RestApi.Interface
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<string> UpdateFileAsync(string existingFileName, IFormFile newFile);
        Task<bool> DeleteFileAsync(string fileName);
        Task<string> GetFileUrlAsync(string fileName);
    }
}
