using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;


//written by James
public class SettingsManager : MonoBehaviour
{
    public string jsonFile; //file name in Resources folder

    public delegate void SetupHandler();
    public event SetupHandler OnSetupCompleted;

    private bool isSetupCompleted = false;

    private List<Dictionary<string, object>> settingsList = new List<Dictionary<string, object>>();

    void Start()
    {
        //create list on startup
        PopulateOptions();

        // Notify other scripts that the setup is completed, if they're waiting
        isSetupCompleted = true;
        OnSetupCompleted?.Invoke();
    }

    /// <summary>
    /// function that loads the settings in from a json file
    /// </summary>
    public void PopulateOptions()
    {
        string filePath = Path.Combine(Application.persistentDataPath, jsonFile + ".json"); // persistent path
        JObject jsonObject = null;

        if (File.Exists(filePath)) //if we already have a persistent json file use that instead
        {
            string json = File.ReadAllText(filePath);
            jsonObject = JObject.Parse(json);
        }
        else
        {
            TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFile); // Load in the JSON file as a TextAsset
            if (jsonTextAsset != null) //check if the data has been read correctly
            {
                jsonObject = JObject.Parse(jsonTextAsset.text); // Parse the text as a json object

            }
            else
            {
                Debug.LogError("Settings file could not be loaded!");
            }

        }

        JArray settingsArray = (JArray)jsonObject["settings"]; // get the array of settings as json objects

        if (settingsArray != null) // if the json is successfully read in
        {
            Debug.Log("settings successfully parsed!");
            // Iterate through each object in the "settings" array
            foreach (JObject settingObject in settingsArray)
            {
                Dictionary<string, object> settingDictionary = settingObject.Properties().ToDictionary(p => p.Name, p => (object)p.Value);
                settingsList.Add(settingDictionary); // add to list
            }

        }
        else
        {
            Debug.LogError("Failed to parse settings data");
        }
    }

    /// <summary>
    /// returns a list of dictionaries that contain setting information
    /// </summary>
    public List<Dictionary<string, object>> GetOptions()
    {
        return settingsList;
    }

    /// <summary>
    /// returns a specific settings dictionary
    /// </summary>
    public Dictionary<string, object> GetOption(string settingName)
    {
        //find the position of the relevent setting
        int foundIndex = settingsList.FindIndex(entry => //find the list index
        {
            return entry["name"].ToString() == settingName;
        });

        if (foundIndex != -1)
        {
            return settingsList[foundIndex];
        }
        else
        {
            Debug.LogWarning($"Setting with name '{settingName}' not found");
        }

        return new Dictionary<string, object>();
    }


    /// <summary>
    /// function that saves the settings to the json file
    /// </summary>
    public void SaveOptions()
    {
        if (settingsList.Count != 0)
        {
            // Create a JSON object to represent the settings data
            JObject jsonObject = new JObject();
            jsonObject["settings"] = new JArray(settingsList.Select(setting =>
                new JObject(setting.Select(kv => new JProperty(kv.Key, JToken.FromObject(kv.Value))))));

            // Convert the JSON object to a JSON string
            string json = jsonObject.ToString(Formatting.Indented);
            Debug.Log(json);

            // Write the JSON string back to the file
            string filePath = Path.Combine(Application.persistentDataPath, jsonFile + ".json");
            File.WriteAllText(filePath, json);

            Debug.Log("Settings saved successfully");
        }
        else
        {
            Debug.LogWarning("No settings to save");
        }
    }

    /// <summary>
    /// update one of the locally stored settings
    /// </summary>
    /// <param name="settingName">the name of the setting to update</param>
    /// <param name="value"> the new value</param>
    public void UpdateSetting(string settingName, object value)
    {

        //find the position of the relevent setting
        int foundIndex = settingsList.FindIndex(entry =>
        {
            return entry["name"].ToString() == settingName;
        });

        if (foundIndex != -1)
        {
            // Update the value if the setting is found
            settingsList[foundIndex]["value"] = value;
            Debug.Log($"{settingName} updated with value {value.ToString()}");

        }
        else
        {
            Debug.LogWarning($"Setting with name '{settingName}' not found");
        }
    }

    public bool isSetup()
    {
        return isSetupCompleted;
    }
}