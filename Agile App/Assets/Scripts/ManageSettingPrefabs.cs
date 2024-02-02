using System;
using System.Collections.Generic;
using UnityEngine;

//Written by James
public class ManageSettingPrefabs : MonoBehaviour
{
    private SettingsManager manager;
    public GameObject togglePrefab; // Prefab for the UI element
    public Transform contentParent; // content Obj

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<SettingsManager>(); //finds the settings manager, since there should only be one we don't need to pass it into the script
        
        if (manager.isSetup() == true) { // check if the settingsManager has already been setup
            createPrefabSettings();
        }
        else {
            manager.OnSetupCompleted += createPrefabSettings; //set a listener to wait till the settings have been loaded in
        }
    }

    private void createPrefabSettings()
    {
        Debug.Log("create prefabs called"); //output shared values
        List<Dictionary<string, object>> settingsList = manager.GetOptions();

        if (settingsList.Count != 0) //check if we have any settings
        {
            foreach (Dictionary<string, object> setting in settingsList)
            {
                switch (setting["type"].ToString())
                {
                    case "toggle":
                        Debug.Log($"creating toggle in state {Convert.ToInt32(setting["value"])}"); //output shared values

                        // Instantiate UI element from prefab without setting parent
                        GameObject element = Instantiate(togglePrefab);

                        // Set the parent after instantiation
                        element.transform.SetParent(contentParent);

                        // Customize the UI element based on PlayerPrefs data (e.g., set text)
                        element.GetComponent<ToggleOption>().Setup(new KeyValuePair<string, int>(setting["name"].ToString(), Convert.ToInt32(setting["value"])));

                        break;
                }
            }
        }
        else
        {
            Debug.LogError("no settings where found");
        }
    }
}
