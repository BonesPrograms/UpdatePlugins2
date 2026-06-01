using System.Xml.Serialization;

namespace UpdatePlugins2;
public class Mod
{
    public List<CSFile> Files = [];
    public string Name;
    public string FolderPath;
    public Mod(List<CSFile> files, string name, string path)
    {
        Files = files;
        Name = name;
        FolderPath = path;
    }
}

[XmlRoot("CSFile")]
public class CSFile
{
    [XmlElement]
    public DateTime? LastWrite;

    [XmlElement]
    public string? Path;

    [XmlElement]
    public string? Name;

    public CSFile()
    {

    }
    public CSFile(DateTime lastWrite, string path)
    {
        LastWrite = lastWrite;
        Path = path;
        Name = MakeName(path);
    }

    static string MakeName(string path)
    {
        string[] split = path.Split('\\');
        string name = split[^1];
        return name[..^".cs".Length];
    }
}