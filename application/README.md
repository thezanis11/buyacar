# AutoClair

Application automobile privée, hébergée sur Windows.

- Utilisation : [GUIDE-DEBUTANT.md](GUIDE-DEBUTANT.md)
- Fonctions vérifiées et limites : [ETAT-DU-PROJET.md](ETAT-DU-PROJET.md)
- Lancement : `Lancer-AutoClair.cmd`
- Raccourci Bureau et ouverture de session : `Installer-Raccourcis.cmd`
- Construction et tests du cœur : `Construire.ps1`

## Organisation

- `Auto.Server/` : API .NET, stockage SQLite, domaine, connecteur, surveillance et interface web.
- `Auto.Desktop/` : fenêtre Windows WPF/WebView2 et zone de notification.
- `Auto.Tests/` : contrôles métier et persistance exécutables sans serveur externe.
- `tools/ui-test.cjs` : tests navigateur sur serveur dédié au port 5198. `AUTO_LIVE_TEST=1` ajoute un appel réel à Karoto.
- `dist/` : fichiers compilés de lancement.
- `.dotnet/`, `.nuget/`, `.ollama/` : outils et modèles locaux, non destinés au suivi de version.

Le développement se fait dans ce dossier. Ne pas modifier les références synchronisées sous `../sources/`. L’espace utilisateur actif est séparé dans `%LOCALAPPDATA%\AutoClair\data`.

## Dépendances principales

Microsoft .NET et Microsoft.Data.Sqlite : MIT ; SQLite : domaine public et composants natifs selon leurs notices ; Microsoft WebView2 : conditions Microsoft ; PdfPig : Apache-2.0 ; Ollama : MIT ; Gemma : conditions Gemma (distinctes du moteur). Ne pas redistribuer les modèles sans respecter leurs conditions.

Sources officielles : https://learn.microsoft.com/dotnet/ ; https://www.nuget.org/packages/Microsoft.Data.Sqlite ; https://www.nuget.org/packages/Microsoft.Web.WebView2 ; https://github.com/UglyToad/PdfPig ; https://github.com/ollama/ollama ; https://ollama.com/library/gemma3:4b ; https://karoto.fr/connecteur-ia.
