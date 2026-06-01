namespace UpdatePlugins2;
public class ProjectFinder //this requires folder names, assembly names, and csproj names to be the same!
{
    const string Root = @"C:\Users\user\Desktop\VietnamWarModLab";

    public Dictionary<string, string> ProjectRoots => _projectRoots ??= GetProjectRoots(); //Key is Name, Value is Folder Path
    Dictionary<string, string>? _projectRoots;
    readonly string[] _projectFiles = Directory.GetFiles(Root, "*.csproj", SearchOption.AllDirectories);
    static string RecombinePath(string[] array) //can do root + projName, string concat, or substring, or path combine
    {                                           
        string path = string.Empty;             
        for (int i = 0; i < array.Length - 1; i++)
        {
            path += i > 0 ? $@"\{array[i]}" : array[i];
        }
        return path;
    }


    Dictionary<string, string> GetProjectRoots()
    {
        Dictionary<string, string> projAndFolders = [];
        foreach (var proj in _projectFiles)
        {
            IEnumerable<string> projFileText = File.ReadLines(proj); //this is better to readalltext because its a stream, you can also use StreamReader for super large files
            if (projFileText.Any(text => text.Contains("<TargetFramework>net6.0"))) //Any and FirstOrDefault short circuit early so it doesnt read the entire steam
            {
                string[] path = proj.Split('\\');
                string projName = path[^1]; //this is path.length - 1
                string slicedName = projName.Substring(0, projName.Length - ".csproj".Length); // projName[..^".csproj".Length]; alternative
                projAndFolders[slicedName] = RecombinePath(path); //can also use path.combine(root, slicedname)
            }
        }
        return projAndFolders;
    }

}