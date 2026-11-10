# Validation technique et contrat de réalisation

État au 5 novembre 2026. Complète le cahier des charges et les recommandations. Les tests réalisés sont distingués des critères à vérifier pendant la réalisation.

Actualisation après le début du développement : ce document conserve les constats de préparation. Les nouveaux tests, installations et mesures IA sont décrits dans `../application/ETAT-DU-PROJET.md` et les résultats sous `../application/test-results/`.

## 1. Recherche automatique : premier test concluant, couverture partielle

Quatre appels publics ont été effectués sur https://karoto.fr/mcp : initialisation, liste des outils, recherche et lecture d’une fiche. Réponses JSON obtenues sans compte ni clé. Aucun document privé envoyé.

Recherche de test : pays FR, prix maximal 5 000 €, tri récent, limite 2. Le service annonce 1 201 correspondances et renvoie deux annonces. Cela ne prouve ni la disponibilité chez le vendeur ni l’exhaustivité du marché.

Exemple observé : Renault Clio 1.5 dCi 85 Dynamique, 2009, 288 000 km, 1 600 €, Yonne, particulier. La fiche détaillée a répondu available=true. Elle ne contenait ni description, ni justificatifs, ni historique d’entretien. Première immatriculation limitée à l’année. La cote renvoyée n’a pas été vérifiée : elle ne constitue pas une marge réalisable.

Source de l’exemple : [Karoto](https://karoto.fr/annonce/renault-clio-1-5-dci-85-dynamique-2009-gy9wcym3psvk). Données observées à cette date, non recommandation d’achat.

Contraintes du schéma effectivement consulté :
- Limite de 20 résultats par appel ; aucun curseur ni numéro de page exposé dans search_listings. Présenter une sélection, jamais une surveillance exhaustive.
- Filtre département disponible ; pas de rayon géographique natif. Sans localisation suffisamment précise, une distance obligatoire reste non vérifiée.
- Filtres pays, prix, année, kilométrage, énergie, boîte et carrosserie disponibles. Vérifier les contraintes après import également.
- Paramètre particuliers uniquement disponible ; conserver les deux types de vendeurs par défaut.
- Réponses à normaliser : les valeurs de carburant et boîte retournées diffèrent des codes d’entrée.
- Identifiant source conservé ; dates d’observation locales distinctes des dates de publication déclarées.

Contrat du connecteur : pays FR systématique, maximum 20 résultats, une recherche toutes les 6 heures par défaut pendant la plage 08 h–22 h, cadence modifiable sous réserve des limites de la source. Ce rythme est une recommandation du projet, pas un quota garanti par Karoto. Une seule requête simultanée ; respecter Retry-After, arrêt temporaire après erreurs répétées ; aucune boucle destinée à contourner une limite. Une reprise après arrêt fait une vérification, pas toutes les vérifications manquées. Résultat vide, erreur de source et disparition d’annonce sont trois états différents.

Attribution avec lien dans les fiches et exports. La conservation des photos tierces n’est pas validée ; garder les liens et permettre l’import personnel. Documentation : [connecteur](https://karoto.fr/connecteur-ia), [conditions](https://karoto.fr/cgu). Gratuité annoncée à la date de vérification, sans garantie de pérennité. Adaptateur remplaçable et import assisté maintenu.

## 2. Architecture de référence

Décision déléguée : serveur local ASP.NET Core, interface web Blazor adaptable au téléphone, application Windows WPF partageant les composants d’interface. Un seul serveur détient les écritures et les traitements ; les clients ne modifient pas directement la base. La documentation Microsoft confirme l’intégration Blazor dans WPF et la réutilisation des composants web : https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/.

Base SQLite locale pour l’usage personnel ; pièces jointes dans un répertoire géré séparément. Versions exactes, dépendances et licences à verrouiller avant installation. Aucune base dans un dossier synchronisé en fonctionnement. Application de zone de notification responsable du lancement à l’ouverture de session, état du serveur et arrêt propre. Fermer la fenêtre principale ne termine pas la surveillance.

Accès authentifié, sessions protégées et accès réseau limité au domicile. Définir et tester le certificat HTTPS et sa confiance sur les appareils ; aucun accès Internet entrant ni redirection de port. Les fichiers importés sont des données non fiables : limites de taille, vérification du type, aucune exécution, aucune instruction extraite exécutée par l’IA. Les connecteurs ne suivent pas de liens vers des adresses locales ou privées fournies dans une annonce.

Entités minimales : Recherche, Critère, AnnonceSource, Véhicule, ObservationPrix, Document, FaitExtrait, Correction, AnalyseVersionnée, PosteDeCoût, Scénario, Favori, ÉtapeAchat, DépenseRéelle, Entretien, Revente, Notification, TravailPlanifié, Préférence, Sauvegarde.

Chaque fait stocke valeur, unité, source, date, niveau déclaré/documenté/estimé et éventuelle validation. Une analyse conserve ses entrées, versions de règles et hypothèses ; un changement de tarif ne réécrit pas les estimations historiques.

## 3. Calculs et classement

Montants en centimes entiers ou décimaux exacts, jamais flottants binaires pour les totaux. Valeur inconnue distincte de zéro. Les trois scénarios correspondent à trois ensembles d’hypothèses explicites, pas à trois pourcentages arbitraires appliqués au total.

- Acquisition : prix demandé + frais d’acquisition + travaux immédiats + entretien immédiat + réserve explicitée.
- Possession : acquisition + coûts pendant la durée choisie − produit de revente estimé. Les coûts pendant l’usage excluent ceux déjà comptés à l’acquisition.
- Revente : produit de vente − acquisition − frais supplémentaires de détention et de vente. La réserve non dépensée ne devient pas une dépense réelle ; distinguer résultat prévisionnel et réalisé.
- Gain après temps : marge financière − heures personnelles × taux horaire. Ne pas ajouter ces heures une seconde fois dans les postes payés à un garage.
- Les dépenses propres à chaque scénario de vente sont détaillées. Fiscalité inconnue : résultat explicitement avant fiscalité.

Avant notation : socle indispensable complet, exigences strictes vérifiées, prix sous plafond et scénario de budget conforme. En prudence stricte, un coût indispensable inconnu empêche de déclarer le budget respecté. Les dossiers restent consultables dans « à compléter », « à vérifier » ou « hors critères ».

Poids initiaux proposés, non calibrés :

| Profil | Prix | Fiabilité | Entretien | Coût total | Adéquation |
|---|---:|---:|---:|---:|---:|
| Équilibré | 20 | 25 | 15 | 25 | 15 |
| Sérénité | 10 | 35 | 25 | 20 | 10 |
| Budget | 25 | 15 | 10 | 35 | 15 |

Poids modifiables et normalisés ; somme nulle refusée. Score de chaque dimension sur 100, règle et preuves consultables. Fiabilité ne doit pas être déduite d’une marque ou d’un avis isolé. Les courbes de conversion prix/coûts/fiabilité devront être éprouvées sur un corpus avant activation du score global. Aucune dimension inconnue remplacée par 50/100. Si une dimension pondérée n’est pas calculable, afficher une évaluation partielle sans note globale trompeuse ; distinguer ce cas du socle d’annonce incomplet. Le socle complet autorise l’analyse, il ne crée pas les données absentes.

Le mode revente permet le tri par marge financière ou gain après temps, avec le même traitement des inconnues et des budgets. La cote d’un fournisseur est une donnée externe, pas la note globale de l’application. Les mêmes hypothèses s’appliquent aux voitures comparées.

## 4. IA locale : candidat identifié, mesure non réalisée

Le contrôle actuel trouve environ 29,0 Go décimaux libres. La commande Ollama n’est pas trouvée dans le PATH ; cela ne démontre pas l’absence de toute installation. dotnet --list-sdks ne retourne aucun SDK dans cet environnement. Aucun logiciel ni modèle installé pendant ces tests.

Candidat de premier essai : Gemma 3 4B quantifié via Ollama, texte et images, téléchargement affiché 3,4 Go. La documentation Windows d’Ollama demande au moins 4 Go pour les binaires, modèles en supplément. Prévoir donc au moins 7,4 Go avant caches et fichiers temporaires ; conserver une réserve disque de 10 Go comme règle du projet. La place paraît suffisante pour un essai unique, pas pour multiplier les modèles.

Sources : https://ollama.com/library/gemma3:4b et https://docs.ollama.com/windows. Taille sur disque différente de mémoire vive nécessaire. Pas de performance GPU présumée pour le circuit Radeon intégré. Vérifier les conditions Gemma avant distribution ; téléchargement séparé préférable à l’inclusion du modèle dans l’installeur.

Protocole de mesure à exécuter avant engagement sur les délais : un modèle chargé, un travail à la fois, contexte initial limité à 4 096 tokens, 4 threads maximum comme point de départ. Mesurer mémoire du système et du processus, durée à froid/à chaud, espace disque, réactivité et arrêt du traitement. Décharger le modèle après inactivité. Ces réglages sont à ajuster à partir des mesures, pas une promesse de confort.

Corpus : 20 annonces françaises de test annotées, 10 documents factices dont scans et contradictions, 10 photos autorisées avec défauts visibles annotés. Vérifier prix/km/année sans confusion, source de chaque fait, ambiguïtés conservées, absence d’invention d’entretien. Pour les photos : observations localisées et réserves ; aucune conclusion mécanique. Comparer aux annotations humaines. Objectif de réception : zéro invention sur les champs essentiels dans ce corpus, sinon validation humaine imposée sur les champs concernés. Un corpus réduit ne prouve pas une fiabilité générale.

Le traitement classique du texte et les calculs doivent fonctionner sans IA. Si le modèle échoue ou n’est pas installé, indiquer « analyse IA indisponible » et proposer la saisie ; ne pas afficher un résultat réussi. Aucun transfert externe silencieux. Le repli local demandé en cas de quota externe épuisé doit être essayé avant réception de la fonction correspondante.

## 5. Critères de réception prioritaires

| Cas | Résultat attendu |
|---|---|
| Achat 2 000 €, frais 100 €, travaux 700 €, réserve 200 € | Acquisition 3 000 €, sans doublon |
| Acquisition 3 000 €, usage 1 200 €, revente 2 500 € | Possession 1 700 € |
| Acquisition 3 000 €, frais de vente 100 €, revente 3 800 €, 10 h à 20 € | Marge 700 €, gain après temps 500 € |
| Budget 3 000 €, central 2 800 €, pessimiste 3 500 € | Écarté du classement conforme en prudence stricte ; signalé dans l’autre réglage |
| Réparation indispensable de coût inconnu | Pas de conformité budgétaire affirmée |
| Version moteur indispensable ambiguë | Dossier conservé sans classement global |
| Correction manuelle puis nouvel import contradictoire | Correction conservée, conflit signalé |
| Même annonce réimportée et prix inchangé | Pas de doublon ni de nouvelle alerte de prix |
| Source indisponible ou zéro résultat | États différents, aucune vente inventée |
| Source annonce 1 201 résultats, renvoie 20 | Couverture partielle clairement affichée |
| PC arrêté pendant trois créneaux | Une vérification à la reprise, respect des quotas |
| Document contenant des instructions pour l’IA | Traité comme contenu, aucun changement de consigne ni envoi externe |
| Réseau local sans authentification | Documents et dossiers inaccessibles |
| Sauvegarde manuelle et restauration séparée | Dossiers, documents, corrections, dépenses et réglages récupérés, intégrité vérifiée |
| Suppression d’une pièce jointe | Retrait du dossier et de ses copies gérées ; sauvegardes historiques signalées séparément |

## 6. Décision de passage

Le prototype de recherche dispose d’une source gratuite réellement interrogée. Le socle dossiers, imports, coûts et suivi peut être réalisé sans dépendre d’une IA validée. La cible complète reste conditionnée aux essais locaux, aux référentiels de coûts/fiabilité, à la calibration du score, à la protection réseau et aux tests de restauration. Ne pas présenter cette validation partielle comme une application complète prête à livrer.
