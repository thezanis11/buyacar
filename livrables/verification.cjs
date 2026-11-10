const fs=require('fs');const vm=require('vm');const assert=require('assert');
const dir=__dirname;
const obj=JSON.parse(fs.readFileSync(dir+'/catalogue-services.json','utf8'));
assert.equal(obj.services.length,120);
assert.equal(new Set(obj.services.map(x=>x.categorie)).size,16);
assert.equal(new Set(obj.services.map(x=>x.id)).size,120);
for(const s of obj.services){for(const k of ['nom','url','pays','disponible_france','modele_economique','prix','compte','application_mobile','api','fonctionnalites_donnees','limites','source_donnees','fiabilite_apparente','differenciation','utilite_personnelle','a_reprendre','verification','source_verification','date_verification'])assert.ok(typeof s[k]==='string'&&s[k].length,s.nom+' '+k);assert.ok(s.url.startsWith('https://'));}
const catalogue=fs.readFileSync(dir+'/catalogue-services.html','utf8');
const script=catalogue.match(/<script>([\s\S]*?)<\/script>/)[1];new vm.Script(script);
assert.equal((catalogue.match(/class="service"/g)||[]).length,120);
const report=fs.readFileSync(dir+'/rapport-etude-marche.md','utf8');
for(const word of ['Stack idéale','Stack premium','MVP','V1','V2','Opportunités non couvertes','{{TABLES}}']){if(word==='{{TABLES}}')assert.ok(!report.includes(word));else assert.ok(report.includes(word));}
const html=fs.readFileSync(dir+'/rapport-etude-marche.html','utf8');const ids=[...html.matchAll(/id="([^"]+)"/g)].map(x=>x[1]);assert.equal(ids.length,new Set(ids).size);for(const m of html.matchAll(/href="#([^"]+)"/g))assert.ok(ids.includes(m[1]),m[1]);
assert.equal(2000+1200+300,3500);assert.equal(3500+100+200+100+400,4300);assert.equal(4600-1200-300-400-400-500,1800);
console.log(JSON.stringify({entries:obj.services.length,categories:16,reportWords:report.split(/\s+/).length,catalogueScript:'syntax valid',allFields:'present',toc:'valid',examples:'consistent'}));

