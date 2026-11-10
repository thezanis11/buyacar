using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Security.Cryptography;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using Forms = System.Windows.Forms;
namespace AutoDesktop;

public partial class App : System.Windows.Application
{
    private Mutex? mutex;
    private Process? server;
    private Forms.NotifyIcon? tray;
    private Window? window;
    private bool quitting;
    private readonly string desktopToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    private readonly HashSet<string> delivered = [];
    private System.Windows.Threading.DispatcherTimer? notificationTimer;
    private readonly string deliveryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"AutoClair","delivered-notices.json");
    private readonly string address = "http://127.0.0.1:5187";
    private readonly string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", ".."));
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        mutex = new Mutex(true, "Local\\AutoClairDesktop", out var first);
        if (!first) { Process.Start(new ProcessStartInfo(address) { UseShellExecute = true }); Shutdown(); return; }
        try
        {
            var start = new ProcessStartInfo(Path.Combine(root, ".dotnet", "dotnet.exe")) { WorkingDirectory = Path.Combine(root, "dist", "server"), UseShellExecute = false, CreateNoWindow = true };
            start.ArgumentList.Add(Path.Combine(root, "dist", "server", "Auto.Server.dll"));
            start.Environment["DOTNET_ROOT"] = Path.Combine(root, ".dotnet");
            start.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
            start.Environment["AUTO_DESKTOP_TOKEN"] = desktopToken;
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };
            bool responding = false;
            try { using var response = await http.GetAsync(address + "/api/session"); responding = response.IsSuccessStatusCode; } catch { }
            if (!responding) server = Process.Start(start);
            for (int i = 0; i < 40 && !responding; i++) { await Task.Delay(300); try { using var response = await http.GetAsync(address + "/api/session"); responding = response.IsSuccessStatusCode; } catch { } }
            if (!responding) throw new InvalidOperationException("Le service ne démarre pas. Consultez le guide de dépannage.");
            window = new Window { Title = "AutoClair — votre espace automobile", Width = 1280, Height = 850, MinWidth = 760, MinHeight = 550, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            var web = new WebView2(); window.Content = web;
            var env = await CoreWebView2Environment.CreateAsync(null, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoClair", "browser"));
            await web.EnsureCoreWebView2Async(env);
            web.CoreWebView2.Settings.AreDevToolsEnabled = false;
            web.CoreWebView2.Settings.IsStatusBarEnabled = false;
            web.CoreWebView2.NavigationStarting += (_, args) => { if (!args.Uri.StartsWith(address + "/", StringComparison.OrdinalIgnoreCase)) { args.Cancel = true; OpenExternal(args.Uri); } };
            web.CoreWebView2.NewWindowRequested += (_, args) => { args.Handled = true; OpenExternal(args.Uri); };
            web.CoreWebView2.WebMessageReceived += (_, args) => {
                if (!args.Source.StartsWith(address + "/", StringComparison.OrdinalIgnoreCase)) return;
                try { using var json = JsonDocument.Parse(args.WebMessageAsJson); if (server is null && json.RootElement.GetProperty("type").GetString() == "notification") tray?.ShowBalloonTip(5000, "AutoClair", json.RootElement.GetProperty("title").GetString() ?? "Nouvel événement", Forms.ToolTipIcon.Info); } catch { }
            };
            var menu = new Forms.ContextMenuStrip();
            menu.Items.Add("Ouvrir AutoClair", null, (_, _) => ShowWindow());
            menu.Items.Add("Ouvrir dans le navigateur", null, (_, _) => Process.Start(new ProcessStartInfo(address) { UseShellExecute = true }));
            var startup = new Forms.ToolStripMenuItem("Démarrer à l’ouverture de session") { CheckOnClick = true, Checked = File.Exists(StartupPath()) };
            startup.CheckedChanged += (_, _) => { try { if (startup.Checked) File.WriteAllText(StartupPath(), $"@echo off\r\nstart \"\" /min \"{Path.Combine(root, "Lancer-AutoClair.cmd")}\" --background\r\n"); else File.Delete(StartupPath()); } catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); } };
            menu.Items.Add(startup); menu.Items.Add("Arrêter le service et quitter", null, (_, _) => Quit());
            tray = new Forms.NotifyIcon { Text = "AutoClair — service actif", Icon = System.Drawing.SystemIcons.Application, ContextMenuStrip = menu, Visible = true };
            tray.DoubleClick += (_, _) => ShowWindow();
            try { if(File.Exists(deliveryPath)) foreach(var item in JsonSerializer.Deserialize<string[]>(File.ReadAllText(deliveryPath)) ?? []) delivered.Add(item); } catch { }
            notificationTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            notificationTimer.Tick += async (_,_) => await PollNotifications(); notificationTimer.Start();
            window.Closing += (_, args) => { if (!quitting) { args.Cancel = true; window.Hide(); } };
            web.Source = new Uri(address);
            if (!e.Args.Contains("--background")) window.Show();
        }
        catch (Exception ex) { System.Windows.MessageBox.Show("AutoClair n’a pas pu démarrer.\n\n" + ex.Message + "\n\nVous pouvez utiliser Lancer-Navigateur.cmd.", "AutoClair"); Quit(); }
    }
    private static string StartupPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "AutoClair.cmd");
    private static void OpenExternal(string uri) { if (Uri.TryCreate(uri, UriKind.Absolute, out var u) && u.Scheme is "https" or "http") Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true }); }
    private void ShowWindow() { window?.Show(); if (window != null) { window.WindowState = WindowState.Normal; window.Activate(); } }
    private async Task PollNotifications()
    {
        if(server is null || quitting)return;
        try {
            using var client=new HttpClient { Timeout=TimeSpan.FromSeconds(5) }; client.DefaultRequestHeaders.Add("X-Auto-Desktop",desktopToken);
            using var response=await client.GetAsync(address+"/api/desktop-events"); if(!response.IsSuccessStatusCode)return;
            using var json=JsonDocument.Parse(await response.Content.ReadAsStringAsync());var p=json.RootElement.GetProperty("preferences");
            int start=p.GetProperty("quietStart").GetInt32(),end=p.GetProperty("quietEnd").GetInt32(),hour=DateTime.Now.Hour;
            bool quiet=start!=end && (start<end ? hour>=start&&hour<end : hour>=start||hour<end);
            if(quiet||!p.GetProperty("notifications").GetBoolean())return;
            var fresh=json.RootElement.GetProperty("notices").EnumerateArray().Where(n=>!delivered.Contains(n.GetProperty("id").GetString()!)).ToArray();
            if(fresh.Length==0)return;
            var title=fresh[0].GetProperty("title").GetString() ?? "Nouvel événement";
            if(fresh.Length>1)title+=$" (+{fresh.Length-1} autres dans l’application)";
            tray?.ShowBalloonTip(5000,"AutoClair",title,Forms.ToolTipIcon.Info);
            foreach(var n in fresh)delivered.Add(n.GetProperty("id").GetString()!);
            Directory.CreateDirectory(Path.GetDirectoryName(deliveryPath)!);File.WriteAllText(deliveryPath,JsonSerializer.Serialize(delivered.TakeLast(1000)));
        }catch { }
    }
    private void Quit() { quitting = true; notificationTimer?.Stop(); tray?.Dispose(); if (server is not null && !server.HasExited) server.Kill(true); window?.Close(); Shutdown(); }
    protected override void OnExit(ExitEventArgs e) { mutex?.Dispose(); base.OnExit(e); }
}
