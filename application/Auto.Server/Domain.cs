using System.Text.Json;
namespace Auto;

public class State
{
    public int Version { get; set; } = 1;
    public List<Vehicle> Vehicles { get; set; } = [];
    public List<Search> Searches { get; set; } = [];
    public List<Notice> Notices { get; set; } = [];
    public Preferences Preferences { get; set; } = new();
    public List<WorkItem> Work { get; set; } = [];
    public List<RepairReference> RepairReferences { get; set; } = [];
}
public class Preferences
{
    public bool Advanced { get; set; }
    public bool Notifications { get; set; } = true;
    public int QuietStart { get; set; } = 22;
    public int QuietEnd { get; set; } = 8;
    public string AiModel { get; set; } = "gemma3:4b";
    public bool AiEnabled { get; set; }
    public bool AutoDeepAnalysis { get; set; } = true;
    public int IdleMinutes { get; set; } = 3;
    public int DeepCandidates { get; set; } = 3;
}
public class Vehicle
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = "Nouveau dossier";
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public string Variant { get; set; } = "";
    public string Fuel { get; set; } = "";
    public string Gearbox { get; set; } = "";
    public int? Year { get; set; }
    public int? Mileage { get; set; }
    public decimal? Price { get; set; }
    public string Country { get; set; } = "FR";
    public string Department { get; set; } = "";
    public string Seller { get; set; } = "";
    public string Body { get; set; } = "";
    public decimal? DistanceKm { get; set; }
    public string Source { get; set; } = "Saisie personnelle";
    public string SourceId { get; set; } = "";
    public string Url { get; set; } = "";
    public string Notes { get; set; } = "";
    public string Text { get; set; } = "";
    public string Obd { get; set; } = "";
    public bool Favorite { get; set; }
    public string Stage { get; set; } = "À étudier";
    public string? ReminderDate { get; set; }
    public string ReminderLabel { get; set; } = "Visite / entretien";
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
    public List<Observation> History { get; set; } = [];
    public List<string> Conflicts { get; set; } = [];
    public bool ManualCorrection { get; set; }
    public List<Cost> Costs { get; set; } = [];
    public bool CostsReviewed { get; set; }
    public List<Expense> Expenses { get; set; } = [];
    public List<Document> Documents { get; set; } = [];
    public List<Analysis> Analyses { get; set; } = [];
    public decimal? ActualPurchase { get; set; }
    public decimal? ActualSale { get; set; }
    public string? PurchaseSnapshot { get; set; }
    public decimal? QuickSale { get; set; }
    public decimal? PatientSale { get; set; }
    public decimal? OwnershipResale { get; set; }
    public decimal? AnnualRunningCost { get; set; }
    public OwnershipBudget Ownership { get; set; } = new();
    public decimal? SaleFees { get; set; }
    public decimal PersonalHours { get; set; }
    public decimal? SourceEstimate { get; set; }
    public List<Comparable> Comparables { get; set; } = [];
    public EvidenceRating Reliability { get; set; } = new();
    public EvidenceRating Maintenance { get; set; } = new();
    public List<FactProposal> Proposals { get; set; } = [];
    public List<string> LockedFields { get; set; } = [];
    public string AnalysisLevel { get; set; } = "Premier tri";
}
public record Observation(DateTimeOffset Date, decimal? Price, string Source);
public class Cost
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Label { get; set; } = "";
    public string Category { get; set; } = "Réparation";
    public string Method { get; set; } = "Garage";
    public decimal? Optimistic { get; set; }
    public decimal? Central { get; set; }
    public decimal? Pessimistic { get; set; }
    public string Source { get; set; } = "Hypothèse personnelle";
    public string Date { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public decimal? Parts { get; set; }
    public decimal? LaborHours { get; set; }
    public decimal? HourlyRate { get; set; }
    public string ReferenceId { get; set; } = "";
}
public record Expense(string Id, string Label, decimal Amount, string Date, string Category);
public record Document(string Id, string Name, string Mime, long Size, DateTimeOffset Date);
public record Analysis(DateTimeOffset Date, string Kind, string Result, string Model);
public record Notice(string Id, string Key, string Title, string? VehicleId, DateTimeOffset Date, bool Read = false);
public class Criterion
{
    public string Field { get; set; } = "";
    public string Value { get; set; } = "";
    public bool Required { get; set; } = true;
}
public class Search
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "Ma recherche";
    public string Mode { get; set; } = "drive";
    public decimal PurchaseMax { get; set; } = 5000;
    public decimal TotalMax { get; set; } = 6500;
    public bool Strict { get; set; } = true;
    public int Years { get; set; } = 3;
    public int KmPerYear { get; set; } = 10000;
    public decimal HourlyRate { get; set; } = 20;
    public bool RankAfterTime { get; set; }
    public List<Criterion> Criteria { get; set; } = [];
    public Dictionary<string, int> Weights { get; set; } = new() { ["Prix"] = 20, ["Fiabilité"] = 25, ["Entretien"] = 15, ["Coût total"] = 25, ["Adéquation"] = 15 };
    public bool Monitor { get; set; }
    public int EveryHours { get; set; } = 6;
    public int StartHour { get; set; } = 8;
    public int EndHour { get; set; } = 22;
    public DateTimeOffset? LastAttempt { get; set; }
    public DateTimeOffset? LastSuccess { get; set; }
    public DateTimeOffset? RetryAt { get; set; }
    public string Status { get; set; } = "Jamais vérifiée";
    public int TotalMatching { get; set; }
    public List<string> Results { get; set; } = [];
}
public record Evaluation(List<string> Missing, List<string> Problems, List<string> Preferences, decimal? Optimistic, decimal? Central, decimal? Pessimistic, string BudgetStatus, decimal? OwnershipCost, decimal? QuickMargin, decimal? PatientMargin, decimal? QuickAfterTime, decimal? PatientAfterTime, decimal? ActualMargin, string Confidence, string ScoreStatus);
public static class Rules
{
    public static string Norm(string? s) => (s ?? "").Trim().ToLowerInvariant();
    public static decimal? Total(Vehicle v, Func<Cost, decimal?> pick) => !v.CostsReviewed || v.Price is null || v.Costs.Any(c => pick(c) is null) ? null : v.Price + v.Costs.Sum(c => pick(c)!.Value);
    public static Evaluation Evaluate(Vehicle v, Search s)
    {
        List<string> missing = [];
        if (v.Price is null) missing.Add("Prix");
        if (string.IsNullOrWhiteSpace(v.Model)) missing.Add("Modèle");
        if (string.IsNullOrWhiteSpace(v.Variant)) missing.Add("Version / moteur précis");
        if (string.IsNullOrWhiteSpace(v.Fuel)) missing.Add("Motorisation");
        if (string.IsNullOrWhiteSpace(v.Gearbox)) missing.Add("Boîte");
        if (v.Year is null) missing.Add("Année");
        if (v.Mileage is null) missing.Add("Kilométrage");
        List<string> problems = [], preferences = [];
        if (v.Country != "FR") problems.Add("Véhicule hors France");
        if (v.Conflicts.Count > 0) missing.Add("Contradictions à résoudre");
        foreach (var c in s.Criteria.Where(c => !string.IsNullOrWhiteSpace(c.Value)))
        {
            var value = c.Field switch { "make" => v.Make, "model" => v.Model, "fuel" => v.Fuel, "gearbox" => v.Gearbox, "department" => v.Department, "seller" => v.Seller, "body" => v.Body, "year_min" => v.Year?.ToString(), "mileage_max" => v.Mileage?.ToString(), "distance_max" => v.DistanceKm?.ToString(System.Globalization.CultureInfo.InvariantCulture), _ => null };
            bool unknown = string.IsNullOrWhiteSpace(value);
            bool ok;
            if (c.Field.EndsWith("_min") || c.Field.EndsWith("_max"))
                ok = decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var n) && decimal.TryParse(c.Value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var limit) && (c.Field.EndsWith("_min") ? n >= limit : n <= limit);
            else ok = Norm(value) == Norm(c.Value);
            if (!ok) (c.Required ? problems : preferences).Add($"{c.Field} : {(unknown ? "à vérifier" : "écart")}, souhaité {c.Value}");
        }
        var o = Total(v, c => c.Optimistic); var central = Total(v, c => c.Central); var p = Total(v, c => c.Pessimistic);
        string budget = v.Price > s.PurchaseMax ? "Hors budget achat" : (s.Strict ? p : central) is null ? "Budget à vérifier" : (s.Strict ? p : central) > s.TotalMax ? "Hors budget total" : !s.Strict && (p is null || p > s.TotalMax) ? "Conforme au central · risque signalé" : "Conforme aux hypothèses";
        decimal? quick = v.QuickSale - central - v.SaleFees, patient = v.PatientSale - central - v.SaleFees;
        return new(missing, problems, preferences, o, central, p, budget, central + OwnershipMath.Annual(v, s.KmPerYear) * s.Years - v.OwnershipResale, quick, patient, quick - v.PersonalHours * s.HourlyRate, patient - v.PersonalHours * s.HourlyRate, v.ActualSale - v.ActualPurchase - v.Expenses.Sum(x => x.Amount), v.Documents.Count == 0 ? "Déclarations uniquement" : "Documents présents · contenu à vérifier", missing.Count > 0 ? "Socle incomplet" : "Évaluation partielle · fiabilité et score non calibrés");
    }
    public static void Validate(Vehicle v)
    {
        OwnershipMath.Validate(v.Ownership);
        if (string.IsNullOrWhiteSpace(v.Title) || v.Title.Length > 300 || v.Notes.Length > 50000 || v.Text.Length > 100000) throw new ArgumentException("Titre ou texte invalide.");
        if (v.Price < 0 || v.Mileage < 0 || v.Year < 1900 || v.Year > DateTime.Today.Year + 1 || v.PersonalHours < 0 || v.DistanceKm < 0 || v.ActualPurchase < 0 || v.ActualSale < 0 || v.QuickSale < 0 || v.PatientSale < 0 || v.SaleFees < 0 || v.AnnualRunningCost < 0 || v.OwnershipResale < 0) throw new ArgumentException("Les montants et caractéristiques doivent être positifs et l’année valide.");
        foreach (var c in v.Costs) if (c.Optimistic < 0 || c.Central < 0 || c.Pessimistic < 0 || c.Optimistic > c.Central || c.Central > c.Pessimistic) throw new ArgumentException("Chaque coût doit respecter optimiste ≤ central ≤ pessimiste.");
        if (v.Costs.Select(c => c.Id).Distinct().Count() != v.Costs.Count) throw new ArgumentException("Poste de coût dupliqué.");
        if (v.Expenses.Any(e => e.Amount < 0)) throw new ArgumentException("Dépense négative.");
        foreach(var rating in new[]{v.Reliability,v.Maintenance})if(rating.Value is <0 or >100 || rating.Value.HasValue&&(string.IsNullOrWhiteSpace(rating.Source)||string.IsNullOrWhiteSpace(rating.Reason)))throw new ArgumentException("Toute appréciation chiffrée doit être entre 0 et 100 et accompagnée de sa source et justification.");
        if(v.Comparables.Any(c=>c.Price<=0||c.Mileage<0||c.Year<1900||string.IsNullOrWhiteSpace(c.Source)))throw new ArgumentException("Comparable invalide : renseignez prix, année, kilométrage et source.");
        if (!string.IsNullOrEmpty(v.Url) && (!Uri.TryCreate(v.Url, UriKind.Absolute, out var u) || u.Scheme is not ("https" or "http"))) throw new ArgumentException("Lien invalide.");
    }
    public static void Validate(Search s)
    {
        if (string.IsNullOrWhiteSpace(s.Name) || s.PurchaseMax <= 0 || s.TotalMax <= 0 || s.Years < 1 || s.Years > 30 || s.KmPerYear < 0 || s.HourlyRate < 0 || s.EveryHours < 1 || s.EveryHours > 168 || s.StartHour < 0 || s.EndHour > 24 || s.StartHour >= s.EndHour) throw new ArgumentException("Vérifiez les budgets, la durée et les horaires.");
        if (s.Weights.Values.Any(w => w < 0 || w > 100) || s.Weights.Values.Sum() == 0) throw new ArgumentException("Les poids doivent être positifs et leur somme non nulle.");
        if(s.Weights.Keys.Any(k=>!new[]{"Prix","Fiabilité","Entretien","Coût total","Adéquation"}.Contains(k)))throw new ArgumentException("Dimension de classement inconnue.");
    }
    public static void Notify(State s, string key, string title, string? vehicle = null)
    {
        if (!s.Notices.Any(n => n.Key == key)) s.Notices.Insert(0, new(Guid.NewGuid().ToString("N"), key, title, vehicle, DateTimeOffset.UtcNow));
    }
    public static Vehicle Merge(State s, Vehicle incoming)
    {
        var old = s.Vehicles.FirstOrDefault(v => v.SourceId == incoming.SourceId && v.Source == incoming.Source && incoming.SourceId != "");
        if (old is null) { incoming.History.Add(new(DateTimeOffset.UtcNow, incoming.Price, incoming.Source)); s.Vehicles.Add(incoming); return incoming; }
        if (old.History.LastOrDefault()?.Price != incoming.Price)
        {
            old.History.Add(new(DateTimeOffset.UtcNow, incoming.Price, incoming.Source));
            if (incoming.Price < old.Price) Notify(s, $"price:{old.Id}:{incoming.Price}:{old.History.Count}", $"Baisse de prix : {old.Title}", old.Id);
            if (old.ManualCorrection) { var conflict = $"Prix source : {incoming.Price} € ; votre valeur : {old.Price} €."; if (!old.Conflicts.Contains(conflict)) old.Conflicts.Add(conflict); }
            else old.Price = incoming.Price;
        }
        old.Updated = DateTimeOffset.UtcNow;
        return old;
    }
}
