using Microsoft.AspNetCore.Http;

namespace TravelEase.Application.Interfaces;
public interface IImageUploader
{
    Task<string> UploadImageAsync(IFormFile file);
}