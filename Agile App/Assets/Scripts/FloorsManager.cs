using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FloorsManager : MonoBehaviour
{
    // TODO: Find building floor prefabs, save target building in playerPrefs? store building-floor mapping in json.
    // TODO: get position, scale and rotation from input manager
    // TODO: get current floors to be shown (NEEDS TESTING)
    // TODO: animate the tiering

    public string jsonFile; //file name in Resources folder
    public InputManager inputManager;
    public float spawnOffset;
    public float spawnRate; //how fast the model is spawned and deleted

    private string currentBuilding = "Library"; // TODO: change to null when floor is pulled from playerprefs.
    private List<string> PrefabNames = new List<string>(); //this should be ordered by floor, ground floor idx 0.

    private List<GameObject> floors = new List<GameObject>(); //holds all floors currently made

    private KeyValuePair<GameObject, float> transitioningObject = new KeyValuePair<GameObject, float>(); //used to hold current moving floor
    private int transitioningDirection = 0; // 0 : floor not moving, 1 : floor adding, -1 : floor deleting

    private int currentFloor = -1;
    private int targetFloor = 0;

    private float floorHeight = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        //load target prefab names from json
        LoadFloorNames();

        //create floor selection list objects


    }

    // Update is called once per frame
    void Update()
    {
        // get position, scale and rotation from input manager
        Vector3 pos = inputManager.GetPos();
        float rotation = inputManager.GetRotation();
        float scale = inputManager.GetScale();

        //TODO: manage spawning of floors

        if (transitioningDirection == 0) //if there's no active floor
        {
            if (currentFloor != targetFloor)
            {
                // TODO: spawn

            }
        }
        else
        {
            switch (transitioningDirection)
            {
                case 1: // move offset down
                    float modifiedValueDown = Mathf.Max(0, transitioningObject.Value - spawnRate * Time.deltaTime);
                    transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, modifiedValueDown);

                    //TODO: check if floor is low enough to be added to stack

                    break;

                case -1: //move offset up
                    float modifiedValueUp = Mathf.Max(0, transitioningObject.Value - spawnRate * Time.deltaTime);
                    transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, modifiedValueUp);

                    //TODO: check if floor is high enough to be deleted
                    

                    break;
            }
        }

        // TODO: move model(s) to position
    }

    private void LoadFloorNames()
    {
        JObject jsonObject = null;

        TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFile); // Load in the JSON file as a TextAsset
        if (jsonTextAsset != null) //check if the data has been read correctly
        {
            jsonObject = JObject.Parse(jsonTextAsset.text); // Parse the text as a json object
        }
        else
        {
            Debug.LogError(jsonFile + " file could not be loaded!");
            return;
        }

        JArray buildingsArray = (JArray)jsonObject["buildings"]; // get the array of settings as json objects

        if (buildingsArray != null) // if the json is successfully read in
        {
            Debug.Log("buildings successfully parsed!");
            foreach (JObject buildingObject in buildingsArray)
            {
                if (buildingObject["buildingName"].ToString() == currentBuilding)
                {
                    JArray floorsArray = (JArray)buildingObject["floors"];
                    foreach (JObject floor in floorsArray)
                    {
                        PrefabNames.Add(floor["floorPrefabName"].ToString());
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Failed to parse settings data");
        }
    }
}
