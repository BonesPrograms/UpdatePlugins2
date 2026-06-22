using System.Xml.Serialization;
using BonesClassLibrary.FileFinders;

namespace UpdatePlugins2;

public class ModSerializer
{
    static readonly string SaveLocation = Path.Combine(VietnamWarModLab.Path, @"UpdatePlugins2\ModData\VietnamWar");
    static readonly string[] SaveFolders = Directory.GetDirectories(SaveLocation);
    readonly HashSet<Mod> NeedUpdate = [];
    static readonly XmlSerializer Serializer = new(typeof(CSFile));

    //runs the serializer/deserializer, dumps mods that need to be updated
    public HashSet<Mod> SaveAndGetUpdatedMods(List<Mod> mods)
    {
        NeedUpdate.Clear();
        foreach (var mod in mods)
        {
            string path = GetSaveFolderPath(mod);
            Dictionary<string, string> saveData = GetSavedData(mod, path, out List<string> newFiles);
            CheckForUpdate(saveData, mod, newFiles);
        }
        return NeedUpdate;
    }

    //compares serialized data to cs file data, re-serializes if cs file data and serialized data arent equal
    //adds mods that have updated cs files to the dump
    void CheckForUpdate(Dictionary<string, string> saveData, Mod mod, List<string> newFiles)
    {
        bool update = false;
        foreach (var save in saveData)
        {
            XmlSerializer data = new(typeof(CSFile));
            CSFile modFile = mod.Files.First(file => file.Name == save.Key);
            CSFile? savedFile = null;
            if (!newFiles.Contains(save.Key)) //when new files are generated, they dont have xml roots, and it will create errors if it tries to deserialize it
            {
                using (var stream = new FileStream(save.Value, FileMode.Open))
                {
                    savedFile = data.Deserialize(stream) as CSFile;
                    if (savedFile != null)
                        Console.WriteLine($"Successfully deserialized {modFile.Name}!");
                }
            }
            if (savedFile == null || modFile.LastWrite != savedFile.LastWrite)
            {
                SerializeCSFile(save.Value, modFile);
                update = true;
            }
        }
        
        if (update)
        {
            NeedUpdate.Add(mod);
        }
    }

    static void SerializeCSFile(string path, CSFile file)
    {
        StreamWriter writer = new(path);
        Serializer.Serialize(writer, file);
        writer.Close();
        Console.WriteLine($"Serialized {file.Name}!");
    }

    //locates the mod's save folder and grabs the XML files that contain the serialized data for the CSFile objects of a mod
    //if they dont exist, it creates them
    Dictionary<string, string> GetSavedData(Mod mod, string saveFolder, out List<string> newFiles)
    {
        Dictionary<string, string> saveFiles = GetNamesWithPaths([.. Directory.GetFiles(saveFolder)]);// key is Name, Value is path
        CheckForDeletion(saveFiles, mod);
        newFiles = AddNewFiles(saveFiles, mod.Files, saveFolder);
        return saveFiles;
    }

    //detects if there is not a sibling xml data file for a cs file in a project, creates it
    static List<string> AddNewFiles(Dictionary<string, string> saveFiles, List<CSFile> csFiles, string path)
    {
        List<string> newfiles = [];
        string[] names = [.. saveFiles.Keys];
        foreach (var file in csFiles)
        {
            if (!names.Contains(file.Name))
            {
                string newFile = @$"{path}\{file.Name}.xml";
                using (var stream = File.Create(newFile))
                {
                    saveFiles[file.Name!] = newFile;
                }
                newfiles.Add(file.Name!);
                Console.WriteLine($"Adding {file.Name} to XML!!");
            }
        }
        return newfiles;
    }

    //if any xmls are created or deleted, the mod is added to the dump

    //detects if there is a leftover xml file for a cs file that was deleted from a project, deletes it
    void CheckForDeletion(Dictionary<string, string> saveFiles, Mod mod)
    {
        foreach (var save in saveFiles.ToArray())
        {
            if (!mod.Files.Any(cs => cs.Name == save.Key))
            {
                File.Delete(save.Value);
                saveFiles.Remove(save.Key);
                Console.WriteLine($"Deleting {save.Value} from XML!!");
                NeedUpdate.Add(mod);
            }
        }
    }

    //gets the full list of xml data files in the save folder
    static Dictionary<string, string> GetNamesWithPaths(IEnumerable<string> files)
    {
        Dictionary<string, string> namesAndPaths = [];
        foreach (var file in files)
        {
            string[] path = file.Split('\\');
            string sliced = path[^1][..^".xml".Length];
            namesAndPaths[sliced] = file;
        }
        return namesAndPaths;
    }


    //finds or creates the save folder for a mod if there isnt one
    static string GetSaveFolderPath(Mod mod)
    {
        string? path = CompareFolderNames(mod);
        if (path == null)
        {
            Console.WriteLine($"Creating save directroy for {mod.Name}");
            path = @$"{SaveLocation}\{mod.Name}";
            Directory.CreateDirectory(path);
        }
        else
            Console.WriteLine($"Found save dirctory for {mod.Name!}");
        return path;
    }

    //checks to see if a save folder for a mod exists
    static string? CompareFolderNames(Mod mod)
    {
        foreach (var folder in SaveFolders)
        {
            string[] path = folder.Split('\\');
            string name = path[^1];
            if (name == mod.Name)
                return folder;
        }
        return null;
    }

}

