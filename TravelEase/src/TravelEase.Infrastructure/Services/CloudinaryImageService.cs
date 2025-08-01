﻿using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.TravelEase.Infrastructure.Services
{
    public class CloudinaryImageService : IImageUploader
    {
        private readonly ICloudinaryWrapper _wrapper;

        public CloudinaryImageService(ICloudinaryWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "hotels",
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = false
            };

            var result = await _wrapper.UploadImageAsync(uploadParams);
            return result.SecureUrl?.ToString() ?? string.Empty;
        }
    }
}