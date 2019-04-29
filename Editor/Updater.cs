using System.IO;
using UnityEditor;
using UnityEngine;

public class Updater : Editor
{
    [MenuItem("Popcron/Gizmos/Update")]
    public static void Update()
    {
        const string PackageName = "com.popcron.gizmos";

        //get the manifest.json file
        string path = Application.dataPath;
        path = Directory.GetParent(path).FullName;
        path = Path.Combine(path, "Packages", "manifest.json");
        if (File.Exists(path))
        {
            string text = File.ReadAllText(path);
            int index = text.IndexOf("\"lock\"");
            int start = index + text.Substring(index).IndexOf("\"" + PackageName + "\"");
            int end = start + text.Substring(start).IndexOf("}") + 2;
            string entry = text.Substring(start, end - start);

            text = text.Replace(entry, "");
            File.WriteAllText(path, text);

            AssetDatabase.Refresh();
        }
    }
}