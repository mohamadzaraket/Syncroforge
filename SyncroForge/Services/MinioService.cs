using Minio;
using Minio.DataModel.Args;

namespace SyncroForge.Services
{
    public class MinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;
        private readonly string _endpoint;
        public MinioService(IConfiguration configuration)
        {
            _endpoint = configuration["Minio:Endpoint"];
            var accessKey = configuration["Minio:AccessKey"];
            var secretKey = configuration["Minio:SecretKey"];
            _bucketName = configuration["Minio:BucketName"];
            string formattedEndpoint = _endpoint.Replace("http://", "").Replace("https://", "");

            _minioClient = new MinioClient()
                .WithEndpoint(formattedEndpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(false)
                .Build();
        }
        private async Task EnsureBucketExists()
        {
            try
            {
                var found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
                if (!found)
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring bucket exists: {ex.Message}");
                throw;
            }
        }
        public async Task<string> UploadFileAsync(Stream fileStream, string objectName)
        {
            try
            {
                await EnsureBucketExists();

                

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType("application/octet-stream");

                await _minioClient.PutObjectAsync(putObjectArgs);

                return $"{_endpoint}/{_bucketName}/{objectName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw;
            }
        }
        public async Task DeleteFileAsync(string objectName)
        {
            try
            {
                Uri uri = new Uri(objectName);

                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(uri.Segments.Last().Trim('/')));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                throw;
            }
        }
    }
}
