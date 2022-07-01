using Givt.Platform.Email.Enums;
using Givt.Platform.Email.Interfaces;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PostmarkDotNet;
using System.Globalization;

namespace Givt.Platform.Email.Postmark;

// Setup polymorphic dispatch, this handler is now a "generic" handler for every IEmailNotification
public class PostmarkEmailService<TNotification> : INotificationHandler<TNotification>
    where TNotification : IEmailNotification
{
    private const string S_DEBUG_PREFIX = "{DEBUG} ";

    private readonly PostmarkOptions _options;
    private readonly PostmarkClient _postmarkClient;
    private readonly string[] _forbiddenExts = { "vbs", "exe", "bin", "bat", "chm", "com", "cpl", "crt", "hlp", "hta", "inf", "ins", "isp", "jse", "lnk", "mdb", "pcd", "pif", "reg", "scr", "sct", "shs", "vbe", "vba", "wsf", "wsh", "wsl", "msc", "msi", "msp", "mst" };

    public PostmarkEmailService(PostmarkOptions options)
    {
        _options = options;
        _postmarkClient = new PostmarkClient(_options.ApiKey);
        // TODO: let Postmark use a HttpClient from "our" pool
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        if (notification is not IEmailNotification emailData)
            throw new ArgumentException("IEmailNotification needed");
        EnsureSettingsAreValid();
        if (String.IsNullOrWhiteSpace(emailData.To))
            throw new ArgumentNullException("emailData.To");

        Thread.CurrentThread.CurrentCulture = new CultureInfo(emailData.Locale);
        string template;
        PostmarkMessageBase message;
        switch (emailData.EmailType)
        {
            case EmailType.Plain:
                template = String.Empty;
                break;
            case EmailType.SingleDonation:
                template = _options.MailReportSingleDonationTemplate;
                break;
            default:
                throw new NotSupportedException(emailData.EmailType.ToString());
        }
        if (String.IsNullOrWhiteSpace(template))
        {
            if (String.IsNullOrWhiteSpace(emailData.Subject))
                throw new ArgumentNullException("emailData.Subject");
            if (String.IsNullOrWhiteSpace(emailData.HtmlBody))
                throw new ArgumentNullException("emailData.HtmlBody");
            message = new PostmarkMessage()
            {
                Subject = _options.EnvironmentName == "Development" ?
                    S_DEBUG_PREFIX + emailData.Subject :
                    emailData.Subject,
                HtmlBody = emailData.HtmlBody
            };
        }
        else
        {
            message = new TemplatedPostmarkMessage()
            {
                TemplateAlias = template,
                TemplateModel = ConvertTemplateData(emailData.TemplateData)
            };
        }

        message.To = emailData.To;
        message.From = emailData.From ?? $"{_options.SupportName} {_options.SupportAddress}"; // To include a name, use the format "Full Name sender@domain.com" for the address.
        message.TrackLinks = LinkTrackingOptions.HtmlAndText;

        ValidateAndAssignCcAddresses(message, emailData.Cc);
        ValidateAndAssignAttachment(message,
            emailData.Attachment,
            emailData.AttachmentFileName,
            emailData.AttachmentContentType);
        ValidateAndAddAttachments(message, emailData.AttachmentFiles);

        var result = String.IsNullOrWhiteSpace(template) ?
            await _postmarkClient.SendMessageAsync(message as PostmarkMessage) :
            await _postmarkClient.SendMessageAsync(message as TemplatedPostmarkMessage);

        if (result.Status != PostmarkStatus.Success)
            throw new Exception("Postmark Response = " + result.Status);
    }

    private JObject ConvertTemplateData(object templateData)
    {
        // convert the object passed in emailData to a JObject to send to the mail server
        // add special serializer that ignores reference loops and converts locale to Postmark stuff
        var serializerSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        serializerSettings.Converters.Add(new LocaleConverter(templateData.GetType()));
        var result = JObject.FromObject(templateData, JsonSerializer.CreateDefault(serializerSettings));

        if (_options.EnvironmentName == "Development")
            result["SubjectPrefix"] = S_DEBUG_PREFIX;

        return result;
    }

    #region private helpers
    private void ValidateAndAssignCcAddresses(PostmarkMessageBase message, string cc)
    {
        if (cc == null)
            return;
        var addresses = cc.Split(',', ';', ' ');
        var validEmailAddresses = addresses.Where(address => !String.IsNullOrWhiteSpace(address));
        message.Cc = string.Join(",", validEmailAddresses);
    }
    private void ValidateAndAssignAttachment(PostmarkMessageBase message, byte[] attachment, string attachmentName,
        string attachmentContentType)
    {
        if (attachmentName == null && attachment == null)
            return;

        if ((attachmentName == null && attachment != null) || (attachmentName != null && attachment == null))
            throw new ArgumentNullException("attachmentName or attachment is missing");

        var ext = Path.GetExtension(attachmentName).TrimStart('.');
        if (_forbiddenExts.Any(forbiddenValue => ext.Equals(forbiddenValue, StringComparison.InvariantCulture)))
            throw new Exception($"This attachment extension of '{attachmentName}' is forbidden by postmark");

        if (attachment.Length <= 0)
            throw new ArgumentException("Zero sized attachments are not allowed");

        message.AddAttachment(attachment, attachmentName, attachmentContentType ?? "application/octet-stream");
    }
    private void ValidateAndAddAttachments(PostmarkMessageBase message, List<string> attachmentFiles)
    {
        if (attachmentFiles == null)
            return;

        foreach (var path in attachmentFiles)
        {
            if (path == null)
                return;
            if (!File.Exists(path))
                throw new ArgumentNullException($"File to attach does not exists '{path}'");

            if (_forbiddenExts.Any(forbiddenValue => path.EndsWith(forbiddenValue, StringComparison.InvariantCulture)))
                throw new Exception($"This attachment extension {path} is forbidden by postmark");
            using var stream = File.OpenRead(path);
            message.AddAttachment(stream, Path.GetFileName(path));
        }
    }

    private void EnsureSettingsAreValid()
    {
        if (string.IsNullOrEmpty(_options.ApiKey))
            throw new ArgumentNullException(nameof(PostmarkOptions.ApiKey));

        if (string.IsNullOrEmpty(_options.SupportName))
            throw new ArgumentNullException(nameof(PostmarkOptions.SupportName));

        if (string.IsNullOrEmpty(_options.SupportAddress))
            throw new ArgumentNullException(nameof(PostmarkOptions.SupportAddress));
    }

    #endregion
}
