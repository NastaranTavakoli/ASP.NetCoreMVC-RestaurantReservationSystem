using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ReservationSystemDraft_v1.Helpers;

namespace ReservationSystemDraft_v1.Services
{
    public class PhotoService
    {
        private readonly PhotoStorageService _photoStorageService;


        public PhotoService(PhotoStorageService photoStorageService)
        {
            _photoStorageService = photoStorageService;
        }

        public async Task UploadPhotos(List<IFormFile> files, Restaurant restaurant, AzureStorageConfig storageConfig)
        {
            var fileNames = await _photoStorageService.StorePhotos(files, storageConfig);
            var photos = fileNames.Select(f => new Photo() {FileName = f}).ToList();
            restaurant.Photos = photos;
        }
    }
}
