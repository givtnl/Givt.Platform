using Google.Apis.Drive.v3;
using File = Google.Apis.Drive.v3.Data.File;

namespace Givt.Platform.Pdf.GoogleDocs.Helpers;

public class GoogleDriveService : BaseGoogleService<DriveService>
{
    private readonly DriveService _driveService;

    public GoogleDriveService(GoogleDocsOptions options) : base(options)
    {
        _driveService = BuildService();
    }

    internal async Task<string> CopyFile(string fileId, string newFileName, CancellationToken token)
    {
        var file = new File { Name = newFileName };
        var copyRequest = _driveService.Files.Copy(file, fileId);
        var newFile = await copyRequest.ExecuteAsync(token);
        return newFile.Id;
    }

    public async Task DeleteFile(string newFileId, CancellationToken token)
    {
        var deleteRequest = _driveService.Files.Delete(newFileId);
        await deleteRequest.ExecuteAsync(token);
    }

    internal byte[] DownloadFile(string fileId)
    {
        using var stream = new MemoryStream();
        var request = _driveService.Files.Export(fileId, "application/pdf");
        request.DownloadWithStatus(stream);
        return stream.ToArray();
    }

    protected sealed override DriveService BuildService()
    {
        return new DriveService(BuildInitializer());
    }
}