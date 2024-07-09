﻿namespace ExamPortalApp.Contracts.Data.Dtos.Custom
{
    public class EmailDto
    {
        public string MessageBody { get; set; } = string.Empty;
        public List<string> EmailAddesses { get; set; } = [];
        public List<string> CcAddresses { get; set; } = [];
        public List<string> BccAddresses { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
    }
}
