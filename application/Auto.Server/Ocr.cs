using Tesseract;
namespace Auto;
public static class Ocr
{
    private static readonly object Gate=new();
    public static string Read(byte[] bytes)
    {
        lock(Gate)
        {
            try
            {
                using var image=Pix.LoadFromMemory(bytes);
                if((long)image.Width*image.Height>25000000)throw new ArgumentException("Image trop grande pour l’OCR : réduisez-la à 25 millions de pixels maximum.");
                using var engine=new TesseractEngine(Path.Combine(AppContext.BaseDirectory,"tessdata"),"fra",EngineMode.LstmOnly);
                using var result=engine.Process(image);
                return $"[OCR local français — indicateur interne {result.GetMeanConfidence():P0}, non garantie ; vérifier chiffres et dates]\n"+result.GetText();
            }
            catch(ArgumentException){throw;}
            catch(Exception){throw new ArgumentException("OCR local indisponible ou image illisible. Vérifiez l’installation du moteur et importez une image nette en PNG/JPEG.");}
        }
    }
}
