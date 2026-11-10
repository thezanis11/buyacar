# AutoClair — guide de démarrage

Version de développement du 5 novembre 2026. Le logiciel a été construit dans le dossier `application`. Il ne s’agit pas encore de la version complète du cahier des charges ; les limites sont détaillées dans `ETAT-DU-PROJET.md`.

## 1. Ouvrir le logiciel

1. Ouvrez l’Explorateur Windows avec **Windows + E**.
2. Collez ce chemin dans sa barre d’adresse puis appuyez sur Entrée :

   `C:\Projets\AutoClair\application`

3. Double-cliquez sur **Lancer-AutoClair.cmd**. L’extension `.cmd` peut être masquée par Windows.
4. À la première ouverture, choisissez un mot de passe d’au moins 12 caractères. Il protège votre espace local ; aucun compte externe n’est créé. Conservez-le dans votre gestionnaire de mots de passe : il n’existe pas encore de récupération par e-mail.

Les fichiers nécessaires sont déjà compilés dans `dist`. Vous n’avez pas besoin de lancer `Construire.ps1` pour utiliser cette version.

Si la fenêtre Windows ne s’ouvre pas, utilisez **Lancer-Navigateur.cmd**. Le site s’ouvre alors à l’adresse `http://127.0.0.1:5187` sur ce PC. Les deux interfaces utilisent les mêmes données. La fenêtre Windows utilise Microsoft WebView2 ; si ce composant manque, le navigateur reste utilisable.

## 2. Ajouter votre premier véhicule

- Cliquez sur **Ajouter une annonce**.
- Renseignez titre, prix, modèle, version/moteur, motorisation, boîte, année et kilométrage.
- Collez le lien et le texte de l’annonce si vous les avez. Le lien sert de référence ; aucun site arbitraire n’est aspiré.
- Enregistrez. Les informations absentes sont signalées dans le dossier.
- Dans **Coûts & scénarios**, ajoutez les frais, travaux, entretien immédiat et réserve. Utilisez vos devis ou vos hypothèses explicites. Une case vide signifie « inconnu », et non zéro.
- Cochez la revue des coûts uniquement lorsque vous avez passé en revue les postes nécessaires. Les totaux s’affichent alors si les montants sont renseignés.

Pour chaque poste, les montants doivent respecter : optimiste ≤ central ≤ pessimiste. Ils représentent des hypothèses, pas des probabilités garanties. Le prix demandé reste la base de calcul.

## 3. Rechercher des annonces

Dans **Recherches**, créez une recherche avec vos deux plafonds et vos filtres. Chaque critère peut être obligatoire ou souhaité. Choisissez **Chercher maintenant**.

La source initiale est [Karoto](https://karoto.fr/connecteur-ia). Jusqu’à 20 annonces sont renvoyées par recherche ; le nombre de correspondances annoncé ne signifie pas que toutes ont été importées. L’application affiche cette limite. Les annonces ne sont pas automatiquement considérées comme de bonnes affaires.

Activez la surveillance pour une vérification selon les heures choisies. Le PC et le service doivent fonctionner. Après une interruption, une seule vérification rattrape les créneaux manqués. En cas d’erreur, les anciens résultats sont conservés et une pause protège la source.

Les données reçues peuvent être incomplètes : complétez notamment modèle, version et localisation. Une distance obligatoire sans distance vérifiée ne sera pas considérée comme respectée.

## 4. Comparer et suivre

- Dans **Mes dossiers**, cochez **Comparer** sur deux à quatre véhicules.
- Ouvrez **Comparer**, puis choisissez une recherche commune : mêmes budgets, durée et valorisation du temps.
- L’onglet **Vue d’ensemble** d’un dossier permet d’ajouter notes, étape d’achat et rappel.
- Au passage **Acheté**, une copie des prévisions est conservée. Dans **Après l’achat**, ajoutez le prix réellement payé et les autres dépenses. Ne comptez pas l’achat deux fois.
- Les marges affichées sont avant fiscalité. Une valeur manquante empêche le calcul concerné.

## 5. Documents, photos et rapport de visite

Dans **Documents & analyse**, importez PDF, JPEG, PNG ou texte, jusqu’à 20 Mo par fichier. Le bouton **Extraire le texte** lit les PDF contenant du texte et les fichiers texte, en conservant les numéros de pages. Un PDF scanné sans texte doit être fourni sous forme d’image pour l’analyse visuelle.

Les résultats d’analyse ne modifient pas vos données à votre insu. Relisez-les et corrigez les caractéristiques vous-même dans cette première version.

**Rapport / PDF** prépare un document imprimable comprenant les informations, scénarios, questions de visite, travaux et sources. Dans la fenêtre d’impression, choisissez **Enregistrer au format PDF** dans le navigateur, ou **Microsoft Print to PDF** dans Windows. Transférez ce fichier sur votre téléphone avant de partir.

Les codes OBD peuvent être collés ou saisis. Quelques codes génériques sont expliqués ; les autres sont à vérifier dans la documentation constructeur. Aucun remplacement de pièce n’est déduit automatiquement.

## 6. IA locale

L’application peut utiliser Ollama sur ce PC pour analyser le texte ou une image. Les fichiers du moteur et les modèles sont dans `.ollama`, sous le dossier de l’application. L’option est désactivée au départ : activez-la dans **Réglages** après lecture des résultats de validation dans `ETAT-DU-PROJET.md`.

Lors d’une analyse, le moteur installé peut démarrer automatiquement en arrière-plan. Il ne télécharge pas de modèle à votre insu. Le modèle par défaut est `gemma3:4b`. Aucun fournisseur d’IA externe n’est configuré.

Une seule analyse s’exécute à la fois, avec un contexte limité et quatre threads. Le modèle est déchargé après une minute d’inactivité. Les essais ont révélé des erreurs sur un prix, un montant de facture et un kilométrage lu sur image. L’option est donc expérimentale et ne remplit pas automatiquement les champs. Une photo ne permet pas de garantir l’état mécanique. Vérifiez chaque valeur utile dans l’original.

## 7. Sauvegarder et restaurer

Dans **Réglages**, cliquez sur **Télécharger ma sauvegarde** et placez l’archive sur un autre support. La sauvegarde est manuelle, sans calendrier automatique. Elle contient vos données privées sans chiffrement : ne la partagez pas publiquement.

Pour restaurer, sauvegardez d’abord l’état actuel, choisissez votre archive, cochez la confirmation et lancez la restauration. L’application vérifie les empreintes des fichiers et crée une copie séparée de contrôle avant de remplacer les dossiers actifs. Le mot de passe actuel est conservé. Sur une autre installation, créez d’abord un nouveau compte local puis restaurez l’archive.

Vos données actives sont dans `%LOCALAPPDATA%\AutoClair\data`. Le dossier de conception et les dossiers de tests ne sont pas vos données personnelles de production. Les anciennes sauvegardes et copies de restauration peuvent encore contenir un document supprimé de l’application.

## 8. Raccourci et démarrage Windows

Double-cliquez sur **Installer-Raccourcis.cmd** pour créer le raccourci sur le Bureau et activer le démarrage à l’ouverture de session.

Fermer la fenêtre masque AutoClair : la surveillance continue. Faites un clic droit sur son icône dans la zone de notification Windows pour rouvrir le logiciel, désactiver le démarrage automatique ou choisir **Arrêter le service et quitter**.

Le lanceur de secours dans le navigateur ne fournit pas l’icône de notification Windows. Pour l’usage quotidien et l’arrêt simple, privilégiez le lanceur AutoClair.

## 9. Utiliser le téléphone à la maison

Cette étape nécessite une configuration propre à votre réseau ; elle n’a pas été validée sur votre téléphone.

1. Créez d’abord le compte sur le PC et fermez le service.
2. Lancez **Configurer-Reseau.cmd**. Il affiche les adresses privées du PC. Choisissez celle du Wi-Fi ou de l’Ethernet domestique, pas un réseau virtuel ou un VPN.
3. Le programme affiche une adresse `https://…:5188` et crée un certificat public `AutoClair-reseau.cer` dans `%LOCALAPPDATA%\AutoClair\data`.
4. Le pare-feu Windows doit autoriser le port TCP 5188 sur le profil **Privé**, depuis le **sous-réseau local seulement**. Ne créez aucune redirection de port sur la box. Cette règle n’est pas ajoutée automatiquement par le script.
5. Installez le certificat public comme certificat de confiance sur vos appareils personnels. La procédure dépend d’Android ou d’iOS. Ne transférez jamais `lan.pfx` ni `lan.json` : ces fichiers contiennent des secrets du serveur.
6. Relancez AutoClair. Sur le téléphone connecté au même Wi-Fi, ouvrez l’adresse HTTPS affichée et connectez-vous avec votre mot de passe local.

Si l’adresse du PC change, il faut refaire cette configuration et renouveler la confiance du certificat. Pour revenir au PC uniquement, service arrêté, renommez `lan.json` en `lan-desactive.json`. Le logiciel écoutera seulement sur le PC au redémarrage.

## 10. Dépannage

- **Aucune annonce** : distinguez « Aucun résultat » d’une erreur de source. Réessayez après la pause indiquée. L’import manuel reste disponible.
- **Budget à vérifier** : un prix, un coût nécessaire ou la revue des postes manque. Ne mettez pas zéro pour faire disparaître l’alerte.
- **Pas de note globale** : cette version ne dispose pas encore d’un barème de fiabilité validé. C’est une limite affichée, pas une panne.
- **Analyse locale indisponible** : vérifiez le modèle dans les réglages. Vous pouvez continuer à saisir les faits et à utiliser les calculs.
- **Programme non compilé** : les fichiers `dist` ont été supprimés ou déplacés. Le développement peut les reconstruire avec `Construire.ps1` ; aucune compétence technique n’est nécessaire pour utiliser les fichiers déjà publiés.
- **Port occupé / lancement impossible** : vérifiez qu’une autre instance AutoClair n’est pas déjà ouverte. N’effacez pas le dossier de données.

L’utilisation courante de cette version ne dépend pas d’un service payant. La gratuité et la disponibilité des sources externes restent susceptibles d’évoluer.

## Nouveaux outils dans chaque dossier

Dans **Évaluation & négociation**, consultez les raisons de l’évaluation. Ajoutez au moins trois annonces comparables récentes et renseignez des appréciations documentées si vous disposez des preuves. Sans les données nécessaires, le logiciel affiche une évaluation partielle. Le score est un indice de vos préférences, pas une expertise mécanique.

Le simulateur de prix négocié laisse le prix de l’annonce intact. Son brouillon de message reste à modifier et envoyer vous-même.

Dans **Coûts & scénarios**, vous pouvez enregistrer les pièces et les heures d’un devis comme référence, puis calculer le coût en garage ou en réalisation personnelle. Après ajout, revérifiez vos frais et cochez la revue des coûts. Les heures personnelles se renseignent séparément ; elles ne sont pas des dépenses payées.

Le bloc **Coût d’usage selon le kilométrage** remplace, si vous l’activez, le forfait annuel par un calcul d’énergie, assurance, entretien et autres frais. Une électrique utilise les kWh/100 km ; un hybride rechargeable peut utiliser deux lignes (carburant et électricité). Indiquez des consommations moyennes sur l’ensemble du parcours. Les données manquantes ne deviennent jamais zéro.

Dans **Documents & analyse**, les faits proposés peuvent être validés ou écartés avec leur source visible. Dans **Réglages**, la file des traitements permet de consulter, annuler ou relancer les approfondissements. L’IA doit être activée séparément ; les analyses automatiques lourdes attendent que le PC soit inactif et disposent d’assez de mémoire. Aucun traitement ne fonctionne PC éteint.

## Lire une facture photographiée ou scannée

Importez le fichier PNG, JPEG ou PDF dans **Documents & analyse**, puis cliquez sur **Extraire le texte**. Le moteur français fourni fonctionne sur votre PC, même avec l’IA désactivée. Il lit les images et les pages scannées compatibles. Pour limiter le travail du PC, divisez les scans en lots de 10 pages au maximum. Les PDF composés de texte acceptent jusqu’à 50 pages.

Vérifiez les chiffres, dates et noms dans le document d’origine. Si une image du PDF ne peut pas être décodée, le résultat le signale : importez cette page séparément en PNG ou JPEG. Un résultat OCR n’est jamais appliqué automatiquement aux caractéristiques du véhicule.
