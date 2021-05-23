#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using pwr_msi.Models;

namespace pwr_msi.Services {
    public class S3Service {
        private readonly AppConfig _appConfig;
        private readonly AmazonS3Client _s3Client;
        private readonly ILogger<S3Service> _logger;

        public S3Service(AppConfig appConfig, ILogger<S3Service> logger) {
            _appConfig = appConfig;
            _logger = logger;

            var s3Config = new AmazonS3Config {
                RegionEndpoint = _appConfig.S3Region, ServiceURL = _appConfig.S3Url, ForcePathStyle = true,
            };
            _s3Client = new AmazonS3Client(_appConfig.S3AccessKey, _appConfig.S3SecretKey, s3Config);
        }

        internal async Task<GetObjectResponse> GetFile(string fileName) {
            return await _s3Client.GetObjectAsync(_appConfig.S3BucketName, fileName);
        }

        public async Task<FileStreamResult> GetFileAsResponse(string fileName) {
            var response = await GetFile(fileName);
            var contentType = response.Headers.ContentType;
            var fileBaseName = Path.GetFileName(fileName);
            return new FileStreamResult(response.ResponseStream, new MediaTypeHeaderValue(contentType)) {
                FileDownloadName = fileBaseName
            };
        }

        internal async Task PutFile(string fileName, string contentType, Stream stream) {
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _appConfig.S3BucketName);
            if (!bucketExists) {
                _logger.LogInformation("Creating S3 bucket {BucketName}", _appConfig.S3BucketName);
                await _s3Client.PutBucketAsync(_appConfig.S3BucketName);
            }

            var transferUtility = new TransferUtility(_s3Client);
            var uploadRequest = new TransferUtilityUploadRequest {
                InputStream = stream, BucketName = _appConfig.S3BucketName, Key = fileName, ContentType = contentType,
            };
            await transferUtility.UploadAsync(uploadRequest);
        }

        public async Task<string> HandleImageUpload(int restaurantId, IFormFile formFile) {
            var ext = Path.GetExtension(formFile.FileName);
            var contentType = ext switch {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                // ReSharper disable once NotResolvedInText
                _ => throw new ArgumentOutOfRangeException("fileName", "Only .jpg and .png files are supported"),
            };
            var baseName = Guid.NewGuid().ToString();
            var fileName = $"{restaurantId}/{baseName}{ext}";
            await using var stream = formFile.OpenReadStream();
            await PutFile(fileName, contentType, stream);
            return fileName;
        }
    }
}
