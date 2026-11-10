using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
namespace Auto;
public static class Documents
{
    public static string Extract(string path, string mime)
    {
        if (mime == "text/plain") return File.ReadAllText(path);
        if (mime is "image/jpeg" or "image/png") return Ocr.Read(File.ReadAllBytes(path));
        if (mime != "application/pdf") throw new ArgumentException("Format non pris en charge pour la lecture.");
        try
        {
            using var document = PdfDocument.Open(path);
            if (document.NumberOfPages > 50) throw new ArgumentException("Découpez le document : maximum 50 pages par extraction.");
            var text = new StringBuilder(); int scanned=0;
            foreach (var page in document.GetPages())
            {
                text.AppendLine($"[Page {page.Number}]"); text.AppendLine(ContentOrderTextExtractor.GetText(page));
                if(page.Letters.Count<20){
                    if(++scanned>10)throw new ArgumentException("Découpez le scan en lots de 10 pages au maximum.");
                    var images=page.GetImages().ToArray();if(images.Length==0)text.AppendLine("[Aucun texte ou image exploitable ; page à vérifier visuellement.]");
                    if(images.Length>8)throw new ArgumentException("Page composée de trop de fragments : importez une image complète de la page.");
                    foreach(var image in images){if((long)image.WidthInSamples*image.HeightInSamples>25000000)throw new ArgumentException("Image du scan trop grande : réduisez sa résolution.");
                        byte[] bytes;if(image.TryGetPng(out var png))bytes=png;else if(image.RawBytes.Length>2&&image.RawBytes[0]==255&&image.RawBytes[1]==216)bytes=image.RawBytes.ToArray();else{ text.AppendLine("[Image non décodable ; importez cette page en PNG/JPEG.]");continue;}
                        text.AppendLine(Ocr.Read(bytes));
                    }
                }
                if (text.Length > 100000) throw new ArgumentException("Texte trop volumineux : importez une partie du document.");
            }
            
            return text.ToString();
        }
        catch (ArgumentException) { throw; }
        catch (Exception) { throw new ArgumentException("PDF illisible ou protégé. Importez une copie lisible ou collez son texte."); }
    }
}

