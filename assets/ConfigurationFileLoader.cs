using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class ConfigurationFileLoader : MonoBehaviour
{
    public string filename = "SimulationConfiguration.config";
    // Use this for initialization
    void Start()
    {
        string path = Application.dataPath.Replace('/', '\\') + "\\" + filename;
        if (File.Exists(path))
            GetConfigParameters(path);
        LoadDefaultConfig();
    }

    private bool ManualRotation = false;
    private float ClickDistance = 2f;
    private float DefaultLookAngle = 0;


    void GetConfigParameters(string filename)
    {
        StreamReader reader = new StreamReader(filename);
        string file = reader.ReadToEnd();
        reader.Close();
        ManualRotation = getBoolFromFileString(file, "ManualRotation", ManualRotation);
        ClickDistance = getFloatFromFileString(file, "ClickDistance", ClickDistance);
        DefaultLookAngle = getFloatFromFileString(file, "DefaultLookAngle", DefaultLookAngle);
    }

    void LoadDefaultConfig()
    {
        PlayerPrefs.SetInt("ManualRotation", ManualRotation ? 1 : 0);
        PlayerPrefs.SetFloat("ClickDistance", ClickDistance);
        PlayerPrefs.SetFloat("DefaultLookAngle", DefaultLookAngle);
    }

    bool getBoolFromFileString(string file, string tag, bool defaultValue)
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

    float getFloatFromFileString(string file, string tag, float defaultValue)
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

    int getIntFromFileString(string file, string tag, int defaultValue)
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

    string getStringFromFileString(string file, string tag, string defaultValue)
    {
        string token = getTokenFromFileString(file, tag);
        if (token.Length == 0)
            return defaultValue;
        return token;
    }

    string getTokenFromFileString(string file, string tag)
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
