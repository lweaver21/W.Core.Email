namespace W.Core.Email.Templating;

/// <summary>
/// Builder for creating styled HTML email templates.
/// </summary>
public class HtmlEmailBuilder
{
    private string? _preheader;
    private string? _headerHtml;
    private string? _bodyHtml;
    private string? _footerHtml;
    private string _primaryColor = "#3498db";
    private string _backgroundColor = "#f4f4f4";
    private string _textColor = "#333333";
    private string _fontFamily = "Arial, Helvetica, sans-serif";

    /// <summary>
    /// Sets the preheader text (preview text in email clients).
    /// </summary>
    public HtmlEmailBuilder WithPreheader(string preheader)
    {
        _preheader = preheader;
        return this;
    }

    /// <summary>
    /// Sets the header section HTML.
    /// </summary>
    public HtmlEmailBuilder WithHeader(string headerHtml)
    {
        _headerHtml = headerHtml;
        return this;
    }

    /// <summary>
    /// Sets the header with a logo image.
    /// </summary>
    public HtmlEmailBuilder WithLogoHeader(string logoUrl, string? altText = null, int width = 150)
    {
        _headerHtml = $@"<img src=""{logoUrl}"" alt=""{altText ?? "Logo"}"" width=""{width}"" style=""display: block; margin: 0 auto;"" />";
        return this;
    }

    /// <summary>
    /// Sets the header with text.
    /// </summary>
    public HtmlEmailBuilder WithTextHeader(string text)
    {
        _headerHtml = $@"<h1 style=""color: {_primaryColor}; font-size: 24px; margin: 0; padding: 20px 0;"">{text}</h1>";
        return this;
    }

    /// <summary>
    /// Sets the main body content.
    /// </summary>
    public HtmlEmailBuilder WithBody(string bodyHtml)
    {
        _bodyHtml = bodyHtml;
        return this;
    }

    /// <summary>
    /// Sets the footer section.
    /// </summary>
    public HtmlEmailBuilder WithFooter(string footerHtml)
    {
        _footerHtml = footerHtml;
        return this;
    }

    /// <summary>
    /// Sets a simple text footer with company info.
    /// </summary>
    public HtmlEmailBuilder WithSimpleFooter(string companyName, string? unsubscribeUrl = null)
    {
        var unsubscribeLink = unsubscribeUrl != null
            ? $@"<br><a href=""{unsubscribeUrl}"" style=""color: #999999;"">Unsubscribe</a>"
            : "";
        _footerHtml = $@"<p style=""color: #999999; font-size: 12px;"">&copy; {DateTime.UtcNow.Year} {companyName}. All rights reserved.{unsubscribeLink}</p>";
        return this;
    }

    /// <summary>
    /// Sets the primary brand color.
    /// </summary>
    public HtmlEmailBuilder WithPrimaryColor(string hexColor)
    {
        _primaryColor = hexColor;
        return this;
    }

    /// <summary>
    /// Sets the background color.
    /// </summary>
    public HtmlEmailBuilder WithBackgroundColor(string hexColor)
    {
        _backgroundColor = hexColor;
        return this;
    }

    /// <summary>
    /// Sets the text color.
    /// </summary>
    public HtmlEmailBuilder WithTextColor(string hexColor)
    {
        _textColor = hexColor;
        return this;
    }

    /// <summary>
    /// Sets the font family.
    /// </summary>
    public HtmlEmailBuilder WithFontFamily(string fontFamily)
    {
        _fontFamily = fontFamily;
        return this;
    }

    /// <summary>
    /// Builds the complete HTML email body.
    /// </summary>
    public string Build()
    {
        var preheaderHtml = string.IsNullOrEmpty(_preheader) ? "" :
            $@"<div style=""display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden;"">{_preheader}</div>";

        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email</title>
</head>
<body style=""margin: 0; padding: 0; font-family: {_fontFamily}; background-color: {_backgroundColor};"">
    {preheaderHtml}
    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {_backgroundColor};"">
        <tr>
            <td align=""center"" style=""padding: 40px 10px;"">
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    {BuildHeaderSection()}
                    {BuildBodySection()}
                    {BuildFooterSection()}
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string BuildHeaderSection()
    {
        if (string.IsNullOrEmpty(_headerHtml)) return "";

        return $@"
                    <tr>
                        <td style=""padding: 30px 40px; text-align: center; border-bottom: 1px solid #eeeeee;"">
                            {_headerHtml}
                        </td>
                    </tr>";
    }

    private string BuildBodySection()
    {
        return $@"
                    <tr>
                        <td style=""padding: 40px; color: {_textColor}; font-size: 16px; line-height: 1.6;"">
                            {_bodyHtml ?? ""}
                        </td>
                    </tr>";
    }

    private string BuildFooterSection()
    {
        if (string.IsNullOrEmpty(_footerHtml)) return "";

        return $@"
                    <tr>
                        <td style=""padding: 20px 40px; text-align: center; border-top: 1px solid #eeeeee; background-color: #fafafa; border-radius: 0 0 8px 8px;"">
                            {_footerHtml}
                        </td>
                    </tr>";
    }

    /// <summary>
    /// Creates a call-to-action button HTML.
    /// </summary>
    public string CreateButton(string text, string url, string? backgroundColor = null)
    {
        var bgColor = backgroundColor ?? _primaryColor;
        return $@"<table role=""presentation"" cellpadding=""0"" cellspacing=""0"" style=""margin: 20px auto;"">
    <tr>
        <td style=""background-color: {bgColor}; border-radius: 6px; padding: 12px 30px;"">
            <a href=""{url}"" style=""color: #ffffff; text-decoration: none; font-weight: bold; font-size: 16px; display: inline-block;"">{text}</a>
        </td>
    </tr>
</table>";
    }

    /// <summary>
    /// Creates a styled heading.
    /// </summary>
    public string CreateHeading(string text, int level = 2)
    {
        var fontSize = level switch
        {
            1 => "28px",
            2 => "22px",
            3 => "18px",
            _ => "16px"
        };
        return $@"<h{level} style=""color: {_textColor}; font-size: {fontSize}; margin: 0 0 15px 0;"">{text}</h{level}>";
    }

    /// <summary>
    /// Creates a paragraph.
    /// </summary>
    public string CreateParagraph(string text)
    {
        return $@"<p style=""margin: 0 0 15px 0;"">{text}</p>";
    }

    /// <summary>
    /// Creates a divider line.
    /// </summary>
    public string CreateDivider()
    {
        return @"<hr style=""border: none; border-top: 1px solid #eeeeee; margin: 20px 0;"" />";
    }
}
