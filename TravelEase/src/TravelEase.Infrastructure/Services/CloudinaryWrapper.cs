using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.TravelEase.Infrastructure.Services
{
    public class CloudinaryWrapper : ICloudinaryWrapper
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryWrapper(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams)
        {
            return _cloudinary.UploadAsync(uploadParams);
        }
    }
}