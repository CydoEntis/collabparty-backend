namespace CollabParty.Application.Common.Interfaces;

public interface IEmailTemplateService
{
    string GetEmailTemplate(string templateName, Dictionary<string, string> placeholders);
}