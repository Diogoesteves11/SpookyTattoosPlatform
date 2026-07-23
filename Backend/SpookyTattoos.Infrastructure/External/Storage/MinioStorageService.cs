/*
Copyright 2026 Diogo Esteves, Guilherme Mattos

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using SpookyTattoos.Application.Interfaces.External;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SpookyTattoos.Infrastructure.External.Storage;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _publicEndpoint;

    public MinioStorageService(IMinioClient minioClient, IConfiguration configuration)
    {
        _minioClient = minioClient;
        _publicEndpoint = configuration["Minio:PublicEndpoint"] ?? throw new ArgumentNullException("Minio:PublicEndpoint is missing");
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, string bucketName)
    {
        var uniqueFileName = $"{Guid.NewGuid()}-{fileName.Replace(" ", "_")}";

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(uniqueFileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs);

        return $"{_publicEndpoint}/{bucketName}/{uniqueFileName}";
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        var (bucketName, objectName) = ExtractBucketAndObjectFromUrl(fileUrl);

        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs);
    }

    public async Task<string> MoveFileAsync(string fileUrl, string destinationBucket)
    {
        var (sourceBucket, objectName) = ExtractBucketAndObjectFromUrl(fileUrl);

        if (sourceBucket.Equals(destinationBucket, StringComparison.OrdinalIgnoreCase))
        {
            return fileUrl;
        }

        var copySourceArgs = new CopySourceObjectArgs()
            .WithBucket(sourceBucket)
            .WithObject(objectName);

        var copyObjectArgs = new CopyObjectArgs()
            .WithBucket(destinationBucket)
            .WithObject(objectName)
            .WithCopyObjectSource(copySourceArgs);

        await _minioClient.CopyObjectAsync(copyObjectArgs);

        await DeleteFileAsync(fileUrl);

        return $"{_publicEndpoint}/{destinationBucket}/{objectName}";
    }

    /// <summary>
    /// Helper para extrair o nome do bucket e do objeto a partir do URL completo.
    /// Exemplo: http://localhost:9000/catalog-private/foto.jpg -> ("catalog-private", "foto.jpg")
    /// </summary>
    private (string BucketName, string ObjectName) ExtractBucketAndObjectFromUrl(string fileUrl)
    {
        var uri = new Uri(fileUrl);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length < 2)
        {
            throw new ArgumentException("URL de imagem inválido.");
        }

        var bucketName = segments[0];
        var objectName = string.Join("/", segments, 1, segments.Length - 1);

        return (bucketName, objectName);
    }
}