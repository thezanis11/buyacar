namespace Auto;
public class EnergyUse
{
    public string Name { get; set; } = "";
    public decimal? Per100 { get; set; }
    public decimal? UnitPrice { get; set; }
}
public class OwnershipBudget
{
    public bool Enabled { get; set; }
    public List<EnergyUse> Energies { get; set; } = [];
    public decimal? Insurance { get; set; }
    public decimal? Maintenance { get; set; }
    public decimal? Other { get; set; }
    public string Source { get; set; } = "";
}
public static class OwnershipMath
{
    public static decimal? Annual(Vehicle v,int km)
    {
        var o=v.Ownership;if(!o.Enabled)return v.AnnualRunningCost;
        if(o.Energies.Count==0||o.Energies.Any(e=>!e.Per100.HasValue||!e.UnitPrice.HasValue)||!o.Insurance.HasValue||!o.Maintenance.HasValue||!o.Other.HasValue||string.IsNullOrWhiteSpace(o.Source))return null;
        return decimal.Round(o.Energies.Sum(e=>e.Per100!.Value*e.UnitPrice!.Value*km/100)+o.Insurance.Value+o.Maintenance.Value+o.Other.Value,2);
    }
    public static void Validate(OwnershipBudget o)
    {
        if(o.Insurance<0||o.Maintenance<0||o.Other<0||o.Energies.Count>2||o.Energies.Any(e=>string.IsNullOrWhiteSpace(e.Name)||e.Per100<0||e.UnitPrice<0))throw new ArgumentException("Les hypothèses d’usage doivent être positives, avec au plus deux énergies nommées.");
    }
}
