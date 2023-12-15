using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using TMPro;
using System.IO;

public class OptionsMenu : MonoBehaviour
{
    public GameObject elementPrefab; // Prefab for the UI element
    public Transform contentParent;   // content Obj

    void Start()
    {
        //create list on startup
        PopulateOptions();
    }

    void PopulateOptions()
    {
        List<KeyValuePair<string, int>> allPlayerPrefs = GetAllPlayerPrefs();

        foreach (var pair in allPlayerPrefs)
        {
            
            // Instantiate UI element from prefab without setting parent
            GameObject element = Instantiate(elementPrefab);

            // Set the parent after instantiation
            element.transform.SetParent(contentParent);

            // Customize the UI element based on PlayerPrefs data (e.g., set text)
            element.GetComponent<ToggleOption>().Setup(pair);
        }
    }


    //this function is to access the saved settings
    List<KeyValuePair<string, int>> GetAllPlayerPrefs()
    {
        string dbName = Path.Combine(Application.persistentDataPath, "settings.db");
        List<KeyValuePair<string, int>> Optionslist = new List<KeyValuePair<string, int>>();

        //read from db
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM test;";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Optionslist.Add(new KeyValuePair<string, int>(Convert.ToString(reader["key"]), Convert.ToInt32(reader["value"])));
                    }
                }
            }
            connection.Close();
        }

        foreach (var keyValuePair in Optionslist)
        {
            Debug.Log("key: " + keyValuePair.Key + "\tvalue: " + keyValuePair.Value);
        }

        return Optionslist;
    }
}