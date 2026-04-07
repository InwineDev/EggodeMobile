using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UserContentPaths
{
    public const string ResourceMapsFolder = "Maps";
    public const string ResourceMapsFolderLower = "maps";

    public static string MapsPersistent => EnsureDirectory(Path.Combine(Application.persistentDataPath, "maps"));
    public static string SkinsPersistent => EnsureDirectory(Path.Combine(Application.persistentDataPath, "skins"));
    public static string SkinJsonPath => Path.Combine(SkinsPersistent, "skin.json");

    public static IEnumerable<string> EnumeratePersistentMapFiles()
    {
        var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string root in GetCandidatePersistentFolders())
        {
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
                continue;

            try
            {
                foreach (string pattern in new[] { "*.json", "*.eggodemap" })
                {
                    foreach (string file in Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories))
                        results.Add(file);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to enumerate map files in '{root}': {ex.Message}");
            }
        }

        return results;
    }

    public static IEnumerable<TextAsset> LoadBuiltInMapAssets()
    {
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string folder in GetCandidateResourceFolders())
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>(folder);
            if (assets == null || assets.Length == 0)
                continue;

            foreach (TextAsset asset in assets)
            {
                if (asset == null)
                    continue;

                if (seenNames.Add(asset.name))
                    yield return asset;
            }
        }
    }

    public static string ResolveResourceMapPath(string mapName)
    {
        if (string.IsNullOrWhiteSpace(mapName))
            return null;

        foreach (string folder in GetCandidateResourceFolders())
        {
            string path = string.IsNullOrEmpty(folder) ? mapName : folder + "/" + mapName;
            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset != null)
                return path;
        }

        return ResourceMapsFolder + "/" + mapName;
    }

    public static string LoadSkinJsonOrDefault()
    {
        try
        {
            if (File.Exists(SkinJsonPath))
            {
                string json = File.ReadAllText(SkinJsonPath);
                if (!string.IsNullOrWhiteSpace(json))
                    return json;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Failed to load skin json: " + ex.Message);
        }

        return "{\n  \"body\": 0,\n  \"nose\": 0,\n  \"mouth\": 0,\n  \"eye\": 0,\n  \"hat\": 0\n}";
    }

    public static void SaveSkinJson(string json)
    {
        try
        {
            File.WriteAllText(SkinJsonPath, string.IsNullOrWhiteSpace(json) ? LoadSkinJsonOrDefault() : json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Failed to save skin json: " + ex.Message);
        }
    }

    public static bool LooksLikeWebUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return Uri.TryCreate(value, UriKind.Absolute, out Uri uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    public static bool LooksLikeAbsoluteFilePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (value.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            return true;

        return Path.IsPathRooted(value);
    }

    public static Texture2D LoadTextureFromAbsolutePath(string absolutePath)
    {
        if (string.IsNullOrWhiteSpace(absolutePath))
            return null;

        string normalizedPath = absolutePath;
        if (normalizedPath.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            normalizedPath = new Uri(normalizedPath).LocalPath;

        if (!File.Exists(normalizedPath))
            return null;

        try
        {
            byte[] bytes = File.ReadAllBytes(normalizedPath);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes))
            {
                UnityEngine.Object.Destroy(texture);
                return null;
            }

            return texture;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load texture '{normalizedPath}': {ex.Message}");
            return null;
        }
    }

    private static IEnumerable<string> GetCandidatePersistentFolders()
    {
        string persistent = Application.persistentDataPath;
        yield return persistent;
        yield return MapsPersistent;
        yield return Path.Combine(persistent, "Maps");
        yield return Path.Combine(persistent, "maps");
        yield return Path.Combine(persistent, "UserMaps");
        yield return Path.Combine(persistent, "CustomMaps");
    }

    private static IEnumerable<string> GetCandidateResourceFolders()
    {
        yield return ResourceMapsFolder;
        yield return ResourceMapsFolderLower;
    }

    private static string EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }
}
