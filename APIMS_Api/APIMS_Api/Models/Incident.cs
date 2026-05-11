namespace APIMS_Api.Models
{
    public class Incident
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ReportedBy { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public DateTime ReportedAt { get; set; }
    }

    public class CreateIncidentRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ReportedBy { get; set; } = string.Empty;
    }

    public class UpdateIncidentRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
    }
}
