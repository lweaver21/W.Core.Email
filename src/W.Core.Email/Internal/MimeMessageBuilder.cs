using MimeKit;
using W.Core.Email.Models;

namespace W.Core.Email.Internal;

/// <summary>
/// Builds MimeKit messages from EmailMessage objects.
/// </summary>
internal static class MimeMessageBuilder
{
    /// <summary>
    /// Builds a MimeMessage from an EmailMessage.
    /// </summary>
    /// <param name="email">The email message.</param>
    /// <param name="defaultFromEmail">Default sender email if not specified.</param>
    /// <param name="defaultFromName">Default sender name if not specified.</param>
    /// <returns>A MimeKit message ready to send.</returns>
    public static MimeMessage Build(EmailMessage email, string defaultFromEmail, string? defaultFromName = null)
    {
        var message = new MimeMessage();

        // Set sender
        var fromEmail = email.From ?? defaultFromEmail;
        var fromName = email.FromName ?? defaultFromName;
        message.From.Add(new MailboxAddress(fromName ?? fromEmail, fromEmail));

        // Set recipients
        foreach (var to in email.To)
        {
            message.To.Add(MailboxAddress.Parse(to));
        }

        foreach (var cc in email.Cc)
        {
            message.Cc.Add(MailboxAddress.Parse(cc));
        }

        foreach (var bcc in email.Bcc)
        {
            message.Bcc.Add(MailboxAddress.Parse(bcc));
        }

        // Set reply-to
        if (!string.IsNullOrEmpty(email.ReplyTo))
        {
            message.ReplyTo.Add(MailboxAddress.Parse(email.ReplyTo));
        }

        // Set subject
        message.Subject = email.Subject;

        // Set priority
        message.Priority = email.Priority switch
        {
            EmailPriority.Low => MessagePriority.NonUrgent,
            EmailPriority.High => MessagePriority.Urgent,
            EmailPriority.Urgent => MessagePriority.Urgent,
            _ => MessagePriority.Normal
        };

        if (email.Priority == EmailPriority.Urgent)
        {
            message.Headers.Add("X-Priority", "1");
        }

        // Set custom headers
        foreach (var header in email.CustomHeaders)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        // Build body with attachments
        var body = BuildBody(email);
        message.Body = body;

        return message;
    }

    private static MimeEntity BuildBody(EmailMessage email)
    {
        var hasAttachments = email.Attachments.Count > 0;

        // Create text part
        TextPart textPart;
        if (email.IsHtml)
        {
            textPart = new TextPart("html") { Text = email.Body };
        }
        else
        {
            textPart = new TextPart("plain") { Text = email.Body };
        }

        if (!hasAttachments)
        {
            return textPart;
        }

        // Build multipart with attachments
        var multipart = new Multipart("mixed");
        multipart.Add(textPart);

        foreach (var attachment in email.Attachments)
        {
            var mimePart = new MimePart(attachment.ContentType)
            {
                Content = new MimeContent(new MemoryStream(attachment.Content)),
                ContentDisposition = new ContentDisposition(
                    attachment.IsInline ? ContentDisposition.Inline : ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = attachment.FileName
            };

            if (!string.IsNullOrEmpty(attachment.ContentId))
            {
                mimePart.ContentId = attachment.ContentId;
            }

            multipart.Add(mimePart);
        }

        return multipart;
    }

    /// <summary>
    /// Encodes a MimeMessage to base64url format for Gmail API.
    /// </summary>
    /// <param name="message">The MIME message.</param>
    /// <returns>Base64url-encoded message.</returns>
    public static string ToBase64UrlString(MimeMessage message)
    {
        using var stream = new MemoryStream();
        message.WriteTo(stream);
        var bytes = stream.ToArray();
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
