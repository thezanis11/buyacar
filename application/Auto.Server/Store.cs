using Microsoft.Data.Sqlite;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
namespace Auto;

public class Store
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    public readonly string Root;
    private readonly object gate = new();
    public Store(string root)
    {
        Root = Path.GetFullPath(root); Directory.CreateDirectory(Root); Directory.CreateDirectory(Path.Combine(Root, "documents"));
        using var c = Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS app_state (id INTEGER PRIMARY KEY CHECK(id=1), json TEXT NOT NULL); CREATE TABLE IF NOT EXISTS account (id INTEGER PRIMARY KEY CHECK(id=1), salt TEXT NOT NULL, hash TEXT NOT NULL);"; cmd.ExecuteNonQuery();
        cmd.CommandText = "INSERT OR IGNORE INTO app_state VALUES(1,$json)"; cmd.Parameters.AddWithValue("$json", JsonSerializer.Serialize(new State(), Json)); cmd.ExecuteNonQuery();
    }
    private SqliteConnection Open() { var c = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Path.Combine(Root, "automobile.db"), Pooling = false }.ToString()); c.Open(); return c; }
    private State Load(SqliteConnection c) { using var cmd = c.CreateCommand(); cmd.CommandText = "SELECT json FROM app_state WHERE id=1"; return JsonSerializer.Deserialize<State>((string)cmd.ExecuteScalar()!, Json)!; }
    public State Read() { lock (gate) { using var c = Open(); return Load(c); } }
    public T Update<T>(Func<State, T> update)
    {
        lock (gate) { using var c = Open(); var s = Load(c); var result = update(s); using var cmd = c.CreateCommand(); cmd.CommandText = "UPDATE app_state SET json=$json WHERE id=1"; cmd.Parameters.AddWithValue("$json", JsonSerializer.Serialize(s, Json)); cmd.ExecuteNonQuery(); return result; }
    }
    public bool HasAccount() { lock (gate) { using var c = Open(); using var cmd = c.CreateCommand(); cmd.CommandText = "SELECT count(*) FROM account"; return (long)cmd.ExecuteScalar()! > 0; } }
    public void CreateAccount(string password)
    {
        if (password.Length < 12 || password.Length > 256) throw new ArgumentException("Choisissez un mot de passe de 12 caractères au minimum.");
        lock (gate) { if (HasAccount()) throw new ArgumentException("Le compte existe déjà."); using var c = Open(); using var cmd = c.CreateCommand(); var salt = RandomNumberGenerator.GetBytes(32); var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 210000, HashAlgorithmName.SHA256, 32); cmd.CommandText = "INSERT INTO account VALUES(1,$salt,$hash)"; cmd.Parameters.AddWithValue("$salt", Convert.ToBase64String(salt)); cmd.Parameters.AddWithValue("$hash", Convert.ToBase64String(hash)); cmd.ExecuteNonQuery(); }
    }
    public bool CheckPassword(string password)
    {
        if (password.Length > 256) return false;
        lock (gate) { using var c = Open(); using var cmd = c.CreateCommand(); cmd.CommandText = "SELECT salt,hash FROM account WHERE id=1"; using var r = cmd.ExecuteReader(); if (!r.Read()) return false; var hash = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(r.GetString(0)), 210000, HashAlgorithmName.SHA256, 32); return CryptographicOperations.FixedTimeEquals(hash, Convert.FromBase64String(r.GetString(1))); }
    }
    public byte[] Backup()
    {
        lock (gate)
        {
            var state = Read(); using var output = new MemoryStream(); using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true))
            {
                var hashes = new Dictionary<string, string>();
                void Add(string name, byte[] bytes) { var e = zip.CreateEntry(name, CompressionLevel.Fastest); using var stream = e.Open(); stream.Write(bytes); hashes[name] = Convert.ToHexString(SHA256.HashData(bytes)); }
                Add("state.json", JsonSerializer.SerializeToUtf8Bytes(state, Json));
                foreach (var d in state.Vehicles.SelectMany(v => v.Documents)) Add("documents/" + d.Id, File.ReadAllBytes(DocumentPath(d.Id)));
                var manifest = zip.CreateEntry("manifest.json"); using var m = manifest.Open(); JsonSerializer.Serialize(m, new { version = 1, created = DateTimeOffset.UtcNow, hashes }, Json);
            }
            return output.ToArray();
        }
    }
    public string Restore(byte[] bytes)
    {
        if (bytes.Length > 200_000_000) throw new ArgumentException("Archive trop volumineuse (200 Mo maximum).");
        using var input = new MemoryStream(bytes); using var zip = new ZipArchive(input, ZipArchiveMode.Read);
        if (zip.Entries.Sum(e => e.Length) > 500_000_000 || zip.Entries.Count > 10000) throw new ArgumentException("Archive trop volumineuse après décompression.");
        var manifest = zip.GetEntry("manifest.json") ?? throw new ArgumentException("Manifeste absent.");
        using var ms = manifest.Open(); using var doc = JsonDocument.Parse(ms); if (doc.RootElement.GetProperty("version").GetInt32() != 1) throw new ArgumentException("Version de sauvegarde incompatible.");
        var files = new Dictionary<string, byte[]>();
        foreach (var p in doc.RootElement.GetProperty("hashes").EnumerateObject())
        {
            if (p.Name != "state.json" && !(p.Name.StartsWith("documents/") && Guid.TryParseExact(p.Name[10..], "N", out _))) throw new ArgumentException("Chemin d’archive interdit.");
            var entry = zip.GetEntry(p.Name) ?? throw new ArgumentException("Fichier manquant."); using var stream = entry.Open(); using var data = new MemoryStream(); stream.CopyTo(data); var b = data.ToArray();
            if (Convert.ToHexString(SHA256.HashData(b)) != p.Value.GetString()) throw new ArgumentException("Archive endommagée : empreinte incorrecte."); files.Add(p.Name, b);
        }
        var state = JsonSerializer.Deserialize<State>(files["state.json"], Json) ?? throw new ArgumentException("Données absentes.");
        if (state.Version != 1) throw new ArgumentException("Version de données incompatible.");
        foreach (var v in state.Vehicles) { Rules.Validate(v); foreach (var d in v.Documents) if (!files.ContainsKey("documents/" + d.Id)) throw new ArgumentException("Document manquant."); }
        foreach (var s in state.Searches) Rules.Validate(s);
        foreach(var r in state.RepairReferences)Intelligence.Validate(r);
        WorkQueue.Recover(state);
        lock (gate)
        {
            var folder = Path.Combine(Root, "restaurations", DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N")[..6]);
            var restored = new Store(folder); foreach (var p in files.Where(p => p.Key.StartsWith("documents/"))) File.WriteAllBytes(restored.DocumentPath(p.Key[10..]), p.Value);
            restored.Update(s => { s.Vehicles = state.Vehicles; s.Searches = state.Searches; s.Notices = state.Notices; s.Preferences = state.Preferences;s.Work=state.Work;s.RepairReferences=state.RepairReferences; return true; });
            // Restore the verified copy into the active store only after all attachments are present.
            foreach (var p in files.Where(p => p.Key.StartsWith("documents/"))) File.WriteAllBytes(DocumentPath(p.Key[10..]), p.Value);
            Update(s => { s.Vehicles = state.Vehicles; s.Searches = state.Searches; s.Notices = state.Notices; s.Preferences = state.Preferences;s.Work=state.Work;s.RepairReferences=state.RepairReferences; return true; });
            return folder;
        }
    }
    public string DocumentPath(string id) { if (!Guid.TryParseExact(id, "N", out _)) throw new ArgumentException("Identifiant invalide."); return Path.Combine(Root, "documents", id); }
}
