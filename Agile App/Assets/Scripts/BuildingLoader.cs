using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class BuildingLoader : MonoBehaviour
{
    public string buildingFilename;  // The filename of the JSON file containing building data
    public BuildingPicker buildingPicker;  // Reference to the BuildingPicker script to pass the loaded building data


    /* Start is called before the first frame update */
    void Start()
    {
        /* Initialize JSON object */
        JObject jsonObject = null;

        /* Load the JSON file as a TextAsset */
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(buildingFilename);

        /* Check if the data has been read correctly */
        if (jsonTextAsset != null)
        {
            /* Parse the text as a JSON object */
            jsonObject = JObject.Parse(jsonTextAsset.text);
        }
        else
        {
            /* Log error if the file could not be loaded */
            Debug.LogError("BuildingLoader::Start -> " + buildingFilename + " file could not be loaded!");
            return;
        }

        /* Get the array of building settings as JSON objects */
        JArray buildingsArray = (JArray)jsonObject["buildings"];

        /* Check if the JSON is successfully read */
        if (buildingsArray != null)
        {
            /* Log success message */
            Debug.Log("BuildingLoader::Start -> Buildings successfully parsed.");

            /* Create a list to store building names */
            List<string> buildings = new List<string>();

            /* Iterate through each building object in the array */
            foreach (JObject buildingObject in buildingsArray)
            {
                /* Log adding each building to the list */
                Debug.Log("BuildingLoader::Start -> Adding " + buildingObject["buildingName"]?.ToString() + " to the list.");

                /* Add the building name to the list */
                buildings.Add(buildingObject["buildingName"]?.ToString());
            }

            /* Pass the list of building names to the BuildingPicker script */
            buildingPicker.Setup(buildings);
        }
        else
        {
            /* Log error if failed to parse building data */
            Debug.LogError("BuildingLoader::Start -> Failed to parse buildings data");
        }
    }
}
