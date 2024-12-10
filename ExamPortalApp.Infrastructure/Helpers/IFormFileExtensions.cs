using Aspose.Words;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;

namespace Microsoft.AspNetCore.Http
{
    internal static class IFormFileExtensions
    {
        //public static byte[] ConvertToPdf(this IFormFile wordFile)
        //{
        //    // Check the file extension
        //    string fileExtension = Path.GetExtension(wordFile.FileName);
        //    if (!string.Equals(fileExtension, ".doc", StringComparison.OrdinalIgnoreCase) &&
        //        !string.Equals(fileExtension, ".docx", StringComparison.OrdinalIgnoreCase))
        //    {
        //        throw new ArgumentException("Invalid file type. Only Word documents (.doc, .docx) are supported.");
        //    }

        //    // Create a temporary file path for the Word document
        //    string tempFilePath = Path.GetTempFileName();

        //    // Save the uploaded Word document to the temporary file path
        //    using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
        //    {
        //        wordFile.CopyTo(stream);
        //    }

        //    // Initialize Word application
        //    var wordApplication = new Microsoft.Office.Interop.Word.Application();

        //    // Open the Word document
        //    var wordDocument = wordApplication.Documents.Open(tempFilePath);

        //    // Save the Word document as PDF to a memory stream
        //    MemoryStream pdfStream = new MemoryStream();
        //    wordDocument.SaveAs(pdfStream, WdSaveFormat.wdFormatPDF);

        //    // Close the Word document and application
        //    wordDocument.Close();
        //    wordApplication.Quit();

        //    // Delete the temporary Word document file
        //    File.Delete(tempFilePath);

        //    // Return the PDF document as a byte array
        //    return pdfStream.ToArray();
        //}

        //public static byte[] ConvertToPdf(this IFormFile file)
        //{
        //    using (var stream = file.OpenReadStream())
        //    {
        //        var document = new Document(stream);

        //        using (var output = new MemoryStream())
        //        {
        //            document.Watermark.Remove();
        //            document.Save(output, SaveFormat.Pdf);
        //            //Document pdfDoc = new Document(ArtifactsDir + "PDF2Word.ConvertPdfToDocx.pdf");
        //            return output.ToArray();
        //        }
        //    }
        //}

       //public static byte[] ConvertToPdfSyncfusion(this IFormFile file)
       // {
       //      FileStream docStream = new FileStream(file.FileName, FileMode.Open, FileAccess.Read);
       //         //Loads file stream into Word document
       //         WordDocument wordDocument = new WordDocument(docStream, Syncfusion.DocIO.FormatType.Automatic);
       //         //Instantiation of DocIORenderer for Word to PDF conversion
       //         DocIORenderer render = new DocIORenderer();
       //         //Sets Chart rendering Options.
       //         render.Settings.ChartRenderingOptions.ImageFormat =  ExportImageFormat.Jpeg;
       //         //Converts Word document into PDF document
       //         PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);
      
       // }

        //public static byte[] ConvertToPdf(this IFormFile file)
        //{
        //    // Step 1: Create a Word document.
        //    FileStream docStream = new FileStream(file.FileName, FileMode.Open, FileAccess.Read);
        //    //         //Loads file stream into Word document
        //    WordDocument wordDocument = new WordDocument(docStream, Syncfusion.DocIO.FormatType.Automatic);
        //    // document.EnsureMinimal();

        //    // Add content to the document.
        //    //IWSection section = document.AddSection();
        //    //IWParagraph paragraph = section.AddParagraph();
        //    // paragraph.AppendText("Hello, this is a sample document converted to PDF.");

        //    // Step 2: Create an instance of DocIORenderer.
        //    using (DocIORenderer renderer = new DocIORenderer())
        //    {
        //        // Step 3: Convert Word document to PDF.
        //        using (MemoryStream pdfStream = new MemoryStream())
        //        {
        //            // Convert the Word document to a PDF.
        //            PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument);

        //            // Step 4: Save the PDF document to the MemoryStream.
        //            pdfDocument.Save(pdfStream);

        //            // Close the PDF document.
        //            pdfDocument.Close(true);

        //            // Optionally, write the stream to a file to verify the result (for testing purposes).
        //            File.WriteAllBytes("Output.pdf", pdfStream.ToArray());
                   
        //            return pdfStream.ToArray();
        //        }
        //    }
        //      wordDocument.Close();
        //    // Dispose the Word document.
            

        //}

        //public static byte[] ConvertToPdf(IFormFile file)
        public static byte[] ConvertToPdf(this IFormFile file)
        {
            // Check if the file is provided
            //if (file == null || file.Length == 0)
            //{
            //    return BadRequest("No file uploaded.");
            //}

            // Convert the uploaded Word document to PDF
            using (MemoryStream pdfStream = new MemoryStream())
            using (MemoryStream wordStream = new MemoryStream())
            {
                // Copy the uploaded file to a MemoryStream
                file.CopyTo(wordStream);
                wordStream.Position = 0; // Reset the stream position to the beginning

                // Load the document from the stream
                using (WordDocument document = new WordDocument(wordStream, FormatType.Docx))
                {
                    // Create an instance of DocIORenderer
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        // Convert the Word document to PDF
                        PdfDocument pdfDocument = renderer.ConvertToPDF(document);

                        // Save the PDF document to the output MemoryStream
                        pdfDocument.Save(pdfStream);
                        //return pdfStream.ToArray();
                        pdfDocument.Close(true);

                        // Set the position of the stream to the beginning
                        pdfStream.Position = 0;

                        // Return the PDF as a downloadable file
                       // return File(pdfStream, "application/pdf", "ConvertedDocument.pdf");
                        ///return wordStream.ToArray();
                        byte[] pdfBytes = pdfStream.ToArray();
                        return pdfBytes; 
                    }
                }
            }
        }

        //public static byte[] ConvertToPdf(this IFormFile file)
        //{
        //    using (var stream = file.OpenReadStream())
        //    {
        //        var document = new Document(stream);

        //        using (var output = new MemoryStream())
        //        {
        //            document.Watermark.Remove();
        //            document.Save(output, SaveFormat.Pdf);
        //            //Document pdfDoc = new Document(ArtifactsDir + "PDF2Word.ConvertPdfToDocx.pdf");
        //            return output.ToArray();
        //        }
        //    }
        //}

        public static byte[] ToByteArray(this IFormFile file, string? extension = null) 
        {
            if (extension is not null)
            {
                // Check if the file is a PDF
                string fileExtension = Path.GetExtension(file.FileName);
                if (!string.Equals(fileExtension, extension, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Invalid file type. Only {extension} files are supported.");
                }
            }

            using (MemoryStream memoryStream = new())
            {
                // Copy the uploaded file to the memory stream
                file.CopyTo(memoryStream);

                // Return the file content as a byte array
                return memoryStream.ToArray();
            }
        }
    }
}
