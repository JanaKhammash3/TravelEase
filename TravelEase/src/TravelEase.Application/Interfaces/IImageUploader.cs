using Microsoft.AspNetCore.Http;

namespace TravelEase.TravelEase.Application.Interfaces;

public interface IImageUploader
{
    Task<string> UploadImageAsync(IFormFile file);
}