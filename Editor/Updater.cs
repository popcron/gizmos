using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

public class Updater : Editor
{
    public class Package
    {
        public string version;
    }

    private const string PackageName = "com.popcron.gizmos";
    private const string PackageURL = "https://raw.githubusercontent.com/popcron/gizmos/master/package.json";
    private const string CanUpdateKey = "Popcron.Gizmos.CanUpdate";
    private const string CheckForUpdateText = "Popcron/Gizmos/Check for updates";
    private const string UpdateText = "Popcron/Gizmos/Update";

    private static async Task<bool> IsUpdateAvailable()
    {
        WebClient wc = new WebClient();
        string json = await wc.DownloadStringTaskAsync(PackageURL);
        string versionText = JsonUtility.FromJson<Package>(json).version;
        Version version = Version.Parse(versionText);
        Version currentVersion = await GetLocalVersion();

        if (currentVersion != null)
        {
            bool updateAvailable = currentVersion.CompareTo(version) < 0;
            return updateAvailable;
        }
        else
        {
            return false;
        }
    }

    private static async Task<Version> GetLocalVersion()
    {
        ListRequest listRequest = Client.List(true);
        while (!listRequest.IsCompleted)
        {
            await Task.Delay(1);
        }

        foreach (PackageInfo pack in listRequest.Result)
        {
            if (pack.name == PackageName)
            {
                if (pack.source == PackageSource.Local) continue;

                Version localVersion = Version.Parse(pack.version);
                return localVersion;
            }
        }

        return null;
    }

    [MenuItem(CheckForUpdateText, false, 0)]
    [DidReloadScripts]
    private static async void CheckForUpdates()
    {
        //check for updates
        bool canUpdate = await IsUpdateAvailable();
        EditorPrefs.SetBool(CanUpdateKey, canUpdate);
    }

    [MenuItem(UpdateText, false, 0)]
    public static void Update()
    {
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

    [MenuItem(UpdateText, true)]
    private static bool CanUpdate()
    {
        return EditorPrefs.GetBool(CanUpdateKey);
    }
}