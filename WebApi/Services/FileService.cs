using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Interface;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RestApi.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadFolder;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(string uploadFolder, IHttpContextAccessor httpContextAccessor)
        {
            _uploadFolder = Path.Combine(uploadFolder, "Attachments"); // Ensure directory is 'Attachments' inside 'uploadFolder'
            _httpContextAccessor = httpContextAccessor;

            // Ensure the directory exists
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded.");
            }

            // Generate a random string and number combination for the file name
            var randomFileName = GenerateRandomFileName(Path.GetExtension(file.FileName));

            var filePath = Path.Combine(_uploadFolder, randomFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return randomFileName; // Return just the file name
        }

        public async Task<string> UpdateFileAsync(string existingFileName, IFormFile newFile)
        {
            var oldFilePath = Path.Combine(_uploadFolder, existingFileName);

            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }

            return await UploadFileAsync(newFile);
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_uploadFolder, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }

        public async Task<string> GetFileUrlAsync(string fileName)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
            {
                throw new InvalidOperationException("Request context is not available.");
            }

            // Generate the URL to access the file
            var fileUrl = $"{request.Scheme}://{request.Host}/Attachments/{fileName}";

            return fileUrl;
        }

        private string GenerateRandomFileName(string fileExtension)
        {
            // Create a random string with letters and numbers
            var randomString = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

            // Generate a random number
            var randomNumber = new Random().Next(1000, 9999);

            // Combine the random string and number, and append the file extension
            return $"{randomString}_{randomNumber}{fileExtension}";
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream",
            };
        }
    }
}
