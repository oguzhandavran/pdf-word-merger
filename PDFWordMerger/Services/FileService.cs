using System;
using System.IO;

namespace PDFWordMerger.Services
{
    public class FileService
    {
        private readonly PdfService _pdfService;
        private readonly WordService _wordService;

        public FileService()
        {
            _pdfService = new PdfService();
            _wordService = new WordService();
        }

        public int GetPageCount(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            try
            {
                if (extension == ".pdf")
                {
                    return _pdfService.GetPageCount(filePath);
                }
                else if (extension == ".docx" || extension == ".doc")
                {
                    return _wordService.GetPageCount(filePath);
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
