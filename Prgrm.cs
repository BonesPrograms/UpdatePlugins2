using UpdatePlugins2;

ProjectFinder finder = new();
Dictionary<string, string> roots = finder.ProjectRoots;

ModFetcher fetcher = new(roots);
List<Mod> mods = fetcher.Mods;

ModSerializer serializer = new(mods);
HashSet<Mod> needsUpdate = serializer.CheckAndSave();

DLLUpdater updater = new(roots, needsUpdate);
updater.UpdatePlugins();


// bool DLLDelegate(string dll)
// {
//     bool isNet6 = dll.IndexOf("net6.0") >= 0; //we check for net6 earlier by reading the csproj file
//     bool isNotRef = dll.IndexOf("ref") < 0;
//     return isNet6 && isNotRef;
// }

// public readonly struct ProjectData
// {
//     public readonly string DLLPath;

//     public readonly string Root;

//     public readonly string Name;
// }

// Dictionary<string, string> GetProjectDLLs()
// {
//     Dictionary<string, string> projDLLs = [];
//     foreach (var proj in _projectRoots)
//     {
//         string folder = proj.Value;
//         string name = proj.Key;
//         IEnumerable<string> dlls = Directory.EnumerateFiles(folder, $"{name}*.dll", SearchOption.AllDirectories);
//         string? dll = dlls.FirstOrDefault(dll => !dll.Contains("ref"));
//         if (dll == null)
//         {
//             Console.WriteLine($"Building {name}.dll for the first time.");
//             TryBuildDLL(proj.Key, proj.Value, out dll);
//         }
//         projDLLs[name] = dll!;
//     }
//     return projDLLs;
//