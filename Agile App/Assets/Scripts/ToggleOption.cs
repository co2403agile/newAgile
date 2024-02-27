using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOption : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI label;
    public Image backgroundPanel;

    private SettingsManager manager;
    
    public void Setup(KeyValuePair<string, int> init)
    {
        manager = FindObjectOfType<SettingsManager>(); //find the setting manager

        label.text = init.Key; //sets label

        //currently only manages boolean settings
        toggle.isOn = Convert.ToBoolean(init.Value); //sets state of switch

        toggle.onValueChanged.AddListener(OnToggleValueChanged); //creates the observer
        OnToggleValueChanged(toggle.isOn);

    }

    //changes color based on the state
    void OnToggleValueChanged(bool isOn)
    {
        manager.UpdateSetting(label.text, (object)Convert.ToInt32(isOn));
        if (isOn)
        {
            backgroundPanel.color = Color.green;
        }
        else
        {
            backgroundPanel.color = Color.red;
        }
    }

}
