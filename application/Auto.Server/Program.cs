using Auto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;
using System.Text.Json;
using System.Net;

var root = Environment.GetEnvironmentVariable("AUTO_DATA") ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoClair", "data");
if (args.Length == 2 && args[0] == "--configure-lan") { Lan.Configure(root, args[1]); return; }
var builder = WebApplication.CreateBuilder(args);
Directory.CreateDirectory(root);
var protection = builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(root, "keys")));
if (OperatingSystem.IsWindows()) protection.ProtectKeysWithDpapi();
var store = new Store(root);
builder.Services.AddSingleton(store); builder.Services.AddSingleton<Karoto>(); builder.Services.AddSingleton<Auto.Monitor>(); builder.Services.AddHostedService(sp => sp.GetRequiredService<Auto.Monitor>());
builder.Services.AddSingleton(_ => new LocalAi(Environment.GetEnvironmentVariable("AUTO_AI_ROOT") ?? Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath,"..","..",".ollama"))));
builder.Services.AddSingleton<AnalysisService>();builder.Services.AddHostedService<DeepWorker>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => {
    o.Cookie.Name = "AutoClair.Session"; o.Cookie.HttpOnly = true; o.Cookie.SameSite = SameSiteMode.Strict; o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    o.ExpireTimeSpan = TimeSpan.FromHours(8); o.SlidingExpiration = true;
    o.Events.OnRedirectToLogin = c => { c.Response.StatusCode = 401; return Task.CompletedTask; };
});
builder.Services.AddAuthorization();
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 210_000_000);
LanConfig? lan = null;
if (string.IsNullOrEmpty(builder.Configuration["urls"])) {
    if (File.Exists(Path.Combine(root, "lan.json"))) {
        lan = JsonSerializer.Deserialize<LanConfig>(File.ReadAllText(Path.Combine(root, "lan.json")), Store.Json);
        builder.WebHost.ConfigureKestrel(o => { o.Listen(IPAddress.Loopback, 5187); o.Listen(IPAddress.Parse(lan!.Address), 5188, listen => listen.UseHttps(Path.Combine(root, "lan.pfx"), lan.Password)); });
    } else builder.WebHost.UseUrls("http://127.0.0.1:5187");
}
var app = builder.Build();
app.Use(async (ctx, next) => {
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
    ctx.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' blob:; connect-src 'self'; frame-ancestors 'none'; object-src 'none'; base-uri 'none'; form-action 'self'";
    var remote = ctx.Connection.RemoteIpAddress;
    bool loopback = remote is not null && IPAddress.IsLoopback(remote);
    if (remote is null || !Lan.Private(remote)) { ctx.Response.StatusCode = 403; return; }
    if (!loopback && !ctx.Request.IsHttps) { ctx.Response.StatusCode = 403; await ctx.Response.WriteAsJsonAsync(new { error = "L’accès réseau nécessite HTTPS." }); return; }
    if (ctx.Request.Path.StartsWithSegments("/api"))
    {
        ctx.Response.Headers.CacheControl = "no-store";
        if (ctx.Request.Method is not ("GET" or "HEAD") && ctx.Request.Headers["X-Auto-Request"] != "1") { ctx.Response.StatusCode = 403; return; }
        var origin = ctx.Request.Headers.Origin.ToString();
        if (origin.Length > 0 && origin != $"{ctx.Request.Scheme}://{ctx.Request.Host}") { ctx.Response.StatusCode = 403; return; }
    }
    try { await next(); }
    catch (ArgumentException e) { ctx.Response.StatusCode = 400; await ctx.Response.WriteAsJsonAsync(new { error = e.Message }); }
    catch (Exception e) when (e is JsonException or InvalidDataException or KeyNotFoundException) { ctx.Response.StatusCode = 400; await ctx.Response.WriteAsJsonAsync(new { error = "Données invalides ou incomplètes." }); }
    catch (Exception e) { app.Logger.LogError(e, "Opération échouée"); if (!ctx.Response.HasStarted) { ctx.Response.StatusCode = 500; await ctx.Response.WriteAsJsonAsync(new { error = "L’opération a échoué. Vos données enregistrées sont conservées." }); } }
});
app.UseDefaultFiles(); app.UseStaticFiles(); app.UseAuthentication(); app.UseAuthorization();
app.MapGet("/api/session", (HttpContext ctx) => new { configured = store.HasAccount(), authenticated = ctx.User.Identity?.IsAuthenticated == true });
var desktopToken = Environment.GetEnvironmentVariable("AUTO_DESKTOP_TOKEN");
app.MapGet("/api/desktop-events", (HttpContext ctx) => {
    if (string.IsNullOrEmpty(desktopToken) || !IPAddress.IsLoopback(ctx.Connection.RemoteIpAddress!) || ctx.Request.Headers["X-Auto-Desktop"] != desktopToken) return Results.StatusCode(403);
    var s = store.Read(); return Results.Ok(new { preferences = s.Preferences, notices = s.Notices.Where(n=>!n.Read).Take(100).ToArray() });
});
var loginGate = new SemaphoreSlim(1);
var failures = 0; var blockedUntil = DateTimeOffset.MinValue;
app.MapPost("/api/login", async (HttpContext ctx, Login request) => {
    await loginGate.WaitAsync();
    try {
        if (blockedUntil > DateTimeOffset.UtcNow) return Results.Json(new { error = "Trop de tentatives. Patientez une minute." }, statusCode: 429);
        if (!store.HasAccount()) {
            if (!IPAddress.IsLoopback(ctx.Connection.RemoteIpAddress!)) return Results.Forbid();
            store.CreateAccount(request.Password);
        }
        if (!store.CheckPassword(request.Password)) { failures++; if (failures >= 5) { blockedUntil = DateTimeOffset.UtcNow.AddMinutes(1); failures = 0; } return Results.Json(new { error = "Mot de passe incorrect." }, statusCode: 401); }
        failures = 0;
        await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "proprietaire")], CookieAuthenticationDefaults.AuthenticationScheme)));
        return Results.Ok(new { ok = true });
    } finally { loginGate.Release(); }
});
var api = app.MapGroup("/api").RequireAuthorization();
api.MapPost("/logout", async (HttpContext ctx) => { await ctx.SignOutAsync(); return Results.Ok(); });
api.MapGet("/state", () => store.Read());
api.MapGet("/network", () => new { configured = lan is not null, address = lan is null ? null : $"https://{lan.Address}:5188" });
api.MapGet("/network/certificate", () => File.Exists(Path.Combine(root,"AutoClair-reseau.cer")) ? Results.File(Path.Combine(root,"AutoClair-reseau.cer"),"application/pkix-cert","AutoClair-reseau.cer") : Results.NotFound());
api.MapPut("/preferences", (Preferences p) => { if (p.QuietStart is < 0 or > 23 || p.QuietEnd is < 0 or > 23 || p.IdleMinutes is <1 or >60 || p.DeepCandidates is <1 or >10 || !System.Text.RegularExpressions.Regex.IsMatch(p.AiModel, @"^[a-zA-Z0-9._:/-]{1,100}$")) throw new ArgumentException("Réglages invalides."); return store.Update(s => {s.Preferences = p;if(p.AutoDeepAnalysis)foreach(var v in s.Vehicles.Where(v=>v.Favorite))WorkQueue.Deep(s,v);return p;}); });
api.MapPost("/vehicles", (Vehicle v) => { Rules.Validate(v); v.Id = Guid.NewGuid().ToString("N"); v.SourceId = ""; v.Source = "Saisie personnelle"; v.Documents = []; v.Analyses = []; v.History = [new(DateTimeOffset.UtcNow, v.Price, v.Source)]; return store.Update(s => { s.Vehicles.Add(v); return v; }); });
api.MapPut("/vehicles/{id}", (string id, Vehicle v) => {
    Rules.Validate(v);
    return store.Update(s => { var old = s.Vehicles.FirstOrDefault(v => v.Id == id) ?? throw new ArgumentException("Dossier introuvable.");
        v.Id = old.Id; v.Source = old.Source; v.SourceId = old.SourceId; v.Created = old.Created; v.Updated = DateTimeOffset.UtcNow; v.Documents = old.Documents; v.Analyses = old.Analyses; v.History = old.History; v.ManualCorrection = old.ManualCorrection || old.Price != v.Price;
        v.Proposals=old.Proposals;v.LockedFields=old.LockedFields;v.AnalysisLevel=old.AnalysisLevel;v.SourceEstimate=old.SourceEstimate;
        foreach(var change in new[]{("make",v.Make,old.Make),("model",v.Model,old.Model),("variant",v.Variant,old.Variant),("fuel",v.Fuel,old.Fuel),("gearbox",v.Gearbox,old.Gearbox),("department",v.Department,old.Department),("body",v.Body,old.Body)})if(change.Item2!=change.Item3&&!v.LockedFields.Contains(change.Item1))v.LockedFields.Add(change.Item1);
        if (old.Price != v.Price) v.History.Add(new(DateTimeOffset.UtcNow, v.Price, "Correction personnelle"));
        v.PurchaseSnapshot = old.PurchaseSnapshot;
        if (v.Stage == "Acheté" && old.Stage != "Acheté" && v.PurchaseSnapshot is null) v.PurchaseSnapshot = JsonSerializer.Serialize(old, Store.Json);
        s.Vehicles[s.Vehicles.IndexOf(old)] = v;if(v.Favorite&&s.Preferences.AutoDeepAnalysis)WorkQueue.Deep(s,v);return v;
    });
});
api.MapDelete("/vehicles/{id}", (string id) => store.Update(s => { var v = s.Vehicles.FirstOrDefault(v => v.Id == id) ?? throw new ArgumentException("Dossier introuvable."); foreach (var d in v.Documents) File.Delete(store.DocumentPath(d.Id)); s.Vehicles.Remove(v); foreach(var search in s.Searches) search.Results.Remove(id); return new { ok = true }; }));
api.MapPost("/searches", (Search s) => { Rules.Validate(s); s.Id = Guid.NewGuid().ToString("N"); s.Results = []; return store.Update(state => { state.Searches.Add(s); return s; }); });
api.MapPut("/searches/{id}", (string id, Search s) => { Rules.Validate(s); return store.Update(state => { var old = state.Searches.FirstOrDefault(x => x.Id == id) ?? throw new ArgumentException("Recherche introuvable."); s.Id = old.Id; s.Results = old.Results; s.LastAttempt = old.LastAttempt; s.LastSuccess = old.LastSuccess; s.RetryAt = old.RetryAt; s.TotalMatching = old.TotalMatching; s.Status = old.Status; state.Searches[state.Searches.IndexOf(old)] = s; return s; }); });
api.MapDelete("/searches/{id}", (string id) => store.Update(s => new { removed = s.Searches.RemoveAll(x => x.Id == id) }));
api.MapPost("/searches/{id}/run", async (string id, Auto.Monitor monitor, CancellationToken ct) => await monitor.Run(id, ct));
api.MapGet("/evaluate/{id}", (string id, string? searchId) => { var s = store.Read(); var v = s.Vehicles.FirstOrDefault(v => v.Id == id) ?? throw new ArgumentException("Dossier introuvable."); var assessment = Intelligence.Assess(v, s.Searches.FirstOrDefault(s => s.Id == searchId) ?? new Search(), s.Vehicles); return assessment.Costs with { ScoreStatus = assessment.Score.HasValue ? $"{assessment.Score} / 100 · {assessment.Status}" : assessment.Status }; });
api.MapGet("/assessment/{id}",(string id,string? searchId)=>{var s=store.Read();return Intelligence.Assess(s.Vehicles.First(v=>v.Id==id),s.Searches.FirstOrDefault(x=>x.Id==searchId)??new Search(),s.Vehicles);});
api.MapGet("/negotiation/{id}",(string id,string? searchId,decimal? proposedPrice)=>{var s=store.Read();return Intelligence.Negotiate(s.Vehicles.First(v=>v.Id==id),s.Searches.FirstOrDefault(x=>x.Id==searchId)??new Search(),s.Vehicles,proposedPrice);});
api.MapGet("/ranking/{searchId}",(string searchId)=>{var s=store.Read();var search=s.Searches.First(x=>x.Id==searchId);var rows=s.Vehicles.Where(v=>search.Results.Contains(v.Id)||v.Favorite).Select(v=>new{vehicle=v,assessment=Intelligence.Assess(v,search,s.Vehicles)});return rows.OrderByDescending(r=>r.assessment.Score.HasValue).ThenByDescending(r=>search.Mode=="resell"?(search.RankAfterTime?r.assessment.Costs.QuickAfterTime:r.assessment.Costs.QuickMargin):r.assessment.Score).ToArray();});
api.MapPost("/vehicles/{id}/deepen",(string id)=>store.Update(s=>{WorkQueue.Deep(s,s.Vehicles.First(v=>v.Id==id));return new{queued=true};}));
api.MapPost("/work/{id}/cancel",(string id)=>store.Update(s=>{var w=s.Work.First(w=>w.Id==id);w.Status="Annulée";return w;}));
api.MapPost("/work/{id}/retry",(string id)=>store.Update(s=>{var w=s.Work.First(w=>w.Id==id);if(w.Status=="En cours")throw new ArgumentException("Traitement en cours.");w.Status="En attente";w.Attempts=0;w.NextAttempt=null;return w;}));
api.MapPost("/vehicles/{id}/proposals",(string id)=>store.Update(s=>{var v=s.Vehicles.First(v=>v.Id==id);Proposals.Add(v,Proposals.FromText(v.Text,"Texte de l’annonce"));return v.Proposals;}));
api.MapPost("/vehicles/{id}/proposals/{proposalId}",(string id,string proposalId,ProposalDecision decision)=>store.Update(s=>{var v=s.Vehicles.First(v=>v.Id==id);Proposals.Resolve(v,proposalId,decision.Accept);return v;}));
api.MapPost("/repair-references",(RepairReference r)=>{Intelligence.Validate(r);r.Id=Guid.NewGuid().ToString("N");return store.Update(s=>{s.RepairReferences.Add(r);return r;});});
api.MapDelete("/repair-references/{id}",(string id)=>store.Update(s=>new{removed=s.RepairReferences.RemoveAll(r=>r.Id==id)}));
api.MapPost("/vehicles/{id}/estimate-repair",(string id,RepairRequest request)=>store.Update(s=>{var v=s.Vehicles.First(v=>v.Id==id);var r=s.RepairReferences.First(r=>r.Id==request.ReferenceId);var cost=Intelligence.Estimate(r,v,request.Diy);v.Costs.Add(cost);v.CostsReviewed=false;return cost;}));
api.MapPost("/extract", (TextRequest r) => Extraction.Preview(r.Text));
api.MapPost("/obd", (TextRequest r) => Extraction.Obd(r.Text));
api.MapPost("/notices/read", () => store.Update(s => { s.Notices = s.Notices.Select(n => n with { Read = true }).ToList(); return new { ok = true }; }));
api.MapGet("/backup", () => Results.File(store.Backup(), "application/zip", $"AutoClair-{DateTime.Now:yyyyMMdd-HHmmss}.zip"));
api.MapPost("/restore", async (HttpContext ctx) => { using var ms = new MemoryStream(); await ctx.Request.Body.CopyToAsync(ms); var folder = store.Restore(ms.ToArray()); return Results.Ok(new { restored = true, verifiedCopy = folder }); });
api.MapPost("/vehicles/{id}/documents", async (string id, HttpContext ctx) => {
    if (!store.Read().Vehicles.Any(v => v.Id == id)) throw new ArgumentException("Dossier introuvable.");
    if (!ctx.Request.HasFormContentType) throw new ArgumentException("Fichier attendu.");
    var form = await ctx.Request.ReadFormAsync(); var file = form.Files.FirstOrDefault() ?? throw new ArgumentException("Fichier absent.");
    if (file.Length is <= 0 or > 20_000_000) throw new ArgumentException("Maximum 20 Mo par fichier.");
    using var ms = new MemoryStream(); await file.CopyToAsync(ms); var b = ms.ToArray();
    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    var mime = ext switch { ".pdf" when b.AsSpan().StartsWith("%PDF-"u8) => "application/pdf", ".png" when b.AsSpan().StartsWith(new byte[] {137,80,78,71,13,10,26,10}) => "image/png", ".jpg" or ".jpeg" when b.Length > 3 && b[0] == 255 && b[1] == 216 && b[2] == 255 => "image/jpeg", ".txt" when !b.Contains((byte)0) => "text/plain", _ => throw new ArgumentException("Formats acceptés : PDF, PNG, JPEG, texte UTF-8. Le contenu doit correspondre au format.") };
    var d = new Document(Guid.NewGuid().ToString("N"), Path.GetFileName(file.FileName), mime, file.Length, DateTimeOffset.UtcNow);
    File.WriteAllBytes(store.DocumentPath(d.Id), b);
    return store.Update(s => {var v=s.Vehicles.First(v=>v.Id==id);v.Documents.Add(d);if(v.Favorite&&s.Preferences.AutoDeepAnalysis)WorkQueue.Deep(s,v);return d;});
});
api.MapGet("/documents/{id}", (string id) => { var d = store.Read().Vehicles.SelectMany(v => v.Documents).FirstOrDefault(d => d.Id == id) ?? throw new ArgumentException("Document introuvable."); return Results.File(store.DocumentPath(id), d.Mime, d.Name); });
api.MapDelete("/vehicles/{id}/documents/{docId}", (string id, string docId) => store.Update(s => { var v = s.Vehicles.First(v => v.Id == id); if (v.Documents.Any(d => d.Id == docId)) { File.Delete(store.DocumentPath(docId)); v.Documents.RemoveAll(d => d.Id == docId); } return new { ok = true }; }));
api.MapPost("/vehicles/{id}/documents/{docId}/extract", (string id, string docId) => {
    var v = store.Read().Vehicles.FirstOrDefault(v=>v.Id==id) ?? throw new ArgumentException("Dossier introuvable.");
    var d = v.Documents.FirstOrDefault(d=>d.Id==docId) ?? throw new ArgumentException("Document introuvable.");
    var extracted = Documents.Extract(store.DocumentPath(docId),d.Mime);
    var a = new Analysis(DateTimeOffset.UtcNow,"Texte extrait : "+d.Name,extracted,"Extraction locale PdfPig / OCR Tesseract, à relire");
    return store.Update(s=>{var current=s.Vehicles.First(v=>v.Id==id);current.Analyses.Add(a);Proposals.Add(current,Proposals.FromText(extracted,d.Name));return a;});
});
api.MapGet("/ai/status", async (LocalAi localAi, CancellationToken ct) => { using var h = new HttpClient { Timeout = TimeSpan.FromSeconds(3) }; try { if(store.Read().Preferences.AiEnabled)await localAi.EnsureRunning(ct); var r = await h.GetFromJsonAsync<JsonElement>("http://127.0.0.1:11435/api/tags",ct); return Results.Ok(new { available = true, models = r.GetProperty("models").EnumerateArray().Select(x => x.GetProperty("name").GetString()).ToArray() }); } catch (Exception) { return Results.Ok(new { available = false, models = Array.Empty<string>() }); } });
api.MapPost("/vehicles/{id}/analyze", async (string id, AiRequest request, AnalysisService service, CancellationToken ct) => await service.Run(id,request.DocumentId,ct));
app.MapFallbackToFile("index.html");
app.Run();
public record Login(string Password);
public record TextRequest(string Text);
public record AiRequest(string? DocumentId);
public record ProposalDecision(bool Accept);
public record RepairRequest(string ReferenceId,bool Diy);


