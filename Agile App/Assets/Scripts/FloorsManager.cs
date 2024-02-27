using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FloorsManager : MonoBehaviour
{

    // TODO: Find building floor prefabs, save target building in playerPrefs? store building-floor mapping in json. 

    public string jsonFile; //file name in Resources folder
    public InputManager inputManager;
    public PrefabListProxyManager floorsListManager;
    
    public float spawnOffset;
    public float spawnRate; //how fast the model is spawned and deleted

    private string currentBuilding; // change to null when floor is pulled from playerprefs.
    private List<string> PrefabNames = new List<string>(); //this should be ordered by floor, ground floor idx 0.

    private List<GameObject> floors = new List<GameObject>(); //holds all floors currently made
    private KeyValuePair<GameObject, float> transitioningObject = new KeyValuePair<GameObject, float>(); //used to hold current moving floor

    enum buildingState
    {
        AddingFloor,
        DeletingFloor,
        Nothing
    }

    private buildingState transitioningDirection = buildingState.Nothing;

    private int currentFloor = -1;

    private float floorHeight = 0.07f;

    // Start is called before the first frame update
    void Start()
    {
        //get the current building
        currentBuilding = PlayerPrefs.GetString("building");
        Debug.Log("FloorsManager::Start -> Current Building set to " + PlayerPrefs.GetString("building"));

        //load target prefab names from json
        LoadFloorNames();

        inputManager.onUpdate += UpdateModelPositions; // add UpdateModelPositions as a listener for the model positions

        //create floor selection list objects
        floorsListManager.GenerateList(PrefabNames.Count);

    }

    // Update is called once per frame
    void Update()
    {
        if (!inputManager.GetRaycastFlag()) return;  // Base case: checks to see if the user has at least one raycast

        if (transitioningDirection == buildingState.Nothing) // if there's no active floor
        {
            if (currentFloor < floorsListManager.getTargetFloor() && currentFloor < PrefabNames.Count-1) // if we need to add more floors, and there are more floors to add
            {
                transitioningDirection = buildingState.AddingFloor; // transition to the adding floor state

                GameObject prefab = null;
                try
                {
                    prefab = Resources.Load<GameObject>("Prefabs/" + PrefabNames[currentFloor + 1]); // load the next floor as a prefab
                }
                catch
                {
                    Debug.LogError("FloorsManager::Update -> failed to load " + PrefabNames[currentFloor + 1] + " as a prefab");
                }

                // Check if the prefab is loaded successfully
                if (prefab != null)
                {
                    Debug.Log("FloorsManager::Update -> successfully loaded " + PrefabNames[currentFloor + 1] + " as a prefab");
                    transitioningObject = new KeyValuePair<GameObject, float>(Instantiate(prefab), spawnOffset);
                }
                else
                {
                    // Handle the case where the prefab could not be loaded
                    Debug.Log("FloorsManager::Update -> defaulting to basic floor model");
                    prefab = Resources.Load<GameObject>("Prefabs/" + "defaultFloor"); // load the next floor as a prefab
                    transitioningObject = new KeyValuePair<GameObject, float>(Instantiate(prefab), spawnOffset);
                }
                UpdateTransitioningObject();
            }
            else if (currentFloor > floorsListManager.getTargetFloor()) // if we need to remove floors
            {
                Debug.Log("FloorsManager::Update -> need to delete a floor, moving last floor to transitioningObject");

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
                    float modifiedValueDown =
                        transitioningObject.Value - spawnRate * Time.deltaTime; //calculate new position

                    if (modifiedValueDown <= 0) //if the model is at the target position
                    {
                        Debug.Log("FloorsManager::Update -> floor is at target, adding to stack...");

                        currentFloor++;
                        transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, 0); //updates the object with the new offset
                        UpdateTransitioningObject();

                        //move floor onto stack
                        floors.Add(transitioningObject.Key);
                        transitioningObject = new KeyValuePair<GameObject, float>();
                        ;

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
                    float modifiedValueUp = transitioningObject.Value + spawnRate * Time.deltaTime; //calculate new position

                    if (modifiedValueUp >= spawnOffset) //if the model is at the target position
                    {
                        Debug.Log("FloorsManager::Update -> floor is at target, deleting floor");

                        currentFloor--;

                        //delete floor
                        GameObject.Destroy(transitioningObject.Key);

                        //go to idle
                        transitioningDirection = buildingState.Nothing;
                    }
                    else
                    {
                        //move model down
                        transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, modifiedValueUp); UpdateTransitioningObject();
                    }
                    break;
            }
        }
    }

    // check if raycast postion has changed and move stack
    private void UpdateModelPositions()
    {
        // get position, scale and rotation from input manager
        Vector3 raycastPos = inputManager.GetPos();
        float raycastRotation = inputManager.GetRotation();
        float raycastScale = inputManager.GetScale();

        //calculate new stationary floor positions
        for (int i = 0; i < floors.Count; i++)
        {
            Vector3 newPos = raycastPos;
            newPos.y += i * floorHeight;

            floors[i].transform.position = newPos;
        }

        // is there is a floor moving
        if (transitioningDirection != buildingState.Nothing) 
        {
            Vector3 newPos = raycastPos;
            newPos.y += floors.Count * floorHeight + transitioningObject.Value;

            transitioningObject.Key.transform.position = newPos;
        }
    }

    private void UpdateTransitioningObject()
    {
        // get position, scale and rotation from input manager
        Vector3 pos = inputManager.GetPos();
        float rotation = inputManager.GetRotation();
        float scale = inputManager.GetScale();

        pos.y += floors.Count * floorHeight + transitioningObject.Value; // add offset
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
            Debug.LogError("FloorsManager::LoadFloorNames -> " + jsonFile + " file could not be loaded!");
            return;
        }

        JArray buildingsArray = (JArray)jsonObject["buildings"]; // get the array of settings as json objects

        if (buildingsArray != null) // if the json is successfully read in
        {
            Debug.Log("FloorsManager::LoadFloorNames -> buildings successfully parsed!");
            foreach (JObject buildingObject in buildingsArray)
            {
                if (buildingObject["buildingName"]?.ToString() == currentBuilding)
                {
                    Debug.Log("FloorsManager::LoadFloorNames -> " + currentBuilding + " has been found.");

                    JArray floorsArray = (JArray)buildingObject["floors"];

                    foreach (JObject floor in floorsArray)
                    {
                        Debug.Log("FloorsManager::LoadFloorNames -> " + floor["floorPrefabName"]?.ToString());
                        PrefabNames.Add(floor["floorPrefabName"]?.ToString());
                    }
                }

                if (PrefabNames.Count == 0)  // if there were no floors or building found
                {
                    Debug.LogWarning("FloorsManager::LoadFloorNames -> Failed to find floors or building!");
                }
            }
        }
        else
        {
            Debug.LogError("FloorsManager::LoadFloorNames -> Failed to parse buildings data");
        }
    }
}
