using CloudinaryDotNet.Actions;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface ICloudinaryWrapper
    {
        Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams);
    }
}