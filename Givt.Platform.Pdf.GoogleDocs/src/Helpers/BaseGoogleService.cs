using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Http;
using Google.Apis.Services;

namespace Givt.Platform.Pdf.GoogleDocs.Helpers;

public abstract class BaseGoogleService<T> where T : BaseClientService
{
    private readonly GoogleDocsOptions _options;
    protected readonly string[] _userScopes = { DocsService.Scope.Drive, DocsService.Scope.Documents };
    

    protected BaseGoogleService(GoogleDocsOptions options)
    {
        _options = options;
    }

    protected abstract T BuildService();

    internal BaseClientService.Initializer BuildInitializer()
    {
        // create credentials
        return new BaseClientService.Initializer()
        {
            HttpClientInitializer = LoadCredentials(),
            ApplicationName = _options.ApplicationName,            
        };
    }
    
    private IConfigurableHttpClientInitializer LoadCredentials()
    {
        return new ServiceAccountCredential(
            new ServiceAccountCredential.Initializer(_options.ServiceAccountEmail)
            {
                Scopes = _userScopes
            }.FromPrivateKey(_options.PrivateKey));
    }
    
}