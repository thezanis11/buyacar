const {chromium}=require('@playwright/test');
const fs=require('node:fs');const path=require('node:path');
(async()=>{
 const browser=await chromium.launch({channel:'chrome',headless:true});
 const page=await browser.newPage({viewport:{width:1440,height:1000}});
 const errors=[];page.on('pageerror',e=>errors.push(e.message));
 const base='http://127.0.0.1:5198';
 await page.goto(base);await page.getByLabel(/mot de passe/i).fill('essai-local-automobile-123');await page.getByRole('button',{name:/Créer mon espace|Ouvrir mon espace/}).click();await page.getByRole('heading',{name:'La bonne voiture, au bon coût.'}).waitFor();
 await page.getByRole('button',{name:'＋ Ajouter une annonce'}).click();
 await page.getByLabel('Titre du dossier').fill('Clio test parcours');await page.getByLabel('Prix affiché (€)',{exact:true}).fill('2000');await page.getByLabel('Marque',{exact:true}).fill('Renault');await page.getByLabel('Modèle',{exact:true}).fill('Clio');await page.getByLabel('Version et moteur précis').fill('1.5 dCi 85');await page.locator('select[name=fuel]').selectOption('Diesel');await page.locator('select[name=gearbox]').selectOption('Manuelle');await page.getByLabel('Année',{exact:true}).fill('2009');await page.getByLabel('Kilométrage',{exact:true}).fill('200000');await page.getByRole('button',{name:'Enregistrer le dossier',exact:true}).click();await page.getByRole('heading',{name:'Clio test parcours',exact:true}).waitFor();
 await page.getByRole('button',{name:'Coûts & scénarios',exact:true}).click();await page.getByRole('button',{name:'＋ Ajouter un poste'}).click();await page.getByLabel('Poste',{exact:true}).fill('Travaux et frais test');await page.getByLabel('Coût optimiste').fill('700');await page.getByLabel('Coût central').fill('1000');await page.getByLabel('Coût pessimiste').fill('1500');await page.getByLabel(/J’ai passé en revue/).check();await page.getByRole('button',{name:'Enregistrer les coûts',exact:true}).click();await page.waitForTimeout(500);
 const evaluated=await page.evaluate(async()=>{const s=await(await fetch('/api/state')).json();const v=s.vehicles.find(v=>v.title==='Clio test parcours');return(await fetch('/api/evaluate/'+v.id)).json()});if(evaluated.central!==3000||evaluated.pessimistic!==3500)throw Error('Calculs UI non persistés');
 await page.getByRole('button',{name:'Documents & analyse',exact:true}).click();await page.getByLabel('Ajouter un document').setInputFiles({name:'preuve.txt',mimeType:'text/plain',buffer:Buffer.from('Facture exemple, pièce 100 euros')});await page.getByRole('button',{name:'Importer',exact:true}).click();await page.getByRole('link',{name:'preuve.txt'}).waitFor();
 await page.getByLabel('Codes ou texte du rapport').fill('P0300 P0420');await page.getByRole('button',{name:'Enregistrer et expliquer les codes'}).click();await page.getByRole('heading',{name:'P0300',exact:true}).waitFor();
 const pdfPage=await browser.newPage();await pdfPage.setContent('<h1>FACTURE TEST</h1><p>Freinage : 100 EUR</p>');const pdf=await pdfPage.pdf({format:'A4'});await pdfPage.close();
 await page.getByLabel('Ajouter un document').setInputFiles({name:'facture-test.pdf',mimeType:'application/pdf',buffer:pdf});await page.getByRole('button',{name:'Importer',exact:true}).click();await page.getByRole('link',{name:'facture-test.pdf'}).waitFor();
 await page.locator('.notice').filter({has:page.getByRole('link',{name:'facture-test.pdf'})}).getByRole('button',{name:'Extraire le texte'}).click();await page.getByRole('heading',{name:/Texte extrait : facture-test.pdf/}).waitFor();if(!await page.locator('pre').filter({hasText:'FACTURE TEST'}).count())throw Error('Extraction PDF absente');
 await page.getByRole('link',{name:'Vue d’ensemble',exact:false}).first().click();await page.getByRole('button',{name:'Créer ma recherche →'}).click();await page.getByLabel('Nom de la recherche').fill('Budget test');await page.getByLabel('Budget après travaux maximal (€)').fill('3000');await page.getByRole('button',{name:'Enregistrer la recherche'}).click();await page.getByRole('heading',{name:'Budget test'}).waitFor();
 await page.goto(base+'/#vehicles');await page.getByRole('heading',{name:'Mes dossiers',exact:true}).waitFor();
 const out=path.resolve(__dirname,'../test-results');fs.mkdirSync(out,{recursive:true});await page.screenshot({path:path.join(out,'dossiers-desktop.png'),fullPage:true});
 await page.setViewportSize({width:390,height:844});await page.screenshot({path:path.join(out,'dossiers-mobile.png'),fullPage:true});if(await page.evaluate(()=>document.documentElement.scrollWidth>window.innerWidth))throw Error('Débordement horizontal mobile');
 const noAuth=await browser.newContext();const unauth=await noAuth.request.get(base+'/api/state');if(unauth.status()!==401)throw Error('Accès anonyme non protégé');
 const csrf=await page.request.post(base+'/api/notices/read',{data:{}});if(csrf.status()!==403)throw Error('Protection requêtes absente');
 const native=await page.request.get(base+'/api/desktop-events');if(native.status()!==403)throw Error('Canal bureau exposé sans jeton');
 const current=await page.evaluate(async()=>{const s=await(await fetch('/api/state')).json();return s.vehicles.find(v=>v.title==='Clio test parcours').id});
 const invalidFile=await page.request.post(base+'/api/vehicles/'+current+'/documents',{headers:{'X-Auto-Request':'1'},multipart:{file:{name:'faux.pdf',mimeType:'application/pdf',buffer:Buffer.from('<script>not a pdf</script>')}}});if(invalidFile.status()!==400)throw Error('Faux PDF accepté');
 await page.setViewportSize({width:1100,height:900});await page.getByRole('link',{name:'Voir le dossier →'}).first().click();await page.evaluate(()=>window.print=()=>{});await page.getByRole('button',{name:'Rapport / PDF'}).click();await page.locator('#print-report').waitFor({state:'attached'});await page.pdf({path:path.join(out,'rapport-visite-test.pdf'),format:'A4',printBackground:true});await page.evaluate(()=>window.dispatchEvent(new Event('afterprint')));
 if(process.env.AUTO_LIVE_TEST==='1'){
 const r=await page.evaluate(async()=>{const s=await(await fetch('/api/state')).json();const id=s.searches[0].id;const r=await fetch('/api/searches/'+id+'/run',{method:'POST',headers:{'X-Auto-Request':'1','Content-Type':'application/json'},body:'{}'});return{status:r.status,data:await r.json()}});
 if(r.status!==200||!r.data.results?.length)throw Error('Recherche réelle échouée : '+JSON.stringify(r));console.log('Recherche réelle OK : '+r.data.status);
 }
 if(errors.length)throw Error(errors.join('\n'));
 console.log('Parcours UI OK : compte, dossier, coûts, documents et extraction PDF, rapport PDF, OBD, recherche, mobile, authentification et protection des mutations.');
 await browser.close();
})().catch(e=>{console.error(e);process.exit(1)});

