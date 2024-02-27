using TMPro;
using UnityEngine;

public class PrefabFloorSelection : MonoBehaviour
{
    public TextMeshProUGUI label;
    private PrefabListProxyManager ProxyManager;
    private int floorNumber = -1;

    public void Setup(int _floorNumber, PrefabListProxyManager proxyManager) {
        ProxyManager = proxyManager;
        label.text = _floorNumber.ToString(); //set label
        this.floorNumber = _floorNumber;
    }

    public void PushFloorChange()
    {
        Debug.Log("PrefabFloorSelection::PushFloorChange -> pushing new target floor");
        ProxyManager.setTargetFloor(floorNumber);
    }
}
