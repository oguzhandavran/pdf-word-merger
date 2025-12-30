using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using OpenXmlTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using OpenXmlDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Layout;
using PdfParagraph = iText.Layout.Element.Paragraph;
using PdfDocument = iText.Layout.Document;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.Linq;

namespace PDFWordMerger.Services
{
    public class WordService
    {
        public int GetPageCount(string wordPath)
        {
            try
            {
                if (wordPath.EndsWith(".doc", StringComparison.OrdinalIgnoreCase))
                {
                    return EstimatePageCount(wordPath);
                }

                using var wordDocument = WordprocessingDocument.Open(wordPath, false);
                var body = wordDocument.MainDocumentPart?.Document?.Body;
                
                if (body == null)
                    return 1;

                var pageBreaks = body.Descendants<OpenXmlParagraph>()
                    .Count(p => p.Descendants<Break>().Any(b => b.Type != null && b.Type == BreakValues.Page));

                int estimatedPages = Math.Max(1, pageBreaks + 1);
                
                var paragraphCount = body.Descendants<OpenXmlParagraph>().Count();
                if (paragraphCount > 50)
                {
                    estimatedPages = Math.Max(estimatedPages, paragraphCount / 40);
                }

                return estimatedPages;
            }
            catch
            {
                return 1;
            }
        }

        private int EstimatePageCount(string wordPath)
        {
            try
            {
                var fileInfo = new FileInfo(wordPath);
                long fileSizeKB = fileInfo.Length / 1024;
                return Math.Max(1, (int)(fileSizeKB / 50));
            }
            catch
            {
                return 1;
            }
        }

        public void ConvertWordToPdf(string wordPath, string pdfPath)
        {
            try
            {
                using var wordDocument = WordprocessingDocument.Open(wordPath, false);
                using var pdfWriter = new PdfWriter(pdfPath);
                using var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter);
                using var document = new PdfDocument(pdfDocument);

                var body = wordDocument.MainDocumentPart?.Document?.Body;
                if (body == null)
                {
                    document.Add(new PdfParagraph("Unable to read Word document content."));
                    return;
                }

                var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                
                foreach (var element in body.Elements())
                {
                    if (element is OpenXmlParagraph para)
                    {
                        var text = para.InnerText;
                        
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            var paragraph = new PdfParagraph(text);
                            paragraph.SetFont(font);
                            
                            var runProps = para.Descendants<RunProperties>().FirstOrDefault();
                            if (runProps != null)
                            {
                                if (runProps.Bold != null)
                                {
                                    var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                                    paragraph.SetFont(boldFont);
                                }
                                
                                if (runProps.FontSize != null && int.TryParse(runProps.FontSize.Val, out int fontSize))
                                {
                                    paragraph.SetFontSize(fontSize / 2f);
                                }
                            }
                            
                            var paraProps = para.ParagraphProperties;
                            if (paraProps != null)
                            {
                                var justification = paraProps.Justification;
                                if (justification?.Val?.Value == JustificationValues.Center)
                                {
                                    paragraph.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                }
                                else if (justification?.Val?.Value == JustificationValues.Right)
                                {
                                    paragraph.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                                }
                            }
                            
                            document.Add(paragraph);
                        }
                        else
                        {
                            document.Add(new PdfParagraph("\n"));
                        }
                    }
                    else if (element is OpenXmlTable table)
                    {
                        document.Add(new PdfParagraph("[Table content]"));
                    }
                }
            }
            catch (Exception ex)
            {
                using var pdfWriter = new PdfWriter(pdfPath);
                using var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter);
                using var document = new PdfDocument(pdfDocument);
                
                document.Add(new PdfParagraph($"Error converting Word document: {ex.Message}"));
                document.Add(new PdfParagraph($"\nOriginal file: {Path.GetFileName(wordPath)}"));
                document.Add(new PdfParagraph("\nNote: This is a placeholder. The original Word document could not be fully converted."));
            }
        }
    }
}
