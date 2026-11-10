# Modèle OCR français

- Moteur : [Tesseract .NET 5.2.0](https://github.com/charlesw/tesseract), paquet NuGet épinglé dans le projet et son fichier de verrouillage.
- Modèle : [tessdata_fast, version 4.1.0, fra.traineddata](https://github.com/tesseract-ocr/tessdata_fast/tree/4.1.0), licence Apache-2.0 du dépôt.
- URL du téléchargement : https://raw.githubusercontent.com/tesseract-ocr/tessdata_fast/4.1.0/fra.traineddata
- SHA-256 local : CED037562E8C80C13122DECE28DD477D399AF80911A28791A66A63AC1E3445CA
- La lecture des images PDF suit [l’API PdfPig](https://github.com/UglyToad/PdfPig/wiki/Images).
- Reconnaissance entièrement locale. Aucun service distant utilisé pour OCR.

Une copie de la licence du modèle est fournie dans `LICENSE-APACHE-2.0.txt`.
