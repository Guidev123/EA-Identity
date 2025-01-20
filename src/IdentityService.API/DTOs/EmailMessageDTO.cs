namespace IdentityService.API.DTOs
{
    public class EmailMessageDTO
    {
        public EmailMessageDTO(string to, string subject, string content, IFormFile? attachments = null)
        {
            To = to;
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }

        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public IFormFile? Attachments { get; set; }
    }
}
