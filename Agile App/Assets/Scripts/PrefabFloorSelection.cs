using TMPro;
using UnityEngine;

/* Script for managing prefab floor selection */
/* Written by James*/
public class PrefabFloorSelection : MonoBehaviour
{
    public TextMeshProUGUI label;  // Reference to the text label displaying the floor number
    private PrefabListProxyManager ProxyManager;  // Reference to the prefab list proxy manager
    private int floorNumber = -1;  // The floor number associated with this selection

    /* Setup the floor selection with initial values */
    public void Setup(int _floorNumber, PrefabListProxyManager proxyManager)
    {
        /* Assign the proxy manager */
        ProxyManager = proxyManager;
        
        /* Set the label to display the floor number */
        label.text = _floorNumber.ToString();
        
        /* Update the floor number */
        this.floorNumber = _floorNumber; 
    }

    /* PushFloorChange: Pushes the floor change to the prefab list proxy manager */
    public void PushFloorChange()
    {
        Debug.Log("PrefabFloorSelection::PushFloorChange -> Pushing new target floor: " + floorNumber);

        /* Notify the proxy manager about the floor change */
        ProxyManager.setTargetFloor(floorNumber);
    }
}