using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FloorsManager : MonoBehaviour
{
    /* Name of the JSON file containing building and floor information */
    public string jsonFile;

    /* Reference to the InputManager script for handling user input */
    public InputManager inputManager;

    /* Reference to the PrefabListProxyManager script for managing the list of floor prefabs */
    public PrefabListProxyManager floorsListManager;

    /* Offset for spawning floors */
    public float spawnOffset;

    /* Rate at which the model is spawned and deleted */
    public float spawnRate;

    /* Current building selected by the player */
    private string currentBuilding;

    /* List of prefab names for floors */
    private List<string> PrefabNames = new List<string>();

    /* List of instantiated floor GameObjects */
    private List<GameObject> floors = new List<GameObject>();

    /* KeyValuePair containing a GameObject and its current transition offset */
    private KeyValuePair<GameObject, float> transitioningObject = new KeyValuePair<GameObject, float>();

    /* Enum representing the state of the building (AddingFloor, DeletingFloor, Nothing) */
    private enum BuildingState
    {
        AddingFloor,
        DeletingFloor,
        Nothing
    }

    /* Current direction of transition for the building */
    private BuildingState transitioningDirection = BuildingState.Nothing;

    /* Current floor index */
    private int currentFloor = -1;

    /* Height of each floor */
    private float floorHeight = 0.07f;

    /* Start: called before the first frame update */
    void Start()
    {
        /* Get the current building from PlayerPrefs */
        currentBuilding = PlayerPrefs.GetString("building");
        Debug.Log("FloorsManager::Start -> Current Building set to " + PlayerPrefs.GetString("building"));

        /* Load floor names from JSON */
        LoadFloorNames();

        /* Add UpdateModelPositions as a listener for the model positions */
        inputManager.onUpdate += UpdateModelPositions;

        /* Generate floor selection list objects */
        floorsListManager.GenerateList(PrefabNames.Count);
    }

    /* Update: called once per frame */
    void Update()
    {
        /* Base case: checks if the user has at least one raycast */
        if (!inputManager.GetRaycastFlag()) return;

        /* If there's no active floor transition */
        if (transitioningDirection == BuildingState.Nothing)
        {
            /* If we need to add more floors and there are more floors to add */
            if (currentFloor < floorsListManager.getTargetFloor() && currentFloor < PrefabNames.Count - 1)
            {
                /* Transition to adding floor state */
                transitioningDirection = BuildingState.AddingFloor;

                GameObject prefab = null;
                try
                {
                    /* Load the next floor prefab */
                    prefab = Resources.Load<GameObject>("Prefabs/" + PrefabNames[currentFloor + 1]);
                }
                catch
                {
                    Debug.LogError("FloorsManager::Update -> Failed to load " + PrefabNames[currentFloor + 1] + " as a prefab");
                }

                /* Check if the prefab is loaded successfully */
                if (prefab != null)
                {
                    Debug.Log("FloorsManager::Update -> Successfully loaded " + PrefabNames[currentFloor + 1] + " as a prefab");
                    transitioningObject = new KeyValuePair<GameObject, float>(Instantiate(prefab), spawnOffset);
                }
                else
                {
                    /* Handle the case where the prefab could not be loaded */
                    Debug.Log("FloorsManager::Update -> Defaulting to basic floor model");
                    prefab = Resources.Load<GameObject>("Prefabs/" + "defaultFloor");
                    transitioningObject = new KeyValuePair<GameObject, float>(Instantiate(prefab), spawnOffset);
                }
                UpdateTransitioningObject();
            }

            /* If we need to remove floors */
            else if (currentFloor > floorsListManager.getTargetFloor())
            {
                Debug.Log("FloorsManager::Update -> Need to delete a floor, moving last floor to transitioningObject");

                /* Transition to removing floor state */
                transitioningDirection = BuildingState.DeletingFloor;

                /* Get the index of the last item in the list */
                int lastIndex = floors.Count - 1;

                /* Move game object to the transitioning object state */
                transitioningObject = new KeyValuePair<GameObject, float>(floors[lastIndex], 0);

                /* Remove the last item from the list */
                floors.RemoveAt(lastIndex);
            }
        }
        /* If we're transitioning a floor in or out */
        else
        {
            switch (transitioningDirection)
            {

                case BuildingState.AddingFloor:
                    /* Move offset down */
                    float modifiedValueDown = transitioningObject.Value - spawnRate * Time.deltaTime;

                    /* If the model is at the target position */
                    if (modifiedValueDown <= 0)
                    {
                        Debug.Log("FloorsManager::Update -> Floor is at target, adding to stack...");

                        /* Update the transitioning object with the new offset */
                        transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, 0);
                        UpdateTransitioningObject();

                        /* Move floor onto stack */
                        floors.Add(transitioningObject.Key);
                        transitioningObject = new KeyValuePair<GameObject, float>();

                        /* Go to idle state */
                        transitioningDirection = BuildingState.Nothing;
                    }
                    else
                    {
                        /* Move model down */
                        transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, modifiedValueDown);
                        UpdateTransitioningObject();
                    }
                    break;

                case BuildingState.DeletingFloor: 
                    /* Move offset up */
                    float modifiedValueUp = transitioningObject.Value + spawnRate * Time.deltaTime;

                    /* If the model is at the target position */
                    if (modifiedValueUp >= spawnOffset)
                    {
                        Debug.Log("FloorsManager::Update -> Floor is at target, deleting floor");

                        /* Decrement current floor */
                        currentFloor--;

                        /* Delete floor */
                        GameObject.Destroy(transitioningObject.Key);

                        /* Go to idle state */
                        transitioningDirection = BuildingState.Nothing;
                    }
                    else
                    {
                        /* Move model up */
                        transitioningObject = new KeyValuePair<GameObject, float>(transitioningObject.Key, modifiedValueUp);
                        UpdateTransitioningObject();
                    }
                    break;
            }
        }
    }


    /* UpdateModelPositions: Check if raycast postion has changed and move stack */
    private void UpdateModelPositions()
    {
        /* Get position, scale and rotation from input manager */
        Vector3 raycastPos = inputManager.GetPos();
        float raycastRotation = inputManager.GetRotation();
        float raycastScale = inputManager.GetScale();

        /* Calculate new stationary floor positions */
        for (int i = 0; i < floors.Count; i++)
        {
            Vector3 newPos = raycastPos;
            newPos.y += i * floorHeight;

            floors[i].transform.position = newPos;
        }

        /* Is there is a floor moving */
        if (transitioningDirection != BuildingState.Nothing) 
        {
            Vector3 newPos = raycastPos;
            newPos.y += floors.Count * floorHeight + transitioningObject.Value;

            transitioningObject.Key.transform.position = newPos;
        }
    }

    private void UpdateTransitioningObject()
    {
        /* get position, scale and rotation from input manager */
        Vector3 pos = inputManager.GetPos();
        float rotation = inputManager.GetRotation();
        float scale = inputManager.GetScale();

        /* Add offset */
        pos.y += floors.Count * floorHeight + transitioningObject.Value; 
        transitioningObject.Key.transform.position = pos;
    }

    /* LoadFloorNames: Loads floor names from the JSON file. */
    private void LoadFloorNames()
    {
        JObject jsonObject = null;

        /* Load the JSON file as a TextAsset */
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFile);

        /* Check if the JSON data has been read correctly */
        if (jsonTextAsset != null)
        {
            /* Parse the JSON text as a JObject */
            jsonObject = JObject.Parse(jsonTextAsset.text);
        }
        else
        {
            Debug.LogError("FloorsManager::LoadFloorNames -> Failed to load JSON file: " + jsonFile);
            return;
        }

        /* Get the array of buildings from the JSON object */
        JArray buildingsArray = (JArray)jsonObject["buildings"];

        /* Check if the JSON data contains building information */
        if (buildingsArray != null)
        {
            Debug.Log("FloorsManager::LoadFloorNames -> Buildings successfully parsed!");

            /* Iterate through each building object in the JSON array */
            foreach (JObject buildingObject in buildingsArray)
            {
                /* Check if the building name matches the current building */
                if (buildingObject["buildingName"]?.ToString() == currentBuilding)
                {
                    Debug.Log("FloorsManager::LoadFloorNames -> Found building: " + currentBuilding);

                    /* Get the array of floors from the building object */
                    JArray floorsArray = (JArray)buildingObject["floors"];

                    /* Iterate through each floor object in the JSON array */
                    foreach (JObject floor in floorsArray)
                    {
                        /* Get the name of the floor prefab and add it to the list */
                        string floorPrefabName = floor["floorPrefabName"]?.ToString();
                        Debug.Log("FloorsManager::LoadFloorNames -> Adding floor: " + floorPrefabName);
                        PrefabNames.Add(floorPrefabName);
                    }
                }

                /* If no floors or building found, issue a warning */
                if (PrefabNames.Count == 0)
                {
                    Debug.LogWarning("FloorsManager::LoadFloorNames -> No floors or building found!");
                }
            }
        }
        else
        {
            Debug.LogError("FloorsManager::LoadFloorNames -> Failed to parse building data from JSON");
        }
    }

}
