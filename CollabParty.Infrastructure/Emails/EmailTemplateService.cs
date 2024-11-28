namespace CollabParty.Infrastructure.Emails
{
    using CollabParty.Application.Common.Interfaces;
    using System.Collections.Generic;
    using System.IO;

    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly string _templateDirectory;

        public EmailTemplateService()
        {
            var solutionDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\"));
            _templateDirectory = Path.Combine(solutionDirectory, "CollabParty.Infrastructure", "Emails", "Templates");
        }

        public string GetEmailTemplate(string templateName, Dictionary<string, string> placeholders)
        {
            var filePath = Path.Combine(_templateDirectory, $"{templateName}.html");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Email template '{templateName}' not found at {filePath}");
            }

            var templateContent = File.ReadAllText(filePath);

            foreach (var placeholder in placeholders)
            {
                templateContent = templateContent.Replace($"[{placeholder.Key}]", placeholder.Value);
            }

            return templateContent;
        }
    }
}