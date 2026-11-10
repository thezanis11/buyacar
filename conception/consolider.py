from pathlib import Path
import re

p = Path(__file__).with_name('cahier-des-charges.md')
s = p.read_text(encoding='utf-8')
blocks = re.findall(r'^### (\d+)\. (.*?)(?=^## |^### |\Z)', s, re.M | re.S)
assert len(blocks) == 37 and len({n for n, _ in blocks}) == 37
material = s.split('## Matériel observé', 1)[1].split('## Décisions encore ouvertes', 1)[0]
out = '# Application automobile — cahier des charges en construction\n\n'
out += 'Décisions consolidées le 5 novembre 2026. Les exigences sont définies ; les sources, les performances IA et les règles de calcul doivent encore être validées avant transmission pour développement complet.\n\n'
out += '## Méthode de conception actualisée\n\n'
out += '- Conserver tous les choix déjà validés.\n- Pour les décisions restantes, appliquer directement les recommandations de l’assistant, conformément à la demande du 5 novembre 2026.\n- Réserver les questions aux arbitrages réellement incertains, les regrouper et proposer des choix cliquables.\n- Laisser le mode export hors du périmètre pour le moment, conformément au retrait de cette proposition dans le dernier message.\n- Les recommandations ne valent pas autorisation de dépense, de publication ou de transmission de documents.\n\n## Décisions confirmées\n\n'
for n, body in sorted(blocks, key=lambda pair: int(pair[0])):
    body = body.strip()
    body = body.replace('- L’accès depuis l’extérieur du domicile reste à décider.', '- Accès limité au réseau domestique, conformément à la décision 10.')
    body = body.replace('- La liste des éléments indispensables au classement reste à définir.', '- Le socle indispensable est fixé à la décision 15.')
    out += f'### {n}. {body}\n\n'
out += '### 38. Critères — obligatoires ou souhaités\n- Permettre de choisir pour chaque critère s’il est obligatoire ou souhaité.\n- Exclure sur les exigences strictes ; utiliser les préférences pour le classement et signaler les écarts.\n- Maintenir les règles de budget séparées.\n- Si une exigence obligatoire ne peut pas être vérifiée, afficher « à vérifier » et ne pas présenter le véhicule comme conforme.\n\n'
out += '## Matériel observé' + material.strip('\n') + '\n\n'
out += '## Travaux nécessaires avant développement complet\n\n- Vérifier les sources accessibles, licences, formats et coûts réels.\n- Formaliser et éprouver les calculs, les comparables, les scores et les scénarios.\n- Tester extraction documentaire et analyse visuelle sur la machine cible.\n- Finaliser architecture, écrans, modèle de données et critères de réception.\n- Voir `recommandations-et-faisabilite.md` pour les décisions déléguées et les vérifications récentes.\n'
p.write_text(out, encoding='utf-8')
print(f'{len(blocks)+1} décisions consolidées ; méthode actualisée.')
