using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabListProxyManager : MonoBehaviour
{
    public GameObject prefab; // Prefab for the UI element
    public Transform contentParent; // content Obj

    private int numberOfFloors = 0;
    private int targetFloor = 0;

    public void GenerateList(int newNumFloors)
    {
        Debug.Log("PrefabListProxyManager::GenerateList -> called with " + newNumFloors + " floors.");

        targetFloor = 0;  // reset target to ground floor
        if (newNumFloors == numberOfFloors) return;  // Base Case: the new list is the same size as the old

        // Destroy existing children
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
            Debug.Log("PrefabListProxyManager::GenerateList -> prefab destroyed");
        }

        for (int i = 0; i < newNumFloors; i++)
        {
            // Instantiate UI element from prefab without setting parent
            GameObject element = Instantiate(prefab);

            // Set the parent after instantiation
            element.transform.SetParent(contentParent);

            //call the setup routine with values
            element.GetComponentInChildren<PrefabFloorSelection>().Setup(i, this);
        }
    }

    public int getTargetFloor()
    {
        return targetFloor;
    }

    public void setTargetFloor(int floor)
    {
        targetFloor = floor;
    }
}

