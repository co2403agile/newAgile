using System;
using System.Collections.Generic;
using UnityEngine;

/* Script for managing setting prefabs */
/* Written by James */
public class ManageSettingPrefabs : MonoBehaviour
{
    /* Reference to the SettingsManager */
    private SettingsManager manager; 
    
    /* Prefab for the toggle UI element */
    public GameObject togglePrefab;

    /* Parent object to hold instantiated UI elements */
    public Transform contentParent;

    /* Start is called before the first frame update */
    void Start()
    {
        /* Find the SettingsManager in the scene */
        manager = FindObjectOfType<SettingsManager>();

        /* Check if the SettingsManager has been set up */
        /* If already set up, create prefab settings immediately */
        /* If not set up, wait for setup completion event */
        if (manager.isSetup() == true) createPrefabSettings();
        else manager.OnSetupCompleted += createPrefabSettings;
    }

    /* Create prefab settings based on loaded options */
    private void createPrefabSettings()
    {
        Debug.Log("ManageSettingPrefabs::createPrefabSettings -> Creating prefabs...");

        /* Retrieve list of settings from the SettingsManager */
        List<Dictionary<string, object>> settingsList = manager.GetOptions();

        /* Check if settings are available */
        if (settingsList.Count != 0)
        {
            /* Iterate through each setting */
            foreach (Dictionary<string, object> setting in settingsList)
            {
                /* Check setting type */
                switch (setting["type"].ToString())
                {
                    /* For toggle settings */
                    case "toggle": 
                        Debug.Log($"ManageSettingPrefabs::createPrefabSettings -> Creating toggle setting: {setting["name"]}, value: {Convert.ToInt32(setting["value"])}");

                        /* Instantiate UI element from toggle prefab */
                        GameObject element = Instantiate(togglePrefab);

                        /* Set the parent after instantiation */
                        element.transform.SetParent(contentParent);

                        /* Customize the UI element based on setting data */
                        element.GetComponent<ToggleOption>().Setup(new KeyValuePair<string, int>(setting["name"].ToString(), Convert.ToInt32(setting["value"])));

                        break;
                }
            }
        }
        else
        {
            /* Log error if no settings are found */
            Debug.LogError("ManageSettingPrefabs::createPrefabSettings -> No settings found");
        }
    }
}