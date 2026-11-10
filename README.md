# Buyacar — AutoClair

Application automobile personnelle pour Windows : rechercher des annonces en France, comparer le coût réel d’un véhicule, préparer un achat et suivre une revente. L’application utilise un serveur local et une interface partagée avec la fenêtre Windows.

- [Conception et cahier des charges](conception/LIRE-MOI.md)
- [Guide de démarrage Windows](application/GUIDE-DEBUTANT.md)
- [Fonctions testées et limites connues](application/ETAT-DU-PROJET.md)

Ce dépôt contient les sources. Les exécutables compilés, données personnelles, caches, SDK et modèles Ollama locaux sont exclus. Sur un nouveau PC, installer le SDK .NET 10 dans `application/.dotnet`, puis exécuter `application/Construire.ps1` avant le premier lancement. Le modèle OCR français est fourni ; l’IA Ollama est facultative.

## Version publique de présentation

Ce dépôt présente les sources de l'application AutoClair sous le nom de dépôt Buyacar. Les captures web brutes de recherche et les liens individuels d'accès à des rapports véhicules ne sont pas inclus dans cette version publique. Le registre des sources et les études produites restent disponibles.

Les éléments tiers distribués avec les sources sont documentés dans `application/THIRD-PARTY-NOTICES.md`, avec les textes de licence associés.
