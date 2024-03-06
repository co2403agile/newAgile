using UnityEngine;

/* Manages the list of prefabs representing floors */
/* Written by James */
public class PrefabListProxyManager : MonoBehaviour
{
    public GameObject prefab; // Prefab for the UI element
    public Transform contentParent; // Parent object to hold instantiated prefabs

    private int numberOfFloors = 0; // Number of floors in the list
    private int targetFloor = 0; // The target floor index

    /* Generates the list of floor prefabs */
    public void GenerateList(int newNumFloors)
    {
        Debug.Log("PrefabListProxyManager::GenerateList -> Called with " + newNumFloors + " floors.");
        
        /* Reset target to ground floor */
        targetFloor = 0; 

        /* Base Case: the new list is the same size as the old */
        if (newNumFloors == numberOfFloors) return; 

        /* Destroy existing children */
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
            Debug.Log("PrefabListProxyManager::GenerateList -> Prefab destroyed");
        }

        /* Instantiate new prefabs for each floor */
        for (int i = 0; i < newNumFloors; i++)
        {
            /* Instantiate UI element from prefab without setting parent */
            GameObject element = Instantiate(prefab);

            /* Set the parent after instantiation */
            element.transform.SetParent(contentParent);

            /* Call the setup routine with values */
            element.GetComponentInChildren<PrefabFloorSelection>().Setup(i, this);
        }
    }

    /* getTargetFloor: Returns the target floor index */
    public int getTargetFloor()
    {
        return targetFloor;
    }

    /* setTargetFloor: Sets the target floor index */
    public void setTargetFloor(int floor)
    {
        targetFloor = floor;
    }
}