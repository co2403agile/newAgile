using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBuildingScript : MonoBehaviour
{
    public void reset() {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Spawnable"); //gets current objects spawned
        foreach (GameObject obj in objectsWithTag) Destroy(obj);
    }
}
