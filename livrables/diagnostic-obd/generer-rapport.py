from pathlib import Path
import re, html, unicodedata, json
p=Path(__file__).parent
md=(p/'rapport-diagnostic-obd.md').read_text(encoding='utf-8')
def slug(s):
    return re.sub(r'[^a-z0-9]+','-',unicodedata.normalize('NFKD',s).encode('ascii','ignore').decode().lower()).strip('-')
def inline(s):
    s=html.escape(s)
    s=re.sub(r'\[([^\]]+)\]\((https?://[^\s)]+)\)',lambda m:'<a href="'+m[2]+'" target="_blank" rel="noopener">'+m[1]+'</a>',s)
    s=re.sub(r'\*\*(.+?)\*\*',r'<strong>\1</strong>',s)
    s=re.sub(r'(?<!\*)\*([^*]+)\*(?!\*)',r'<em>\1</em>',s)
    return s
lines=md.splitlines(); out=[]; toc=[]; para=[]; i=0
def flush():
    if para:
        out.append('<p>'+inline(' '.join(para))+'</p>');para.clear()
while i<len(lines):
    line=lines[i].strip()
    if not line:flush();i+=1;continue
    if line.startswith('#'):
        flush(); m=re.match(r'(#+) (.*)',line); n=len(m[1]); title=m[2]; ident=slug(title)
        if n==1:
            out.append('<header class="hero"><div class="eyebrow">LOGICIEL AUTOMOBILE · GUIDE D’ACHAT</div><h1>'+inline(title)+'</h1><div class="meta">France · 5 octobre 2026 · Budget total 500 €</div></header>')
        else:
            out.append(f'<h{n} id="{ident}">'+inline(title)+f'</h{n}>')
            if n==2 and re.match(r'\d+\.',title):toc.append((ident,title))
        i+=1;continue
    if line.startswith('|'):
        flush(); table=[]
        while i<len(lines) and lines[i].strip().startswith('|'):
            table.append([x.strip() for x in lines[i].strip().strip('|').split('|')]);i+=1
        if len(table)>1 and all(re.fullmatch(r'[:\- ]+',x) for x in table[1]):table.pop(1)
        out.append('<div class="table-wrap"><table><thead><tr>'+''.join('<th>'+inline(x)+'</th>' for x in table[0])+'</tr></thead><tbody>')
        for row in table[1:]:out.append('<tr>'+''.join('<td>'+inline(x)+'</td>' for x in row)+'</tr>')
        out.append('</tbody></table></div>');continue
    if line.startswith('- '):
        flush();out.append('<ul>')
        while i<len(lines) and lines[i].strip().startswith('- '):
            out.append('<li>'+inline(lines[i].strip()[2:])+'</li>');i+=1
        out.append('</ul>');continue
    para.append(line);i+=1
flush()
css="""*{box-sizing:border-box}html{scroll-behavior:smooth}body{margin:0;background:#f3f5f8;color:#202b3a;font:16px/1.68 system-ui,-apple-system,Segoe UI,sans-serif}nav{position:fixed;inset:0 auto 0 0;width:282px;background:#112437;color:#e7eff7;padding:28px 20px;overflow:auto}nav .brand{font-weight:800;font-size:22px;color:white;line-height:1.2}nav .tag{margin:10px 0 23px;color:#99b1c8;font-size:13px}nav a{display:block;color:#d5e3f0;text-decoration:none;padding:8px 9px;font-size:13px;line-height:1.4;border-radius:7px}nav a:hover{background:#253c51;color:white}main{margin-left:282px;max-width:1550px;padding:32px 42px 90px}.hero{border-radius:18px;background:#112437;color:white;padding:36px 40px;margin-bottom:30px;box-shadow:0 12px 30px #0e26351a}.hero h1{font-size:clamp(30px,3vw,46px);line-height:1.13;max-width:900px;letter-spacing:-1.5px;margin:15px 0 20px}.eyebrow{color:#62dec2;letter-spacing:2px;font-size:12px;font-weight:800}.meta{font-size:14px;color:#b9cada}h2{margin:44px 0 18px;font-size:26px;line-height:1.28;color:#112437;scroll-margin-top:24px}h3{font-size:20px;color:#1c5268;margin-top:30px}p,li{max-width:1050px}p{margin:15px 0}strong{color:#102e43}.hero strong{color:white}a{color:#12688b;text-decoration:underline;text-underline-offset:3px}a:hover{color:#0a4056}.table-wrap{overflow:auto;background:white;border:1px solid #d9e1e7;border-radius:12px;margin:23px 0;box-shadow:0 4px 15px #18394707}table{border-collapse:collapse;width:100%;font-size:13px;line-height:1.48}th{background:#17364b;color:white;text-align:left;font-weight:650;padding:13px 14px;min-width:135px}td{padding:12px 14px;border-top:1px solid #e7ebef;vertical-align:top;min-width:135px}td:first-child{font-weight:650;background:#f5f8fa;min-width:165px;position:sticky;left:0}tr:nth-child(even) td:not(:first-child){background:#fbfcfd}ul{padding-left:22px}li{margin:7px 0}.files{border-top:1px solid #375266;margin-top:24px;padding-top:16px}.files a{color:#66dfc4}.footer{border-top:1px solid #d1dce3;margin-top:38px;padding-top:18px;color:#637587;font-size:13px}.notice{background:#fff7e5;border-left:4px solid #dbab36;padding:14px 18px;border-radius:8px;max-width:1050px}@media(max-width:1000px){nav{position:static;width:auto;padding:20px}nav .links{display:none}nav .files{display:flex;flex-wrap:wrap;margin-top:15px;padding-top:6px}main{margin-left:0;padding:22px}.hero{padding:25px}}@media print{nav{display:none}main{margin:0;padding:0}.hero{color:#112437;background:white;box-shadow:none}.hero .eyebrow,.hero .meta{color:#112437}.table-wrap{overflow:visible;box-shadow:none;border-radius:0}table{font-size:9px}td,th{min-width:0;padding:5px}td:first-child{position:static;min-width:0}h2{break-after:avoid}tr{break-inside:avoid}a{color:inherit}body{background:white;font-size:11px}}"""
nav='<nav><div class="brand">DIAGNOSTIC<br>AUTOMOBILE</div><div class="tag">Comparatif documenté · Budget 500 €</div><div class="links">'+''.join('<a href="#'+ident+'">'+html.escape(title)+'</a>' for ident,title in toc)+'</div><div class="files"><a href="comparatif-56-criteres.csv">↓ Comparatif CSV · 56 critères</a><a href="sources-web.csv">↓ Registre des sources</a><a href="rapport-diagnostic-obd.md">↓ Texte intégral Markdown</a></div></nav>'
doc='<!doctype html><html lang="fr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Diagnostic automobile : quel outil sous 500 € ?</title><style>'+css+'</style></head><body>'+nav+'<main>'+''.join(out)+'<div class="footer">Recherche du 5 octobre 2026. Informations documentaires, sans test physique. Les cases non confirmées restent des inconnues.</div></main></body></html>'
(p/'rapport-diagnostic-obd.html').write_text(doc,encoding='utf-8')
from bs4 import BeautifulSoup
s=BeautifulSoup(doc,'html.parser')
ids={x.get('id') for x in s.select('[id]')}
assert all(x['href'][1:] in ids for x in s.select('nav a[href^="#"]'))
assert len(s.select('table'))==len([x for x in lines if x.startswith('|---')])
for t in s.select('table'):
    count=len(t.select('thead th'))
    assert all(len(r.find_all('td',recursive=False))==count for r in t.select('tbody tr'))
assert len(s.select('h1'))==1
assert 'NC-B' in s.get_text()
assert len(s.select('table'))>=8
print(json.dumps({'words':len(md.split()),'html_bytes':len(doc.encode()),'tables':len(s.select('table')),'source_links':len(s.select('main a')),'sections':len(toc)},ensure_ascii=False))

