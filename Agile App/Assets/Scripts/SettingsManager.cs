using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

/* Manages the application settings, including loading, saving, and accessing settings data */
/* Written by James */
public class SettingsManager : MonoBehaviour
{
    public string jsonFile; // Filename of the JSON settings file in the Resources folder

    /* Event delegate for notifying when setup is completed */
    public delegate void SetupHandler();
    public event SetupHandler OnSetupCompleted;

    private bool isSetupCompleted = false; // Flag to indicate whether setup is completed
    private List<Dictionary<string, object>> settingsList = new List<Dictionary<string, object>>(); // List to store settings data
    private string settingsVersion; // Version of the settings data

    /* Start: called before the first frame update */
    void Start()
    {
        /* Populate settings list on startup */
        PopulateOptions();

        /* Notify other scripts that the setup is completed, if they're waiting */
        isSetupCompleted = true;
        OnSetupCompleted?.Invoke();
    }

    /* Checks the version of the settings data */
    private bool CheckVersion()
    {
        JObject defaultJsonObject = null;
        JObject persistentJsonObject = null;

        /* Load default settings JSON */
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFile);

        if (jsonTextAsset != null) defaultJsonObject = JObject.Parse(jsonTextAsset.text);
        else return false;

        /* Load persistent settings JSON */
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, jsonFile + ".json"));
        persistentJsonObject = JObject.Parse(json);

        /* Compare versions */
        if (persistentJsonObject["version"] != null && (string)persistentJsonObject["version"] == (string)defaultJsonObject["version"])
        {
            Debug.Log("SettingsManager::CheckVersion -> Settings are the same version");
            return true;
        }
        else
        {
            Debug.LogWarning("SettingsManager::CheckVersion -> Settings are different versions");
            return false;
        }
    }

    /* PopulateOptions: Populates options from the settings JSON file */
    public void PopulateOptions()
    {
        /* Construct the file path for the JSON settings file in the persistent data path */
        string filePath = Path.Combine(Application.persistentDataPath, jsonFile + ".json");

        JObject jsonObject = null;

        /* Load persistent JSON if exists and versions match, otherwise load default JSON */
        if (File.Exists(filePath) && CheckVersion())
        {
            /* Read the JSON file and parse it into a JObject */
            string json = File.ReadAllText(filePath);
            jsonObject = JObject.Parse(json);
        }

        /* If the JObject is still null, attempt to load the JSON file from Resources */
        if (jsonObject == null)
        {
            TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFile);
            if (jsonTextAsset != null)
            {
                jsonObject = JObject.Parse(jsonTextAsset.text);
            }
            else
            {
                Debug.LogError("SettingsManager::PopulateOptions -> Settings file could not be loaded!");
            }
        }

        /* Extract the settings version from the JObject */
        settingsVersion = (string)jsonObject["version"];

        /* Extract the settings array from the JObject */
        JArray settingsArray = (JArray)jsonObject["settings"];

        /* Parse settings data */
        if (settingsArray != null)
        {
            /* Log a success message if the settings are successfully parsed */
            Debug.Log("SettingsManager::PopulateOptions -> Settings successfully parsed!");

            /* Iterate through each setting object in the settings array */
            foreach (JObject settingObject in settingsArray)
            {
                /* Convert each setting object into a dictionary and add it to the settings list */
                Dictionary<string, object> settingDictionary = settingObject.Properties().ToDictionary(p => p.Name, p => (object)p.Value);
                settingsList.Add(settingDictionary);
            }
        }
        else
        {
            /* Log an error if the settings data cannot be parsed */
            Debug.LogError("SettingsManager::PopulateOptions -> Failed to parse settings data");
        }
    }


    /* GetOptions: Returns the list of settings */
    public List<Dictionary<string, object>> GetOptions()
    {
        return settingsList;
    }

    /* GetOption: Returns a specific setting by name */
    public Dictionary<string, object> GetOption(string settingName)
    {
        /* Find the index of the setting with the specified name */
        int foundIndex = settingsList.FindIndex(entry =>
        {
            return entry["name"].ToString() == settingName;
        });

        /* If the setting is found, return it */
        /* If the setting is not found, log a warning and return null */
        if (foundIndex != -1) return settingsList[foundIndex];
        else Debug.LogWarning($"SettingsManager::GetOption -> Setting with name '{settingName}' not found");

        /* Throw an exception if the setting is not found (should not happen) */
        throw new Exception("SettingsManager::GetOption -> An unknown exception has occurred");
    }

    /* SaveOptions: Saves the settings to a JSON file */
    public void SaveOptions()
    {
        /* Check if there are settings to save */
        /* If not, Log warning that there are no settings to save */
        if (settingsList.Count != 0)
        {
            /* Create a JSON object to represent the settings data */
            JObject jsonObject = new JObject();
            jsonObject["version"] = settingsVersion;
            jsonObject["settings"] = new JArray(settingsList.Select(setting =>
                new JObject(setting.Select(kv => new JProperty(kv.Key, JToken.FromObject(kv.Value))))));

            /* Convert the JSON object to a JSON string */
            string json = jsonObject.ToString(Formatting.Indented);

            /* Write the JSON string back to the file */
            string filePath = Path.Combine(Application.persistentDataPath, jsonFile + ".json");
            File.WriteAllText(filePath, json);

            /* Log success message */
            Debug.Log("SettingsManager::SaveOptions -> Settings saved successfully");
        }
        else Debug.LogWarning("SettingsManager::SaveOptions -> No settings to save");
    }

    /* UpdateSetting: Updates the value of a specific setting */
    public void UpdateSetting(string settingName, object value)
    {
        /* Find the index of the setting in the settings list */
        int foundIndex = settingsList.FindIndex(entry =>
        {
            return entry["name"].ToString() == settingName;
        });

        /* If the setting is found, update its value */
        if (foundIndex != -1)
        {
            settingsList[foundIndex]["value"] = value;
            /* Log a message indicating the setting update */
            Debug.Log($"SettingsManager::UpdateSetting -> {settingName} updated with value {value.ToString()}");
        }
        else
        {
            /* Log a warning if the setting is not found */
            Debug.LogWarning($"SettingsManager::UpdateSetting -> Setting with name '{settingName}' not found");
        }
    }

    /* isSetup: Checks if setup is completed */
    public bool isSetup()
    {
        return isSetupCompleted;
    }
}
