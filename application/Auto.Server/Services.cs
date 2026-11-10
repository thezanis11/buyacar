using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace Auto;

public class Karoto
{
    private readonly HttpClient http = new(new HttpClientHandler { AllowAutoRedirect = false }) { Timeout = TimeSpan.FromSeconds(35) };
    private readonly SemaphoreSlim gate = new(1);
    private DateTimeOffset nextRequest;
    public async Task<JsonElement> Call(string tool, object args, CancellationToken ct = default)
    {
        await gate.WaitAsync(ct);
        try
        {
            if (DateTimeOffset.UtcNow < nextRequest) throw new ArgumentException("Source en pause. Réessayez dans quelques minutes.");
            using var req = new HttpRequestMessage(HttpMethod.Post, "https://karoto.fr/mcp"); req.Headers.Accept.ParseAdd("application/json"); req.Headers.Accept.ParseAdd("text/event-stream"); req.Headers.Add("Mcp-Protocol-Version", "2025-03-26");
            req.Content = JsonContent.Create(new { jsonrpc = "2.0", id = 1, method = "tools/call", @params = new { name = tool, arguments = args } });
            using var res = await http.SendAsync(req, ct);
            if (!res.IsSuccessStatusCode) { nextRequest = res.Headers.RetryAfter?.Date ?? DateTimeOffset.UtcNow.Add(res.Headers.RetryAfter?.Delta ?? TimeSpan.FromMinutes(15)); throw new HttpRequestException($"Karoto indisponible ({(int)res.StatusCode}). Pause temporaire appliquée."); }
            nextRequest = DateTimeOffset.UtcNow.AddSeconds(2);
            var text = await res.Content.ReadAsStringAsync(ct);
            if (text.Length > 2_000_000) throw new ArgumentException("Réponse source trop volumineuse.");
            if (text.StartsWith("event:") || text.StartsWith("data:")) text = string.Join("\n", text.Split('\n').Where(x => x.StartsWith("data:")).Select(x => x[5..].Trim()));
            using var outer = JsonDocument.Parse(text);
            if (outer.RootElement.TryGetProperty("error", out _)) throw new ArgumentException("La source a refusé la recherche.");
            var result = outer.RootElement.GetProperty("result");
            if (result.TryGetProperty("isError", out var error) && error.GetBoolean()) throw new ArgumentException("La source n’a pas pu traiter la demande.");
            var content = result.GetProperty("content").EnumerateArray().First(x => x.GetProperty("type").GetString() == "text").GetProperty("text").GetString()!;
            return JsonDocument.Parse(content).RootElement.Clone();
        }
        finally { gate.Release(); }
    }
    public async Task<(int Total, List<Vehicle> Vehicles)> Search(Search s, CancellationToken ct = default)
    {
        Dictionary<string, object> args = new() { ["country"] = "FR", ["price_max"] = (int)decimal.Floor(s.PurchaseMax), ["sort"] = "newest", ["limit"] = 20 };
        foreach (var c in s.Criteria.Where(c => c.Required && c.Value != ""))
        {
            var value = Rules.Norm(c.Value);
            if (c.Field is "make" or "model" or "department") args[c.Field] = c.Value;
            if (c.Field is "year_min" or "mileage_max" && int.TryParse(c.Value, out var n)) args[c.Field] = n;
            if (c.Field == "fuel") args["fuel"] = value switch { "électrique" => "electrique", "e85" => "ethanol", "hybride rechargeable" => "hybride_rechargeable", _ => value };
            if (c.Field == "gearbox") args["gearbox"] = value;
            if (c.Field == "seller" && value == "particulier") args["private_sellers_only"] = true;
        }
        var r = await Call("search_listings", args, ct);
        return (r.GetProperty("total_matching").GetInt32(), r.GetProperty("results").EnumerateArray().Select(Parse).ToList());
    }
    public static Vehicle Parse(JsonElement j)
    {
        string S(string k) => j.TryGetProperty(k, out var p) && p.ValueKind != JsonValueKind.Null ? p.ToString() : "";
        int? I(string k) => int.TryParse(S(k), out var n) ? n : null;
        decimal? D(string k) => decimal.TryParse(S(k), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var n) ? n : null;
        return new() { Title = S("title"), Make=S("make"),Model=S("model"),Variant=S("version"),SourceEstimate=D("karoto_estimate_eur"),Source = "Karoto", SourceId = S("id"), Url = S("url"), Country = S("country"), Year = I("year"), Mileage = I("mileage_km"), Price = D("price_eur"), Fuel = S("fuel"), Gearbox = S("gearbox"), Seller = S("seller"), Department = S("department"), Body = S("body_type"), Text = S("description") };
    }
}
public class Monitor(Store store, Karoto karoto, ILogger<Monitor> logger) : BackgroundService
{
    private readonly SemaphoreSlim gate = new(1);
    public async Task<Search> Run(string id, CancellationToken ct = default)
    {
        await gate.WaitAsync(ct);
        try
        {
            var s = store.Read().Searches.FirstOrDefault(s => s.Id == id) ?? throw new ArgumentException("Recherche introuvable.");
            if (s.RetryAt > DateTimeOffset.UtcNow) throw new ArgumentException("La source est en pause ; réessayez après l’heure indiquée.");
            if (s.LastAttempt > DateTimeOffset.UtcNow.AddSeconds(-30)) throw new ArgumentException("Cette recherche vient d’être vérifiée. Patientez 30 secondes.");
            store.Update(state => { state.Searches.First(x => x.Id == id).LastAttempt = DateTimeOffset.UtcNow; return true; });
            try
            {
                var response = await karoto.Search(s, ct);
                return store.Update(state => {
                    var current = state.Searches.First(x => x.Id == id); List<string> ids = []; int added = 0;
                    foreach (var item in response.Vehicles) { var exists = state.Vehicles.Any(v => v.Source == item.Source && v.SourceId == item.SourceId); var v = Rules.Merge(state, item); ids.Add(v.Id); if (!exists) added++; }
                    current.Results = ids; current.TotalMatching = response.Total; current.LastSuccess = DateTimeOffset.UtcNow; current.RetryAt = null;
                    if(state.Preferences.AutoDeepAnalysis)foreach(var candidate in state.Vehicles.Where(v=>ids.Contains(v.Id)).OrderByDescending(v=>v.Favorite).ThenByDescending(v=>v.SourceEstimate>0?(v.SourceEstimate-v.Price)/v.SourceEstimate:null).Take(state.Preferences.DeepCandidates))WorkQueue.Deep(state,candidate);
                    current.Status = ids.Count == 0 ? "Aucun résultat renvoyé" : $"{ids.Count} annonces reçues sur {response.Total} correspondances déclarées · couverture partielle";
                    if (added > 0) Rules.Notify(state, $"search:{id}:{string.Join(',', ids.Order())}", $"{current.Name} : {added} nouvelles annonces à vérifier");
                    return current;
                });
            }
            catch (Exception e) when (e is HttpRequestException or TaskCanceledException or JsonException or ArgumentException or KeyNotFoundException)
            {
                store.Update(state => { var current = state.Searches.First(x => x.Id == id); current.Status = "Erreur de source — les anciens résultats sont conservés"; current.RetryAt = DateTimeOffset.UtcNow.AddMinutes(15); return true; });
                logger.LogWarning("Recherche indisponible : {Type}", e.GetType().Name); throw new ArgumentException("Source indisponible. Les résultats précédents sont conservés ; nouvelle tentative possible dans 15 minutes.");
            }
        }
        finally { gate.Release(); }
    }
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var now = DateTimeOffset.Now;
                foreach (var s in store.Read().Searches.Where(s => s.Monitor && now.Hour >= s.StartHour && now.Hour < s.EndHour && (s.LastAttempt is null || now - s.LastAttempt >= TimeSpan.FromHours(s.EveryHours)) && !(s.RetryAt > now)))
                    try { await Run(s.Id, ct); } catch (ArgumentException) { }
                store.Update(s => { foreach (var v in s.Vehicles.Where(v => DateOnly.TryParse(v.ReminderDate, out var d) && d <= DateOnly.FromDateTime(DateTime.Today))) Rules.Notify(s, $"reminder:{v.Id}:{v.ReminderDate}:{v.ReminderLabel}", $"{v.ReminderLabel} : {v.Title}", v.Id); return true; });
            }
            catch (Exception e) when (!ct.IsCancellationRequested) { logger.LogWarning("Surveillance : {Type}", e.GetType().Name); }
            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}
public static partial class Extraction
{
    public static object Preview(string text)
    {
        if (text.Length > 100000) throw new ArgumentException("Texte trop long.");
        object Field(string pattern) { var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase).Select(m => m.Groups[1].Value.Trim()).Distinct().ToArray(); return new { candidates = matches, ambiguous = matches.Length != 1 }; }
        return new { price = Field(@"([\d][\d\s.,]*)\s*(?:€|euros)"), mileage = Field(@"([\d][\d\s.,]*)\s*km\b"), year = Field(@"\b((?:19|20)\d{2})\b"), message = "Extraction indicative : vérifiez les candidats avant de les recopier dans le dossier. Aucune valeur n’est appliquée automatiquement." };
    }
    public static object Obd(string text) => Regex.Matches(text.ToUpperInvariant(), @"\b[PCBU][0-3][0-9A-F]{3}\b").Select(m => m.Value).Distinct().Select(code => new { code, meaning = code switch { "P0300" => "Ratés de combustion multiples/aléatoires signalés.", "P0420" => "Efficacité du système catalytique sous le seuil (banc 1).", "P0171" => "Mélange trop pauvre signalé (banc 1).", "P0401" => "Débit de recirculation des gaz d’échappement insuffisant signalé.", _ => "Définition à vérifier dans la documentation constructeur correspondant au véhicule." }, advice = "Conserver le rapport et les conditions d’apparition. Faire confirmer le diagnostic ; ce code ne suffit pas à décider du remplacement d’une pièce." }).ToArray();
}
