using System;
using TMPro;
using UnityEngine;

/* TextMeshManager: Manages the appearance of a TextMeshProUGUI component based on settings. */
/* Written by James */

public class TextMeshManager : MonoBehaviour
{
    private SettingsManager settingsManager; // Reference to the SettingsManager script
    public TextMeshProUGUI text; // Reference to the TextMeshProUGUI component to manage
    private float biggerTextPercent = 1.2f; // Percentage to increase text size if 'bigger text' option is enabled

    /* Start: called before the first frame update */
    void Start()
    {
        
        /* Find the SettingsManager object */
        settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
        if (settingsManager == null) throw new InvalidOperationException("TextMeshManager::Start -> Couldn't Find a SettingsManager");

        /* Schedule or call UpdateText based on setup completion */
        if (settingsManager.isSetup()) UpdateText();
        else settingsManager.OnSetupCompleted += UpdateText;
    }


    /* UpdateText: Updates the appearance of the TextMeshProUGUI component based on settings. */
    public void UpdateText()
    {
        Debug.Log("TextMeshManager::UpdateText -> Called");

        /* Check and set text to bold based on the 'bold text' setting */
        int isBold = Convert.ToInt32(settingsManager.GetOption("bold text")["value"]);
        if (isBold == 1) text.fontStyle = FontStyles.Bold;

        /* Check and increase text size if the 'bigger text' setting is enabled */
        int isBigText = Convert.ToInt32(settingsManager.GetOption("bigger text")["value"]);
        if (isBigText == 1) text.fontSizeMax *= biggerTextPercent;
    }
}