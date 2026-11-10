# Recommandations retenues et faisabilité

Mise à jour : 5 novembre 2026. Les recommandations ci-dessous sont retenues au titre de la délégation de l’utilisateur. Elles complètent ses choix sans les remplacer. Les points non vérifiés ne constituent pas des promesses de fonctionnement.

## Réglages fonctionnels retenus

- Interface en français, montants en euros, distances en kilomètres, dates au format français ; valeurs sources conservées.
- Usage personnel avec un compte local ; protéger l’accès aux documents depuis le réseau domestique. Pas de comptes publics dans la première version.
- Navigation : tableau de bord, recherches, résultats, comparaison, dossiers suivis, véhicules achetés, notifications et réglages.
- Comparaison de deux à quatre véhicules côte à côte, avec les mêmes hypothèses et affichage des données manquantes.
- Filtres : deux budgets, localisation et rayon, marque, modèle, motorisation, boîte, année, kilométrage, carrosserie, équipements et vendeur. Statut obligatoire ou souhaité par critère.
- Dédoublonnage par identifiant de source et URL normalisée ; rapprochements entre sites proposés avec preuves, sans fusion automatique sur la seule ressemblance du modèle et du prix.
- Historique des prix et des analyses daté. Une annonce inaccessible n’est pas automatiquement considérée comme vendue.
- Les corrections manuelles sont conservées lors des mises à jour ; toute contradiction nouvelle est signalée.
- Import initial : texte, saisie, PDF, JPEG et PNG. Les formats OBD propriétaires ne sont pas réputés pris en charge sans fichier de test.
- Rapport PDF daté : véhicule, prix, coûts par scénario, note, confiance, sources, inconnues, points de contrôle, négociation et espace de notes.
- Alertes regroupées et dédoublonnées. Ne pas notifier de nouveau un événement inchangé. Horaires de silence modifiables.
- Sauvegarde manuelle complète avec manifeste de version et contrôle d’intégrité ; tester la restauration dans un dossier séparé. Ne pas activer de sauvegarde automatique.
- Suppression explicite des documents personnels ; cache temporaire séparé et nettoyable. Afficher l’espace occupé avant de télécharger un modèle IA.

## Principes de calcul retenus

- Calculs monétaires déterministes et traçables ; l’IA extrait et explique, elle ne produit pas seule une note ou un prix sans fondement.
- Séparer réparation avérée, entretien programmé, consommable et risque de panne non confirmé. Ne pas additionner plusieurs fois une même intervention ou main-d’œuvre partagée.
- Coût d’acquisition = achat + frais d’acquisition + travaux et entretien immédiats + réserve explicitée. Une dépense inconnue ne vaut jamais zéro.
- Coût de possession = acquisition + dépenses pendant l’usage − revente estimée, avec périmètre et dates affichés.
- Marge de revente = produit de vente estimé − achat − ensemble des dépenses applicables. Gain après temps personnel = marge − heures personnelles × tarif choisi. Un résultat sans traitement des taxes applicables doit être libellé avant fiscalité, sans supposer une exonération.
- Comparer des véhicules suffisamment proches ; afficher le nombre, les dates et la provenance des comparables. Ne pas assimiler prix affichés et prix de transactions.
- Exiger le socle de la décision 15. S’il est complet mais qu’une dimension manque de données, laisser cette dimension non estimable ; ne pas inventer une moyenne rassurante ou une probabilité de panne.
- Confiance qualitative initiale accompagnée des raisons et de la couverture documentaire. Pas de pourcentage de certitude sans calibration.
- Conserver des scénarios optimiste, central et pessimiste dont les hypothèses sont modifiables. Ils ne sont pas des probabilités garanties.
- Les poids numériques des profils et les seuils restent à calibrer sur des dossiers représentatifs avant de figer le classement.

## Vérifications de sources — 5 novembre 2026

| Source | Constat sur la source officielle | Décision pour le projet |
|---|---|---|
| [Karoto — connecteur IA](https://karoto.fr/connecteur-ia) et [conditions](https://karoto.fr/cgu) | Connecteur MCP documenté comme gratuit, sans compte et en lecture seule, avec recherche d’annonces françaises, détails et estimation. Usage raisonnable et attribution avec lien requis. Les annonces tierces sont résumées et renvoient à leur source ; disponibilité et exactitude non garanties. | Premier candidat à tester pour la recherche automatique France. Documentation consultée, endpoint non testé : couverture, fraîcheur, quotas pratiques et droit de conservation des médias restent à vérifier. Ne pas déduire une autorisation de copie des photos tierces. |
| [CarHunt API](https://carhunt.fr/api) | Essai de 7 jours ; offre Business à 99 €/mois. API seule : estimation indicative sur devis à partir de 20 €/mois. | Pas de dépendance obligatoire : coût récurrent incompatible avec la contrainte. Ne pas confondre recherche gratuite sur le site et accès API gratuit permanent. |
| [RappelConso Open Data](https://rappel.conso.gouv.fr/support/open-data) | Données publiques gratuites, accessibles en fichiers et par API. | Candidat pour les alertes de rappel. Tester la couverture automobile et l’identification précise ; une correspondance de modèle ne prouve pas qu’un VIN est concerné. |
| [AutoUncle estimation](https://www.autouncle.fr/fr/estimation-voiture) | Estimation fondée sur les prix affichés des comparables ; elle ne constate pas l’état mécanique du véhicule. | Référence méthodologique et consultation possible. Aucun accès API gratuit ni droit de collecte automatisée établi par cette page. |

Ces vérifications ne remplacent pas la cartographie existante de 120 fiches : elles l’actualisent sur des points critiques. Aucun compte créé et aucun abonnement souscrit. Mise à jour après les constats du tableau : le connecteur public Karoto a ensuite été interrogé avec succès (initialisation, schéma, recherche, fiche). Le tableau décrit la vérification documentaire initiale ; voir `validation-technique.md` pour les résultats effectifs et les limites de couverture.

## Conditions de faisabilité à résoudre par l’assistant

1. Identifier au moins une source d’annonces utilisable automatiquement et durablement sans coût récurrent. Tant que cela n’est pas démontré, ne pas déclarer la recherche automatique multisite livrable. L’import assisté est déjà accepté mais ne remplace pas silencieusement cette exigence.
2. Vérifier les référentiels d’identification, les coûts de réparation et la fiabilité moteur ; distinguer consultation gratuite et droit d’intégration.
3. Sélectionner des modèles locaux adaptés au PC et mesurer mémoire, stockage, délais et qualité sur textes, documents et photos. Pas de téléchargement volumineux avant d’avoir vérifié la place nécessaire.
4. Vérifier les offres externes gratuites et leurs conditions par type de contenu. Aucun document privé envoyé par défaut ; aucun basculement payant après épuisement d’un quota.
5. Choisir une architecture .NET partageant données et logique métier entre site et interface Windows ; vérifier les bibliothèques et licences sur leurs documentations officielles.
6. Préparer des critères de réception : imports corrigibles, traçabilité, absence de double comptage, respects des plafonds et filtres stricts, reprise après arrêt, limitation de charge, fonctionnement sur le réseau domestique et restauration complète.

## Livraison envisagée

Construire tous les modules retenus dans un ordre vérifiable : dossiers et calculs, recherche et import, analyse documentaire et visuelle, surveillance, suivi après achat, puis installation et validation globale. Ce séquencement ne retire aucune fonction confirmée de la cible.

Le mode export et le critère « strictement moins de trois ans » ne sont pas ajoutés : la proposition a été laissée de côté dans le dernier message de l’utilisateur.
