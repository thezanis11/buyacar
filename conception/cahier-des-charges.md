# Application automobile — cahier des charges en construction

Décisions consolidées le 5 novembre 2026. Les exigences sont définies ; les sources, les performances IA et les règles de calcul doivent encore être validées avant transmission pour développement complet.

## Méthode de conception actualisée

- Conserver tous les choix déjà validés.
- Pour les décisions restantes, appliquer directement les recommandations de l’assistant, conformément à la demande du 5 novembre 2026.
- Réserver les questions aux arbitrages réellement incertains, les regrouper et proposer des choix cliquables.
- Laisser le mode export hors du périmètre pour le moment, conformément au retrait de cette proposition dans le dernier message.
- Les recommandations ne valent pas autorisation de dépense, de publication ou de transmission de documents.

## Décisions confirmées

### 1. Interfaces — option C
- Un site privé accessible depuis un navigateur sur PC et téléphone.
- Un logiciel Windows installé.
- Les deux interfaces partagent les mêmes données.
- Le choix de la technologie de l’interface Windows reste à décider.

### 2. Première version utilisable — option C
- Analyser une annonce et les documents disponibles.
- Sauvegarder les dossiers et comparer plusieurs véhicules.
- Rechercher automatiquement des annonces selon les critères de l’utilisateur et surveiller les nouvelles offres.
- Les sources et modalités de collecte restent à valider selon leur disponibilité, leurs conditions et le budget.

### 3. Marché géographique — France uniquement
- La première version couvre les annonces de voitures situées en France.
- La recherche à l’étranger et le parcours d’importation sont hors du périmètre initial.

### 4. Vendeurs — professionnels et particuliers à égalité
- Inclure les annonces professionnelles et particulières dans la recherche.
- Classer selon l’intérêt du véhicule et les preuves disponibles, sans priorité automatique aux professionnels.
- Signaler les informations manquantes quel que soit le statut du vendeur.
- Cette décision remplace la préférence initiale pour les annonces professionnelles.

### 5. Objectifs — deux modes distincts
- Choisir l’objectif pour chaque recherche : acheter pour rouler ou acheter pour revendre.
- Mode usage personnel : évaluer la fiabilité et le coût total pendant la durée de possession.
- Mode revente : évaluer la marge après travaux et frais, ainsi que le délai de revente.
- Les hypothèses, horizons, postes de coût et règles de classement de chaque mode restent à définir.

### 6. Budget des services externes
- Fonctionnement courant gratuit : pas de service payant obligatoire pour l’usage habituel.
- Achats ponctuels facultatifs, toujours déclenchés par l’utilisateur (par exemple un rapport sur un véhicule présélectionné).
- Aucune dépense automatique autorisée.
- Électricité et achat éventuel de matériel à évaluer séparément.
- La couverture des données et l’automatisation réalisables gratuitement restent à vérifier.

### 7. Collecte — automatique et import assisté
- Rechercher automatiquement sur les sources disposant d’un accès gratuit exploitable et autorisé.
- Compléter la recherche par des annonces ou alertes fournies par l’utilisateur.
- Les sources précises, formats d’import, fréquences de surveillance et mécanismes de dédoublonnage restent à définir.

### 8. Contenus analysés — annonces, justificatifs et photos
- Accepter les annonces par lien, texte copié-collé ou saisie des caractéristiques ; prévoir le copier-coller si le lien est inaccessible.
- Analyser les justificatifs en PDF, scans ou photos : contrôle technique, factures, carnet d’entretien et rapports d’historique.
- Inclure dès la première version une analyse visuelle des photos du véhicule pour relever les défauts apparents et points à vérifier.
- Évaluer la fiabilité de cette analyse ; les photos ne permettent pas de présumer l’état mécanique.
- Cette décision remplace le report initial de l’analyse des photos.
- Le choix des modèles, leurs besoins matériels et les modalités de validation des extractions restent à définir.

### 9. Hébergement initial — PC Windows actuel
- Héberger la première version sur le PC Windows actuel de l’utilisateur.
- L’utilisateur accepte que le service soit accessible uniquement lorsque ce PC est allumé et que le service fonctionne.
- Les recherches et traitements automatiques nécessitent également que le service soit en fonctionnement.
- Vérifier ultérieurement les caractéristiques matérielles pour dimensionner l’IA locale, notamment l’analyse des images.
- Accès limité au réseau domestique, conformément à la décision 10.

### 10. Accès — réseau domestique uniquement
- Utiliser le logiciel Windows sur le PC et le site privé depuis les appareils du réseau domestique.
- Aucun accès distant depuis l’extérieur à prévoir dans le périmètre initial.
- Le PC hébergeant le service doit être allumé et le service en fonctionnement.

### 11. Budget de recherche — deux plafonds
- Configurer pour chaque recherche un prix d’achat maximal et un budget total maximal après remise en état.
- Le coût total inclut le prix d’achat, les réparations, l’entretien immédiat et les frais d’acquisition.
- Les modalités de réserve pour imprévus et le traitement des estimations incertaines restent à définir.

### 12. Risque de dépassement — réglage par recherche
- Proposer deux comportements : prudence stricte ou opportunités avec alerte.
- Sélectionner la prudence stricte par défaut : si le scénario pessimiste dépasse le budget total, retirer le véhicule des résultats conformes au budget, tout en le laissant consultable séparément.
- En mode opportunités avec alerte, conserver le véhicule lorsque l’estimation centrale respecte le plafond et afficher explicitement le dépassement possible.
- Les scénarios sont des estimations, sans garantie ; leur méthode de calcul et le traitement des données insuffisantes restent à définir.

### 13. Motorisations — toutes dès la première version
- Couvrir essence, diesel, hybrides rechargeables et non rechargeables, électriques, GPL et E85.
- Prévoir des critères de fiabilité, entretien et coût adaptés à chaque motorisation.
- Intégrer l’état de la batterie de traction et les coûts de recharge lorsque pertinents et documentés.
- Ne pas déduire un état de batterie mesuré à partir de la seule annonce ; préciser la provenance et les limites des informations.

### 14. Informations manquantes — attendre avant de classer
- Conserver les dossiers incomplets sans leur attribuer de classement global tant que les informations indispensables ne sont pas disponibles.
- Afficher les informations manquantes et permettre de compléter le dossier.
- Les inconnues restent visibles et les estimations sont identifiées comme telles.
- Le socle indispensable est fixé à la décision 15.

### 15. Minimum requis pour le classement — socle de l’annonce
- Exiger le prix, le modèle et la version identifiables, la motorisation, la boîte, l’année et le kilométrage.
- Autoriser le classement lorsque ce socle est disponible, sans imposer de justificatifs.
- Signaler l’absence de justificatifs et limiter en conséquence la confiance de l’analyse.
- Distinguer les déclarations du vendeur des informations étayées par des preuves.
- Cette décision précise les informations indispensables mentionnées à la décision 14.

### 16. Évaluation — note globale et indicateurs détaillés
- Afficher une note globale et des indicateurs pour le prix, la fiabilité, l’entretien, le coût après travaux et l’adéquation à la recherche.
- Expliquer les raisons de chaque évaluation et du classement.
- Afficher un niveau de confiance distinct de l’intérêt économique du véhicule.
- Rendre consultables les coûts estimés et les informations manquantes.
- Les échelles, formules, pondérations et méthodes de validation restent à définir.

### 17. Personnalisation du classement — profils et réglages
- Proposer des profils prédéfinis, puis permettre d’ajuster l’importance de chaque critère : fiabilité, prix, frais et autres dimensions retenues.
- Sauvegarder les préférences pour les réutiliser dans les recherches suivantes.
- Adapter les profils aux objectifs « acheter pour rouler » et « acheter pour revendre ».
- Maintenir les règles de budget et les informations indispensables séparées de la note : une bonne note ne permet pas de contourner ces règles.
- Les profils initiaux, poids, bornes et formules restent à définir et à valider.

### 18. Coût de possession — durée et kilométrage personnalisables
- Dans le mode « acheter pour rouler », permettre de renseigner la durée de possession prévue et le kilométrage annuel.
- Permettre de comparer plusieurs scénarios.
- Comparer les véhicules d’une même recherche avec les mêmes hypothèses d’usage.
- Intégrer achat, entretien, énergie, assurance et valeur de revente estimée ; détailler ultérieurement les autres frais applicables et les sources des estimations.
- Distinguer ce coût de possession du budget d’acquisition après remise en état défini à la décision 11, sans compter deux fois une même dépense.

### 19. Revente — marge financière et valorisation du temps
- Afficher séparément la marge financière et le gain après valorisation du temps personnel.
- Permettre de modifier le tarif horaire personnel et de choisir lequel de ces deux indicateurs sert au classement.
- Inclure les dépenses réellement payées dans les deux calculs.
- Prendre en compte les heures estimées de recherche, déplacements, réparations personnelles, démarches et vente, avec hypothèses modifiables.
- Ne pas compter deux fois une main-d’œuvre déjà facturée par un professionnel ; distinguer temps personnel et durée d’immobilisation du véhicule.
- Les autres frais applicables et les hypothèses de prix et délai de revente restent à définir.

### 20. Extraction des informations — validation ciblée
- Utiliser directement les informations extraites suffisamment claires.
- Demander à l’utilisateur de trancher les ambiguïtés ou contradictions importantes, en présentant les sources concernées.
- Permettre de consulter la provenance des informations et de les corriger.
- Une information indispensable non résolue empêche le classement global, conformément aux décisions 14 et 15.
- Les critères de clarté, d’importance et les règles de gestion des contradictions restent à formaliser.

### 21. Suivi des véhicules — achat et après-achat
- Inclure les dossiers sauvegardés, favoris, comparaison et notes personnelles.
- Suivre le parcours d’achat avec les statuts « à contacter », « visite prévue », « offre faite », « acheté » et « abandonné », ainsi que notes de visite et rappels.
- Après achat, enregistrer les dépenses réelles, travaux, entretien et éventuelle revente.
- Conserver les estimations initiales pour comparer les coûts et résultats réels aux prévisions, sans écraser rétroactivement celles-ci.
- Les champs détaillés, justificatifs associés et règles de suivi restent à définir.

### 22. Notifications — application et Windows
- Fournir un centre de notifications dans l’application avec historique des alertes.
- Afficher également des notifications Windows sur le PC.
- Couvrir les nouvelles annonces intéressantes, baisses de prix, visites et rappels d’entretien.
- Prévoir des réglages pour limiter les interruptions ; seuils, regroupement et horaires restent à définir.
- Ne pas inclure l’envoi d’e-mails dans le périmètre initial.
- La détection automatique dépend du PC allumé, du service actif et des sources accessibles.

### 23. Surveillance — fréquence réglable par recherche
- Permettre de choisir un rythme et des plages horaires pour chaque recherche enregistrée.
- Adapter la fréquence effective aux possibilités et limites de chaque source, et la rendre visible.
- Lorsqu’une vérification a été manquée pendant l’arrêt du PC ou du service, effectuer une vérification au prochain démarrage sans rejouer inutilement toutes les échéances manquées.
- Afficher la date de la dernière vérification réussie et les éventuelles erreurs de source.
- Les rythmes proposés et le comportement lors d’une mise en veille restent à préciser.

### 24. Négociation — prix cible et préparation de l’échange
- Permettre de simuler un prix négocié et d’observer son effet sur budget, note et marge.
- Suggérer une fourchette d’offre fondée sur les comparables et frais documentés, en expliquant les incertitudes.
- Préparer les questions à poser au vendeur et des arguments étayés par les éléments du dossier.
- Produire un brouillon de message modifiable que l’utilisateur envoie lui-même.
- Calculer le classement initial au prix affiché ; ne jamais traiter une remise hypothétique comme acquise.

### 25. Visites — rapport PDF à emporter
- Exporter un rapport PDF consultable sur téléphone et imprimable.
- Inclure l’analyse, les questions au vendeur et une liste de vérifications adaptée au véhicule.
- Permettre de saisir les observations de visite au retour dans l’application.
- Ne pas prévoir de dossier interactif hors connexion sur téléphone ni de synchronisation de notes dans le périmètre initial.
- Conserver l’accès au serveur limité au domicile, conformément à la décision 10.

### 26. Sources d’annonces — aucun site imposé
- Privilégier les sources offrant les données les plus utiles et les accès gratuits les plus fiables.
- Aucun portail particulier n’est indispensable à la première version.
- Évaluer chaque source selon sa couverture, la qualité des données, les conditions d’accès et de réutilisation, les limites et la maintenance nécessaire.
- Maintenir la combinaison de collecte automatique autorisée et d’import assisté définie à la décision 7.
- La sélection des sources doit être étayée par l’étude de marché et des vérifications techniques avant de figer le cahier des charges.

### 27. Traitement IA — configurable par type de contenu
- Permettre de choisir le traitement local ou externe selon le type de contenu.
- Pouvoir autoriser un service gratuit externe pour les annonces publiques tout en conservant documents personnels et photos privées en local.
- Prévoir un repli local lorsque le quota gratuit d’un service externe est épuisé, sans bascule automatique vers une offre payante.
- Le fonctionnement courant doit rester gratuit ; les achats ponctuels demeurent à l’initiative de l’utilisateur.
- Les autorisations par catégorie, les réglages initiaux, les fournisseurs et les capacités matérielles locales restent à préciser.
- Ce choix exprime une exigence pour la future application et ne vaut pas autorisation immédiate d’envoyer des contenus à un fournisseur.

### 28. Analyse automatique — deux étapes
- Effectuer un premier tri sur les informations disponibles pour les annonces respectant les filtres.
- Approfondir automatiquement les meilleures candidates et les annonces ajoutées aux favoris.
- Conserver les autres annonces consultables.
- Afficher le niveau d’analyse réellement effectué ; le premier tri ne doit pas être présenté comme une analyse approfondie.
- Respecter le minimum d’informations requis pour tout classement global, y compris au premier tri.
- Les critères de sélection, le nombre de dossiers approfondis, les possibilités de lancement manuel et les limites de charge restent à définir.

### 29. Travaux — personnalisation par réparation
- Partir des préférences de l’utilisateur, notamment mécanique chiffrée chez un professionnel et certaines réparations de carrosserie réalisables personnellement.
- Pour chaque réparation, permettre de choisir garage ou réalisation personnelle et de modifier les coûts des pièces et de main-d’œuvre.
- Permettre de remplacer une estimation par un devis réel.
- Conserver la source et la date du chiffrage, ainsi que la distinction entre estimation, devis et dépense réelle.
- Répercuter les ajustements dans les scénarios de coût et évaluations concernés, en préservant les estimations initiales pour comparaison.
- Éviter de compter deux fois la main-d’œuvre professionnelle ou le temps personnel valorisé.

### 30. Diagnostic OBD — import et analyse
- Importer les rapports de diagnostic exportés et permettre la saisie de codes défaut.
- Expliquer les défauts et intégrer les vérifications utiles à l’analyse préachat.
- Traiter les codes comme des indices à interpréter, sans déduire automatiquement le remplacement d’une pièce.
- Conserver la provenance du diagnostic et distinguer ses observations des hypothèses de panne.
- Vérifier les formats réellement exportables par les outils concernés avant de fixer les formats pris en charge.
- Ne pas inclure de connexion directe à un boîtier OBD dans la première version.

### 31. Interface — vues synthétique et avancée
- Proposer une vue synthétique par défaut et une vue avancée avec tableaux, filtres et indicateurs détaillés.
- Mémoriser la préférence d’affichage de l’utilisateur.
- Conserver l’accès aux mêmes fonctions dans les deux modes ; adapter leur présentation à l’écran utilisé.
- Appliquer ces principes au site privé et au logiciel Windows.
- Les parcours, maquettes et règles d’affichage détaillées restent à concevoir.

### 32. Sauvegarde — export manuel
- Permettre à l’utilisateur de déclencher une sauvegarde complète des dossiers, documents, réglages et dépenses.
- Prévoir une fonction de restauration permettant de récupérer l’application et ses données.
- Ne pas planifier de sauvegardes automatiques par défaut.
- Le format, la destination sélectionnable et la vérification des archives restent à définir.

### 33. Ressources du PC — priorité au confort
- Limiter les traitements en arrière-plan pour préserver l’usage des autres logiciels.
- Réserver les analyses lourdes aux périodes où le PC est peu utilisé.
- Déterminer les limites de ressources et les critères de faible activité à partir de mesures sur la machine cible.
- La surveillance légère et les analyses lourdes doivent pouvoir être planifiées distinctement, en respectant les fréquences autorisées des sources.
- Ne pas promettre de délai d’analyse avant les essais matériels.

### 34. Démarrage — ouverture de session Windows
- Démarrer automatiquement le service à l’ouverture de la session Windows de l’utilisateur.
- Héberger le site et surveiller les annonces selon les horaires configurés, même si la fenêtre de l’application n’est pas ouverte.
- Afficher l’état du service dans la zone de notification Windows et permettre son arrêt explicite.
- Respecter la priorité au confort pour les traitements lourds.
- Cette décision porte sur la future application ; aucune configuration de démarrage Windows n’a été modifiée pendant la conception.

### 35. Conservation — historique léger et dossiers suivis complets
- Conserver les caractéristiques, prix observés et résultats d’analyse des annonces consultées.
- Réserver les copies de photos et contenus détaillés aux favoris et véhicules suivis, dans les limites permises par les sources.
- Conserver les documents personnels importés et les dépenses jusqu’à suppression explicite par l’utilisateur.
- Définir ultérieurement les limites de stockage, le nettoyage des données temporaires et le comportement lors du retrait d’un favori.

### 36. Création des recherches — formulaire structuré
- Créer les recherches au moyen de champs et filtres : budgets, zone géographique, usage et préférences.
- Permettre de sauvegarder les recherches et leurs réglages.
- Ne pas inclure la conversion d’une demande en langage naturel en filtres dans le périmètre initial.
- La liste détaillée des filtres, leurs valeurs par défaut et les validations restent à définir.

### 37. Revente — comparaison de deux scénarios
- Comparer une hypothèse de vente rapide et une hypothèse de vente plus patiente.
- Afficher les prix de revente envisagés, permettre leur modification et expliciter les incertitudes.
- Chiffrer les délais uniquement lorsque les données disponibles permettent de les étayer ; sinon les laisser non estimés ou utiliser une hypothèse explicitement renseignée par l’utilisateur.
- Intégrer les frais liés à l’immobilisation selon les hypothèses retenues, sans double comptage.
- Distinguer prix d’annonce, estimation de prix de vente et prix de vente réellement constaté.

### 38. Critères — obligatoires ou souhaités
- Permettre de choisir pour chaque critère s’il est obligatoire ou souhaité.
- Exclure sur les exigences strictes ; utiliser les préférences pour le classement et signaler les écarts.
- Maintenir les règles de budget séparées.
- Si une exigence obligatoire ne peut pas être vérifiée, afficher « à vérifier » et ne pas présenter le véhicule comme conforme.

## Matériel observé le 5 novembre 2026
- Processeur : AMD Ryzen 5 PRO 4650U, 6 cœurs et 12 processeurs logiques.
- Mémoire physique déclarée par Windows : environ 15,3 Gio (classe 16 Go).
- Circuit graphique déclaré : AMD Radeon(TM) Graphics ; ne pas assimiler la valeur AdapterRAM Windows à une capacité utilisable garantie pour un modèle IA.
- Espace libre observé sur C: : environ 27 Go, valeur susceptible de changer.
- Aucun modèle IA ni temps d’analyse n’a encore été testé sur cette machine. Dimensionnement, stockage des modèles et limites de charge à valider par des mesures.

## Travaux nécessaires avant développement complet

- Vérifier les sources accessibles, licences, formats et coûts réels.
- Formaliser et éprouver les calculs, les comparables, les scores et les scénarios.
- Tester extraction documentaire et analyse visuelle sur la machine cible.
- Finaliser architecture, écrans, modèle de données et critères de réception.
- Voir `recommandations-et-faisabilite.md` pour les décisions déléguées et les vérifications récentes.
- Voir `validation-technique.md` pour le test réel du connecteur Karoto, l’architecture de référence, les calculs, les critères de réception et le protocole IA encore à exécuter. Le succès du connecteur ne prouve pas une surveillance exhaustive.
