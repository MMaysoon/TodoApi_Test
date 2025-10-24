using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace TodoApi.Services
{
    public class FileService
    {
        //private readonly Cloudinary _cloudinary;

        //public FileService(IConfiguration configuration)
        //{
        //    var account = new Account(
        //        configuration["Cloudinary:CloudName"],
        //        configuration["Cloudinary:ApiKey"],
        //        configuration["Cloudinary:ApiSecret"]);

        //    _cloudinary = new Cloudinary(account);
        //}

        //public async Task<string> UploadFileAsync(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return null;

        //    using var stream = file.OpenReadStream();
        //    var uploadParams = new ImageUploadParams()
        //    {
        //        File = new FileDescription(file.FileName, stream),
        //        PublicId = $"todo_{Guid.NewGuid()}"
        //    };

        //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        //    return uploadResult.SecureUrl.ToString();
        //}

        private readonly Cloudinary _cloudinary;

        public FileService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var stream = file.OpenReadStream();

            var extension = Path.GetExtension(file.FileName).ToLower();

            // لو الملف صورة
            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = $"todo_{Guid.NewGuid()}"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);
                return result.SecureUrl.ToString();
            }

            // لو الملف فيديو
            else if (extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".mkv")
            {
                var uploadParams = new VideoUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = $"todo_{Guid.NewGuid()}"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);
                return result.SecureUrl.ToString();
            }

            // لو أي نوع تاني (PDF, Word, ZIP, إلخ)
            else
            {
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = $"todo_{Guid.NewGuid()}"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);
                return result.SecureUrl.ToString();
            }
        }
    }
}
