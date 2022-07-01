using Givt.Platform.Email.Enums;
using MediatR;

namespace Givt.Platform.Email.Interfaces;

public interface IEmailNotification : INotification
{
    EmailType EmailType { get; set; }
    string From { get; }
    string To { get; }
    string Cc { get; }
    string Bcc { get; }
    string Subject { get; }
    string Tag { get; }
    object TemplateData { get; }
    string HtmlBody { get; }
    string ReplyTo { get; }
    Dictionary<string, string> Headers { get; }
    bool TrackOpens { get; }
    string TrackLinks { get; }
    Dictionary<string, string> Metadata { get; }
    byte[] Attachment { get; }
    string AttachmentFileName { get; }
    string AttachmentContentType { get; set; }
    List<string> AttachmentFiles { get; }
    string Locale { get; }
}

