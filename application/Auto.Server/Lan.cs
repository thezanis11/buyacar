using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
namespace Auto;
public record LanConfig(string Address, string Password);
public static class Lan
{
    public static bool Private(IPAddress a)
    {
        if (a.IsIPv4MappedToIPv6) a = a.MapToIPv4();
        if (IPAddress.IsLoopback(a)) return true;
        var b = a.GetAddressBytes();
        return b.Length == 4 ? b[0] == 10 || b[0] == 192 && b[1] == 168 || b[0] == 172 && b[1] >= 16 && b[1] <= 31 : a.IsIPv6LinkLocal || (b[0] & 254) == 252;
    }
    public static void Configure(string root, string address)
    {
        if (!IPAddress.TryParse(address, out var ip) || !Private(ip) || IPAddress.IsLoopback(ip)) throw new ArgumentException("Indiquez l’adresse privée du PC sur le réseau domestique.");
        Directory.CreateDirectory(root);
        using var caKey = RSA.Create(3072);
        var caReq = new CertificateRequest("CN=AutoClair - reseau personnel", caKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        caReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
        caReq.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign, true));
        using var ca = caReq.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(2));
        using var key = RSA.Create(3072);
        var req = new CertificateRequest("CN=AutoClair", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
        req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));
        req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new("1.3.6.1.5.5.7.3.1") }, true));
        var san = new SubjectAlternativeNameBuilder(); san.AddIpAddress(ip); san.AddIpAddress(IPAddress.Loopback); san.AddDnsName("localhost"); req.CertificateExtensions.Add(san.Build());
        using var signed = req.Create(ca, DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow.AddYears(1), RandomNumberGenerator.GetBytes(16));
        using var certificate = signed.CopyWithPrivateKey(key);
        var password = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        File.WriteAllBytes(Path.Combine(root, "lan.pfx"), certificate.Export(X509ContentType.Pfx, password));
        File.WriteAllBytes(Path.Combine(root, "AutoClair-reseau.cer"), ca.Export(X509ContentType.Cert));
        File.WriteAllText(Path.Combine(root, "lan.json"), JsonSerializer.Serialize(new LanConfig(address, password), Store.Json));
        Console.WriteLine($"Adresse : https://{address}:5188\nCertificat public : {Path.Combine(root, "AutoClair-reseau.cer")}\nRedémarrez AutoClair. Installez ce certificat de confiance uniquement sur vos appareils personnels.");
    }
}
