using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class BuildingLoader : MonoBehaviour
{
    public string buildingFilename;
    public BuildingPicker buildingPicker;

    // Start is called before the first frame update
    void Start()
    {
        JObject jsonObject = null;

        TextAsset jsonTextAsset = Resources.Load<TextAsset>(buildingFilename); // Load in the JSON file as a TextAsset
        if (jsonTextAsset != null) //check if the data has been read correctly
        {
            jsonObject = JObject.Parse(jsonTextAsset.text); // Parse the text as a json object
        }
        else
        {
            Debug.LogError("BuildingLoader::Start -> " + buildingFilename + " file could not be loaded!");
            return;
        }

        JArray buildingsArray = (JArray)jsonObject["buildings"]; // get the array of settings as json objects

        if (buildingsArray != null) // if the json is successfully read in
        {
            Debug.Log("BuildingLoader::Start -> buildings successfully parsed.");

            List<string> buildings = new List<string>();
            foreach (JObject buildingObject in buildingsArray)
            {
                Debug.Log("BuildingLoader::Start -> adding " + buildingObject["buildingName"]?.ToString() + " to list.");
                buildings.Add( buildingObject["buildingName"]?.ToString());
            }

            //Pass list to building Picker script
            buildingPicker.Setup(buildings);
        }
        else
        {
            Debug.LogError("BuildingLoader::Start -> Failed to parse buildings data");
        }
    }
}
