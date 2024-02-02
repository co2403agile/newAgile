using System.Collections.Generic;
using UnityEngine;

public class PrefabListManager : MonoBehaviour
{
    public GameObject prefab; // Prefab for the UI element
    public Transform contentParent; // content Obj


    private void Start()
    {
        //default view
        List<object> list = new List<object>();
        list.Add("test");
        list.Add("test2");
        list.Add("test3");
        list.Add("test4");
        GenerateList(list);
    }

    public void GenerateList(List<object> objList)
    {
        // Destroy existing children
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
            Debug.Log("element destroyed");

        }

        Debug.Log(objList.Count);

        if (objList.Count > 0) //check if we have data to create objects out of
        {
            foreach (object obj in objList)
            {
                Debug.Log("creating prefab");

                // Instantiate UI element from prefab without setting parent
                GameObject element = Instantiate(prefab);

                // Set the parent after instantiation
                element.transform.SetParent(contentParent);
                
                //call the setup routine with values
                element.GetComponentInChildren<AbstractPrefab>().Setup(obj);
            }
        }
        else
        {
            Debug.LogError("nothing was passed");
        }
    }
}
