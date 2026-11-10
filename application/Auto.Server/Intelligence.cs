using System.Globalization;
namespace Auto;

public class EvidenceRating
{
    public decimal? Value { get; set; }
    public string Source { get; set; } = "";
    public string Reason { get; set; } = "";
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
}
public class Comparable
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public string Variant { get; set; } = "";
    public string Fuel { get; set; } = "";
    public string Gearbox { get; set; } = "";
    public int Year { get; set; }
    public int Mileage { get; set; }
    public decimal Price { get; set; }
    public string Source { get; set; } = "";
    public DateTimeOffset Observed { get; set; } = DateTimeOffset.UtcNow;
}
public class RepairReference
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Label { get; set; } = "";
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public string Variant { get; set; } = "";
    public decimal PartsLow { get; set; }
    public decimal Parts { get; set; }
    public decimal PartsHigh { get; set; }
    public decimal HoursLow { get; set; }
    public decimal Hours { get; set; }
    public decimal HoursHigh { get; set; }
    public decimal HourlyRate { get; set; }
    public string Source { get; set; } = "";
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
}
public record Dimension(string Name, decimal? Value, string Reason);
public record Assessment(decimal? Score, string Status, string RuleVersion, List<Dimension> Dimensions, List<Comparable> Comparables, decimal? Median, decimal? LowerQuartile, Evaluation Costs);
public record Negotiation(decimal? SuggestedLow, decimal? SuggestedHigh, decimal? BudgetCeiling, string Reason, string Message, Evaluation? Simulation);
public static class Intelligence
{
    public const string RuleVersion = "preferences-v1";
    public static decimal Median(IEnumerable<decimal> values) { var a=values.Order().ToArray();return a.Length%2==1?a[a.Length/2]:(a[a.Length/2-1]+a[a.Length/2])/2; }
    public static string Key(string s) => string.Concat(Rules.Norm(s).Where(char.IsLetterOrDigit));
    private static bool Match(string a,string b) => Key(a)!="" && Key(a)==Key(b);
    public static List<Comparable> ComparablesFor(Vehicle v, IEnumerable<Vehicle> pool)
    {
        var available=v.Comparables.Concat(pool.Where(x=>x.Id!=v.Id && x.Price>0 && x.Year.HasValue && x.Mileage.HasValue && x.Url!="").Select(x=>new Comparable {Id=x.Id,Make=x.Make,Model=x.Model,Variant=x.Variant,Fuel=x.Fuel,Gearbox=x.Gearbox,Year=x.Year!.Value,Mileage=x.Mileage!.Value,Price=x.Price!.Value,Source=x.Url,Observed=x.Updated}));
        return available.Where(c=>Match(v.Make,c.Make)&&Match(v.Model,c.Model)&&Match(v.Variant,c.Variant)&&Match(v.Fuel,c.Fuel)&&Match(v.Gearbox,c.Gearbox)&&v.Year.HasValue&&Math.Abs(c.Year-v.Year.Value)<=2&&v.Mileage.HasValue&&Math.Abs(c.Mileage-v.Mileage.Value)<=Math.Max(25000,v.Mileage.Value*.25)&&c.Price>0&&c.Source!=""&&NormalizeSource(c.Source)!=NormalizeSource(v.Url)&&c.Observed<=DateTimeOffset.UtcNow.AddMinutes(1)&&c.Observed>=DateTimeOffset.UtcNow.AddDays(-90)).GroupBy(c=>NormalizeSource(c.Source)).Select(g=>g.OrderByDescending(c=>c.Observed).First()).ToList();
    }
    public static string NormalizeSource(string source) => Uri.TryCreate(source,UriKind.Absolute,out var u)?u.GetLeftPart(UriPartial.Path).TrimEnd('/').ToLowerInvariant():Rules.Norm(source);
    private static decimal? Rating(EvidenceRating r) => r.Value is >=0 and <=100 && !string.IsNullOrWhiteSpace(r.Source)&&!string.IsNullOrWhiteSpace(r.Reason)&&r.Date<=DateTimeOffset.UtcNow.AddMinutes(1)&&r.Date>=DateTimeOffset.UtcNow.AddYears(-1)?r.Value:null;
    public static Assessment Assess(Vehicle v,Search s,IEnumerable<Vehicle> pool)
    {
        var costs=Rules.Evaluate(v,s); var comps=ComparablesFor(v,pool);decimal? median=comps.Count>=3?Median(comps.Select(c=>c.Price)):null;
        decimal? low=comps.Count>=3?comps.Select(c=>c.Price).Order().ElementAt((comps.Count-1)/4):null;
        int wishes=s.Criteria.Count(c=>!c.Required&&!string.IsNullOrWhiteSpace(c.Value));
        List<Dimension> dims=[
            new("Prix",median is >0 && v.Price.HasValue?Math.Clamp(50+100*(median.Value-v.Price.Value)/median.Value,0,100):null,median is null?"Au moins 3 comparables récents et suffisamment proches sont nécessaires.":$"50 au prix médian affiché ({median:0.00} €), +1 point par 1 % sous la médiane ; borné entre 0 et 100. Ce ne sont pas des prix de transaction."),
            new("Fiabilité",Rating(v.Reliability),"Appréciation documentée de l’utilisateur, non probabilité de panne. "+v.Reliability.Reason),
            new("Entretien",Rating(v.Maintenance),"Appréciation documentée de l’utilisateur. "+v.Maintenance.Reason),
            new("Coût total",costs.Pessimistic.HasValue?Math.Clamp(100-50*costs.Pessimistic.Value/s.TotalMax,0,100):null,"100 sans dépense, 50 au plafond total, 0 au double du plafond. Calcul au scénario pessimiste."),
            new("Adéquation",costs.Problems.Count>0?null:wishes==0?100:100m*(wishes-costs.Preferences.Count)/wishes,"Part des préférences souhaitées respectées. Sans préférence souhaitée : 100. Une exigence stricte non vérifiée bloque le classement.")];
        var weighted=dims.Where(d=>s.Weights.GetValueOrDefault(d.Name)>0).ToList();
        bool gate=costs.Missing.Count==0&&costs.Problems.Count==0&&costs.BudgetStatus.StartsWith("Conforme");
        bool complete=weighted.Count>0&&weighted.All(d=>d.Value.HasValue);
        decimal? score=gate&&complete?decimal.Round(weighted.Sum(d=>d.Value!.Value*s.Weights[d.Name])/weighted.Sum(d=>s.Weights[d.Name]),1):null;
        return new(score,!gate?"Non classé : socle, exigences ou budget à vérifier":!complete?"Évaluation partielle : dimension pondérée manquante":"Indice de préférence personnalisé — barème explicite, non expertise",RuleVersion,dims,comps,median,low,costs);
    }
    public static Negotiation Negotiate(Vehicle v,Search s,IEnumerable<Vehicle> pool,decimal? proposedPrice)
    {
        var a=Assess(v,s,pool);decimal? ceiling=a.Costs.Pessimistic-v.Price is decimal extra?Math.Max(0,Math.Min(s.PurchaseMax,s.TotalMax-extra)):null;
        decimal? high=a.Median.HasValue&&ceiling.HasValue?Math.Min(a.Median.Value,Math.Min(ceiling.Value,v.Price??a.Median.Value)):null;
        decimal? low=high.HasValue&&a.LowerQuartile.HasValue?Math.Min(high.Value,a.LowerQuartile.Value):null;
        Evaluation? simulation=null;
        if(proposedPrice.HasValue){if(proposedPrice<0)throw new ArgumentException("Prix proposé négatif.");var copy=System.Text.Json.JsonSerializer.Deserialize<Vehicle>(System.Text.Json.JsonSerializer.Serialize(v,Store.Json),Store.Json)!;copy.Price=proposedPrice;simulation=Rules.Evaluate(copy,s);}
        var reason=high is null?"Cible non chiffrable : comparables ou frais pessimistes manquants.":"Fourchette de discussion plafonnée par votre budget et les prix affichés des comparables. Aucune remise n’est acquise. L’état des comparables reste à vérifier ; les travaux ne sont pas soustraits deux fois.";
        var message=$"Bonjour, votre {v.Title} m’intéresse. Pourriez-vous confirmer la version, l’entretien et les défauts connus ?";
        if(high.HasValue) message+=$" Sous réserve de la visite, des justificatifs et des travaux à confirmer, une discussion autour de {high:0.00} € serait-elle envisageable ?";
        if(v.Costs.Count>0)message+=" Je souhaite notamment vérifier : "+string.Join(", ",v.Costs.Select(c=>c.Label))+".";
        return new(low,high,ceiling,reason,message,simulation);
    }
    public static void Validate(RepairReference r)
    {
        if(string.IsNullOrWhiteSpace(r.Label)||string.IsNullOrWhiteSpace(r.Make)||string.IsNullOrWhiteSpace(r.Model)||string.IsNullOrWhiteSpace(r.Variant)||string.IsNullOrWhiteSpace(r.Source))throw new ArgumentException("Renseignez le véhicule précis, la réparation et la source du devis ou tarif.");
        if(r.PartsLow<0||r.Parts<r.PartsLow||r.PartsHigh<r.Parts||r.HoursLow<0||r.Hours<r.HoursLow||r.HoursHigh<r.Hours||r.HourlyRate<0||r.Date>DateTimeOffset.UtcNow.AddMinutes(1))throw new ArgumentException("Fourchettes de pièces, temps ou date invalides.");
    }
    public static Cost Estimate(RepairReference r,Vehicle v,bool diy)
    {
        Validate(r);
        if(!Match(r.Make,v.Make)||!Match(r.Model,v.Model)||!Match(r.Variant,v.Variant))throw new ArgumentException("La référence ne correspond pas exactement à la marque, au modèle et à la version.");
        if(r.Date<DateTimeOffset.UtcNow.AddYears(-1))throw new ArgumentException("Référence de plus d’un an : actualisez son tarif avant réutilisation.");
        if(v.Costs.Any(c=>c.ReferenceId==r.Id))throw new ArgumentException("Cette référence a déjà été ajoutée au dossier.");
        var rate=diy?0:r.HourlyRate;
        return new Cost{Label=r.Label,Parts=r.Parts,LaborHours=r.Hours,HourlyRate=rate,Optimistic=decimal.Round(r.PartsLow+r.HoursLow*rate,2),Central=decimal.Round(r.Parts+r.Hours*rate,2),Pessimistic=decimal.Round(r.PartsHigh+r.HoursHigh*rate,2),Source=r.Source,Date=r.Date.ToString("yyyy-MM-dd"),Method=diy?"Personnel":"Garage",ReferenceId=r.Id};
    }
}
