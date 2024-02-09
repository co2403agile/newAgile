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




    enum buildingState
    {
        AddingFloor, DeletingFloor, Nothing
    }
    private buildingState transitioningDirection = buildingState.Nothing;

    private int currentFloor = -1;
    private int targetFloor = 0;


    private float floorHeight = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        //load target prefab names from json
        LoadFloorNames();

        //create floor selection list objects
        //TODO: create floor selection list objects

    }

    // Update is called once per frame
    void Update()
    {
        // TODO: check if raycast postion has changed and move stack




        if (transitioningDirection == buildingState.Nothing) //if there's no active floor
        {
            if (currentFloor < targetFloor) // if we need to add more floors
            {
                transitioningDirection = buildingState.AddingFloor; //transition to the adding floor state

                GameObject prefab = Resources.Load<GameObject>(PrefabNames[currentFloor + 1]); //load the next floor as a prefab

                // Check if the prefab is loaded successfully
                if (prefab != null)
                {
                    transitioningObject = new KeyValuePair<GameObject, float>(Instantiate(prefab), spawnOffset);
                }
                else
                {
                    //TODO: create prefab with default error asset
                    // Handle the case where the prefab could not be loaded

                }

                UpdateTransitioningObject();
            }
            else if (currentFloor > targetFloor) // if we need to remove floors
            {
                transitioningDirection = buildingState.DeletingFloor; //transition to the removing floor state

                // Get the index of the last item in the list
                int lastIndex = floors.Count - 1;

                // move game object to the transitioning object state
                transitioningObject = new KeyValuePair<GameObject, float>(floors[lastIndex], 0);

                // Remove the last item from the list
                floors.RemoveAt(lastIndex);
            }
        }
        else // if we're transitioning a floor in or out
        {
            switch (transitioningDirection)
            {
                case buildingState.AddingFloor: // move offset down
                    float modifiedValueDown = Mathf.Max(0, transitioningObject.Value - spawnRate * Time.deltaTime); //calculate new position

                    if (modifiedValueDown <= 0) //if the model is at the target position
                    {
                        currentFloor++;
                        transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, 0); //updates the object with the new offset
                        UpdateTransitioningObject();

                        //move floor onto stack
                        floors.Add(transitioningObject.Key);
                        transitioningObject = new KeyValuePair<GameObject, float>(); ;

                        //go to idle
                        transitioningDirection = buildingState.Nothing;
                    }
                    else
                    {
                        //move model down
                        transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, modifiedValueDown); //updates the object with the new offset
                        UpdateTransitioningObject();
                    }

                    break;

                case buildingState.DeletingFloor: //move offset up
                    float modifiedValueUp = Mathf.Max(0, transitioningObject.Value - spawnRate * Time.deltaTime); //calculate new position

                    if (modifiedValueUp >= spawnOffset) //if the model is at the target position
                    {
                        currentFloor--;

                        //delete floor
                        GameObject.Destroy(transitioningObject.Key);

                        //go to idle
                        transitioningDirection = buildingState.Nothing;
                    }
                    else
                    {
                        //move model down
                        transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, modifiedValueUp);
                    }

                    //Update transitioning model translation matrix to correct position
                    UpdateTransitioningObject();

                    break;
            }
        }
    }

    private void UpdateTransitioningObject()
    {
        // get position, scale and rotation from input manager
        Vector3 pos = inputManager.GetPos();
        float rotation = inputManager.GetRotation();
        float scale = inputManager.GetScale();

        pos.y += transitioningObject.Value; // add offset
        transitioningObject.Key.transform.position = pos;
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
