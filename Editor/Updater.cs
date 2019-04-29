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

            //doesnt end with a comma, so remove the comma at the beginning of this entry if it exists because its the last entry
            if (!entry.EndsWith(","))
            {
                if (text.Substring(start - 2).Contains(","))
                {
                    //4 spaces for tabs and 3 for quote, comma and }
                    int comma = (start - 7) + text.Substring(start - 7).IndexOf(",");
                    text = text.Remove(comma, 1);
                }
            }

            text = text.Replace(entry, "");
            File.WriteAllText(path, text);

            AssetDatabase.Refresh();
        }
    }
}