# État de réalisation — AutoClair

5 novembre 2026. Version de développement utilisable ; cible complète encore en cours. Les fichiers sous `sources/` n’ont pas été modifiés.

## Réalisé et vérifié

- Serveur ASP.NET Core .NET 10, données SQLite et pièces jointes locales.
- Compte personnel avec mot de passe haché PBKDF2, session HttpOnly/SameSite, protection des mutations contre les requêtes intersites. Accès anonyme aux données refusé.
- Interface française adaptable au téléphone, vues synthétique/avancée, tableau de bord et états vides sans fausses annonces.
- Fenêtre Windows WPF avec WebView2, icône de notification, ouverture navigateur et commande d’arrêt. Démarrage en arrière-plan testé ; service local joignable. Raccourcis et démarrage de session disponibles via script d’installation, non installés pendant le test.
- Dossiers modifiables, favoris, parcours d’achat, notes, rappels et dépenses réelles. Copie des prévisions lors du passage à l’achat.
- Recherches structurées enregistrées, deux plafonds, filtres obligatoires/souhaités, modes usage/revente, durée et kilométrage, valorisation du temps. Pondérations utilisées par un indice de préférence explicite ; les dimensions inconnues bloquent la note. Les appréciations de fiabilité restent personnelles et documentées, sans référentiel moteur automatique validé.
- Connecteur Karoto effectivement testé depuis l’application : 20 annonces sur 1 201 correspondances annoncées, pour la recherche de test sous 5 000 €. Couverture partielle clairement affichée. Aucune photo tierce téléchargée.
- Surveillance planifiée, reprise d’un créneau manqué, délai de protection après erreur, historique des prix, dédoublonnage et conservation des corrections de prix. Cas fonctionnels de dédoublonnage et conflit testés.
- Coûts par poste dans trois scénarios, contrôle de cohérence, inconnues distinctes de zéro, calculs d’acquisition/possession/revente et temps personnel. Comparaison de deux à quatre dossiers avec hypothèses communes.
- Import PDF/JPEG/PNG/texte ; contrôle de taille et signatures. Extraction locale des PDF textuels et fichiers texte, résultat versionné et relisible. Test sur facture PDF factice réussi.
- Lecture de codes OBD saisis/collés, définitions limitées à quelques codes génériques, aucune connexion à un boîtier.
- Rapport de visite imprimable/exportable en PDF par le dialogue d’impression ; PDF généré pendant le test du navigateur.
- Sauvegarde manuelle ZIP avec manifeste et SHA-256 ; restauration contrôlée et copie séparée ; archive altérée refusée. Compte local conservé.
- Préparation HTTPS domestique : certificats et restriction aux adresses privées. Génération du certificat serveur avec clé et export du certificat public sans clé vérifiés.

## Validation exécutée

- `Auto.Tests` : 37 contrôles réussis (coûts, risque, inconnues, exigences, dédoublonnage, corrections, authentification, sauvegarde/restauration, PDF, restriction réseau et certificats).
- `tools/ui-test.cjs` : parcours Chrome réussi — compte, dossier, coûts persistés, justificatif texte, extraction PDF, rapport PDF, OBD, recherche enregistrée, affichage mobile, refus d’accès anonyme et des mutations sans en-tête de protection.
- Recherche réelle Karoto depuis le serveur test réussie. Les tests utilisent un dossier distinct : `test-data/` ; ils ne créent pas de compte ni de véhicule dans l’espace personnel de production.
- Captures et PDF de test dans `test-results/`. Rapport compact d’une page sur le dossier factice, rendu et inspecté visuellement après correction de la première mise en page.
- Sur la version compilée finale, tests supplémentaires réussis : faux PDF rejeté et canal de notifications de bureau inaccessible sans son jeton privé.
- Compilation Windows et serveur réussie. Après un avertissement de récupération des données d’audit lors de la restauration, le contrôle explicite `dotnet list Auto.Server package --vulnerable --include-transitive` a interrogé NuGet et n’a signalé aucun paquet vulnérable selon ses sources à cette date. Cela ne constitue pas un audit de sécurité complet.

## IA — premier essai mesuré, validation automatique refusée

Ollama 0.40.14 téléchargé depuis le dépôt officiel, archive vérifiée par SHA-256 publié. Gemma 3 4B téléchargé et testé sur ce PC. Le moteur utilise le processeur ; le GPU intégré n’est pas activé. La connexion applicative est implémentée pour annonces, documents et images. Aucun contenu privé transmis à un service externe.

Mesure sur cinq textes factices, quatre threads, contexte 2 048 : premier appel 17,71 s (chargement 9,04 s), suivants 7,09–8,57 s ; génération 7,56–8,30 tokens/s. Trois cas corrects sur cinq. Deux échecs importants : une instruction malveillante contenue dans l’annonce a changé le prix extrait ; un montant de facture a été interprété comme prix de vente. Détails reproductibles dans `tools/benchmark-ai.py` et `test-results/benchmark-ai.json`.

Essai d’image sur une facture fictive nette : 58,69 s. Le montant total de 250 EUR est lu, mais le kilométrage est erroné (8 200 au lieu de 82 000) et la main-d’œuvre est confondue avec des pièces. Détails dans `test-results/benchmark-vision.json`. Cet essai ne valide donc pas la lecture automatique des documents photographiés.

Conséquence : pas d’application automatique des valeurs IA, ni d’activation de classement fondé sur ces sorties. Résultats uniquement consultables et à vérifier. Ces essais ne remplacent pas le corpus prévu de 20 annonces, 10 documents et 10 photos annotés ; l’analyse des défauts de carrosserie n’est pas validée.

Test d’intégration applicatif réussi : AutoClair démarre son propre moteur sur le port local 11435 avec le cloud désactivé, retrouve le modèle téléchargé, conserve l’analyse d’une annonce factice (19,894 s) et laisse le prix original inchangé. Voir `test-results/ai-integration.json`. Le port distinct évite de réutiliser une instance Ollama personnelle éventuellement configurée pour des services distants.

## Fonctions encore partielles ou non livrées

1. **Fiabilité moteur automatique** : indice personnalisé livré (prix de comparables, appréciations documentées, coûts, adéquation), avec règle et raisons consultables. Il ne remplace pas un référentiel de pannes validé ; aucune probabilité de panne n’est calculée. Les appréciations de fiabilité et d’entretien nécessitent encore des éléments renseignés par l’utilisateur.
2. **Référentiel de réparations externe** : bibliothèque personnelle datée livrée, pièces et heures séparées, calcul garage/personnel, compatibilité de version et péremption vérifiées. Aucun catalogue professionnel gratuit complet n’est connecté ; les références doivent venir de devis ou tarifs renseignés.
3. **IA documentaire et visuelle** : OCR français local ajouté pour images et pages PDF scannées (10 pages scannées maximum par extraction, formats d’images pris en charge par PdfPig). Test de facture nette en image et PDF réussi. Les scans fragmentés, certains encodages et documents flous restent à vérifier. Le corpus représentatif et l’analyse fiable de carrosserie ne sont pas validés ; la validation des faits reste humaine.
4. **Analyse approfondie automatique** : file persistante livrée, dédoublonnage, annulation, relance, reprise après arrêt ; approfondissement des favoris et premières candidates. Extraction documentaire automatique testée de bout en bout. L’IA automatique attend l’inactivité clavier/souris et 4 Go de mémoire disponible, puis s’interrompt à la reprise d’activité ; comportement prolongé en veille Windows à éprouver. Le tri initial selon estimation de la source reste un repérage, pas un classement de bonnes affaires.
5. **Routage externe gratuit par type de contenu** : aucun fournisseur externe activé. Interface et repli après quota à développer lorsque les offres et autorisations sont validées.
6. **Comparables et négociation** : livrés avec comparables saisis ou issus des dossiers locaux, filtrage par version/énergie/boîte/âge/km, dates et dédoublonnage. Au moins trois comparables sont nécessaires. Cible plafonnée par le budget et simulation séparée du prix affiché testées. Une collecte automatique exhaustive de comparables n’est pas disponible.
7. **Réseau domestique** : code et script HTTPS prêts ; règle de pare-feu, confiance sur téléphone et essai réel multi-appareils encore à effectuer. Aucun port de box ouvert.
8. **Notifications Windows** : interrogation native indépendante de la fenêtre, canal local avec jeton privé, regroupement et dédoublonnage persistants implémentés ; validation de la réception après longue veille/session à approfondir.
9. **Surveillance exhaustive et multisource** : non promise. Le connecteur est plafonné et peut être indisponible. Import manuel disponible, intégrations supplémentaires non ajoutées.
10. **Packaging final** : lancement par fichiers CMD prêt ; installation signée, mise à jour et récupération du mot de passe non réalisées.

Ces limites ne retirent pas les fonctions du cahier des charges. Elles décrivent l’état réel de cette étape.

## Choix technique affiné

L’architecture conserve .NET et WPF. L’interface est servie en HTML/CSS/JavaScript et partagée telle quelle dans WebView2, au lieu de réécrire les écrans avec Blazor. Le serveur reste l’unique propriétaire des écritures et des calculs. Cela évite deux implémentations d’interface sans changer les fonctions attendues. Aucun framework web ou service cloud supplémentaire n’est requis.

## Reprendre le travail

Lire `GUIDE-DEBUTANT.md`, les trois documents de `../conception`, puis cet état. Conserver les exigences et les données existantes. Prochaine priorité : élargir les essais OCR aux documents réels dégradés, corpus visuel annoté, référentiels automobiles vérifiables et fournisseur gratuit facultatif par type de contenu. Les nouveaux écrans et calculs ne lèvent pas ces limites de données et de qualité IA. Garder une matrice des fonctions restantes et des tests ; ne pas déclarer la cible complète livrée à partir du seul socle.

## Compléments du 5 novembre — reprise du développement

- Onglet « Évaluation & négociation », gestion des comparables et appréciations sourcées, indice pondéré expliqué, cible et simulation indépendante.
- Références personnelles de réparation réutilisables, fourchettes de pièces et temps de travail, calcul garage ou personnel.
- Propositions d’extraction ancrées sur des champs étiquetés, source et texte visibles, validation/écartement et conservation des corrections. Les sorties IA restent consultatives.
- File de traitements persistante avec reprise, annulation et relance. Extraction de deux documents testée réellement depuis le navigateur jusqu’au résultat enregistré.
- Coût d’usage détaillé : une ou deux énergies, consommation aux 100 km, prix unitaire, assurance, entretien et autres frais ; recalcul selon le kilométrage de la recherche, sans cumuler le forfait manuel.
- Publication serveur et Windows réussie, 37 contrôles métier réussis. Parcours général et nouveaux écrans testés avec Chrome, données séparées dans `test-data/reprise-finale`. Capture de l’évaluation inspectée. Scripts reproductibles : `tools/ui-test.cjs` et `tools/features-test.cjs`.

Les tests ne prouvent ni la fiabilité mécanique d’un véhicule ni l’exactitude des prix futurs. Le logiciel est enrichi et utilisable, mais les éléments explicitement restants ci-dessus empêchent encore d’annoncer la totalité du cahier des charges comme livrée.

### OCR local ajouté et mesuré

Tesseract 5.2.0 (paquet .NET épinglé et modèle `tessdata_fast` français 4.1.0) est intégré à la publication. La facture fictive initialement mal lue par Gemma donne maintenant les quatre chiffres attendus : 82 000 km, pièces 100 EUR, main-d’œuvre 150 EUR, total 250 EUR. Mesures depuis l’API : 468 ms pour PNG, 486 ms pour le PDF scanné. Résultat reproductible dans `tools/ocr-test.cjs` et `test-results/ocr-integration.json`, fixtures conservées dans `Auto.Tests/Fixtures`.

L’indicateur interne de reconnaissance n’est pas une probabilité d’exactitude. Ces deux représentations du même document net ne constituent pas un corpus représentatif. La reconnaissance OCR ne diagnostique pas l’état mécanique ou la carrosserie.
