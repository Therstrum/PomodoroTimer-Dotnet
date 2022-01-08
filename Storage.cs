using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

public static class LocalStorage
{
    static string appDataDirectory = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TorbanDev");
    static string settingsFileName = Path.Combine(appDataDirectory, "SpotifySettings.txt");
    static string configFileName = Path.Combine(appDataDirectory, "SpotifyAppSettings.config");
    static string spotifyAppPath = Path.Combine(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spotify"), "Spotify.exe");
    static string haSettingsFilePath = Path.Combine(appDataDirectory, "HomeAssistantSettings.txt");
    public enum ConfigFiles { SPOTIFYSETTINGS, SPOTIFYCONFIG, HOMEASSISTANTCONFIG };

    public static void StartSpotifyApp()
    {
        Process.Start(spotifyAppPath);
    }

    static public LocalData GetTokenData(Enum fileType)
    {
        string fileName = "";
        switch (fileType)
        {
            case ConfigFiles.SPOTIFYCONFIG:
                fileName = configFileName;
                break;
            case ConfigFiles.SPOTIFYSETTINGS:
                fileName = settingsFileName;
                break;
            case ConfigFiles.HOMEASSISTANTCONFIG:
                fileName = haSettingsFilePath;
                break;
            default:
                break;
        }

        if (!ValidateFilePath(appDataDirectory, fileName)) return null;

        string text = File.ReadAllText(fileName);

        if (text == null || text == "")
        {
            return null;
        }
        LocalData ld = new LocalData();

        switch (fileType)
        {
            case ConfigFiles.SPOTIFYCONFIG:
                ld = JsonSerializer.Deserialize<ConfigData>(text);
                break;
            case ConfigFiles.SPOTIFYSETTINGS:
                ld = JsonSerializer.Deserialize<TokenData>(text);
                break;
            case ConfigFiles.HOMEASSISTANTCONFIG:
                ld = JsonSerializer.Deserialize<HaTokenData>(text);
                break;
            default:
                break;
        }
        return ld;
    }

    static bool ValidateFilePath(string directory, string filePath)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!File.Exists(filePath))
        {
            using (FileStream fs = File.Create(filePath))
            {

            }
        }
        return true;
    }
    static public void SaveTokenDataPublic(string tokenText, Enum fileType)
    {
        string file = "";
        switch (fileType)
        {
            case ConfigFiles.SPOTIFYCONFIG:
                file = configFileName;
                break;
            case ConfigFiles.SPOTIFYSETTINGS:
                file = settingsFileName;
                break;
            case ConfigFiles.HOMEASSISTANTCONFIG:
                file = haSettingsFilePath;
                break;
            default:
                break;
        }
        SaveTokenData(tokenText, file);
    }
    static void SaveTokenData(string tokenText, string filepath)
    {
        using (TextWriter tr = new StreamWriter(filepath))
        {
            tr.Write(tokenText);
        }
    }

}

public class TokenData : LocalData
{
    public string access_token { get; set; }
    public DateTime expires_in { get; set; }
    public string refresh_token { get; set; }
}

public class ConfigData : LocalData
{
    public string client_id { get; set; }
    public string client_Secret { get; set; }
}


class HaTokenData : LocalData
{
    // Class to maintain API integration config items.
    public string token { get; set; }
    public string url { get; set; }
    public HomeAssistant.HaAutomationData[] services { get; set; }
    public HaTokenData(string Token, string Url, HomeAssistant.HaAutomationData[] Services)
    {
        this.token = Token;
        this.url = Url;
        this.services = Services;
    }
}

public class LocalData
{

}