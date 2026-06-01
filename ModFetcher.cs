namespace UpdatePlugins2;
public class ModFetcher
{

    public List<Mod> Mods => _mods ??= FetchMods();
    List<Mod>? _mods;
    readonly Dictionary<string, string> _projectRoots;
    public ModFetcher(Dictionary<string, string> roots)
    {
        _projectRoots = roots;
    }

    List<Mod> FetchMods()
    {
        List<Mod> mods = [];
        foreach (var proj in _projectRoots)
        {
            IEnumerable<string> filePaths = Directory.EnumerateFiles(proj.Value, "*.cs", SearchOption.AllDirectories).Where(path => !path.Contains(@"\obj\"));
            List<CSFile> csFiles = GetCSFiles(filePaths);
            mods.Add(new Mod(csFiles, proj.Key, proj.Value));
        }
        return mods;
    }

    static List<CSFile> GetCSFiles(IEnumerable<string> filePaths)
    {
        List<CSFile> modFiles = [];
        foreach (var path in filePaths)
        {
            FileInfo info = new(path);
            DateTime lastWrite = info.LastWriteTime;
            modFiles.Add(new CSFile(lastWrite, path));
        }
        return modFiles;
    }
}
