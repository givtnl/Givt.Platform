namespace Givt.Platform.FileStorage.Interfaces;

public interface IFileStorage
{
    // TODO: change return type. Was CloudFile from WindowsAzure.Storage
    Task<object> DownloadFile(string containerName, string fileName, CancellationToken token);
    Task UploadFile(string containerName, string path, Stream file, string contentType, CancellationToken cancellationToken = default);
    Task<bool> FileExists(string containerName, string path, CancellationToken cancellationToken);
    Task DeleteFile(string containerName, string path, CancellationToken cancellationToken);
}