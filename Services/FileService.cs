using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace TodoApi.Services
{
    public class FileService
    {
        private readonly Cloudinary _cloudinary;

        public FileService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        // لرفع الصور
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = $"todo_{Guid.NewGuid()}",
                    Folder = "todo-app"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Cloudinary error: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload error: {ex.Message}");
                return null;
            }
        }

        // دالة جديدة مخصوص لـ PDF والملفات
        public async Task<string> UploadPdfFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                using var stream = file.OpenReadStream();

                // استخدمي AttachmentUploadParams علشان الملفات بتكون قابلة للتحميل
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = $"file_{Guid.NewGuid()}",
                    Folder = "todo-app-files"
                };

                //var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                var uploadResult = await _cloudinary.UploadAsync(new RawUploadParams
                 {
                     File = new FileDescription(file.FileName, stream),
                     PublicId = $"file_{Guid.NewGuid()}",
                     Folder = "todo-app-files"
                 });



                if (uploadResult.Error != null)
                {
                    throw new Exception($"Cloudinary error: {uploadResult.Error.Message}");
                }

                // علشان الـ PDF يتحط كـ attachment ويقدر يتحمل
                string downloadUrl = uploadResult.SecureUrl.ToString();

                // لو ده PDF، غيري الـ URL علشان يforce download
                if (file.FileName.ToLower().EndsWith(".pdf"))
                {
                    downloadUrl = downloadUrl.Replace("/upload/", "/upload/fl_attachment/");
                }

                return downloadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PDF upload error: {ex.Message}");
                return null;
            }
        }

        // دالة علشان تجيبي الـ PDF بشكل صحيح
        public string GetPdfDownloadUrl(string publicId)
        {
            // علشان تجيبي الـ PDF كـ file للتحميل
            var url = _cloudinary.Api.UrlImgUp.Transform(new Transformation())
                .BuildUrl(publicId);

            // إضافة fl_attachment علشان يforce download
            url = url.Replace("/upload/", "/upload/fl_attachment/");

            return url;
        }
    }
}