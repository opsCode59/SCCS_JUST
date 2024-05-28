﻿using API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Repository;
public class PhotoRepository : IPhotoRepository
    {
    private readonly Cloudinary cloudinary;
    public PhotoRepository(IOptions<CloudinarySettings> config)
    {
        var acc=new Account { 
          Cloud=config.Value.CloudName,
          ApiKey=config.Value.ApiKey,
          ApiSecret = config.Value.ApiSecret 
          };
        cloudinary=new Cloudinary(acc);
    }
    public async Task<ImageUploadResult> AddPhotoAsync( IFormFile file )
        {
        var uploadResult=new ImageUploadResult();
        if (file.Length > 0)
            {
            using var stream= file.OpenReadStream();
            var uploadParams = new ImageUploadParams
                {
                File=new FileDescription(file.FileName,stream),
                Transformation=new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder="da-net7"
                };
            uploadResult=await cloudinary.UploadAsync(uploadParams);
            }
        return uploadResult;
        }

    public async Task<DeletionResult> DeletePhotoAsync( string publicId )
        {
        var deleteParams=new DeletionParams (publicId);
        return await cloudinary.DestroyAsync(deleteParams);
        }
    }
public interface IPhotoRepository
    {
    public Task<ImageUploadResult> AddPhotoAsync( IFormFile file );
    public Task<DeletionResult> DeletePhotoAsync( string publicId );
    }
