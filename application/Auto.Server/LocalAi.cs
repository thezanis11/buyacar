using System.Diagnostics;
namespace Auto;
public sealed class LocalAi : IDisposable
{
    private Process? process;
    private readonly string folder;
    private readonly SemaphoreSlim gate = new(1);
    public LocalAi(string folder) { this.folder = folder; }
    public async Task EnsureRunning(CancellationToken ct)
    {
        await gate.WaitAsync(ct);
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            try { using var r = await http.GetAsync("http://127.0.0.1:11435/api/tags", ct); if (r.IsSuccessStatusCode) return; } catch (HttpRequestException) { } catch (TaskCanceledException) when (!ct.IsCancellationRequested) { }
            var runtime = Path.Combine(folder,"runtime","ollama.exe");
            if (!File.Exists(runtime)) throw new ArgumentException("Ollama n’est pas installé dans le projet. Consultez le guide débutant.");
            if (process is null || process.HasExited)
            {
                var start = new ProcessStartInfo(runtime) { UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = folder };
                start.ArgumentList.Add("serve"); start.Environment["OLLAMA_HOST"]="127.0.0.1:11435"; start.Environment["OLLAMA_MODELS"]=Path.Combine(folder,"models"); start.Environment["OLLAMA_NO_CLOUD"]="1"; start.Environment["OLLAMA_MAX_LOADED_MODELS"]="1"; start.Environment["OLLAMA_NUM_PARALLEL"]="1";
                process = Process.Start(start);
                try { process!.PriorityClass = ProcessPriorityClass.BelowNormal; } catch { }
            }
            for(int i=0;i<20;i++) { await Task.Delay(500,ct); try { using var r=await http.GetAsync("http://127.0.0.1:11435/api/tags",ct); if(r.IsSuccessStatusCode)return; } catch(HttpRequestException){} catch(TaskCanceledException) when(!ct.IsCancellationRequested){} }
            throw new ArgumentException("L’IA locale ne démarre pas. Aucune analyse n’a été produite.");
        }
        finally { gate.Release(); }
    }
    public void Dispose() { if(process is not null) { try { if(!process.HasExited)process.Kill(true); }catch{} process.Dispose(); } gate.Dispose(); }
}

