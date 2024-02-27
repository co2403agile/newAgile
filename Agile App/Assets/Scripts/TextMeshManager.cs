using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMeshManager : MonoBehaviour
{
    private SettingsManager settingsManager;
    public TextMeshProUGUI text;

    private float biggerTextPercent = 1.2f;

    void Start()
    {
        settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>(); //finds settingsManager object
        if (settingsManager == null) throw new InvalidOperationException("TextMeshManager::Start -> Couldn't Find a SettingsManager");

        //schedule or call UpdateText
        if (settingsManager.isSetup()) UpdateText();
        else settingsManager.OnSetupCompleted += UpdateText;
    }

    public void UpdateText()
    {
        Debug.Log("TextMeshManager::UpdateText -> Called");

        // check and set text to bold
        int isBold = Convert.ToInt32(settingsManager.GetOption("bold text")["value"]);
        if (isBold == 1) text.fontStyle = FontStyles.Bold;

        // check and set text to bigger size
        int isBigText = Convert.ToInt32(settingsManager.GetOption("bigger text")["value"]);
        if (isBigText == 1) text.fontSizeMax *= biggerTextPercent;
    }
}
