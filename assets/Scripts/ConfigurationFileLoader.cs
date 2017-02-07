using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class ConfigurationFileLoader : MonoBehaviour
{
    public static string configFilePlayerPrefsString = "configFile";

    private static string filename;
    // Use this for initialization
    public static void Init()
    {
        filename = PlayerPrefs.GetString(configFilePlayerPrefsString);
        string path = Application.dataPath.Replace('/', '\\') + "\\" + filename;
        if (File.Exists(path))
            GetConfigParameters(path);
        LoadDefaultConfig();
    }

    private static bool ManualRotation = false;
    private static float ClickDistance = 2f;
    private static float DefaultLookAngle = 0;


    static void GetConfigParameters(string filename)
    {
        StreamReader reader = new StreamReader(filename);
        string file = reader.ReadToEnd();
        reader.Close();
        ManualRotation = getBoolFromFileString(file, "ManualRotation", ManualRotation);
        ClickDistance = getFloatFromFileString(file, "ClickDistance", ClickDistance);
        DefaultLookAngle = getFloatFromFileString(file, "DefaultLookAngle", DefaultLookAngle);
    }

    static void LoadDefaultConfig()
    {
        PlayerPrefs.SetInt("ManualRotation", ManualRotation ? 1 : 0);
        PlayerPrefs.SetFloat("ClickDistance", ClickDistance);
        PlayerPrefs.SetFloat("DefaultLookAngle", DefaultLookAngle);
    }

    static bool getBoolFromFileString(string file, string tag, bool defaultValue)
    {
        string token = getTokenFromFileString(file, tag);
        if (token.Length == 0)
            return defaultValue;
        bool parsedToken = defaultValue;
        try
        {
            parsedToken = bool.Parse(token);
        }
        catch (Exception) { }
        return parsedToken;
    }

    static float getFloatFromFileString(string file, string tag, float defaultValue)
    {
        string token = getTokenFromFileString(file, tag);
        if (token.Length == 0)
            return defaultValue;
        float parsedToken = defaultValue;
        try
        {
            parsedToken = float.Parse(token);
        }
        catch (Exception) { }
        return parsedToken;
    }

    static int getIntFromFileString(string file, string tag, int defaultValue)
    {
        string token = getTokenFromFileString(file, tag);
        if (token.Length == 0)
            return defaultValue;
        int parsedToken = defaultValue;
        try
        {
            parsedToken = int.Parse(token);
        }
        catch (Exception) { }
        return parsedToken;
    }

    static string getStringFromFileString(string file, string tag, string defaultValue)
    {
        string token = getTokenFromFileString(file, tag);
        if (token.Length == 0)
            return defaultValue;
        return token;
    }

    static string getTokenFromFileString(string file, string tag)
    {
        string[] split = file.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < split.Length; i++)
        {
            string line = split[i].Trim();
            if (line.StartsWith(tag + "="))
            {
                string[] tokens = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 2)
                {
                    string token = tokens[1].Trim();
                    return token;
                }
            }
        }
        return "";
    }
}
