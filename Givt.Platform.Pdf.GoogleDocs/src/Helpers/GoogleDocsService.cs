using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;

namespace Givt.Platform.Pdf.GoogleDocs.Helpers;

public class GoogleDocsService : BaseGoogleService<DocsService>
{
    public GoogleDocsService(GoogleDocsOptions options) : base(options) 
    { }

    protected override DocsService BuildService()
    {
        return new DocsService(BuildInitializer());
    }

    internal async Task FillInAFile(string fileId, Dictionary<string, string> parameters, CancellationToken token)
    {
        if (!parameters.Any())
            return;
        // Create Google Docs API service.
        var service = BuildService();

        // Fill in the file
        var requests = new List<Request>();

        // TODO: make this "text hiding" stuff configurable
        if (String.IsNullOrWhiteSpace(parameters["Last4"]))
        {
            requests.Add(CreateRemoveTextRequest("nr. x{Last4}"));
            requests.Add(CreateRemoveTextRequest("nr x{Last4}")); // both variants
        }

        foreach (var parameter in parameters)
        {
            requests.Add(CreateFillTagRequest(parameter.Key, parameter.Value));
        }

        // remove text labels for lines without values
        if (String.IsNullOrWhiteSpace(parameters["RSIN"]))
            requests.Add(CreateRemoveLineRequest("RSIN:"));
        if (String.IsNullOrWhiteSpace(parameters["HmrcReference"]))
            requests.Add(CreateRemoveLineRequest("HMRC Reference:"));
        if (String.IsNullOrWhiteSpace(parameters["CharityID"]))
            requests.Add(CreateRemoveLineRequest("Charity Number:"));

        var body = new BatchUpdateDocumentRequest { Requests = requests };


        var request = service.Documents.BatchUpdate(body, fileId);
        await request.ExecuteAsync(token);
    }

    private static Request CreateFillTagRequest(string key, string value)
    {
        return new Request()
        {
            ReplaceAllText = new ReplaceAllTextRequest
            {
                ContainsText = new SubstringMatchCriteria
                {
                    Text = "{" + key + "}",
                    MatchCase = false,
                },
                ReplaceText = value
            },
        };
    }

    private static Request CreateRemoveTextRequest(string text)
    {
        return new Request()
        {
            ReplaceAllText = new ReplaceAllTextRequest
            {
                ContainsText = new SubstringMatchCriteria
                {
                    Text = text,
                    MatchCase = false,
                },
                ReplaceText = String.Empty
            },
        };
    }

    private static Request CreateRemoveLineRequest(string searchText)
    {
        // Google Docs do not have named Bookmarks or other stuff that allows to do something generic.
        // So here we only search for some text and replace it with nothing. 
        return new Request()
        {
            ReplaceAllText = new ReplaceAllTextRequest
            {
                ContainsText = new SubstringMatchCriteria
                {
                    Text = searchText, // + "\t" this works for the TAB, but we also need the newline character(s)
                    MatchCase = false,
                },
                ReplaceText = String.Empty
            }
        };
    }
}