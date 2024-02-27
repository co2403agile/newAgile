using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPicker : MonoBehaviour
{
    public GameObject prefab; // Prefab for the UI element
    public Transform contentParent; // content Obj

    public void Setup(List<string> _buildings)
    {
        //Go through buildings and create prefabs
        foreach (var building in _buildings)
        {
            // Instantiate UI element from prefab and set parent
            GameObject element = Instantiate(prefab, contentParent);

            //call the setup routine with values
            element.GetComponentInChildren<UniqueBuildingOption>().Setup(building, this);
            Debug.Log("BuildingPicker::Setup -> created " + building);
        }
    }

    public void setTargetBuilding(string _building)
    {
        // Set the current building using playerPrefs
        PlayerPrefs.SetString("building", _building);
        Debug.Log("BuildingPicker::setTargetBuilding -> building successfully set to " + _building);

    }

}
