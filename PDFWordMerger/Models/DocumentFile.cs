using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace PDFWordMerger.Models
{
    public class DocumentFile : INotifyPropertyChanged
    {
        private int _order;
        private int _pageCount;

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileTypeIcon { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public int Order
        {
            get => _order;
            set
            {
                _order = value;
                OnPropertyChanged();
            }
        }

        public int PageCount
        {
            get => _pageCount;
            set
            {
                _pageCount = value;
                OnPropertyChanged();
            }
        }

        public DocumentFile(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            
            string extension = Path.GetExtension(filePath).ToLower();
            
            if (extension == ".pdf")
            {
                FileType = "PDF Document";
                FileTypeIcon = "📄";
            }
            else if (extension == ".docx" || extension == ".doc")
            {
                FileType = "Word Document";
                FileTypeIcon = "📝";
            }
            else
            {
                FileType = "Unknown";
                FileTypeIcon = "📋";
            }

            FileInfo fileInfo = new FileInfo(filePath);
            ModifiedDate = fileInfo.LastWriteTime;
            CreatedDate = fileInfo.CreationTime;
            PageCount = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
