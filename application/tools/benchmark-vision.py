import base64,json,pathlib,time,urllib.request
root=pathlib.Path(__file__).resolve().parents[1]
image=base64.b64encode((root/'test-results/facture-fictive.png').read_bytes()).decode()
payload={"model":"gemma3:4b","stream":False,"keep_alive":"1m","options":{"num_ctx":2048,"num_thread":4,"temperature":0,"num_predict":180},"messages":[{"role":"system","content":"Lis les faits visibles du document. Distingue facture et prix de vente du véhicule. N’invente aucune information. Réponds en français brièvement."},{"role":"user","content":"Quels sont les montants, le kilométrage et le prix de vente du véhicule visibles ?","images":[image]}]}
start=time.monotonic()
req=urllib.request.Request('http://127.0.0.1:11434/api/chat',json.dumps(payload).encode(),{'Content-Type':'application/json'})
with urllib.request.urlopen(req,timeout=300) as response: answer=json.load(response)
result={"seconds":round(time.monotonic()-start,2),"response":answer['message']['content'],"expected":"100 EUR pièces, 150 EUR main-d’œuvre, 250 EUR total facture, 82000 km, prix vente absent","scope":"Un document factice net : aucun test de défaut de carrosserie"}
(root/'test-results/benchmark-vision.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8');print(json.dumps(result,ensure_ascii=False))
