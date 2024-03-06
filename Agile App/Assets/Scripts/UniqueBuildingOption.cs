using TMPro;
using UnityEngine;

/* UniqueBuildingOption: Manages the behavior of a unique building option in the user interface. */
/* Written by James */

public class UniqueBuildingOption : MonoBehaviour
{
    public TextMeshProUGUI label; // Reference to the TextMeshProUGUI component for the label
    private BuildingPicker buildingPicker; // Reference to the BuildingPicker script


    /* Setup: Initializes the unique building option with the specified building name and BuildingPicker reference. */
    public void Setup(string building, BuildingPicker proxyManager)
    {
        buildingPicker = proxyManager;
        label.text = building.ToString();
    }

    /* PushBuildingChange: Attempts to switch the target building to the one associated with this option. */
    public void PushBuildingChange()
    {
        Debug.Log("UniqueBuildingOption::PushBuildingChange -> Attempting to switch target building to " + label.text);

        /* Call the setTargetBuilding method in the BuildingPicker script */
        buildingPicker.setTargetBuilding(label.text); 
    }
}