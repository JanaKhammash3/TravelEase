using CloudinaryDotNet.Actions;
using System.Threading.Tasks;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface ICloudinaryWrapper
    {
        Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams);
    }
}