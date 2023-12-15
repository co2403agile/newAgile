using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOption : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI label;
    public void Setup(KeyValuePair<string, int> init)
    {
        label.text = init.Key; //sets label

        //currently only manages boolean settings
        toggle.isOn = Convert.ToBoolean(init.Value); //sets state of switch
    }
}
