namespace SmtpToPaperless
{
    public class Configuration
    {
        public Uri? PaperlessBaseUrl { get; set; }
        public string? PaperlessUsername { get; set; }
        public string? PaperlessPassword { get; set; }
        public string? RelayFrom { get; set; }
        public string? RelayHost { get; set; }
        public int? RelayPort { get; set; } = 587;
        public string? RelayUsername { get; set; }
        public string? RelayPassword { get; set; }
        public string? RelayFor { get; set; }
        internal string[]? RelayForSplit =>
            RelayFor?.Split(",");
    }
}