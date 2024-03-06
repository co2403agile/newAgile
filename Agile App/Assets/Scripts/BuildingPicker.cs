using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPicker : MonoBehaviour
{
    public GameObject prefab; // The prefab representing the UI element for buildings
    public Transform contentParent; // The parent object in which instantiated UI elements will reside

    /* Method to set up the UI with building options */
    public void Setup(List<string> _buildings)
    {
        /* Iterate through the list of buildings and create UI elements */
        foreach (var building in _buildings)
        {
            /* Instantiate the UI element from the prefab and set its parent */
            GameObject element = Instantiate(prefab, contentParent);

            /* Call the setup routine of the UniqueBuildingOption script attached to the UI element */
            element.GetComponentInChildren<UniqueBuildingOption>().Setup(building, this);

            /* Log the creation of the building UI element */
            Debug.Log("BuildingPicker::Setup -> Created " + building);
        }
    }

    /* Method to set the target building */
    public void setTargetBuilding(string _building)
    {
        /* Set the current building using PlayerPrefs */
        PlayerPrefs.SetString("building", _building);

        /* Log the successful setting of the building */
        Debug.Log("BuildingPicker::setTargetBuilding -> Building successfully set to " + _building);
    }
}