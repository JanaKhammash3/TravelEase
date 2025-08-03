using CloudinaryDotNet.Actions;

namespace TravelEase.Application.Interfaces
{
    public interface ICloudinaryWrapper
    {
        Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams);
    }
}