using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PDFWordMerger.Services
{
    public class PdfService
    {
        public int GetPageCount(string pdfPath)
        {
            try
            {
                using var pdfReader = new PdfReader(pdfPath);
                using var pdfDocument = new PdfDocument(pdfReader);
                return pdfDocument.GetNumberOfPages();
            }
            catch
            {
                return 0;
            }
        }

        public void AddPageNumbers(string inputPath, string outputPath, int startPageNumber = 1)
        {
            using var pdfReader = new PdfReader(inputPath);
            using var pdfWriter = new PdfWriter(outputPath);
            using var pdfDocument = new PdfDocument(pdfReader, pdfWriter);
            using var document = new Document(pdfDocument);

            int numberOfPages = pdfDocument.GetNumberOfPages();

            for (int i = 1; i <= numberOfPages; i++)
            {
                var page = pdfDocument.GetPage(i);
                var pageSize = page.GetPageSize();
                
                float x = pageSize.GetWidth() / 2;
                float y = 20;

                document.ShowTextAligned(
                    new Paragraph($"Page {startPageNumber + i - 1}"),
                    x, y, i,
                    TextAlignment.CENTER,
                    VerticalAlignment.BOTTOM, 0);
            }
        }

        public void CopyPdfPages(PdfDocument sourcePdf, PdfDocument targetPdf, int startPage, int endPage)
        {
            sourcePdf.CopyPagesTo(startPage, endPage, targetPdf);
        }

        public void AddPageNumbersToPdf(PdfDocument pdfDocument, int startPageNumber)
        {
            using var document = new Document(pdfDocument);
            int numberOfPages = pdfDocument.GetNumberOfPages();

            for (int i = 1; i <= numberOfPages; i++)
            {
                var page = pdfDocument.GetPage(i);
                var pageSize = page.GetPageSize();
                
                float x = pageSize.GetWidth() / 2;
                float y = 20;

                document.ShowTextAligned(
                    new Paragraph($"Page {startPageNumber + i - 1}"),
                    x, y, i,
                    TextAlignment.CENTER,
                    VerticalAlignment.BOTTOM, 0);
            }
        }
    }
}
