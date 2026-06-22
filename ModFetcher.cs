using System.Collections.Immutable;

namespace UpdatePlugins2;
public static class ModFetcher
{
    public static List<Mod> FetchMods(Dictionary<string, string> roots)
    {
        List<Mod> mods = [];
        foreach (var proj in roots)
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
