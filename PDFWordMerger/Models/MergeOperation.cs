using System;
using System.Collections.Generic;

namespace PDFWordMerger.Models
{
    public class MergeOperation
    {
        public List<DocumentFile> Documents { get; set; }
        public string OutputPath { get; set; }
        public bool AddPageNumbers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public MergeOperation()
        {
            Documents = new List<DocumentFile>();
            OutputPath = string.Empty;
            AddPageNumbers = true;
        }
    }
}
