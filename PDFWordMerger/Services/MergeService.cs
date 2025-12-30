using System;
using System.IO;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using PDFWordMerger.Models;

namespace PDFWordMerger.Services
{
    public class MergeService
    {
        private readonly PdfService _pdfService;
        private readonly WordService _wordService;

        public MergeService()
        {
            _pdfService = new PdfService();
            _wordService = new WordService();
        }

        public bool MergeDocuments(MergeOperation operation, IProgress<double> progress)
        {
            operation.StartTime = DateTime.Now;

            try
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "PDFWordMerger_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                var tempPdfFiles = ConvertAllToPdf(operation.Documents, tempDir, progress);

                MergePdfsWithPageNumbers(tempPdfFiles, operation.OutputPath, progress);

                foreach (var tempFile in tempPdfFiles)
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }

                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);

                operation.IsSuccess = true;
                operation.EndTime = DateTime.Now;
                return true;
            }
            catch (Exception ex)
            {
                operation.IsSuccess = false;
                operation.ErrorMessage = ex.Message;
                operation.EndTime = DateTime.Now;
                return false;
            }
        }

        private string[] ConvertAllToPdf(System.Collections.Generic.List<DocumentFile> documents, string tempDir, IProgress<double> progress)
        {
            var tempPdfFiles = new string[documents.Count];
            
            for (int i = 0; i < documents.Count; i++)
            {
                var doc = documents[i];
                string tempPdfPath = Path.Combine(tempDir, $"temp_{i}_{Path.GetFileNameWithoutExtension(doc.FileName)}.pdf");
                
                string extension = Path.GetExtension(doc.FilePath).ToLower();
                
                if (extension == ".pdf")
                {
                    File.Copy(doc.FilePath, tempPdfPath, true);
                }
                else if (extension == ".docx" || extension == ".doc")
                {
                    _wordService.ConvertWordToPdf(doc.FilePath, tempPdfPath);
                }
                
                tempPdfFiles[i] = tempPdfPath;
                
                progress?.Report((i + 1) * 50.0 / documents.Count);
            }
            
            return tempPdfFiles;
        }

        private void MergePdfsWithPageNumbers(string[] pdfFiles, string outputPath, IProgress<double> progress)
        {
            using var pdfWriter = new PdfWriter(outputPath);
            using var mergedPdf = new PdfDocument(pdfWriter);
            using var document = new Document(mergedPdf);

            int totalFiles = pdfFiles.Length;

            for (int fileIndex = 0; fileIndex < pdfFiles.Length; fileIndex++)
            {
                string pdfFile = pdfFiles[fileIndex];
                
                if (!File.Exists(pdfFile))
                    continue;

                using var pdfReader = new PdfReader(pdfFile);
                using var sourcePdf = new PdfDocument(pdfReader);
                
                int numberOfPages = sourcePdf.GetNumberOfPages();
                
                for (int pageNum = 1; pageNum <= numberOfPages; pageNum++)
                {
                    var page = sourcePdf.GetPage(pageNum);
                    var copiedPage = page.CopyTo(mergedPdf);
                    mergedPdf.AddPage(copiedPage);
                }
                
                progress?.Report(50 + ((fileIndex + 1) * 25.0 / totalFiles));
            }

            int totalMergedPages = mergedPdf.GetNumberOfPages();
            
            for (int i = 1; i <= totalMergedPages; i++)
            {
                var page = mergedPdf.GetPage(i);
                var pageSize = page.GetPageSize();
                
                float x = pageSize.GetWidth() / 2;
                float y = 20;

                document.ShowTextAligned(
                    new Paragraph($"Page {i}"),
                    x, y, i,
                    TextAlignment.CENTER,
                    VerticalAlignment.BOTTOM, 0);
                
                progress?.Report(75 + (i * 25.0 / totalMergedPages));
            }

            progress?.Report(100);
        }
    }
}
