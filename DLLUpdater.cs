using System.Diagnostics;

namespace UpdatePlugins2;
public class DLLUpdater
{
    const string TransferDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\VietnamWar\BepInEx\plugins\";
    readonly Dictionary<string, string> _projectRoots;
    readonly HashSet<Mod> _needsUpdate;

    public DLLUpdater(Dictionary<string, string> roots, HashSet<Mod> needsUpdate)
    {
        _projectRoots = roots;
        _needsUpdate = needsUpdate;
    }

    public void UpdatePlugins()
    {
        string[] names = [.. _projectRoots.Keys];
        foreach (var mod in _needsUpdate)
        {
            if (names.Contains(mod.Name))
            {
                Console.WriteLine($"Updating {mod.Name}!");
                if (!BuildDLL(mod.Name, _projectRoots[mod.Name]))
                    throw new Exception($"Warning: DLL building process failed, aborting. Target project: {mod.Name}");
                string dllpath = @$"{_projectRoots[mod.Name]}\bin\Debug\net6.0\{mod.Name}.dll";
                string pluginpath = @$"{TransferDirectory}{mod.Name}.dll";
                File.Copy(dllpath, pluginpath, true);
                Console.WriteLine($"{mod.Name}.dll succesfully copied to {pluginpath}!");
            }
        }
    }
    static bool BuildDLL(string name, string folder)
    {
       // string path = @$"{folder}\{name}.csproj"; i was never actually using this i was just using the folder path
         //apparently both work but they work slightly differently, its fine here tho
        ProcessStartInfo build = new()
        {
            FileName = "dotnet",
            Arguments = $"build \"{folder}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(build);
        if (process != null)
        {
            Console.WriteLine($"Building {name}");
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();
            int code = process.ExitCode;
            if (code != 0)
            {
                Console.WriteLine(output);
                Console.WriteLine(err);
                Console.ReadLine();
                return false;
            }
        }
        else
        {
            Console.WriteLine($"Process is null when trying to build {name}!");
            Console.ReadLine();
            return false;
        }
        return true;
    }

}
