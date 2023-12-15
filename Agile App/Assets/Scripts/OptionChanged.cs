using TMPro;
using UnityEngine;
using UnityEngine.UI;

//written by James
//listens for changes on the toggle option and changes the colour
public class ToggleBackground : MonoBehaviour {
    public Toggle toggle;
    public Image backgroundPanel;

    void Start() {
        toggle.onValueChanged.AddListener(OnToggleValueChanged); //creates the observer
        OnToggleValueChanged(toggle.isOn); //calls initially to change to the correct color
    }
    
    //changes color based on the state
    void OnToggleValueChanged(bool isOn)  {
        if (isOn) backgroundPanel.color = Color.green;
        else backgroundPanel.color = Color.red;
    }
}