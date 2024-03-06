using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* ToggleOption: Manages the appearance and behavior of a toggle switch UI element for setting options. */
/*Written by James */

public class ToggleOption : MonoBehaviour
{
    public Toggle toggle; // Reference to the Toggle component
    public TextMeshProUGUI label; // Reference to the TextMeshProUGUI component for the label
    public Image backgroundPanel; // Reference to the Image component for the background panel
    private SettingsManager manager; // Reference to the SettingsManager script


    /* Setup: Initializes the toggle switch with the provided initial values. */
    public void Setup(KeyValuePair<string, int> init)
    {
        /* Find the SettingsManager object */
        manager = FindObjectOfType<SettingsManager>(); 

        /* Set the label text */
        label.text = init.Key; 

        /* Set the toggle state based on the initial value (currently only manages boolean settings) */
        toggle.isOn = Convert.ToBoolean(init.Value);

        /* Add a listener to the toggle's value changed event */
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        /* Call the value changed event handler initially */
        OnToggleValueChanged(toggle.isOn);
    }

    /* OnToggleValueChanged: Handles the value changed event of the toggle switch. */
    void OnToggleValueChanged(bool isOn)
    {
        /* Update the setting value in the SettingsManager */
        manager.UpdateSetting(label.text, (object)Convert.ToInt32(isOn));

        /* Change the background panel color based on the toggle state */
        backgroundPanel.color = isOn ? Color.green : Color.red;
    }
}