namespace W.Core.Email.Models;

/// <summary>
/// Represents an email attachment.
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// The file name of the attachment.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// The MIME content type of the attachment (e.g., "application/pdf", "image/png").
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// The binary content of the attachment.
    /// </summary>
    public required byte[] Content { get; set; }

    /// <summary>
    /// Optional Content-ID for inline attachments (used in HTML emails).
    /// </summary>
    public string? ContentId { get; set; }

    /// <summary>
    /// Whether this attachment should be displayed inline (e.g., embedded images).
    /// </summary>
    public bool IsInline { get; set; } = false;

    /// <summary>
    /// Creates an attachment from a file path.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <param name="contentType">Optional content type. If not provided, will be inferred from extension.</param>
    /// <returns>A new EmailAttachment instance.</returns>
    public static EmailAttachment FromFile(string filePath, string? contentType = null)
    {
        var fileName = Path.GetFileName(filePath);
        var content = File.ReadAllBytes(filePath);
        var mimeType = contentType ?? GetMimeType(fileName);

        return new EmailAttachment
        {
            FileName = fileName,
            ContentType = mimeType,
            Content = content
        };
    }

    /// <summary>
    /// Creates an attachment from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the attachment content.</param>
    /// <param name="fileName">The file name for the attachment.</param>
    /// <param name="contentType">The MIME content type.</param>
    /// <returns>A new EmailAttachment instance.</returns>
    public static EmailAttachment FromStream(Stream stream, string fileName, string contentType)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        return new EmailAttachment
        {
            FileName = fileName,
            ContentType = contentType,
            Content = memoryStream.ToArray()
        };
    }

    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".csv" => "text/csv",
            ".zip" => "application/zip",
            ".json" => "application/json",
            ".xml" => "application/xml",
            _ => "application/octet-stream"
        };
    }
}
