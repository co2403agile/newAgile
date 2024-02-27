using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UniqueBuildingOption : MonoBehaviour
{
    public TextMeshProUGUI label;
    private BuildingPicker buildingPicker;

    public void Setup(string building, BuildingPicker proxyManager)
    {
        buildingPicker = proxyManager;
        label.text = building.ToString(); //set label
    }

    public void PushBuildingChange()
    {
        Debug.Log("UniqueBuildingOption::PushBuildingChange -> attempting to switch target building to " + label.text);
        buildingPicker.setTargetBuilding(label.text);
    }
}
